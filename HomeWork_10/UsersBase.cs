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
    static class UsersBase
    {
        /// <summary>
        /// Лист пользователей бота
        /// </summary>
        public static ObservableCollection<TelegramUser> users;

        /// <summary>
        /// Загрузка базы пользователей
        /// </summary>
        /// <param name="path">путь до файла</param>
        static public void getUsersFromFile(string path ="users.txt")
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

        /// <summary>
        /// Метод по добавлениб сообщений пользователя
        /// </summary>
        /// <param name="user">Пользователь телеграмма</param>
        /// <param name="e">сообщение из телеграма</param>
        static public void putUsersMessage (TelegramUser user, Telegram.Bot.Args.MessageEventArgs e)
        {
            

            if (!users.Contains(user)) users.Add(user);

            users[users.IndexOf(user)].addMessage($"{e.Message.Text}");
            saveBase();
        }

        /// <summary>
        /// Сохранение сообщения
        /// </summary>
        /// <param name="e"></param>
        static public void Saver(Telegram.Bot.Args.MessageEventArgs e)
        {
            var user = new TelegramUser(e.Message.Chat.Id, e.Message.Chat.Username);
            UsersBase.putUsersMessage(user, e);
        }
        /// <summary>
        /// Сохранить базу
        /// </summary>
        static public void saveBase()
        {
            string json = JsonConvert.SerializeObject(users);
            File.WriteAllText("users.txt", json);
        }
    }
}
