using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.Unity;
using Prism.Unity.Windows;
using Windows.Globalization.NumberFormatting;
using MyStore.BL.Models;

namespace MyStore.BL.ViewModels
{
    public abstract class ViewModelBase<T> : ViewModelBase, IEquatable<ViewModelBase<T>>,
        INotifyDataErrorInfo where T : Equatable<T>, IEquatable<T>, new()
    {
        private ConcurrentDictionary<string, ICollection<string>> _validationErrors =
            new ConcurrentDictionary<string, ICollection<string>>();

        protected static readonly IUnityContainer Container = PrismUnityApplication.Current.Container;

        public ViewModelBase() : this(new T())
        { }

        public ViewModelBase(T model)
        {
            _model = model ?? new T();
        }

        protected readonly T _model;
        internal T Model { get { return _model; } }

        private CustomerViewModel _currentCustomer;
        public CustomerViewModel CurrentCustomer
        {
            get { return _currentCustomer ?? (_currentCustomer = 
                    Container.Resolve<CustomerViewModel>(nameof(CurrentCustomer))); }
        }

        private ShoppingCartViewModel _currentShoppingCart;
        public ShoppingCartViewModel CurrentShoppingCart
        {
            get
            {
                return _currentShoppingCart ?? (_currentShoppingCart =
                  Container.Resolve<ShoppingCartViewModel>(nameof(CurrentShoppingCart)));
            }
        }

        public abstract bool Update();

        protected override bool SetProperty<P>(ref P storage, P value, 
            Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (!base.SetProperty(ref storage, value, onChanged, propertyName))
                return false;

            Update();   // Persist

            return true;
        }

        bool IEquatable<ViewModelBase<T>>.Equals(ViewModelBase<T> other)
        {
            return _model.Equals(other._model);
        }

        // Overrides System.Object Equals method
        public override bool Equals(object obj)
        {
            if (obj is ViewModelBase<T>)
                return ((IEquatable<ViewModelBase<T>>)this).Equals((ViewModelBase<T>)obj);
            else
                return false;
        }

        // Overrides System.Object GetHashCode method
        public override int GetHashCode()
        {
            return _model.GetHashCode();
        }

        #region INotifyDataErrorInfo members
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            ICollection<string> errorsForName = null;
            _validationErrors.TryGetValue(propertyName, out errorsForName);
            return errorsForName;
        }

        private bool _hasErrors;
        bool INotifyDataErrorInfo.HasErrors
        {
            get { return _hasErrors = _validationErrors.Any(kv => kv.Value != null && kv.Value.Count > 0); }
        }
        #endregion // INotifyDataErrorInfo members

        #region INotifyDataErrorInfo Extensions
        public string GetAggregatedErrors(string propertyName)
        {
            return (this as INotifyDataErrorInfo)
                .GetErrors(propertyName)?.Cast<string>().Aggregate((a, b) => $"{a}\n{b}");
        }

        public bool PropertyHasErrors([CallerMemberName] string propertyName = null)
        {
            return _validationErrors.Any(kv => kv.Key == propertyName && kv.Value != null && kv.Value?.Count > 0);
        }
        #endregion INotifyDataErrorInfo Extensions

        // Data-annotations-based validation
        protected void Validate()
        {
            _validationErrors.Clear();

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();

            ValidationContext validationContext = new ValidationContext(this, null, null);

            bool isValid = Validator.TryValidateObject(this, validationContext, validationResults, true);

            foreach (ValidationResult validationResult in validationResults)
            {
                string propertyName = validationResult.MemberNames.ElementAt(0);

                _validationErrors.AddOrUpdate(propertyName, new List<string> { validationResult.ErrorMessage },
                    (prop, propErrors) =>
                    {
                        propErrors.Add(validationResult.ErrorMessage);
                        return propErrors;
                    });

                RaiseErrorsChanged(propertyName);
            }

            if (_hasErrors == isValid)  // => _hasErrors != (!isValid)
            {
                _hasErrors = isValid;
                RaisePropertyChanged(nameof(INotifyDataErrorInfo.HasErrors));
            }
        }

        // Data-annotations-based validation
        protected void ValidateProperty(object propertyValue, string propertyName)
        {
            ICollection<string> propErrors;
            _validationErrors.TryRemove(propertyName, out propErrors);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();

            ValidationContext validationContext =
                new ValidationContext(this, null, null) { MemberName = propertyName };

            bool isValid = Validator.TryValidateProperty(propertyValue, validationContext, validationResults);

            if (!isValid)
            {
                _validationErrors.TryAdd(propertyName, validationResults.Select(r => r.ErrorMessage).ToList());
            }

            RaiseErrorsChanged(propertyName);
        }
    }
}
