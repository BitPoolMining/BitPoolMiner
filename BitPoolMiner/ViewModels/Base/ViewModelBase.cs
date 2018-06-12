using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace BitPoolMiner.ViewModels.Base
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelBase()
        {
            // Create a new instance of the Notifier
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(6));

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.TopMost = false;
                cfg.DisplayOptions.Width = 250;
            });

            // Clear any left over messages
            notifier.ClearMessages();
        }
        
        /// <summary>
        /// View unload
        /// </summary>
        public void OnUnloaded()
        {
            // Dispose
            notifier.Dispose();
        }

        #endregion
        
        #region Notifier

        public readonly Notifier notifier;

        /// <summary>
        /// Show info message popup
        /// </summary>
        /// <param name="message"></param>
        public void ShowInformation(string message)
        {
            try
            {
                notifier.ShowInformation(message);
            }      
            catch
            { 
                // eat it
            }
        }

        /// <summary>
        /// Show success message popup
        /// </summary>
        /// <param name="message"></param>
        public void ShowSuccess(string message)
        {
            try
            {
                notifier.ShowSuccess(message);
            }
            catch
            {
                // eat it
            }
        }

        /// <summary>
        /// Clear all messages
        /// </summary>
        /// <param name="msg"></param>
        internal void ClearMessages(string msg)
        {
            try
            {
                notifier.ClearMessages(msg);
            }
            catch
            {
                // eat it
            }
        }

        /// <summary>
        /// Show error message popup
        /// </summary>
        /// <param name="message"></param>
        public void ShowError(string message)
        {
            try
            {
                notifier.ShowError(message);
            }
            catch
            {
                // eat it
            }
        }

        #endregion

        #region Property Changed Event Handler

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
