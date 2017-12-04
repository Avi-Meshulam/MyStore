using MyStore.Services;
using MyStore.BL;
using MyStore.BL.Models;
using MyStore.BL.Repositories;
using MyStore.BL.Services;
using MyStore.BL.ViewModels;
using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Globalization.NumberFormatting;
using Windows.System.UserProfile;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using Windows.Networking.Connectivity;
using Windows.Networking;
using Windows.Foundation.Collections;
using System.Linq;
using Windows.Storage;
using System.Threading;
using Windows.ApplicationModel.Background;
using MyStore.BackgroundTasks;
using System.Reflection;

namespace MyStore
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : PrismUnityApplication
    {
        private static readonly string _taskName;
        private static readonly string _taskEntryPoint;

        // Used to launch UpdateTileBackground, regardless of the task's triggers
        private DispatcherTimer _tileTimer;

        static App()
        {
            _taskName = nameof(UpdateTileBackgroundTask);
            _taskEntryPoint = typeof(UpdateTileBackgroundTask).FullName;
        }

        // Used only to test notifications function
        private UpdateTileBackgroundTask _taskInstance;
        public UpdateTileBackgroundTask TaskInstance
        {
            get { return _taskInstance ?? (_taskInstance = new UpdateTileBackgroundTask()); }
            set { _taskInstance = value; }
        }

        protected override UIElement CreateShell(Frame rootFrame)
        {
            var shell = Container.Resolve<MainPage>();
            shell.SetFrame(rootFrame);
            return shell;
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            Window.Current.Activate();
            return Task.FromResult(true);
        }

        protected async override void ConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();

            // Register Services
            Container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());

            //// Register Repositories
            Container.RegisterType<IDataRepository<Catalog>, CatalogsRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDataRepository<Product>, ProductsRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDataRepository<Customer>, CustomersRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDataRepository<Order>, OrdersRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDataRepository<OrderItem>, OrderItemsRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDataRepository<ShoppingCart>, ShoppingCartsRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDataRepository<ShoppingCartItem>, ShoppingCartItemsRepository>(new ContainerControlledLifetimeManager());

            // Register View Models
            Container.RegisterType<MainViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CatalogViewModel>();
            Container.RegisterType<ProductViewModel>();
            Container.RegisterType<ShoppingCartViewModel>();
            Container.RegisterType<OrderViewModel>();
            Container.RegisterType<CustomerViewModel>();

            // Register Instances
            var customer = await GetCurrentCustomer();
            Container.RegisterInstance(nameof(ViewModelBase<Customer>.CurrentCustomer),
                new CustomerViewModel(customer));

            var shoppingCart = GetShoppingCart(customer);
            Container.RegisterInstance(nameof(ViewModelBase<ShoppingCart>.CurrentShoppingCart),
                new ShoppingCartViewModel(shoppingCart));

            RegisterBackgroundTask();

            InitTileTimer();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.Name.Replace("Page", string.Empty);

                var viewModelTypeName = $"{nameof(MyStore)}.{nameof(BL)}.{nameof(BL.ViewModels)}.{viewName}ViewModel, {nameof(MyStore)}.{nameof(BL)}";
                var viewModelType = Type.GetType(viewModelTypeName);

                return viewModelType;
            });
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            Container.RegisterInstance<INavigationService>(NavigationService);
            return base.OnInitializeAsync(args);
        }

        private async Task<Customer> GetCurrentCustomer()
        {
            var (firstName, lastName, nonRoamableId) = await GetCurrentUserCredentials();

            // Check if customer already exists
            var customersRepository = Container.Resolve<IDataRepository<Customer>>();
            var customer = customersRepository.GetByPredicate(c =>
                c.NonRoamableId == nonRoamableId)
                .SingleOrDefault();

            return customer ??
                customersRepository.Add(new Customer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    NonRoamableId = nonRoamableId
                });
        }

        private async Task<(string FirstName, string LastName, string NonRoamableId)> 
            GetCurrentUserCredentials()
        {
            string firstName = default(string);
            string lastName = default(string);

            // Get logged in local user
            var users = await User.FindAllAsync(
                UserType.LocalUser, UserAuthenticationStatus.LocallyAuthenticated);
            User currentUser = users?.FirstOrDefault();

            // Get user's account name
            firstName = (string)await currentUser.GetPropertyAsync(KnownUserProperties.AccountName);

            // If account name is null or empty, try to get user's login credentials
            if (string.IsNullOrEmpty(firstName))
            {
                firstName = (string)await currentUser.GetPropertyAsync(KnownUserProperties.FirstName);
                lastName = (string)await currentUser.GetPropertyAsync(KnownUserProperties.LastName);
            }

            // Last resort => Use default name
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                firstName = "John";
                lastName = "Doe";
            }

            return (firstName, lastName, currentUser.NonRoamableId);
        }

        private ShoppingCart GetShoppingCart(Customer customer)
        {
            var shoppingCartsRepository = Container.Resolve<IDataRepository<ShoppingCart>>();

            var shoppingCart = shoppingCartsRepository.GetByPredicate(
                sh => sh.CustomerId == customer.CustomerId).FirstOrDefault();

            if (shoppingCart == null)
                shoppingCart = shoppingCartsRepository.Add(new ShoppingCart(customer));

            return shoppingCart;
        }

        private async void RegisterBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == _taskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder
                {
                    Name = _taskName,
                    TaskEntryPoint = _taskEntryPoint
                };

                taskBuilder.SetTrigger(new TimeTrigger(30, false));
                taskBuilder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));

                var registration = taskBuilder.Register();
            }
        }

        private void InitTileTimer()
        {
            _tileTimer = new DispatcherTimer();
            _tileTimer.Interval = TimeSpan.FromSeconds(7);
            _tileTimer.Tick += TileTimer_Tick;
            _tileTimer.Start();
        }

        private void TileTimer_Tick(object sender, object e)
        {
            // Run background task, using external timer, just for demonstration
            TaskInstance.Run(null);
        }
    }
}
