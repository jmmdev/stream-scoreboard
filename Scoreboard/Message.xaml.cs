using System.Collections.Generic;
using System.Windows;

namespace Scoreboard
{
    public partial class Message : Window
    {
        public Message(string text, string windowType, Dictionary<string, string> windowButtons)
        {
            InitializeComponent();

            setText(text);
            setWindowType(windowType, windowButtons);
        }

        public void setText(string text)
        {
            lbText.Text = text;
        }

        public void setWindowType(string type, Dictionary<string, string> windowButtons)
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

                    btnYes.Content = windowButtons["yesButton"];
                    btnNo.Content = windowButtons["noButton"];
                }

                if (type == "ok")
                {
                    btnYes.Visibility = Visibility.Collapsed;
                    btnNo.Visibility = Visibility.Collapsed;
                    btnOk.Visibility = Visibility.Visible;

                    btnOk.Content = windowButtons["okButton"];
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
