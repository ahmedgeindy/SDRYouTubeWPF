namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Events
{
    public delegate bool ReturnBoolEventHandler(object sender, RoutedActionEventArgs args);

    public delegate bool SendEventHandler(object sender, SendEventArgs args);

    public delegate void ResponseHandler();
    public delegate void ErrorResponseHandler(string errorMsg);
}
