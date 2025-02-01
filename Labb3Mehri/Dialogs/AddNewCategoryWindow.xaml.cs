using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Labb3Mehri.Dialogs
{
    /// <summary>
    /// Interaction logic for AddNewCategoryWindow.xaml
    /// </summary>
    public partial class AddNewCategoryWindow : Window
    {
        public AddNewCategoryWindow()
        {
            InitializeComponent();
            DataContext = (App.Current.MainWindow as MainWindow).DataContext;
        }
    }
}
