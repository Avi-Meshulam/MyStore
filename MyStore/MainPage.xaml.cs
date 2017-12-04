using MyStore.BL.ViewModels;
using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Background;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MyStore
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public void SetFrame(Frame frame)
        {
            frameHost.Content = frame;
            frame.Navigating += (s, e) => MainViewModel.IsProgressRingVisible = true;
            frame.Navigated += (s, e) =>
            {
                MainViewModel.IsProgressRingVisible = false;
                IMenuItem menuItem = frame.Content as IMenuItem;
                if (menuItem != null)
                    MainViewModel.SelectedMenuIndex = menuItem.MenuIndex;
            };
        }

        public MainViewModel MainViewModel => DataContext as MainViewModel;
    }
}
