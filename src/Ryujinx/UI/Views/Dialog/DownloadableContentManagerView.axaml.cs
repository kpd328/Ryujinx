using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using FluentAvalonia.UI.Controls;
using Ryujinx.Ava.Common.Locale;
using Ryujinx.Ava.Common.Models;
using Ryujinx.Ava.UI.ViewModels;
using Ryujinx.Ava.Systems.AppLibrary;
using Ryujinx.Ava.UI.Controls;
using Ryujinx.Common.Helper;
using System.Threading.Tasks;

namespace Ryujinx.Ava.UI.Views.Dialog
{
    public partial class DownloadableContentManagerView : RyujinxControl<DownloadableContentManagerViewModel>
    {
        public DownloadableContentManagerView()
        {
            InitializeComponent();
        }

        public static async Task Show(ApplicationLibrary applicationLibrary, ApplicationData applicationData)
        {
            ContentDialog contentDialog = new()
            {
                PrimaryButtonText = string.Empty,
                SecondaryButtonText = string.Empty,
                CloseButtonText = string.Empty,
                Title = string.Format(LocaleManager.Instance[LocaleKeys.DlcWindowTitle], applicationData.Name, applicationData.IdBaseString),
                Content = new DownloadableContentManagerView
                {
                    ViewModel = new DownloadableContentManagerViewModel(applicationLibrary, applicationData)
                }
            };

            Style bottomBorder = new(x => x.OfType<Grid>().Name("DialogSpace").Child().OfType<Border>());
            bottomBorder.Setters.Add(new Setter(IsVisibleProperty, false));

            contentDialog.Styles.Add(bottomBorder);

            await contentDialog.ShowAsync();
        }

        private void SaveAndClose(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.Save();
            ((ContentDialog)Parent).Hide();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            ((ContentDialog)Parent).Hide();
        }

        private void RemoveDLC(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: DownloadableContentModel dlc })
            {
                ViewModel.Remove(dlc);
            }
        }

        private void OpenLocation(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: DownloadableContentModel dlc })
            {
                OpenHelper.LocateFile(dlc.ContainerPath);
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (object content in e.AddedItems)
            {
                if (content is DownloadableContentModel model)
                {
                    ViewModel.Enable(model);
                }
            }

            foreach (object content in e.RemovedItems)
            {
                if (content is DownloadableContentModel model)
                {
                    ViewModel.Disable(model);
                }
            }
        }
    }
}
