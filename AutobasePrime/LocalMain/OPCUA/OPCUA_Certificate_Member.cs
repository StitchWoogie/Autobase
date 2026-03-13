using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalMain
{
    public class OPCUA_Certificate_Member : INotifyPropertyChanged
    {
        private string _sname;
        public string sName
        {
            get => _sname;
            set { _sname = value; OnPropertyChanged(nameof(sName)); }
        }

        public string sDirectory = "";

        private string _sfile;

        public string sFile
        {
            get => _sfile;
            set { _sfile = value; OnPropertyChanged(nameof(sFile)); }
        }



        private string _svalue;

        public string sValue
        {
            get => _svalue;
            set { _svalue = value; OnPropertyChanged(nameof(sValue)); }
        }

        private Brush _tColor;
        public Brush tColor
        {
            get => _tColor;
            set
            {
                if (_tColor != value)
                {
                    _tColor = value;
                    OnPropertyChanged(nameof(tColor));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public OPCUA_Certificate_Member(string _name, string _file, string _value)
        {
            this.sDirectory = _file;
            this.sName = _name;
            this.sFile = Path.GetFileName(_file);
            this.sValue = _value;

            if (_value.Contains(string.Format("Trust")))
            {
                this.tColor = Brushes.Lime;
            }
            else if (_value.Contains(string.Format("Reject")))
            {
                this.tColor = Brushes.Red;
            }
            else
            {
                this.tColor = Brushes.Black;
            }
        }
    }
}
