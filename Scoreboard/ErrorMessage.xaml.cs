using System;
using System.Windows;

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
            DialogResult = true;
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ((MainWindow)Owner).EnableSubmitUrl();
            ((MainWindow)Owner).EnableOBSConnection();
        }
    }
}
