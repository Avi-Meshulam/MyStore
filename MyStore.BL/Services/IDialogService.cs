using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MyStore.BL.Services
{
    public interface IDialogService
    {
        Task<ContentDialogResult> ShowDialog(
            string content,
            string title,
            string primaryButtonText = null,
            string secondaryButtonText = null,
            string closeButtonText = null);
    }
}
