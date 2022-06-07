using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Scoreboard
{
    /// <summary>
    /// Lógica de interacción para InfoMessage.xaml
    /// </summary>
    public partial class InfoMessage : Window
    {
        public InfoMessage(string title, string msg, Window owner)
        {
            InitializeComponent();
            Title = title;
            tbMessage.Text = msg;
            Owner = owner;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
