using AlphaChiTech.Virtualization;
using FileReader.Data;
using FileReader.DataVirtualization;
using FileReader.Virtualization;
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

        private VirtualizingObservableCollection<string> observablecollectiondata = null;
        private VirtualizingCollection dataprovider = null;


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

        public VirtualizingObservableCollection<string> ObservableCollectionData
        {
            get
            {
              
                return observablecollectiondata;
            }
            set
            {
                this.observablecollectiondata = value;
                this.RaisePropertyChanged(() => this.ObservableCollectionData);
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
        private  void LoadGrid(string filename)
        {
      
            ObservableCollectionData =
                 new VirtualizingObservableCollection<string>(
                     new PaginationManager<string>(new VirtualizingCollection(filename),pageSize: Constants.Pagination));

        }
        #endregion
    }
}