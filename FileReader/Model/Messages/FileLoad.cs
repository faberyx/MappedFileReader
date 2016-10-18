namespace FileReader.Model.Messages
{
    /// <summary>
    /// File open message -> starts loading a file
    /// </summary>
    public class FileLoad
    {
        /// <summary>
        /// Name of the file opened
        /// </summary>
        public string Filename { get; set; }
    }
}
