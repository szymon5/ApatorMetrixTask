using ApatorMetrixTask.Interfaces;
using ApatorMetrixTask.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace ApatorMetrixTask
{
    public partial class MainWindow : Window
    {
        public PaymentCardManagementSystemViewModel pcmsVM {  get; set; }

        public MainWindow(IRepository repository, IMessageBoxService messageBoxService)
        {
            InitializeComponent();
            PCMS.Width = 1300;
            PCMS.Height = 450;

            pcmsVM = new PaymentCardManagementSystemViewModel(repository, messageBoxService);
            pcmsVM.SelectedTabControlIndex = 0;
            pcmsVM.InitCardCommand.Execute(null);
            DataContext = pcmsVM;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tabControl) PCMS.Height = tabControl.SelectedIndex == 3 ? 900 : 450;
            pcmsVM.ResetVariablesCommand.Execute(null);
        }
    }
}