using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Scoreboard
{
    public partial class NewCommand : Window
    {
        private List<string> keys = new List<string>();
        public bool isEditing = false;
        public NewCommand()
        {
            InitializeComponent();
        }

        private void SaveCommand(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).SaveCommand(tbCommandName.Text, tbCommandValue.Text, isEditing);
            ((MainWindow)Application.Current.MainWindow).IsEnabled = true;
            Close();
        }

        private void tbCommandValue_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            string keyString;

            if (IsAcceptedKey(e.Key))
            {
                string keyValue = e.Key.ToString();

                if (keyValue.ToLower().Contains("system"))
                    keyString = "Alt";
                else if (keyValue.ToLower().Contains("ctrl"))
                    keyString = "Control";
                else if (keyValue.ToLower().Contains("shift"))
                    keyString = "Shift";
                else if (int.TryParse(keyValue.Replace("D", string.Empty), out int value))
                {
                    keyString = value.ToString();
                }
                else
                    keyString = e.Key.ToString();


                if (!ListContainsPrimaryKey())
                {
                    if (IsModifier(keyString))
                    {
                        if (keys.IndexOf(keyString) < 0)
                            keys.Add(keyString);
                    }
                    else
                    {
                        keys.Add(keyString);
                    }

                    UpdateCommandValue();
                }

            }
        }

        private void tbCommandValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsAcceptedKey(e.Key))

                e.Handled = true;

            else if (e.Key == Key.Back)
            {
                e.Handled = true;
                
                if (keys.Count > 0)
                    keys.Remove(keys[keys.Count - 1]);

                UpdateCommandValue();
            }
        }

        private bool IsAcceptedKey(Key key)
        {
            int code = (int)key;
            return (code >= 34 && code <= 69) || (code >= 90 && code <= 101)
                || (code >= 116 && code <= 119) || code == 156 || code == 2;
        }

        private bool IsModifier(string key)
        {
            return key == "Alt" || key == "Shift" || key == "Control";
        }

        private bool ListContainsPrimaryKey()
        {
            foreach (string key in keys)
            {
                if (key != "Control" && key != "Alt" && key != "Shift")
                    return true;
            }

            return false;
        }

        private void UpdateCommandValue()
        {
            tbCommandValue.Text = "";

            foreach (string key in keys)
            {
                tbCommandValue.Text += key;

                if (keys.IndexOf(key) != keys.Count - 1)
                    tbCommandValue.Text += " + ";
            }

            tbCommandValue.SelectionStart = tbCommandValue.Text.Length;
        }

        private void tbCommandName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckTextboxes();
        }

        private void tbCommandValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckTextboxes();
        }

        private void CheckTextboxes()
        {
            bool hasName = !string.IsNullOrEmpty(tbCommandName.Text) && !string.IsNullOrWhiteSpace(tbCommandName.Text);
            bool hasValue = !string.IsNullOrEmpty(tbCommandValue.Text) && !string.IsNullOrWhiteSpace(tbCommandValue.Text);

            btnSaveCommand.IsEnabled = hasName && hasValue;
        }

        public void EditCommand(string name, string value)
        {
            tbCommandName.Text = name;
            tbCommandValue.Text = value;

            char[] separators = { ' ', '+' };
            string[] values = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach(string k in values)
            {
                Console.WriteLine(k);
                keys.Add(k);
            }
        }
    }
}