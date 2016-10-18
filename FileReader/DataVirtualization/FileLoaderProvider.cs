using System.Collections.Generic;
using System.Diagnostics;
using System;
using FileReader.Data;

namespace FileReader.DataVirtualization
{
    /// <summary>
    /// File provider for the async collection
    /// </summary>
    public class FileLoaderProvider : IItemsProvider<string>
    {
        #region Private Properties
        private FileLoader loader;
        private int rowcount;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">Name of the file to open.</param>
        public FileLoaderProvider(FileLoader loader, int rowcount)
        {
            this.rowcount = rowcount;
            this.loader = loader;
        }
        #endregion

        /// <summary>
        /// Total number of items available.
        /// </summary>
        /// <returns></returns>
        public int FetchCount()
        {
            return this.rowcount;
        }

        /// <summary>
        /// Get the right range of items
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns></returns>
        public IList<string> FetchRange(int startIndex, int count)
        {
            try
            {
#if DEBUG
                // Create new stopwatch.
                Stopwatch stopwatch = new Stopwatch();

                // Begin timing.
                stopwatch.Start();
#endif
                var result = loader.GetPage(startIndex, count);
#if DEBUG
                // Stop timing.
                stopwatch.Stop();

                // Write result.
                Debug.WriteLine("FetchRange elapsed: {0} - Start: {1} - Take: {2}", stopwatch.Elapsed, startIndex, count);
#endif

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FetchRange ERROR:" + ex.Message);
                return null;
            }
        }
    }
}
