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
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Practices.Unity;
using Prism.Unity.Windows;

namespace MyStore.BL.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const string MAIN_CATALOG_NAME = "Main";

        private static readonly IUnityContainer _container = PrismUnityApplication.Current.Container;

        #region Services
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        #endregion // Services

        public MainViewModel(
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            LoadCurrentShoppingCart();

            _navigationService.Navigate(nameof(Catalog), FirstCatalog);
        }

        private CatalogViewModel _firstCatalog;
        public CatalogViewModel FirstCatalog
        {
            get
            {
                return _firstCatalog ?? (_firstCatalog = _container.Resolve<CatalogViewModel>(
                    new ParameterOverride("model", GetFirstCatalog())));
            }
        }

        private ShoppingCartViewModel _currentShoppingCart;
        public ShoppingCartViewModel CurrentShoppingCart
        {
            get { return _currentShoppingCart; }
            set { SetProperty(ref _currentShoppingCart, value); }
        }

        private bool _isPaneOpen;
        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { SetProperty(ref _isPaneOpen, value); }
        }

        private bool _isProgressRingVisible;
        public bool IsProgressRingVisible
        {
            get { return _isProgressRingVisible; }
            set { SetProperty(ref _isProgressRingVisible, value); }
        }

        private int _selectedMenuIndex;
        public int SelectedMenuIndex
        {
            get { return _selectedMenuIndex; }
            set { SetProperty(ref _selectedMenuIndex, value); }
        }

        public void TogglePane()
        {
            IsPaneOpen = !IsPaneOpen;
            RaisePropertyChanged(nameof(IsPaneOpen));
        }

        public void OpenPane()
        {
            IsPaneOpen = true;
            RaisePropertyChanged(nameof(IsPaneOpen));
        }

        public void ClosePane()
        {
            IsPaneOpen = false;
            RaisePropertyChanged(nameof(IsPaneOpen));
        }

        public void PaneClosed()
        {
            IsPaneOpen = false;
        }

        public void GoToCart()
        {
            _navigationService.Navigate(nameof(ShoppingCart), CurrentShoppingCart);
        }

        public void GoHome()
        {
            _navigationService.Navigate(nameof(Catalog), FirstCatalog);
        }

        public async void MenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = sender as Selector;

            switch ((MainMenuItems)selector.SelectedIndex)
            {
                case MainMenuItems.Home:
                    _navigationService.Navigate(nameof(Catalog), FirstCatalog);
                    break;
                case MainMenuItems.ShoppingCart:
                    _navigationService.Navigate(nameof(ShoppingCart), CurrentShoppingCart);
                    break;
                case MainMenuItems.Exit:
                    await ShowExitDialog();
                    break;
                default:
                    break;
            }
        }

        #region Helper Methods
        private async void LoadCurrentShoppingCart()
        {
            while (!_container.IsRegistered<ShoppingCartViewModel>(
                nameof(ViewModelBase<ShoppingCart>.CurrentShoppingCart)))
            {
                await Task.Delay(1000);
            }

            CurrentShoppingCart = _container.Resolve<ShoppingCartViewModel>(
                nameof(ViewModelBase<ShoppingCart>.CurrentShoppingCart));
        }

        private Catalog GetFirstCatalog()
        {
            var catalogsRepository = _container.Resolve<IDataRepository<Catalog>>();

            var startCatalog = catalogsRepository.GetAll().FirstOrDefault();
            if (startCatalog == null)
                startCatalog = catalogsRepository.Add(new Catalog { Title = MAIN_CATALOG_NAME });

            return startCatalog;
        }

        private async Task ShowExitDialog()
        {
            var result = await _dialogService.ShowDialog(
                "Are you sure you want to exit?", "Exit", "Yes", "No");
            if (result == ContentDialogResult.Primary)
                Application.Current.Exit();
        }
        #endregion // Helper Methods
    }
}
