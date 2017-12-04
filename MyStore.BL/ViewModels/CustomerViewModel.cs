using Microsoft.Practices.Unity;
using MyStore.BL.Models;
using MyStore.BL.Repositories;
using MyStore.BL.Services;
using Prism.Commands;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.BL.ViewModels
{
    public class CustomerViewModel : ViewModelBase<Customer>
    {
        #region Services
        private readonly IDataRepository<Customer> _customersRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        #endregion // Services

        public CustomerViewModel(
            Customer model,
            IDataRepository<Customer> customersRepository,
            INavigationService navigationService,
            IDialogService dialogService) : base(model)
        {
            _customersRepository = customersRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;
        }

        public CustomerViewModel(Customer model)
            : this(model,
                  Container.Resolve<IDataRepository<Customer>>(),
                  Container.Resolve<INavigationService>(),
                  Container.Resolve<IDialogService>())
        { }

        #region Model Properties Proxies
        private string _firstName;

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName
        {
            get { return PropertyHasErrors() ? _firstName : _model.FirstName; }
            set { SetProperty(ref _firstName, value, () => _model.FirstName = value); }
        }

        private string _lastName;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName
        {
            get { return PropertyHasErrors() ? _lastName : _model.LastName; }
            set { SetProperty(ref _lastName, value, () => _model.LastName = value); }
        }

        private DateTimeOffset _dateEnlisted = DateTimeOffset.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Enlisted")]
        public DateTimeOffset DateEnlisted
        {
            get { return PropertyHasErrors() ? _dateEnlisted : _model.DateEnlisted; }
            set { SetProperty(ref _dateEnlisted, value, () => _model.DateEnlisted = value); }
        }

        private DateTimeOffset? _birthDate;

        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? BirthDate
        {
            get { return PropertyHasErrors() ? _birthDate : _model.BirthDate; }
            set { SetProperty(ref _birthDate, value, () => _model.BirthDate = value); }
        }

        private string _email;

        [EmailAddress]
        [StringLength(50)]
        public string Email
        {
            get { return PropertyHasErrors() ? _email : _model.Email; }
            set { SetProperty(ref _email, value, () => _model.Email = value); }
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
        #endregion // Model Properties Proxies

        #region Repository Proxies
        public override bool Update()
        {
            try
            {
                _customersRepository.Update(_model);
                return true;
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
                return false;
            }
        }

        private void Delete()
        {
            try
            {
                _customersRepository.Delete(_model);
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
                return;
            }
        }
        #endregion // Repository Proxies
    }
}
