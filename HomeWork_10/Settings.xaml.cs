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
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace HomeWork_10
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void TokenChkBox_Checked(object sender, RoutedEventArgs e)
        {
            TokenPanel.Visibility = Visibility.Visible;
        }

        private void TokenChkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TokenPanel.Visibility = Visibility.Collapsed;
        }

        private void TokenButton_Click(object sender, RoutedEventArgs e)
        {
            string token = TokenText.Text;

            if(token.Count() != 46)
                {
                MessageBox.Show($"Введее не правильный токен - {token.Count()}");
                return;
            }
            using (StreamWriter sw = new StreamWriter("token.txt"))
            {
                sw.WriteLine(token);
            }
            MessageBox.Show("Токен принят");
        }

        private void ProxyButton_Click(object sender, RoutedEventArgs e)
        {
            string ip = ipTxt.Text;
            try
            {
                string[] temp = ip.Split(':');
                WebProxy wb = new WebProxy(temp[0], Convert.ToInt32(temp[1]));
                string json = JsonConvert.SerializeObject(wb);
                File.WriteAllText("proxy.txt", json);

                MessageBox.Show("Прокси задан");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }

            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TokenPanel.Visibility = Visibility.Collapsed;
            string temp = String.Empty;
           if(File.Exists("token.txt"))
            {
                using (StreamReader sr = new StreamReader("token.txt"))
                {
                    temp = sr.ReadLine();
                }
                TokenText.Text = temp;
            }
            else
            {
                TokenText.Text = "Не найден токен , введите его в данную область";
            }
                
        }
    }
}
