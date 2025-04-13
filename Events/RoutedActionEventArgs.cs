namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Events
{
    public class RoutedActionEventArgs : YoutubeActionEventArgs
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
    }
}
