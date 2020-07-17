using System;
using System.Collections.Generic;
using System.Text;

namespace BallChasing_replays.ViewModel
{
    class UploadViewViewModel : BindableBase
    {
        private string uploadedFile = "Upload";
         
        public string UploadedFile
        {
            get { return uploadedFile; }
            set { SetProperty(ref uploadedFile, value); }
        }

        private string consoleText;

        public string ConsoleText
        {
            get { return consoleText; }
            set { SetProperty(ref consoleText, value); }
        }

        private string token;

        public string Token
        {
            get { return token; }
            set { SetProperty(ref token, value); }
        }

    }
}
