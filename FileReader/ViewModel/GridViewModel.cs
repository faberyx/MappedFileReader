using FileReader.Data;
using FileReader.DataVirtualization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Threading.Tasks;

namespace FileReader.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// </summary>
    public class GridViewModel : ViewModelBase
    {
        #region Private Properties
        private AsyncVirtualizingCollection<string> logrows;
        #endregion

        #region Public Properties
        /// <summary>
        /// List of all file rows to show on the grid - bound to the gridview UI
        /// </summary>
        public AsyncVirtualizingCollection<string> LogRows
        {
            get
            {
                return this.logrows;
            }

            set
            {
                this.logrows = value;
                this.RaisePropertyChanged(() => this.LogRows);
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public GridViewModel()
        {
            Messenger.Default.Register<Model.Messages.FileLoad>(this, (msg) => LoadGrid(msg.Filename));
        }
        #endregion

        #region Grid Data Loader
        private async void LoadGrid(string filename)
        {
            Messenger.Default.Send(new Model.Messages.UpdateStatusBar { Status = true });
            Messenger.Default.Send(new Model.Messages.TimerControl { Start = true });

            var loader = new FileLoader(filename);
            int rows = await Task.Factory.StartNew<int>(() =>
            {
                return (int)loader.IndexFile();
            });

            Messenger.Default.Send(new Model.Messages.UpdateStatusBar { Status = false, RowCount = rows });
            Messenger.Default.Send(new Model.Messages.TimerControl { Start = false });

            var provider = new FileLoaderProvider(loader, rows);
            this.LogRows = new AsyncVirtualizingCollection<string>(provider, Constants.Pagination, Constants.Pagetimeout);
        }
        #endregion
    }
}