using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using System.IO;
using System.Windows;

namespace HomeWork_10
{
    public class Bot
    {
        /// <summary>
        /// экземпляр телеграм бота
        /// </summary>
        public TelegramBotClient TelegramBot;

        public Bot()
        {
            Start();
        }

        /// <summary>
        /// Метод по запуску функционирования бота
        /// </summary>
        public void Start()
        {
            ProxyParser.LoadProxy();
            this.setBotWithProxy();
        }

        /// <summary>
        /// Задание настроек боту с использованием прокси
        /// </summary>
        public void setBotWithProxy()
        {
            Console.WriteLine("Создаем бота");
            var httpCliendHandler = new HttpClientHandler() { Proxy = ProxyParser.ProxyList[0] };
            HttpClient hc = new HttpClient(httpCliendHandler);

                TelegramBot = new TelegramBotClient(getToken(), hc);
            
            
                
           
            
        }

        /// <summary>
        /// Задание настроек боту без прокси
        /// </summary>
        public  void setBot()
        {
            TelegramBot = new TelegramBotClient(getToken());
        }
        /// <summary>
        /// Получение значения токен ключа
        /// </summary>
        /// <returns>токен</returns>
        private  string getToken()
        {
            using (StreamReader sr = new StreamReader("token.txt"))
            {
               return sr.ReadLine();
            }
 
        }
    }
}
