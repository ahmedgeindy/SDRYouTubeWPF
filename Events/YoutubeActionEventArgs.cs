using System.Windows;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Events
{
    public class YoutubeActionEventArgs : RoutedEventArgs
    {
        public ResponseHandler ResponseHandler { get; set; }
        public ErrorResponseHandler ErrorResponseHandler { get; set; }
    }
}
