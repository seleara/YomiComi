using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Yomu
{
    public class Config : INotifyPropertyChanged
    {
        private double dragSpeed = 2.0f;
        public double DragSpeed
        {
            get
            {
                return dragSpeed;
            }
            set
            {
                dragSpeed = value;
                OnPropertyChanged();
            }
        }

        private static readonly Config instance = new Config();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Config Instance
        {
            get
            {
                return instance;
            }
        }

        private Config()
        {

        }
    }
}
