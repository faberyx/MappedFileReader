using AlphaChiTech.Virtualization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace FileReader.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Private Properties
        
        #endregion

        #region Public Properties
        public ICommand OpenFileCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            DispatcherHelper.Initialize();

         

            this.OpenFileCommand = new RelayCommand(OpenFile);

            this.CloseCommand = new RelayCommand(() => {
                System.Windows.Application.Current.Windows[0].Close();
            });
        }

        #endregion

        #region Menu Commands
        /// <summary>
        /// Add a new  file to process
        /// </summary>
        private void OpenFile()
        {
            Microsoft.Win32.OpenFileDialog openPicker = new Microsoft.Win32.OpenFileDialog();

            try
            {
                // Add file filters
                openPicker.DefaultExt = "*.*";
                openPicker.Filter = "All Files|*.*";

                // Display the OpenFileDialog by calling ShowDialog method
                bool? result = openPicker.ShowDialog();

                // Check to see if we have a result 
                if (result == true)
                {
                    Messenger.Default.Send(new Model.Messages.FileLoad { Filename = openPicker.FileName });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error opening the file: {ex.Message}");
            }
        }
        #endregion
    }
}