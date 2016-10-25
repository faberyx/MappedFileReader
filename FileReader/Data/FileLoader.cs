using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;

namespace FileReader.Data
{
    public class FileLoader
    {
        #region Private Properties
        private string filename;
        private Encoding encoding;
        private MemoryMappedFile file;
        private long filesize;
        List<long> posix = new List<long>();
        #endregion

        #region Public Properties

        #endregion

        #region Constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="filename">name of the file</param>
        public FileLoader(string filename)
        {
            this.encoding = this.GetEncoding(filename);
            this.filename = filename;
        }
        #endregion

        #region File Data Manager
        /// <summary>
        /// Open the file and index every position of a new line in the file stream into an array
        /// Returns the nuber of lines in the file and fills up an array with every newline position in the file
        /// </summary>
        public uint IndexFile()
        {
            // Reading buffer for the file stream ==> 6400KB
            int buffSize = 6553600,
                streamSize = 6553600;  
            
            // Number of rows counter
            long rowcount = 1;

            // Add first page to array
            posix.Add(0);

            // Init the buffer
            byte[] bArr = new byte[buffSize];


            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, streamSize, FileOptions.RandomAccess))
            {
                filesize = fs.Length;

                // Fill the buffer
                for (int i = fs.Read(bArr, 0, buffSize); i > 0; i = fs.Read(bArr, 0, buffSize))
                {
                    // Search for a newline (char 10) in the buffer
                    for (int ii = Array.IndexOf<byte>(bArr, 10, 0); ii >= 0; ii = Array.IndexOf<byte>(bArr, 10, ++ii))
                    {
                        // Only add the position for the paginated rows
                        if (rowcount % Constants.Pagination == 0 && rowcount > 0)
                        {
                            // Add the current position of the newline to the array
                            posix.Add((long)(fs.Position - i + ii + 1));

                        }
                        rowcount++;
                    }

                    // Resize the buffer for the last read to matche the exact size of the file
                    if (fs.Position + buffSize > fs.Length)
                    {
                        buffSize = (int)(fs.Length - fs.Position);
                        bArr = new byte[buffSize];
                    }
                }
            }

            // Create new MMF
            this.file = MemoryMappedFile.CreateFromFile(
               //include a readonly shared stream
               File.Open(this.filename, FileMode.Open, FileAccess.Read, FileShare.Read),
               //not mapping to a name
               null,
               //use the file's actual size
               0L,
               //read only access
               MemoryMappedFileAccess.Read,
               //not configuring security
               null,
               //adjust as needed
               HandleInheritability.None,
               //close the previously passed in stream when done
               false);

            // Return the number of lines found in the file
            return (uint)rowcount;
        }

        /// <summary>
        /// Get the page requested from the file
        /// </summary>
        /// <param name="start">Row number to start reading</param>
        /// <param name="rowcount">Number of rows to read</param>
        /// <returns>List of file rows</returns>
        public List<string> GetPage(int start, int rowcount)
        {
            try
            {
                // We are reading from the file in pages and not by row
                int s = start / Constants.Pagination;

                // Get the file offset to set the accessor
                long offset = posix[s];

                // Get the length of the rows to read
                long length = posix.Count == 1 ? filesize : (s + 1) == posix.Count ? filesize - offset : posix[s + 1] - offset;

                // Access MMV data
                using (var stream = file.CreateViewAccessor(offset, length, MemoryMappedFileAccess.Read))
                {
                    byte[] contentArray;
                    // Read from the MMF the bytes[] requested
                    if (this.encoding.EncodingName == "Unicode" && offset > 0)
                    {
                        // If unicode we need to add the BOM data at the beginning or GetString will not decode the byte array
                        contentArray = new byte[length + 2];
                        contentArray[0] = 0xff;
                        contentArray[1] = 0xfe;

                        // Read the data in the accessor
                        stream.ReadArray<byte>(1, contentArray, 2, (int)length - 1);
                    }
                    else
                    {
                        // Read the data in the accessor
                        contentArray = new byte[length];
                        stream.ReadArray<byte>(0, contentArray, 0, (int)length - 1);

                    }
                    return this.encoding.GetString(contentArray).Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList<string>();
                    //return list;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Destructor

        /// <summary>
        /// Destructor - Close the MMF
        /// </summary>
        ~FileLoader()
        {
            this.file.Dispose();
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        private Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
            {
                return Encoding.UTF7;
            }

            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
            {
                return Encoding.UTF8;
            }

            if (bom[0] == 0xff && bom[1] == 0xfe)
            {
                return Encoding.Unicode; // UTF-16LE
            }

            if (bom[0] == 0xfe && bom[1] == 0xff)
            {
                return Encoding.BigEndianUnicode; // UTF-16BE
            }

            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
            {
                return Encoding.UTF32;
            }

            return Encoding.ASCII;
        }
        #endregion
    }
}

