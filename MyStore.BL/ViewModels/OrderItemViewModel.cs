using Microsoft.Practices.Unity;
using MyStore.BL.Models;
using MyStore.BL.Repositories;
using MyStore.BL.Services;
using Prism.Commands;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Windows.Globalization.NumberFormatting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyStore.BL.ViewModels
{
    public class OrderItemViewModel : ViewModelBase<OrderItem>
    {
        public event EventHandler ItemDeleted;

        #region Services
        private readonly IDataRepository<OrderItem> _orderItemsRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        #endregion // Services

        public OrderItemViewModel(
            OrderItem model,
            IDataRepository<OrderItem> orderItemsRepository,
            INavigationService navigationService,
            IDialogService dialogService) : base(model)
        {
            _orderItemsRepository = orderItemsRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;

            Product = new ProductViewModel(_model.Product);
            Product.PropertyChanged += Product_PropertyChanged;

            LoadData();
        }

        public OrderItemViewModel(OrderItem model)
            : this(model,
                  Container.Resolve<IDataRepository<OrderItem>>(),
                  Container.Resolve<INavigationService>(),
                  Container.Resolve<IDialogService>())
        { }

        public ProductViewModel Product { get; private set; }

        public decimal FullPrice
        {
            get { return Quantity * ListPrice; }
        }

        public decimal DiscountedPrice
        {
            get { return FullPrice * (1 - (DiscountPercentage / 100)); }
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

        private decimal _listPrice;

        /// <summary>
        /// The price of the product for the time of the order
        /// </summary>
        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal ListPrice
        {
            get { return PropertyHasErrors() ? _listPrice : _model.ListPrice; }
            set { SetProperty(ref _listPrice, value, () => _model.ListPrice = value); }
        }

        private decimal _discountPercentage;

        [Range(0, 100)]
        public decimal DiscountPercentage
        {
            get { return PropertyHasErrors() ? _discountPercentage : _model.DiscountPercentage; }
            set { SetProperty(ref _discountPercentage, value, () => _model.DiscountPercentage = value); }
        }

        private int _quantity;

        [Range(1, int.MaxValue)]
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
                _orderItemsRepository.Update(_model);
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
                _orderItemsRepository.Delete(_model);
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
            if (Product == null)
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
