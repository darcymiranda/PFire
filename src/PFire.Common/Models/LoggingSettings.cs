namespace PFire.Common.Models
{
    public class LoggingSettings
    {
        public FileSettings File { get; set; }
    }

    public class FileSettings
    {
        public string Path { get; set; }
        public string Interval { get; set; }
    }
}
