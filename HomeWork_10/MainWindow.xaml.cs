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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace HomeWork_10
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bot bot;
        private Command command;
        public MainWindow()
        {
            InitializeComponent();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            UsersBase.getUsersFromFile();
            ListUser.ItemsSource = UsersBase.users;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string token = tokenTxt.Text;
            using (StreamWriter sw = new StreamWriter("token.txt"))
            {
                sw.WriteLine(token);
            } 
        }

        private void Load_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void Load_Closed(object sender, EventArgs e)
        {
            UsersBase.saveBase();
        }

        private void ChkBoxStartBot_Checked(object sender, RoutedEventArgs e)
        {
            bot = new Bot();
            command = new Command(bot,StatusTxt,this);
            command.Start();
        }


    }
}
