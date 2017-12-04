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
using Windows.Globalization.NumberFormatting;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyStore.BL.ViewModels
{
    public class ProductViewModel : ViewModelBase<Product>
    {
        private readonly static BitmapImage _placeholderImage =
            new BitmapImage(new Uri("ms-appx:///Assets/Products/placeholder.png"));

        public event EventHandler ProductDeleted;

        #region Services
        private readonly IDataRepository<Product> _productsRepository;
        private readonly IDataRepository<ShoppingCart> _shoppingCartsRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        #endregion // Services

        public ProductViewModel(
            Product model,
            IDataRepository<Product> productsRepository,
            IDataRepository<ShoppingCart> shoppingCartsRepository,
            INavigationService navigationService,
            IDialogService dialogService) : base(model)
        {
            _productsRepository = productsRepository;
            _shoppingCartsRepository = shoppingCartsRepository;
            _dialogService = dialogService;
            _navigationService = navigationService;

            SetImageSource();
            InitCommands();
        }

        public ProductViewModel(Product model)
            : this(model,
                  Container.Resolve<IDataRepository<Product>>(),
                  Container.Resolve<IDataRepository<ShoppingCart>>(),
                  Container.Resolve<INavigationService>(),
                  Container.Resolve<IDialogService>())
        { }

        public ImageSource ImageSource { get; private set; }

        #region Commands
        public DelegateCommand<int?> AddToCartCommand { get; private set; }
        #endregion // Commands

        #region Model Properties Proxies
        private string _title;

        [Required]
        [MaxLength(50)]
        public string Title
        {
            get { return PropertyHasErrors() ? _title : _model.Title; }
            set { SetProperty(ref _title, value, () => _model.Title = value); }
        }

        private string _description;

        [Required]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description
        {
            get { return PropertyHasErrors() ? _description : _model.Description; }
            set { SetProperty(ref _description, value, () => _model.Description = value); }
        }

        DateTimeOffset _datePublished = DateTimeOffset.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Date Published")]
        public DateTimeOffset DatePublished
        {
            get { return PropertyHasErrors() ? _datePublished : _model.DatePublished; }
            set { SetProperty(ref _datePublished, value, () => _model.DatePublished = value); }
        }

        private decimal _listPrice;

        /// <summary>
        /// The current price of the product
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

        private byte[] _image
        {
            get { return _model.Image; }
            set
            {
                _model.Image = value;
                SetImageSource();
            }
        }
        #endregion // Model Properties Proxies

        #region Repository Proxies
        private async void AddToCart(int? quantity)
        {
            Utils.SetProgressRing(true);

            bool result = false;
            try
            {
                result = CurrentShoppingCart.AddItem(this, quantity ?? 1);
            }
            finally
            {
                Utils.SetProgressRing(false);
            }

            if(result == false)
                return;

            var dialogResult = await _dialogService.ShowDialog(
                "Product has been successfully added to shopping cart.\n" +
                "What would you like to do?", "Add To Cart",
                "Go To Cart", "Continue Shopping");

            if (dialogResult == ContentDialogResult.Primary)
            {
                _navigationService.Navigate(nameof(ShoppingCart), CurrentShoppingCart);
            }
        }

        public override bool Update()
        {
            try
            {
                _productsRepository.Update(_model);
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
                _productsRepository.Delete(_model);
                ProductDeleted(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
                return;
            }
        }
        #endregion // Repository Proxies

        #region Helper Methods
        private void InitCommands()
        {
            AddToCartCommand = new DelegateCommand<int?>(AddToCart);
        }

        private async void SetImageSource()
        {
            if (_image == null)
                ImageSource = _placeholderImage;
            else
                ImageSource = await IOUtils.ByteArrayToBitmapImage(_image);

            RaisePropertyChanged(nameof(ImageSource));
        }
        #endregion // Helper Methods
    }
}
