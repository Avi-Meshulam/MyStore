using Microsoft.Practices.Unity;
using MyStore.BL.Models;
using MyStore.BL.Repositories;
using MyStore.BL.Services;
using Prism.Commands;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.NumberFormatting;

namespace MyStore.BL.ViewModels
{
    public class OrderViewModel : ViewModelBase<Order>
    {
        #region Services
        private readonly IDataRepository<Order> _ordersRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        #endregion // Services

        public OrderViewModel(
            Order model,
            IDataRepository<Order> ordersRepository,
            INavigationService navigationService,
            IDialogService dialogService) : base(model)
        {
            _ordersRepository = ordersRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;

            InitData();
        }

        public OrderViewModel(Order model)
            : this(model,
                  Container.Resolve<IDataRepository<Order>>(),
                  Container.Resolve<INavigationService>(),
                  Container.Resolve<IDialogService>())
        { }

        public ObservableCollection<OrderItemViewModel> Items { get; private set; }

        public int ItemsQuantity
        {
            get { return Items.Sum(i => i.Quantity); }
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
            if (e.OldItems != null)
                foreach (OrderItemViewModel item in e.OldItems)
                {
                    _model.Items.Remove(item.Model);
                    UnsubscribeFromItemEvents(item);
                }

            if (e.NewItems != null)
                foreach (OrderItemViewModel item in e.NewItems)
                {
                    _model.Items.Add(item.Model);
                    SubscribeToItemEvents(item);
                }

            _ordersRepository.Update(_model);

            RaisePropertyChanged(nameof(ItemsQuantity));
            RaisePropertyChanged(nameof(FullPrice));
            RaisePropertyChanged(nameof(DiscountedPrice));
        }

        private void Item_ItemDeleted(object sender, EventArgs e)
        {
            Items.Remove(sender as OrderItemViewModel);
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrderItemViewModel.Quantity))
                RaisePropertyChanged(nameof(ItemsQuantity));

            if (e.PropertyName == nameof(OrderItemViewModel.FullPrice))
                RaisePropertyChanged(nameof(FullPrice));

            if (e.PropertyName == nameof(OrderItemViewModel.DiscountedPrice))
                RaisePropertyChanged(nameof(DiscountedPrice));
        }
        #endregion // Data Event Handlers

        #region Model Properties Proxies
        private DateTimeOffset _dateCreated;

        [DataType(DataType.DateTime)]
        public DateTimeOffset DateCreated
        {
            get { return PropertyHasErrors() ? _dateCreated : _model.DateCreated; }
            set { SetProperty(ref _dateCreated, value, () => _model.DateCreated = value); }
        }
        #endregion // Model Properties Proxies

        #region Repository Proxies
        public bool AddItem(ProductViewModel product, int quantity)
        {
            try
            {
                var item = Items.FirstOrDefault(i => i.Product.Equals(product));

                if (item == null)
                    Items.Add(new OrderItemViewModel(
                        new OrderItem(_model, product.Model, quantity)));
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
                _ordersRepository.Update(_model);
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
        #endregion // Repository Proxies

        #region Helper Methods
        private void InitData()
        {
            if (_model.Items == null || _model.Items.Count() == 0)
                LoadItems();

            if (_model.Items == null)
                Items = new ObservableCollection<OrderItemViewModel>();
            else
                Items = new ObservableCollection<OrderItemViewModel>(
                    _model.Items.Select(i => new OrderItemViewModel(i)));

            Items.CollectionChanged += Items_CollectionChanged;
            Items.ToList().ForEach(i => SubscribeToItemEvents(i));
        }

        private void LoadItems()
        {
            var orderItemsRepository = Container.Resolve<IDataRepository<OrderItem>>();

            var items = orderItemsRepository.GetByPredicate(
                i => i.OrderId == _model.OrderId);

            if (items?.Count > 0)
                items.ForEach(i => _model.Items.Add(i));
        }

        private void SubscribeToItemEvents(OrderItemViewModel item)
        {
            item.ItemDeleted += Item_ItemDeleted;
            item.PropertyChanged += Item_PropertyChanged;
        }

        private void UnsubscribeFromItemEvents(OrderItemViewModel item)
        {
            item.ItemDeleted -= Item_ItemDeleted;
            item.PropertyChanged -= Item_PropertyChanged;
        }
        #endregion // Helper Methods
    }
}
