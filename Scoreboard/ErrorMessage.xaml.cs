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

namespace Scoreboard
{
    /// <summary>
    /// Lógica de interacción para ErrorMessage.xaml
    /// </summary>
    public partial class ErrorMessage : Window
    {
        public ErrorMessage(string msg, Window owner)
        {
            InitializeComponent();
            tbMessage.Text = msg;
            Owner = owner;
        }

    private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ((MainWindow)Owner).EnableSubmitUrl();
        }
    }
}
