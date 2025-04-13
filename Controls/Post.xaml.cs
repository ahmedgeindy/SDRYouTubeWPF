using Genesyslab.Desktop.Modules.YoutubeWorkItem.Events;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls
{
    /// <summary>
    /// Interaction logic for Post.xaml
    /// </summary>
    public partial class Post : UserControl
    {

        public Post()
        {
            InitializeComponent();
        }

        public event ReturnBoolEventHandler CommentClick;

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (CommentClick != null)
            {
                var vm = this.DataContext as PostViewModel;
                if (vm != null)
                {
                    CommentClick(this, new RoutedActionEventArgs() { ID = vm.Id });
                }
            }
        }
    }
}
