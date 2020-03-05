using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Collections.ObjectModel;

namespace HomeWork_10
{
     public class UsersBase
    {
        public UsersBase()
        {
            getUsersFromFile();
        }
        /// <summary>
        /// Лист пользователей бота
        /// </summary>
        private ObservableCollection<TelegramUser> users;

        /// <summary>
        /// Загрузка базы пользователей
        /// </summary>
        /// <param name="path">путь до файла</param>
         public void getUsersFromFile(string path ="users.txt")
        {
            if (!File.Exists(path))
            {
               FileStream fs = File.Create(path);
                fs.Close();
            }
            string json1 = File.ReadAllText(path);
            users = JsonConvert.DeserializeObject<ObservableCollection<TelegramUser>>(json1);
            if (users == null) users = new ObservableCollection<TelegramUser>();
        }

        public ObservableCollection<TelegramUser> GetUsers()
        {
            return users;
        }
        /// <summary>
        /// Метод по добавлениб сообщений пользователя
        /// </summary>
        /// <param name="user">Пользователь телеграмма</param>
        /// <param name="e">сообщение из телеграма</param>
          public void putUsersMessage (TelegramUser user, Telegram.Bot.Args.MessageEventArgs e,MainWindow w)
        {

            w.Dispatcher.Invoke(() =>
            {
                if (!users.Contains(user)) users.Add(user);

                users[users.IndexOf(user)].Name = e.Message.Chat.FirstName;
                users[users.IndexOf(user)].addMessage($"{e.Message.Date:g} - {e.Message.Text}");
                var temp = users[users.IndexOf(user)];
                users.RemoveAt(users.IndexOf(user));
                users.Insert(0,temp);
            });
            saveBase();

        }

        /// <summary>
        /// Сохранение сообщения
        /// </summary>
        /// <param name="e"></param>
         public void Saver(Telegram.Bot.Args.MessageEventArgs e,MainWindow w)
        {
            var user = new TelegramUser(e.Message.Chat.Id, e.Message.Chat.Username);
            putUsersMessage(user, e,w);
        }
        /// <summary>
        /// Сохранить базу
        /// </summary>
         public void saveBase()
        {
            string json = JsonConvert.SerializeObject(users);
            File.WriteAllText("users.txt", json);
        }
    }
}
