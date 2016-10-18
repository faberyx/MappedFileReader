using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;

namespace FileReader.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// </summary>
    public class StatusBarViewModel : ViewModelBase
    {
        #region Private Properties
        private string loadprogress;
        private string rowcount;
        private string clocktime;
        private int secondsDuration;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private bool isloading;
        #endregion

        #region Public Properties
        /// <summary>
        /// File being loaded 
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this.isloading;
            }

            set
            {
                this.isloading = value;
                this.RaisePropertyChanged(() => this.IsLoading);
            }
        }

        /// <summary>
        /// Statusbar text
        /// </summary>
        public string LoadProgress
        {
            get
            {
                return this.loadprogress;
            }

            set
            {
                this.loadprogress = value;
                this.RaisePropertyChanged(() => this.LoadProgress);
            }
        }

        /// <summary>
        /// Clock Timer text
        /// </summary>
        public string ClockTime
        {
            get
            {
                return this.clocktime;
            }

            set
            {
                this.clocktime = value;
                this.RaisePropertyChanged(() => this.ClockTime);
            }
        }

        /// <summary>
        /// File Rowcount
        /// </summary>
        public string RowCount
        {
            get
            {
                return this.rowcount;
            }

            set
            {
                this.rowcount = value;
                this.RaisePropertyChanged(() => this.RowCount);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        #region Constructor
        public StatusBarViewModel()
        {
            Messenger.Default.Register<Model.Messages.UpdateStatusBar>(this, msg => UpdateStatus(msg.Status, msg.RowCount));

            Messenger.Default.Register<Model.Messages.TimerControl>(this, msg => { this.StartTimer(msg.Start);});

            // Init Clock
            this.dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            this.dispatcherTimer.Tick += new EventHandler(this.DispatcherTimer);
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

        }
        #endregion

        private void UpdateStatus(bool status, int? rowcount)
        {
            this.IsLoading = status;
            this.LoadProgress = status ? "Indexing file....." : "";

            if (rowcount.HasValue)
            {
                this.RowCount = rowcount.Value.ToString("N0");
            }
        }

        /// <summary>
        /// Starts the loading timer
        /// </summary>
        private void StartTimer(bool start)
        {
            // Control Timer event
            if (start)
            {
                this.secondsDuration = 0;
                this.dispatcherTimer.Start();
            }
            else
            {
                this.dispatcherTimer.Stop();
            }
        }

        /// <summary>
        /// Updates the timer each second
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatcherTimer(object sender, EventArgs e)
        {
            TimeSpan t = TimeSpan.FromSeconds(++this.secondsDuration);
            string loadtime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);

            // Dispatch back to the main thread
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.ClockTime = $"{loadtime}";
            });
        }

        /// <summary>
        /// Stops the loading timer
        /// </summary>
        private void StopTimer()
        {
            this.dispatcherTimer.Stop();
        }
    }
}