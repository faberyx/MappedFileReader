namespace FileReader.Model.Messages
{
    /// <summary>
    /// TimerControl message -> controls the statusbar clock
    /// </summary>
    public class TimerControl
    {
        /// <summary>
        /// Name of the file opened
        /// </summary>
        public bool Start { get; set; }
    }
}
