using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphaChiTech.Virtualization;
using FileReader.Data;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace FileReader.Virtualization
{
    public class VirtualizingCollection : IPagedSourceProviderAsync<string>
    {
        private FileLoader loader;

        public VirtualizingCollection(string filename)
        {
            loader = new FileLoader(filename);
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Task<int> GetCountAsync()
        {
            Messenger.Default.Send(new Model.Messages.UpdateStatusBar { Status = true });
            Messenger.Default.Send(new Model.Messages.TimerControl { Start = true });

            return Task.Factory.StartNew<int>(() =>
            {
                int result = (int)loader.IndexFile();

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Messenger.Default.Send(new Model.Messages.UpdateStatusBar { Status = false, RowCount = result });
                    Messenger.Default.Send(new Model.Messages.TimerControl { Start = false });
                });

                return result;
            });
    }

    public PagedSourceItemsPacket<string> GetItemsAt(int pageoffset, int count, bool usePlaceholder)
    {
        throw new NotImplementedException();
    }

    public Task<PagedSourceItemsPacket<string>> GetItemsAtAsync(int pageoffset, int count, bool usePlaceholder)
    {

        return Task.Factory.StartNew<PagedSourceItemsPacket<string>>(()=> {


            var item = new PagedSourceItemsPacket<string>();

            item.LoadedAt = DateTime.Now;

#if DEBUG
            // Create new stopwatch.
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing.
            stopwatch.Start();
#endif
            item.Items = loader.GetPage(pageoffset, count);
#if DEBUG
            // Stop timing.
            stopwatch.Stop();

            // Write result.
            Debug.WriteLine("FetchRange elapsed: {0} - Start: {1} - Take: {2}", stopwatch.Elapsed, pageoffset, count);
#endif

            return item;
        });

    }

    public string GetPlaceHolder(int index, int page, int offset)
    {
        return "test";
    }

    public int IndexOf(string item)
    {
        throw new NotImplementedException();
    }

    public Task<int> IndexOfAsync(string item)
    {
        throw new NotImplementedException();
    }

    public void OnReset(int count)
    {
        throw new NotImplementedException();
    }
}
}
