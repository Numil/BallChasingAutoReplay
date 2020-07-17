using BallChasing_replays.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using Ookii.Dialogs.Wpf;
using System.Threading.Tasks;

namespace BallChasing_replays.Views
{
    /// <summary>
    /// Logique d'interaction pour TokenView.xaml
    /// </summary>
    public partial class TokenView : Page
    {
        TokenViewViewModel ViewModel;
        public TokenView()
        {
            ViewModel = new TokenViewViewModel();
            DataContext = ViewModel;
            InitializeComponent();
            if(Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Rocket League\TAGame\Demos")){
                ViewModel.Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Rocket League\TAGame\Demos";
            }
        }

        private void TokenButton_Click(object sender, RoutedEventArgs e)
        {
            UploadButton.IsEnabled = false;
            Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(ViewModel.Token) && !string.IsNullOrEmpty(ViewModel.Path))
                {
                    string html = string.Empty;
                    string url = @"https://ballchasing.com/api/";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Headers.Add("Authorization", ViewModel.Token);
                    request.Timeout = 5000;
                    try
                    {
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        using (Stream stream = response.GetResponseStream())
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                NavigationService ns = NavigationService.GetNavigationService(this);
                                UploadView page = new UploadView(ViewModel.Token, ViewModel.Path);
                                ns.Navigate(page);
                            }));
                        }
                    }
                    catch (WebException err)
                    {
                        if (((HttpWebResponse)err.Response).StatusCode == HttpStatusCode.Unauthorized)
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                Notifier notifier = new Notifier(cfg =>
                                {
                                    cfg.PositionProvider = new WindowPositionProvider(
                                        parentWindow: Application.Current.MainWindow,
                                        corner: Corner.TopRight,
                                        offsetX: 10,
                                        offsetY: 10);

                                    cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                                        notificationLifetime: TimeSpan.FromSeconds(3),
                                        maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                                    cfg.Dispatcher = Application.Current.Dispatcher;
                                });

                                notifier.ShowError("Bad Token");
                            }));
                        }
                    }
                    finally
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => { UploadButton.IsEnabled = true; }));
                    }
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if(dialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    ViewModel.Path = dialog.SelectedPath;
                }
            }
        }
    }
}
