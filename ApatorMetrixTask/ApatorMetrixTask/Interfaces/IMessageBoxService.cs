using System.Windows;

namespace ApatorMetrixTask.Interfaces
{
    public interface IMessageBoxService
    {
        public MessageBoxResult ShowWithButtonAndImage(string message, string caption, MessageBoxButton mbButton, MessageBoxImage mbImage);
    }
}
