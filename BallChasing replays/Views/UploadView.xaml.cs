using BallChasing_replays.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace BallChasing_replays.Views
{
    /// <summary>
    /// Logique d'interaction pour UploadView.xaml
    /// </summary>
    public partial class UploadView : Page
    {
        UploadViewViewModel ViewModel;
        FileSystemWatcher watcher;

        public UploadView(string token, string directory)
        {
            ViewModel = new UploadViewViewModel();
            DataContext = ViewModel;
            ViewModel.Token = token;
            InitializeComponent();
            Run(directory);
        }
        private void Run(string directory)
        {

            watcher = new FileSystemWatcher();
            watcher.Path = directory;

            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            watcher.NotifyFilter = NotifyFilters.LastAccess
                                    | NotifyFilters.LastWrite
                                    | NotifyFilters.FileName
                                    | NotifyFilters.DirectoryName;

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            watcher.IncludeSubdirectories = true;
            watcher.Filter= "*.replay";

            watcher.Created += OnReplayAdded;
            ViewModel.ConsoleText += "Your replays will be uploaded automatically.";
        }

        private void OnReplayAdded(object source, FileSystemEventArgs e) {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization, ViewModel.Token);
                byte[] responseArray = new byte[0];
                Task.Run(() =>
                {
                    ViewModel.ConsoleText += "\n" + "Replay to upload: '" + Path.GetFileName(e.FullPath) + "'";
                    ConsoleTextBox1.Dispatcher.Invoke(new Action(() => { ConsoleTextBox1.ScrollToEnd(); }));
                    bool isUploaded = false;
                    while (!isUploaded)
                    {
                        try
                        {
                            client.UploadFile(new Uri("https://ballchasing.com/api/v2/upload??visibility=public"), "POST", e.FullPath);
                            ViewModel.ConsoleText += "\n" + "Replay uploaded: " + Path.GetFileName(e.FullPath);
                            isUploaded = true;
                        }
                        catch (WebException err)
                        {
                            if (err.Response != null)
                            {
                                switch (((HttpWebResponse)err.Response).StatusCode)
                                {
                                    case HttpStatusCode.Conflict:
                                        ViewModel.ConsoleText += "\n" + "File already uploaded: " + Path.GetFileName(e.FullPath);
                                        isUploaded = true;
                                        break;
                                    case HttpStatusCode.InternalServerError:
                                        ViewModel.ConsoleText += "\n" + "An error has occured on Ballchasing servers";
                                        break;
                                    case HttpStatusCode.BadRequest:
                                        ViewModel.ConsoleText += "\n" + "Flo est un con.";
                                        break;
                                    case HttpStatusCode.Unauthorized:
                                        ViewModel.ConsoleText += "\n" + "Bad token.";
                                        break;
                                    default:
                                        ViewModel.ConsoleText += "\n" + "Error: "+ ((HttpWebResponse)err.Response).StatusCode;
                                        break;
                                }
                            }
                            else
                            {
                                ViewModel.ConsoleText += "\n" + err.Message + ":" + err.StackTrace + err.InnerException;
                            }
                        }
                        catch (IOException)
                        {

                        }
                        ConsoleTextBox1.Dispatcher.Invoke(new Action(() => { ConsoleTextBox1.ScrollToEnd(); }));
                    }
                    
                });
            }
            ConsoleTextBox1.Dispatcher.Invoke(new Action(() => { ConsoleTextBox1.ScrollToEnd(); }));
        }
    }
}
