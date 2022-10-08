using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Scoreboard
{
    public partial class Message : Window
    {

        public Message(string text, string windowType)
        {
            InitializeComponent();

            setText(text);
            setWindowType(windowType);
        }

        public void setText(string text)
        {
            lbText.Text = text;
        }

        public void setWindowType(string type)
        {
            if (type == "wait")
            {
                textGrid.Height = 116;

                btnYes.Visibility = Visibility.Collapsed;
                btnNo.Visibility = Visibility.Collapsed;
                btnOk.Visibility = Visibility.Collapsed;
            }
            else
            {
                textGrid.Height = 80;

                if (type == "yesno")
                {
                    btnYes.Visibility = Visibility.Visible;
                    btnNo.Visibility = Visibility.Visible;
                    btnOk.Visibility = Visibility.Collapsed;
                }

                if (type == "ok")
                {
                    btnYes.Visibility = Visibility.Collapsed;
                    btnNo.Visibility = Visibility.Collapsed;
                    btnOk.Visibility = Visibility.Visible;
                }
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
