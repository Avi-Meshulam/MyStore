using Microsoft.Practices.Unity;
using MyStore.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Prism.Unity.Windows;
using MyStore.BL.ViewModels;

namespace MyStore.BL
{
    public static class Utils
    {
        private static readonly IUnityContainer _container = PrismUnityApplication.Current.Container;
        private static readonly Random _rand = new Random();

        private static IDialogService _dialogService;
        public static IDialogService DialogService
        {
            get { return _dialogService ?? (_dialogService = _container.Resolve<IDialogService>()); }
        }

        private static MainViewModel _mainViewModel;
        public static MainViewModel MainViewModel
        {
            get { return _mainViewModel ?? (_mainViewModel = _container.Resolve<MainViewModel>()); }
        }

        public async static void HandleException(
            Exception ex, [CallerMemberName] string caller = null)
        {
            StringBuilder sb = new StringBuilder(ex.Message);

            while (ex.InnerException != null)
            {
                sb.Append($"\n{ex.InnerException}");
                ex = ex.InnerException;
            }

            await DialogService.ShowDialog(sb.ToString(), caller.SplitCamelCase());
        }

        public static void SetProgressRing(bool isVisible)
        {
            MainViewModel.IsProgressRingVisible = isVisible;
        }

        #region Random Utils
        public static int GetRandom(int minValue, int maxValue)
        {
            if (maxValue == int.MaxValue)
                maxValue -= 1;
            return _rand.Next(minValue, maxValue + 1);
        }

        public static int GetRandom(int maxValue)
        {
            if (maxValue == int.MaxValue)
                maxValue -= 1;
            return GetRandom(0, maxValue + 1);
        }

        public static int GetRandom()
        {
            return GetRandom(0, int.MaxValue - 1);
        }

        public static DateTime GetRandomDate(int yearsBackwards)
        {
            return DateTime.Now.Add(new TimeSpan(-_rand.Next(365, 365 * yearsBackwards), 0, 0, 0)).Date;
        }
        #endregion // Random Utils
    }
}
