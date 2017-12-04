using MyStore.BL;
using MyStore.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace MyStore.Services
{
    public class DialogService : IDialogService
    {
        public const int MAX_FRAMEWORK_ALLOWED_DIALOG_COMMANDS = 3;

        /// <summary>
        /// Displays a content dialog with up to 3 buttons
        /// </summary>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="primaryButtonText"></param>
        /// <param name="secondaryButtonText"></param>
        /// <param name="cancelButtonText"></param>
        /// <returns></returns>
        public async Task<ContentDialogResult> ShowDialog(
            string content,
            string title,
            string primaryButtonText = null, 
            string secondaryButtonText = null, 
            string closeButtonText = null)
        {
            var dialog = new ContentDialog
            {
                Content = content,
                Title = title,
                PrimaryButtonText = primaryButtonText ?? "Close",
                SecondaryButtonText = secondaryButtonText ?? string.Empty,
                CloseButtonText = closeButtonText ?? string.Empty
            };

            ContentDialogResult result;

            try
            {
                result = await dialog.ShowAsync();
            }
            catch (Exception)
            {
                result = (ContentDialogResult)await ShowMessageDialog(
                    title, content, primaryButtonText, secondaryButtonText, closeButtonText);
            }

            return result;
        }

        /// <summary>
        /// Displays a message dialog with variable number of commands (up to 3). 
        /// Returns a one based index of the selected command
        /// </summary>
        /// <param name="content"></param>
        /// <param name="title"></param>
        /// <param name="commands"></param>
        /// <returns></returns>
        private async Task<int> ShowMessageDialog(
            string content, 
            string title, 
            params string[] commands)
        {
            var dialog = new MessageDialog(content, title.SplitCamelCase());

            int counter = 1;
            foreach (var command in commands)
            {
                if (dialog.Commands.Count >= MAX_FRAMEWORK_ALLOWED_DIALOG_COMMANDS)
                    break;
                if (!string.IsNullOrWhiteSpace(command))
                    dialog.Commands.Add(new UICommand() { Id = counter++, Label = command });
            }

            if (dialog.Commands.Count == 0)
                dialog.Commands.Add(new UICommand() { Id = counter, Label = "Close" });

            IUICommand res = await dialog.ShowAsync();

            if ((int)res.Id == MAX_FRAMEWORK_ALLOWED_DIALOG_COMMANDS)
                return 0;
            else
                return (int)res.Id;
        }
    }
}
