using AlphaChiTech.Virtualization;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FileReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 
        /// </summary>
        public App()
        {
            
            this.Dispatcher.UnhandledException += this.OnDispatcherUnhandledException;
        }

        /// <summary>
        /// Catch all Exceptions in the Application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Hide Load Spinner
            Messenger.Default.Send(new Model.Messages.UpdateStatusBar() {  Status = false });

            string message;

            // Trying to get inner exceptions
            if (e.Exception.InnerException != null)
            {
                if (e.Exception.InnerException.InnerException != null)
                {
                    message = e.Exception.InnerException.InnerException.Message;
                }
                else
                {
                    message = e.Exception.InnerException.Message;
                }
            }
            else
            {
                message = e.Exception.Message;
            }
     
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
