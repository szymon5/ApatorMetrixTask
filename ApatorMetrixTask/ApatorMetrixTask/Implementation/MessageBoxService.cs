using ApatorMetrixTask.Interfaces;
using System.Windows;

namespace ApatorMetrixTask.Implementation
{
    public class MessageBoxService : IMessageBoxService
    {
        public MessageBoxResult ShowWithButtonAndImage(string message, string caption, MessageBoxButton mbButton, MessageBoxImage mbImage) =>
            MessageBox.Show(message, caption, mbButton, mbImage);
    }
}
