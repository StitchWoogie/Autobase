using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using System.Threading.Tasks;

using System.Collections.Generic;
using System;
using Avalonia.Threading;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Interactivity;
using AutobaseApp.WinformMethod;
using Avalonia.Media.TextFormatting;


namespace LocalMain
{
    public partial class FormMessageBox : Window
    {
        public enum Icons
        {
            None,
            Battery,
            Database,
            Error,
            Folder,
            Forbidden,
            Info,
            Plus,
            Question,
            Setting,
            SpeakerLess,
            SpeakerMore,
            Stop,
            Stopwatch,
            Success,
            Warning,
            Wifi,
            Lock,
        }

        

       
        public Icons icons = Icons.None;
        public MessageBoxButtons buttons = MessageBoxButtons.OK;
        public DialogResult result = DialogResult.None;

        public string sTitle = "Title";
        public string sMessage = "Message";
        public FontFamily Ffont;

        private Window parent_window;

        public FormMessageBox()
        {
            InitializeComponent();
            this.Loaded += Page_Loaded;
        }

        public FormMessageBox(Window parent, string msg, string header, Icons icon , MessageBoxButtons button, FontFamily font)
        {
            InitializeComponent();
            if (msg is not null)
            {
                if (msg.Length > 0)
                    sMessage = msg;
            }
            if( header is not null)
            {
                if (header.Length > 0)
                    sTitle = header;
            }
            
            icons = icon;
            buttons = button;
            if (Ffont is not null)
            {
                Ffont = font;
            }
            else Ffont = FontFamily.Default;
            parent_window = parent;

            this.Loaded += Page_Loaded;
            this.Unloaded += Page_Unloaded;
            
        }

        private void Page_Loaded(object? sender, EventArgs e)
        {
            Set();
        }

        // Graphics 의 EndInteraction 처리하려고 추가
        private void Page_Unloaded(object? sender, EventArgs e)
        {
           
        }

        private void OnClick_Yes(object? sender, RoutedEventArgs e)
        {
            if (buttons == MessageBoxButtons.OK || buttons == MessageBoxButtons.OKCancel) result = DialogResult.OK;
            else result = DialogResult.Yes;

            this.Close(result);
        }

        private void OnClick_No(object? sender, RoutedEventArgs e)
        {
            result = DialogResult.No;
            this.Close(result);
        }

        private void OnClick_Cancel(object? sender, RoutedEventArgs e)
        {
            result = DialogResult.Cancel;
            this.Close(result);
        }
        public void Set()
        {
            Uri uri;
            Avalonia.Media.Imaging.Bitmap bmp;

            if (sTitle.Length > 50) sTitle.Substring(0, 50);
            if (sMessage.Length > 400) sMessage.Substring(0, 400);

            if (Ffont is not null)
            {
                this.text_title.FontFamily = Ffont;
                this.text_message.FontFamily = Ffont;
            }

            this.text_title.Text = sTitle;
            this.text_message.Text = sMessage;

           
            switch (icons)
            {
                case Icons.Question:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/question.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }

                case Icons.Battery:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/battery.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }

                case Icons.Database:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/database.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }
                case Icons.Info:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/info.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }

                case Icons.Warning:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/warning.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }

                case Icons.Success:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/success.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }

                case Icons.SpeakerLess:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/speakerless.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }

                case Icons.Error:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/error.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }

                case Icons.Folder:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/folder.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }

                case Icons.Forbidden:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/forbidden.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }
                case Icons.Setting:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/setting.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }
                case Icons.Stop:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/stop.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }
                case Icons.Stopwatch:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/stopwatch.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }
                case Icons.Wifi:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/wifi.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }
                case Icons.Plus:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/plus.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }
                case Icons.Lock:
                    {
                        uri = new Uri("avares://LauncherMain/Assets/Icons/lock.png");
                        bmp = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(uri));
                        image.Source = bmp;
                        break;
                    }
                case Icons.None:
                    {
                        image.Source = null;
                        break;
                    }
                default:
                    {
                        image.Source = null;
                        break;
                    }
            }

            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    {
                        this.ok.Content = "Ok";
                        this.no.IsVisible = false;
                        this.buttonGrid.ColumnDefinitions[1].Width = new GridLength(0);
                        this.cancel.IsVisible = false;
                        this.buttonGrid.ColumnDefinitions[2].Width = new GridLength(0);
                        this.ok.Focus();
                        break;
                    }
                case MessageBoxButtons.OKCancel:
                    {
                        this.ok.Content = "Ok";
                        this.no.IsVisible = false;
                        this.buttonGrid.ColumnDefinitions[1].Width = new GridLength(0);
                        this.cancel.Content = "Cancel";
                        break;
                    }
                case MessageBoxButtons.YesNo:
                    {
                        this.ok.Content = "Yes";
                        this.no.Content = "No";
                        this.cancel.IsVisible = false;
                        this.buttonGrid.ColumnDefinitions[2].Width = new GridLength(0);
                        break;
                    }
                case MessageBoxButtons.YesNoCancel:
                    {
                        this.ok.Content = "Ok";
                        this.no.Content = "No";
                        this.cancel.Content = "Cancel";
                        break;
                    }
                default:
                    {
                        this.ok.Content = "Ok";
                        this.no.IsVisible = false;
                        this.buttonGrid.ColumnDefinitions[1].Width = new GridLength(0);
                        this.cancel.IsVisible = false;
                        this.buttonGrid.ColumnDefinitions[2].Width = new GridLength(0);
                        break;
                    }
            }

            Dispatcher.UIThread.Post(() =>
            {
                GetY();
            });
           

           

        }

        // Message가 길어질 때 높이를 길게 만드려고 추가함
        void GetY()
        {
            var point = this.buttonGrid.TranslatePoint(new Point(0, 0), this);

            if (point.HasValue)
            {
                double yPosition = point.Value.Y;
                
                if( yPosition + 40 > this.Height)
                {
                    this.Height = yPosition + 40;
                }
            }
        }

       





    }

}