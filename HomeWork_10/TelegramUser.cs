using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork_10
{
    public class TelegramUser : INotifyPropertyChanged,IEquatable <TelegramUser>
    {
        /// <summary>
        /// Коснтруктор класса пользователь телеграмм
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <param name="name">Имя иди Nick</param>
        public TelegramUser (long id,string name)
        {
            this.id = id;
            this.name = name;
            Messages = new ObservableCollection<string>();

        }
        private long id;
        /// <summary>
        /// Свойство по получению и установке значения ID для пользователя
        /// </summary>
        public long Id
        {
            get { return this.id; }
            set { id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Id)));
            }
        }

        private string name;
        /// <summary>
        /// Свойство по получению и установке значения Name для пользователя
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set {
                this.name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Name)));
            }
        }

        public ObservableCollection<string> Messages
        {
            get;set;
        }

    

       

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Добавление сообщения в список
        /// </summary>
        /// <param name="msg"></param>
        public void addMessage(string msg) {
            Messages.Insert(0,msg);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Messages)));
        }



        /// <summary>
        /// Метод для сравнения пользователей
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Equals(TelegramUser user) => user.Id == this.id;

        public override string ToString()
        {
            return this.id+" " + this.Name + " ";
        }
    }
}
