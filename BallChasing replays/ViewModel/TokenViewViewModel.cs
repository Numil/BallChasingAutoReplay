using System;
using System.Collections.Generic;
using System.Text;

namespace BallChasing_replays.ViewModel
{
    class TokenViewViewModel : BindableBase
    {
        private string token;

        public string Token
        {
            get { return token; }
            set { SetProperty(ref token, value); }
        }

        private string path;

        public string Path
        {
            get { return path; }
            set { SetProperty(ref path, value); }
        }

    }
}
