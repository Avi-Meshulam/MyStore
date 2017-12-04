using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Windows.Mvvm;
using Prism.Commands;
using Prism.Windows.Navigation;
using Prism.Windows.AppModel;
using MyStore.BL.Models;
using MyStore.BL.Services;
using MyStore.BL.Repositories;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using MyStore.Data;
using Windows.Storage;
using Prism.Unity.Windows;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MyStore.BL.ViewModels
{
    public class CatalogViewModel : ViewModelBase<Catalog>
    {
        #region Services
        private readonly IDataRepository<Catalog> _catalogsRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        #endregion // Services

        public CatalogViewModel(
            Catalog model,
            IDataRepository<Catalog> catalogsRepository,
            INavigationService navigationService,
            IDialogService dialogService) : base(model)
        {
            _catalogsRepository = catalogsRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;

            InitData();
            InitCommands();
        }

        public CatalogViewModel(Catalog model)
            : this(model,
                  Container.Resolve<IDataRepository<Catalog>>(),
                  Container.Resolve<INavigationService>(),
                  Container.Resolve<IDialogService>())
        { }

        public ObservableCollection<ProductViewModel> Products { get; private set; }

        #region Commands
        public DelegateCommand<ProductViewModel> AddProductCommand { get; private set; }
        public DelegateCommand<ProductViewModel> DeleteProductCommand { get; private set; }
        #endregion // Commands

        #region Data Event Handlers
        private void Products_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (ProductViewModel product in e.OldItems)
                {
                    _model.Products.Remove(product.Model);
                    UnsubscribeFromProductEvents(product);
                }

            if (e.NewItems != null)
                foreach (ProductViewModel product in e.NewItems)
                {
                    _model.Products.Add(product.Model);
                    SubscribeToProductEvents(product);
                }

            _catalogsRepository.Update(_model);
        }

        private void Product_ProductDeleted(object sender, EventArgs e)
        {
            Products.Remove(sender as ProductViewModel);
        }
        #endregion // Data Event Handlers

        #region Repository Proxies
        private void AddProduct(ProductViewModel product)
        {
            if (Products.Contains(product))
                return;

            try
            {
                Products.Add(product);
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
                return;
            }
        }

        private void DeleteProduct(ProductViewModel product)
        {
            try
            {
                Products.Remove(product);
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
                return;
            }
        }

        public override bool Update()
        {
            try
            {
                _catalogsRepository.Update(_model);
                return true;
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
                return false;
            }
        }
        #endregion // Repository Proxies

        #region Helper Methods
        private void InitCommands()
        {
            AddProductCommand = new DelegateCommand<ProductViewModel>(AddProduct);
            DeleteProductCommand = new DelegateCommand<ProductViewModel>(DeleteProduct);
        }

        private async void InitData()
        {
            if (_model.Products == null || _model.Products.Count() == 0)
                await LoadProducts();

            if (_model.Products == null)
                Products = new ObservableCollection<ProductViewModel>();
            else
                Products = new ObservableCollection<ProductViewModel>(
                    _model.Products.Select(p => new ProductViewModel(p)));

            Products.CollectionChanged += Products_CollectionChanged;
            Products.ToList().ForEach(p => SubscribeToProductEvents(p));

            RaisePropertyChanged(nameof(Products));
        }

        private async Task LoadProducts()
        {
            var productsRepository = Container.Resolve<IDataRepository<Product>>();

            var products = productsRepository.GetByPredicate(
                i => i.CatalogId == _model.CatalogId);

            if (products?.Count > 0)
                products.ForEach(i => _model.Products.Add(i));
            else
            {
                await GenerateProducts();
                _catalogsRepository.Update(_model);
            }
        }

        private async Task GenerateProducts()
        {
            int count = 1;
            foreach (var product in _products)
            {
                product.CatalogId = _model.CatalogId;
                product.DatePublished = Utils.GetRandomDate(3);

                var imageFile = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(($"ms-appx:///Assets/Products/{count++:D2}.jpg")));

                product.Image = await IOUtils.ImageFileToByteArray(imageFile);

                _model.Products.Add(product);
            }
        }

        private void SubscribeToProductEvents(ProductViewModel product)
        {
            product.ProductDeleted += Product_ProductDeleted;
        }

        private void UnsubscribeFromProductEvents(ProductViewModel product)
        {
            product.ProductDeleted -= Product_ProductDeleted;
        }
        #endregion // Helper Methods

        #region Data Lists
        private static List<Product> _products = new List<Product>
        {
            new Product
            {
                Title = "Samsung UN65KS8000 65-Inch Premium Flat 4K SUHD TV",
                Description = "Samsung UN65KS8000 65-Inch 4K Ultra HD Smart LED TV (2016 Model)",
                ListPrice = 1499.99M
            },
            new Product
            {
                Title = "Sony XBR43X800D 43-Inch 4K Ultra HD TV",
                Description = "Sony XBR43X800D 43-Inch 4K Premium 4K HDR Ultra HD TV (2016 Model)",
                ListPrice = 648
            },
            new Product
            {
                Title = "LG Electronics Flat 55-Inch 4K Ultra HD Smart TV",
                Description = "LG Electronics OLED55B6P Flat 55-Inch 4K Ultra HD Smart OLED TV (2016 Model)",
                ListPrice = 2297
            },
            new Product
            {
                Title = "KitchenAid Stand Mixer",
                Description = "KitchenAid Stand Mixer RRK150wh Artisan Tilt White 325-watt with 10 speeds",
                ListPrice = 199.99M
            },
            new Product
            {
                Title = "K&H Backpack Pet Carrier",
                Description = "K&H Manufacturing Comfy Go Backpack Pet Carrier",
                ListPrice =60.99M
            },
            new Product
            {
                Title = "Canon T6 DSLR Camera 18-55 & 75-300mm Lens",
                Description = "Canon EOS Rebel T6 DSLR Camera w/ EF-S 18-55mm + EF 75-300mm Lens Printer Bundle",
                ListPrice =699
            },
            new Product
            {
                Title = "20 Inch Expandable Hardside Carry-On Luggage",
                Description = "Travelers Club Luggage Madison 20 Inch Hardside Expandable Carry-On",
                ListPrice =29.99M
            },
            new Product
            {
                Title = "Copper Frying Pan",
                Description = "2 Inch Deep Square Copper Frying Pan",
                ListPrice =16.99M
            },
            new Product
            {
                Title = "FPV Drone with HD Camera",
                Description = "DJI Phantom 3 Standard FPV Drone with 2.7K 12 Megapixel HD Camera",
                ListPrice =335
            },
            new Product
            {
                Title = "2010 Chevrolet Corvette Grand Sport Coupe 2-Door",
                Description = "2010 CHEVY CORVETTE Z16 GRAND SPORT 3LT Z51 NAV HUD 18K #106444 Texas Direct",
                ListPrice =30100
            },
            new Product
            {
                Title = "Samsung Gear S2 Classic T-Mobile Smartwatch",
                Description = "Samsung Gear S2 Classic T-Mobile Smartwatch w/ Leather Band LARGE Black SM-735T",
                ListPrice =125.99M
            },
            new Product
            {
                Title = "Jacob Bromwell All-American Flour Sifter",
                Description = "Sift up to 5 cups of flour in a design that remains true to the original.",
                ListPrice =79.98M
            }
        };
        #endregion //Data Lists
    }
}
