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
using Windows.Globalization.NumberFormatting;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Unity.Windows;
using System.ComponentModel;
using System.Collections.Specialized;

namespace MyStore.BL.ViewModels
{
    public class ShoppingCartViewModel : ViewModelBase<ShoppingCart>
    {
        #region Services
        private readonly IDataRepository<ShoppingCart> _shoppingCartsRepository;
        private readonly IDataRepository<Order> _ordersRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        #endregion // Services

        public ShoppingCartViewModel(
            ShoppingCart model,
            IDataRepository<ShoppingCart> shoppingCartsRepository,
            IDataRepository<Order> ordersRepository,
            INavigationService navigationService,
            IDialogService dialogService) : base(model)
        {
            _shoppingCartsRepository = shoppingCartsRepository;
            _ordersRepository = ordersRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;

            InitData();
        }

        public ShoppingCartViewModel(ShoppingCart model)
            : this(model,
                  Container.Resolve<IDataRepository<ShoppingCart>>(),
                  Container.Resolve<IDataRepository<Order>>(),
                  Container.Resolve<INavigationService>(),
                  Container.Resolve<IDialogService>())
        { }

        public ObservableCollection<ShoppingCartItemViewModel> Items { get; private set; }

        public int ItemsQuantity
        {
            get { return Items.Sum(i => i.Quantity); }
        }

        public bool IsEmpty
        {
            get { return Items.Count == 0; }
        }

        public decimal FullPrice
        {
            get { return Items.Sum(i => i.FullPrice); }
        }

        public decimal DiscountedPrice
        {
            get { return Items.Sum(i => i.DiscountedPrice); }
        }

        #region Data Event Handlers
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.OldItems != null)
                foreach (ShoppingCartItemViewModel item in e.OldItems)
                {
                    _model.Items.Remove(item.Model);
                    UnsubscribeFromItemEvents(item);
                }

            if (e.NewItems != null)
                foreach (ShoppingCartItemViewModel item in e.NewItems)
                {
                    _model.Items.Add(item.Model);
                    SubscribeToItemEvents(item);
                }

            _shoppingCartsRepository.Update(_model);

            RaisePropertyChanged(nameof(IsEmpty));
            RaisePropertyChanged(nameof(ItemsQuantity));
            RaisePropertyChanged(nameof(FullPrice));
            RaisePropertyChanged(nameof(DiscountedPrice));
        }

        private void Item_ItemDeleted(object sender, EventArgs e)
        {
            Items.Remove(sender as ShoppingCartItemViewModel);
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ShoppingCartItemViewModel.Quantity))
                RaisePropertyChanged(nameof(ItemsQuantity));

            if (e.PropertyName == nameof(ShoppingCartItemViewModel.FullPrice))
                RaisePropertyChanged(nameof(FullPrice));

            if (e.PropertyName == nameof(ShoppingCartItemViewModel.DiscountedPrice))
                RaisePropertyChanged(nameof(DiscountedPrice));
        }
        #endregion // Data Event Handlers

        #region Repository Proxies
        public bool AddItem(ProductViewModel product, int quantity)
        {
            try
            {
                var item = Items.FirstOrDefault(i => i.Product.Equals(product));

                if (item == null)
                    Items.Add(new ShoppingCartItemViewModel(
                        new ShoppingCartItem(_model, product.Model, quantity)));
                else
                    item.Quantity += quantity;

                return true;
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
                return false;
            }
        }

        public override bool Update()
        {
            try
            {
                _shoppingCartsRepository.Update(_model);
                return true;
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
                return false;
            }
        }

        public void Clear()
        {
            try
            {
                _model.Items.Clear();
                Items.Clear();
            }
            catch (Exception ex)
            {
                Utils.HandleException(ex);
                return;
            }
        }

        public void Checkout()
        {
            Utils.SetProgressRing(true);

            try
            {
                // Add new order
                var order = _ordersRepository.Add(new Order(CurrentCustomer.Model));
                Items.ToList().ForEach(i =>
                    order.Items.Add(new OrderItem(order, i.Product.Model, i.Quantity)));
                order.State = OrderState.Closed;
                _ordersRepository.Update(order);

                // Clear shopping cart
                Clear();
            }
            finally
            {
                Utils.SetProgressRing(false);
            }
        }
        #endregion // Repository Proxies

        #region Helper Methods
        private void InitData()
        {
            if (_model.Items == null || _model.Items.Count() == 0)
                LoadItems();

            if (_model.Items == null)
                Items = new ObservableCollection<ShoppingCartItemViewModel>();
            else
                Items = new ObservableCollection<ShoppingCartItemViewModel>(
                    _model.Items.Select(i => new ShoppingCartItemViewModel(i)));

            Items.CollectionChanged += Items_CollectionChanged;
            Items.ToList().ForEach(i => SubscribeToItemEvents(i));
        }

        private void LoadItems()
        {
            var shoppingCartItemsRepository = 
                Container.Resolve<IDataRepository<ShoppingCartItem>>();

            var items = shoppingCartItemsRepository.GetByPredicate(
                i => i.ShoppingCartId == _model.ShoppingCartId);

            if (items?.Count > 0)
                items.ForEach(i => _model.Items.Add(i));
        }

        private void SubscribeToItemEvents(ShoppingCartItemViewModel item)
        {
            item.ItemDeleted += Item_ItemDeleted;
            item.PropertyChanged += Item_PropertyChanged;
        }

        private void UnsubscribeFromItemEvents(ShoppingCartItemViewModel item)
        {
            item.ItemDeleted -= Item_ItemDeleted;
            item.PropertyChanged -= Item_PropertyChanged;
        }
        #endregion // Helper Methods
    }
}
