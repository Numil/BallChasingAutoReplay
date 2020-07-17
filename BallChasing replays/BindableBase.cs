using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace BallChasing_replays
{
    public class BindableBase : INotifyPropertyChanged
    {
        public SynchronizationContext Context { get; set; }

        public BindableBase()
        {
            Context = SynchronizationContext.Current;
        }

        #region INotifyPropertyChanged Membres

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Set the value of the given property and generates the PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="property">The reference to the property</param>
        /// <param name="value">The value to set</param>
        /// <param name="force">Force update even if the value is the same (useful when value is a CodeFluent Entity)</param>
        /// <param name="propertyName">The name of the property</param>
        protected bool SetProperty<T>(ref T property, T value, bool force = false, [CallerMemberName] string propertyName = null)
        {
            if (!force && Equals(property, value))
            {
                return false;
            }

            property = value;
            NotifyPropertyChanged(propertyName);

            return true;
        }

        #endregion

        public void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                if (Context != null)
                {
                    Context.Post(s => handler(this, new PropertyChangedEventArgs(propertyName)), null);
                }
                else
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}
