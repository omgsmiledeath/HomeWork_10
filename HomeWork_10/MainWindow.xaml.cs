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
using System.Threading;
using Microsoft.Win32;
using Telegram.Bot.Types.InputFiles;

namespace HomeWork_10
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bot bot;
        private Command command;
        public UsersBase usersBase;
        /// <summary>
        /// Конструктор главного окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            usersBase = new UsersBase();
            ListUser.ItemsSource = usersBase.GetUsers();
        }
        /// <summary>
        /// Кнопка запуска окна с настройками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Settings settings = new Settings();
            settings.Owner = this;
            settings.Show();

        }

        
        private void Load_Loaded(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Обработка при закрытии окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Closed(object sender, EventArgs e)
        {
            usersBase.saveBase();
        }

        /// <summary>
        /// Запуск бота
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void ButtonStartBot_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("token.txt"))
            {
                try
                {
                    bot = new Bot();
                    command = new Command(bot, usersBase, StatusTxt, this);
                    command.Start();
                }
                catch (Exception)
                {
                    MessageBox.Show("Введен токен не верного формата");
                }
               
            }
            else
            {
                MessageBox.Show("Токен не найден , задайте его в открывшемся окне настроек");
                Button_Click(sender, e);
            }

        }
        /// <summary>
        /// Кнопка отправки сообщений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (idBox.Text != string.Empty)
            {
                string message = txtSend.Text;
                txtSend.Text = String.Empty;

                long id = long.Parse(idBox.Text);
                bot.TelegramBot.SendTextMessageAsync(id, message);
            }
            else MessageBox.Show("Не выбран пользователь");
        }
        /// <summary>
        /// Кнопка отправки файлов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonSendFile_Click(object sender, RoutedEventArgs e)
        {
            if (idBox.Text != string.Empty)
            {
                OpenFileDialog ofd = new OpenFileDialog();

            Nullable<bool> result = ofd.ShowDialog();
            string path = ofd.FileName;
           
                if (result == true)
                {
                    using (BufferedStream bs2 = new BufferedStream(File.OpenRead(path)))
                    {
                        InputOnlineFile iof = new InputOnlineFile(bs2, new FileInfo(path).Name);
                        await bot.TelegramBot.SendDocumentAsync(chatId: long.Parse(idBox.Text),
                            document: iof,
                            caption: "Ваш файл");
                    }
                }
            }
            else MessageBox.Show("Не выбран пользователь");
        }

        /// <summary>
        /// Выход из приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Уверенны что хотите выйти?", "Выход", MessageBoxButton.YesNo);
            if(result == MessageBoxResult.Yes)
            this.Close();
        }

        /// <summary>
        /// Добавление возможности перетаскивания окна по экрану
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

