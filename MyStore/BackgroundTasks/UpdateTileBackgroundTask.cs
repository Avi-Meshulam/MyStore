using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.Web.Syndication;
using MyStore.BL.Models;
using MyStore.BL.Repositories;
using MyStore;
using MyStore.BL.ViewModels;
using Prism.Unity.Windows;
using Microsoft.Practices.Unity;
using NotificationsExtensions.TileContent;
using NotificationsExtensions.BadgeContent;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;

namespace MyStore.BackgroundTasks
{
    public sealed class UpdateTileBackgroundTask : IBackgroundTask
    {
        private const string SHOPPING_CART_MESSAGE =
            "You have {0} items in your shopping cart";
        private const string EMPTY_SHOPPING_CART_MESSAGE = 
            "Welcome to My Store App! Step in and find the best deals on the market!";
        private const string STORE_LOGO_ALT = "My Store Logo";

        private static IUnityContainer _container = PrismUnityApplication.Current.Container;

        private ITileSquare150x150PeekImageAndText04 _tileSquareContent;
        private ITileWide310x150PeekImage03 _tileWideContent;

        public UpdateTileBackgroundTask()
        {
            _tileSquareContent = TileContentFactory.CreateTileSquare150x150PeekImageAndText04();
            _tileSquareContent.Image.Src = "ms-appx:///Assets/Square150x150Logo.png";
            _tileSquareContent.Image.Alt = STORE_LOGO_ALT;

            _tileWideContent =
                TileContentFactory.CreateTileWide310x150PeekImage03();
            _tileWideContent.Image.Src = "ms-appx:///Assets/Wide310x150Logo.png";
            _tileWideContent.Image.Alt = STORE_LOGO_ALT;
        }

        private ShoppingCartViewModel _currentShoppingCart;
        public ShoppingCartViewModel CurrentShoppingCart
        {
            get
            {
                if(_currentShoppingCart == null)
                {
                    if(_container.IsRegistered<ShoppingCartViewModel>(
                        nameof(ViewModelBase<ShoppingCart>.CurrentShoppingCart)))
                    {
                        _currentShoppingCart = _container.Resolve<ShoppingCartViewModel>(
                            nameof(ViewModelBase<ShoppingCart>.CurrentShoppingCart));
                    }
                }

                return _currentShoppingCart;
            }
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            if (CurrentShoppingCart == null)
                return;

            var badgeContent =
                   new BadgeNumericNotificationContent((uint)CurrentShoppingCart.ItemsQuantity);

            BadgeUpdateManager.CreateBadgeUpdaterForApplication()
                .Update(badgeContent.CreateNotification());

            if (CurrentShoppingCart.Items.Count > 0)
            {
                _tileSquareContent.TextBodyWrap.Text = 
                    string.Format(SHOPPING_CART_MESSAGE, CurrentShoppingCart.ItemsQuantity);

                _tileWideContent.TextHeadingWrap.Text = 
                    string.Format(SHOPPING_CART_MESSAGE, CurrentShoppingCart.ItemsQuantity);
            }
            else
            {
                _tileSquareContent.TextBodyWrap.Text = EMPTY_SHOPPING_CART_MESSAGE;
                _tileWideContent.TextHeadingWrap.Text = EMPTY_SHOPPING_CART_MESSAGE;
            }

            _tileWideContent.Square150x150Content = _tileSquareContent;

            TileUpdateManager.CreateTileUpdaterForApplication()
                .Update(_tileWideContent.CreateNotification());
        }
    }
}