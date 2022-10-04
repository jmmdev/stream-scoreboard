using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Scoreboard
{
    public partial class Message : Window
    {

        public Message(string text, string messageType, string windowType)
        {
            InitializeComponent();

            setText(text);
            setMessageType(messageType);
            setWindowType(windowType);
        }

        public void setText(string text)
        {
            lbText.Text = text;
        }

        public void setMessageType(string type)
        {
            Image myImage = new Image();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new System.Uri(type + "-circle.png", System.UriKind.Relative);
            bi.EndInit();
            myImage.Stretch = Stretch.Fill;
            iconImg.Source = bi;
        }

        public void setWindowType(string type)
        {
            if(type == "yesno")
            {
                btnYes.Visibility = Visibility.Visible;
                btnNo.Visibility = Visibility.Visible;
                btnOk.Visibility = Visibility.Collapsed;
            }

            if(type == "ok")
            {
                btnYes.Visibility = Visibility.Collapsed;
                btnNo.Visibility = Visibility.Collapsed;
                btnOk.Visibility = Visibility.Visible;
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
