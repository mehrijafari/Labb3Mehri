using Labb3Mehri.Model;
using Labb3Mehri.ViewModel;
using System.Windows;

namespace Labb3Mehri
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

           

            
        }
    }
}