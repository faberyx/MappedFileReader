namespace FileReader.Model.Messages
{
    /// <summary>
    /// Message class to update the status bar
    /// </summary>
    public class UpdateStatusBar
    {
        /// <summary>
        /// Statusbar status
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Number of rows in the file
        /// </summary>
        public int? RowCount { get; set; }
    }
}
