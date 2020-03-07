using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HomeWork_10
{
    public class Command
    {
        private MainWindow w;
        public UsersBase userbase;
        private Bot bot;
        private TextBlock status;
        /// <summary>
        /// Запуск команд бота
        /// </summary>
        public async Task Start()
        {
            try
            {
                status.Text = "Пытаюсь подключится";
                status.Foreground = Brushes.Black;
                var u = await bot.TelegramBot.TestApiAsync();
     
                if (u)
                {

                    status.Text = "Запущен";
                    status.Foreground = Brushes.Green;
                    ProxyParser.SaveCurrentProxy();
                    bot.TelegramBot.OnMessage += MessageParser;
                    bot.TelegramBot.OnCallbackQuery += TypeOfFile;

                    bot.TelegramBot.StartReceiving();
                    w.gridSendMessage.Visibility = Visibility.Visible;
                    w.infoTxt1.Visibility = Visibility.Hidden;
                }
                else
                {
                   
                    status.Text = "Не верный токен";
                    status.Foreground = Brushes.Red;
                }

                
            }
            catch (Exception ex)
            {
                status.Text = "Попытка не удалась , меняю проки";
                status.Foreground = Brushes.Green;
                ProxyParser.BadProxyRemove();
                bot.setBotWithProxy();
                Start();
                return;
                                
            }


        }

        /// <summary>
        /// Метод который ожидает в сообщении ответ выбора с клавиатуры , указанного типа файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         async void TypeOfFile(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
           Console.WriteLine(e.CallbackQuery.Message.MessageId);
            await bot.TelegramBot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id,e.CallbackQuery.Message.MessageId,$"Выбор сделан");
            var mess = e.CallbackQuery.Data;
            Console.WriteLine(mess);
            switch (mess)
            {
                case "Document":
                        DirectoryInfo di = new DirectoryInfo(e.CallbackQuery.Message.Chat.Id + @"\" + "Document");
                    if (di.Exists)
                    {
                        var files = di.GetFiles();
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
                            "Выбрана отправка документов");
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Вам необходимо отправить полностью название " +
                            "файла укзанное в < >", replyMarkup: new ReplyKeyboardRemove());
                        foreach (var file in files)
                        {
                            await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, $"Имя файла - <{file.Name}>: \n" +
                                $"Размер в КБ - {(float)file.Length / 1_024}");
                        }
                        bot.TelegramBot.OnMessage -= MessageParser;
                        bot.TelegramBot.OnMessage += DocumentSender;
                    }
                    else
                    {
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
                            "Сохраненных документов не обнаруженно");

                    }
                        break;

                    case "Audio":
                    
                        di = new DirectoryInfo(e.CallbackQuery.Message.Chat.Id + @"\" + "Audio");
                    if (di.Exists)
                    {
                        var files = di.GetFiles();
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Выбрана отправка аудио");
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Вам необходимо отправить полностью название " +
                            "файла укзанное в < >");
                        foreach (var file in files)
                        {
                            await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, $"Имя файла - <{file.Name}>: \n" +
                                $"Размер в КБ - {(float)file.Length / 1_024}");
                        }
                        bot.TelegramBot.OnMessage -= MessageParser;
                        bot.TelegramBot.OnMessage += AudioSender;
                    }
                    else
                    {
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
                            "Сохраненных аудио не обнаруженно");
                    }
                    break;

                    case "Sticker":

                        di = new DirectoryInfo(e.CallbackQuery.Message.Chat.Id + @"\" + "Sticker");
                    if (di.Exists)
                    {
                        var files = di.GetFiles();
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Выбрана отправка стикеров");
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Вам необходимо отправить полностью название " +
                            "файла укзанное в < >");
                        foreach (var file in files)
                        {
                            await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, $"Имя файла - <{file.Name}>: \n" +
                                $"Размер в КБ - {(float)file.Length / 1_024}");
                        }
                        bot.TelegramBot.OnMessage -= MessageParser;
                        bot.TelegramBot.OnMessage += StickerSender;
                    }
                    else
                    {
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
                            "Сохраненных стикеров не обнаруженно");
                    }
                    break;

                    case "Location":
                    await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Выбрана отправка данных локаций");
                    di = new DirectoryInfo(e.CallbackQuery.Message.Chat.Id + @"\" + "Location");
                    if (di.Exists)
                    {
                        var files = di.GetFiles();
                        foreach (var file in files)
                        {
                            var loc = DeserializeLocation(file.FullName);
                            await bot.TelegramBot.SendLocationAsync(e.CallbackQuery.Message.Chat.Id, loc.latitude, loc.laptitude);
                        }
                    }
                    else
                    {
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
                            "Сохраненных локаций не обнаруженно");
                    }
                    break;
                    case "Photo":
                        di = new DirectoryInfo(e.CallbackQuery.Message.Chat.Id + @"\" + "Photo");
                    if (di.Exists)
                    {
                        var files = di.GetFiles();
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Выбрана отправка фото");
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Вам необходимо отправить полностью название " +
                            "файла укзанное в < >");
                        foreach (var file in files)
                        {
                            await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, $"Имя файла - <{file.Name}>: \n" +
                                $"Размер в КБ - {(float)file.Length / 1_024}");
                        }
                        bot.TelegramBot.OnMessage -= MessageParser;
                        bot.TelegramBot.OnMessage += PhotoSender;
                    }
                    else
                    {
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
                            "Сохраненных фото не обнаруженно");
                    }
                    break;
                    case "Voice":
                        di = new DirectoryInfo(e.CallbackQuery.Message.Chat.Id + @"\" + "Voice");
                    if (di.Exists)
                    {
                       var files = di.GetFiles();
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Выбрана отправка голосового файла");
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Вам необходимо отправить полностью название " +
                            "файла укзанное в < >");
                        foreach (var file in files)
                        {
                            await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, $"Имя файла - <{file.Name}>: \n" +
                                $"Размер в КБ - {(float)file.Length / 1_024}");
                        }
                        bot.TelegramBot.OnMessage -= MessageParser;
                        bot.TelegramBot.OnMessage += VoiceSender;
                    }
                    else
                    {
                        await bot.TelegramBot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
                            "Сохраненных голосовых сообщений не обнаруженно");
                    }
                    break;
            }
  
        }
        /// <summary>
        /// ДЕсериализация файла с данными локации
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        (float latitude,float laptitude) DeserializeLocation(string path)
        {
            string json = String.Empty;
            using (StreamReader sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<ValueTuple<float, float>>(json);
        }

        /// <summary>
        /// Метод который ожидает название  файла в сообщении и отправляет указанный файл пользователю
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void DocumentSender (object sender,Telegram.Bot.Args.MessageEventArgs e)
        {
            var mess = e.Message.Text;
            string path = CreaterPath(e.Message.Chat.Id, mess, MessageType.Document);
            userbase.Saver(e,w);
            if (File.Exists(path))
            {
                Send(path, e.Message.Chat.Id, MessageType.Document);
                bot.TelegramBot.OnMessage -= DocumentSender;
                bot.TelegramBot.OnMessage += MessageParser;
            }
            else await bot.TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Вы указали не верное имя файла, повторите ввод");
        }

        /// <summary>
        /// Метод который ожидает название аудио файла в сообщении и отправляет указанный файл пользователю
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         async void AudioSender(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var mess = e.Message.Text;
            string path = CreaterPath(e.Message.Chat.Id, mess, MessageType.Audio);
            userbase.Saver(e,w);
            if (File.Exists(path))
            {
                Send(path, e.Message.Chat.Id, MessageType.Audio);
                bot.TelegramBot.OnMessage -= AudioSender;
                bot.TelegramBot.OnMessage += MessageParser;
             }
            else await bot.TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Вы указали не верное имя файла, повторите ввод");
    }

        /// <summary>
        /// Метод который ожидает название стикера в сообщении и отправляет указанный файл пользователю
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void StickerSender(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var mess = e.Message.Text;
            string path = CreaterPath(e.Message.Chat.Id, mess, MessageType.Sticker);
            userbase.Saver(e,w);
            if (File.Exists(path))
            {
                Send(path, e.Message.Chat.Id, MessageType.Sticker);
                bot.TelegramBot.OnMessage -= StickerSender;
                bot.TelegramBot.OnMessage += MessageParser;
            }
            else await bot.TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Вы указали не верное имя файла, повторите ввод");
        }

        /// <summary>
        /// Метод который ожидает название голосового файла в сообщении и отправляет указанный файл пользователю
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         async void VoiceSender(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var mess = e.Message.Text;
            string path = CreaterPath(e.Message.Chat.Id, mess, MessageType.Voice);
            userbase.Saver(e,w);
            if (File.Exists(path))
            {
                Send(path, e.Message.Chat.Id, MessageType.Voice);
                bot.TelegramBot.OnMessage -= VoiceSender;
                bot.TelegramBot.OnMessage += MessageParser;
            }
            else await bot.TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Вы указали не верное имя файла, повторите ввод");
        }

        /// <summary>
        /// Метод который ожидает название файла в сообщении и отправляет указанный файл пользователю
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void PhotoSender(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var mess = e.Message.Text;
            string path = CreaterPath(e.Message.Chat.Id, mess, MessageType.Photo);
            userbase.Saver(e,w);
            if (File.Exists(path))
            {
                Send(path, e.Message.Chat.Id, MessageType.Photo);
                bot.TelegramBot.OnMessage -= PhotoSender;
                bot.TelegramBot.OnMessage += MessageParser;
            }
            else await bot.TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Вы указали не верное имя файла, повторите ввод");
            
        }


        /// <summary>
        /// Метод для отправки клавиатуры выбора типа файлов для отправки пользователю
        /// </summary>
        /// <param name="id">ID пользователя в телеграмме</param>
        async void GetType(long id)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                                {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("Document", $"{MessageType.Document}"),
                            InlineKeyboardButton.WithCallbackData("Photo", $"{MessageType.Photo}"),
                            InlineKeyboardButton.WithCallbackData("Audio", $"{MessageType.Audio}"),
                            InlineKeyboardButton.WithCallbackData("Stiker", $"{MessageType.Sticker}"),
                            InlineKeyboardButton.WithCallbackData("Location", $"{MessageType.Location}"),
                            InlineKeyboardButton.WithCallbackData("Voice", $"{MessageType.Voice}")
                        }
                    });
            try
            {
                await bot.TelegramBot.SendTextMessageAsync(id, "Выберите нужный тип файла:", replyMarkup: inlineKeyboard);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                GetType(id);
                return;
            }
            
        }

        /// <summary>
        /// Метод вызывающий конкретный обработчик для сообщения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">сообщений из телеграма</param>
        async void MessageParser(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if(e.Message.Text=="/start")
            {
                await bot.TelegramBot.SendTextMessageAsync(e.Message.Chat.Id,
                    $"Добро пожаловать в бот файловое облако. Для описания бота /info ");
                return;
            }
            if (e.Message.Text=="/info")
            {
                await bot.TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, 
                    $"Вы можете отправлять : файлы , фото , аудио файлы, данные локации,голосовые сообщения и стикеры. Чтоб получить файлы обратно напишите команду /export ");
            }

            switch (e.Message.Type)
            {
                case MessageType.Text:
                    MessageSeaker(e);
                    break;
                case MessageType.Photo:
                    PhotoSeaking(e);
                    break;
                case MessageType.Document:
                    FileSeaking(e);
                    break;
                case MessageType.Audio:
                    AudioSeaking(e);
                    break;
                case MessageType.Sticker:
                    StikerSeaking(e);
                    break;
                case MessageType.Location:
                    LocationSeaker(e);
                    break;
                case MessageType.Voice:
                    VoiceSeaker(e);
                    break;
            }

        }

        /// <summary>
        /// Метод обрабатывающий сообщение из телеграма на предмет голосовых сообщений
        /// </summary>
        /// <param name="e">входящее сообщение из телеграма</param>
        void VoiceSeaker(Telegram.Bot.Args.MessageEventArgs e)
        {
            Console.WriteLine(e.Message.Voice.FileUniqueId);
            string path = CreaterPath(e.Message.Chat.Id, e.Message.Voice.FileUniqueId, e.Message.Type);
            DownLoad(e.Message.Voice.FileId, path);  
        }


        /// <summary>
        /// Метод обрабатывающий сообщение из телеграма на предмет данных локации
        /// </summary>
        /// <param name="e">входящее сообщение из телеграма</param>
        void LocationSeaker(Telegram.Bot.Args.MessageEventArgs e)
        {
           
            var loc = (Latitude:e.Message.Location.Latitude, Longitude: e.Message.Location.Longitude);
            SaveSerializeFile(JsonConvert.SerializeObject(loc),
                $"{ e.Message.MessageId}",
                e.Message.Chat.Id,
                e.Message.Type);
        }

        /// <summary>
        /// Метод для сохранения сериализованных не типичных данных
        /// </summary>
        /// <param name="json">Сериализованные в JSON данные</param>
        /// <param name="path">название файла</param>
        /// <param name="id">id пользователя</param>
        /// <param name="type">тип данных для сохранения в отдельной папке</param>
         void SaveSerializeFile(string json,string path,long id,MessageType type)
        {
            string fullpath = id + @"\" + type + @"\" ;
            FileInfo fi = new FileInfo(fullpath+path);
            createrFile(id, path, type);
            using (StreamWriter sw = fi.CreateText())
            {
                sw.WriteLine(json);
            }
        }

        /// <summary>
        /// Метод обрабатывающий сообщение из телеграма
        /// </summary>
        /// <param name="e">входящее сообщение из телеграма</param>
        void MessageSeaker(Telegram.Bot.Args.MessageEventArgs e)
        {
           
  

                var user = new TelegramUser(e.Message.Chat.Id, e.Message.Chat.FirstName);
                userbase.putUsersMessage(user, e,w);

            
            if (e.Message.Text == @"/export")
            {

                GetType(e.Message.Chat.Id);
            }

        }


        /// <summary>
        /// Метод обрабатывающий сообщение из телеграма на предмет аудио файлов
        /// </summary>
        /// <param name="e">входящее сообщение из телеграма</param>
        async void AudioSeaking(Telegram.Bot.Args.MessageEventArgs e)
        {

            string path = CreaterPath(e.Message.Chat.Id, e.Message.Audio.FileUniqueId, e.Message.Type);
            DownLoad(e.Message.Audio.FileId,
                   path);
            await bot.TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Ваш аудио файл принят");
             
        }
        /// <summary>
        /// Метод обрабатывающий сообщение из телеграма стикеров
        /// </summary>
        /// <param name="e">входящее сообщение из телеграма</param>
         async void StikerSeaking(Telegram.Bot.Args.MessageEventArgs e)
        {
            
            string path = CreaterPath(e.Message.Chat.Id, e.Message.Sticker.FileUniqueId+".webp", e.Message.Type);
            DownLoad(e.Message.Sticker.FileId,
                   path);
            await bot.TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Ваш стикер принят");
        }
        /// <summary>
        /// Метод обрабатывающий сообщение из телеграма на предмет файлов
        /// </summary>
        /// <param name="e">входящее сообщение из телеграма</param>
         async void FileSeaking(Telegram.Bot.Args.MessageEventArgs e)
        {

            string path = CreaterPath(e.Message.Chat.Id, e.Message.Document.FileName, e.Message.Type);
                DownLoad(e.Message.Document.FileId,
                  path);
            await bot.TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Ваш файл принят");
        }

        /// <summary>
        /// Метод обрабатывающий сообщение из телеграма на предмет фото
        /// </summary>
        /// <param name="e">входящее сообщение из телеграма</param>
         async void PhotoSeaking (Telegram.Bot.Args.MessageEventArgs e)
        {
            var type = e.Message.Type;
            var photo = e.Message.Photo;

            DownLoad(photo[2].FileId,
              CreaterPath(e.Message.Chat.Id,
              "ф" + photo[2].FileSize + ".jpg",
              type));
            await bot.TelegramBot.SendTextMessageAsync(e.Message.Chat.Id, "Ваше фото принято");
        }



        /// <summary>
        /// Метод закгружающий файлы 
        /// </summary>
        /// <param name="field">Уникальный id файла</param>
        /// <param name="path">конечный путь куда скачается файл</param>
         async void DownLoad(string field, string path)
        {
            
                var file = await bot.TelegramBot.GetFileAsync(field);

                try
                {
                    using (BufferedStream bs = new BufferedStream(new FileStream(path, FileMode.Create)))
                        await bot.TelegramBot.DownloadFileAsync(file.FilePath, bs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                 DownLoad(field, "_"+path );
                    return;
                }
                Console.WriteLine($"Докачал файл {path}");

        }

        /// <summary>
        /// Метод отправки файлов и тд , пользователю
        /// </summary>
        /// <param name="path">путь до файла</param>
        /// <param name="id">ID телеграм пользователя</param>
        /// <param name="type">тип файла</param>
        async void Send(string path, long id, Telegram.Bot.Types.Enums.MessageType type)
        {

            await bot.TelegramBot.SendTextMessageAsync(id, "Начата отправка файла , процесс может занять какое то время в зависимости от объема файла.");
            
            using (BufferedStream bs2 = new BufferedStream(File.OpenRead(path)))
            {
                InputOnlineFile iof = new InputOnlineFile(bs2, new FileInfo(path).Name);
                switch (type)
                {
                    case MessageType.Audio:
                        await bot.TelegramBot.SendAudioAsync(
                            chatId: id,
                            audio: iof,
                            caption: "Ваш файл"
                            );
                        break;
                    case MessageType.Voice:
                        await bot.TelegramBot.SendVoiceAsync(
                            chatId: id,
                            voice: iof,
                            caption: "Ваш файл"
                            );
                        break;
                    case MessageType.Document:
                        await bot.TelegramBot.SendDocumentAsync(
                            chatId: id,
                            document: iof,
                            caption: "Ваш файл"
                            );
                        break;
                    case MessageType.Photo:
                        await bot.TelegramBot.SendPhotoAsync(
                           chatId: id,
                           photo: iof,
                           caption: "Ваш файл"
                           );
                        break;
                    case MessageType.Sticker:
                        await bot.TelegramBot.SendStickerAsync(
                           chatId: id,
                           sticker: iof
                           );
                        break;
                }

            }
        }
        /// <summary>
        /// Метод для создания файлов в папках у разных пользователей сортированные по типу отправляемых файлов
        /// </summary>
        /// <param name="id">ID пользователя телеграм</param>
        /// <param name="path">путь до файла</param>
        /// <param name="type">тип файла</param>
         void createrFile(long id , string path,Telegram.Bot.Types.Enums.MessageType type)
        {
            FileInfo fi = new FileInfo(id + @"\" + type + @"\" + path);

            if (!Directory.Exists(id + @"\" + type + @"\" + path))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }
        }
        /// <summary>
        /// Метод для создания пути для файлов 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
         string CreaterPath(long id, string path, Telegram.Bot.Types.Enums.MessageType type)
        {
            
            string result = id + @"\" + type + @"\" + path;
            path = path.Trim(new char[] { ' ', '<', '>' });
            Console.WriteLine(result);
            result = result.Trim(new char[] { ' ', '<', '>' });
            Console.WriteLine(result);
            if (!File.Exists(path)) createrFile(id, path, type);
            return result;
        }

        public Command (Bot bot,UsersBase usersBase,TextBlock textBlock,MainWindow w)
        {
            this.bot = bot;
            this.status = textBlock;
            this.w = w;
            this.userbase = usersBase;
        }
    }
}
