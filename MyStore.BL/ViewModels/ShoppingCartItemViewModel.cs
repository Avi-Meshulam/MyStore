using Microsoft.Practices.Unity;
using MyStore.BL.Models;
using MyStore.BL.Repositories;
using MyStore.BL.Services;
using Prism.Commands;
using Prism.Windows.AppModel;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Globalization.NumberFormatting;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyStore.BL.ViewModels
{
    public class ShoppingCartItemViewModel : ViewModelBase<ShoppingCartItem>
    {
        private const int MIN_QUANTITY = 1;
        private const int MAX_QUANTITY = 1000;

        public event EventHandler ItemDeleted;

        #region Services
        private readonly IDataRepository<ShoppingCartItem> _shoppingCartItemsRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        #endregion // Services

        public ShoppingCartItemViewModel(
            ShoppingCartItem model,
            IDataRepository<ShoppingCartItem> shoppingCartItemsRepository,
            INavigationService navigationService,
            IDialogService dialogService) : base(model)
        {
            _shoppingCartItemsRepository = shoppingCartItemsRepository;
            _dialogService = dialogService;
            _navigationService = navigationService;

            LoadData();
        }

        public ShoppingCartItemViewModel(ShoppingCartItem model)
            : this(model,
                  Container.Resolve<IDataRepository<ShoppingCartItem>>(),
                  Container.Resolve<INavigationService>(),
                  Container.Resolve<IDialogService>())
        { }

        public ProductViewModel Product { get; private set; }

        public decimal FullPrice
        {
            get { return Quantity * Product.ListPrice; }
        }

        public decimal DiscountedPrice
        {
            get { return FullPrice * (1 - (DiscountPercentage / 100)); }
        }

        public void IncrementQuantity()
        {
            if (Quantity < MAX_QUANTITY) Quantity++;
        }

        public void DecrementQuantity()
        {
            if(Quantity > MIN_QUANTITY) Quantity--;
        }

        #region Data Event Handlers
        private void Product_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProductViewModel.ImageSource))
                RaisePropertyChanged(e.PropertyName);
        }
        #endregion // Data Event Handlers

        #region Model Properties Proxies
        public string Title
        {
            get { return Product.Title; }
        }

        public string Description
        {
            get { return Product.Description; }
        }

        public ImageSource ImageSource
        {
            get { return Product.ImageSource; }
        }

        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal ListPrice
        {
            get { return Product.ListPrice; }
        }

        [Range(0, 100)]
        public decimal DiscountPercentage
        {
            get { return Product.DiscountPercentage; }
        }

        private int _quantity;

        [Range(MIN_QUANTITY, MAX_QUANTITY)]
        public int Quantity
        {
            get { return PropertyHasErrors() ? _quantity : _model.Quantity; }
            set
            {
                SetProperty(ref _quantity, value, () =>
                {
                    _model.Quantity = value;
                    RaisePropertyChanged(nameof(FullPrice));
                    RaisePropertyChanged(nameof(DiscountedPrice));
                });
            }
        }
        #endregion // Model Properties Proxies

        #region Repository Proxies
        public override bool Update()
        {
            try
            {
                _shoppingCartItemsRepository.Update(_model);
                return true;
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
                return false;
            }
        }

        public void Delete()
        {
            try
            {
                _shoppingCartItemsRepository.Delete(_model);
                Product.PropertyChanged -= Product_PropertyChanged;
                ItemDeleted(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
                return;
            }
        }
        #endregion // Repository Proxies

        #region Helper Methods
        private void LoadData()
        {
            if(Product == null)
            {
                var productsRepository = Container.Resolve<IDataRepository<Product>>();
                var product = productsRepository.GetById(_model.ProductId);
                Product = new ProductViewModel(product);
            }

            Product.PropertyChanged += Product_PropertyChanged;
        }
        #endregion Helper Methods
    }
}
