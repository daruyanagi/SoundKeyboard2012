using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
// using System.Windows.Shapes;

using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

namespace SoundKeyboard2012
{
    /// <summary>
    /// DisplayInputWindow.xaml の相互作用ロジック
    /// </summary>
    [Serializable]
    public partial class DisplayInputWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DisplayInputWindow()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(DisplayInputWindow_Loaded);
            IsVisibleChanged += (_sender, _e) => { CalclateDisplayPosition(); };
        }

        public class Settings
        {
            public bool Visible { get; set; }
            public Position DisplayPosition { get; set; }
            public int DispalayMargin { get; set; }
        }

        public void Load(string dir)
        {
            var serializer = new XmlSerializer(typeof(Settings));

            using (var reader = new StreamReader(Path.Combine(dir, "DisplayInputWindow.config")))
            {
                var settings =  serializer.Deserialize(reader) as Settings;

                Visible = settings.Visible;
                DisplayMargin = settings.DispalayMargin;
                DisplayPosition = settings.DisplayPosition;
            }
        }

        public void Save(string dir)
        {
            var settings = new Settings()
            {
                Visible = Visible,
                DispalayMargin = DisplayMargin,
                DisplayPosition = DisplayPosition,
            };

            using (var writer = new StreamWriter(Path.Combine(dir, "DisplayInputWindow.config")))
            {
                new XmlSerializer(typeof(Settings)).Serialize(writer, settings);
            }
        }

        void  DisplayInputWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PropertyChanged += new PropertyChangedEventHandler(
                DisplayInputWindow_PropertyChanged
            );

            GlobalKeybordMonitor.KeyDown += (_sender, _e) =>
            {
                textBlockDisplayInput.Text = _e.KeyData.ToString();
            };
        }

        void CalclateDisplayPosition()
        {
            switch (DisplayPosition)
            {
                case DisplayInputWindow.Position.TopLeft:
                    Top = DisplayMargin - SystemInformation.CaptionHeight + DisplayMargin;
                    Left = DisplayMargin;
                    textBlockDisplayInput.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                    break;
                case DisplayInputWindow.Position.TopRight:
                    Top = DisplayMargin - SystemInformation.CaptionHeight + DisplayMargin;
                    Left = Screen.PrimaryScreen.Bounds.Width - DisplayMargin - Width;
                    textBlockDisplayInput.FlowDirection = System.Windows.FlowDirection.RightToLeft;
                    break;
                case DisplayInputWindow.Position.BottomLeft:
                    Top = Screen.PrimaryScreen.Bounds.Height - DisplayMargin - Height;
                    Left = DisplayMargin;
                    textBlockDisplayInput.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                    break;
                case DisplayInputWindow.Position.BottomRight:
                    Top = Screen.PrimaryScreen.Bounds.Height - DisplayMargin - Height;
                    Left = Screen.PrimaryScreen.Bounds.Width - DisplayMargin - Width;
                    textBlockDisplayInput.FlowDirection = System.Windows.FlowDirection.RightToLeft;
                    break;
            }
        }

        void DisplayInputWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "DisplayPosition":
                case "DisplayMargin":
                    CalclateDisplayPosition();
                    break;
            }
        }

        public Key Key { get; set; }

        public bool Visible
        {
            get
            {
                return Visibility == Visibility.Visible;
            }
            set
            {
                Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Position DisplayPosition { get; set; }

        public int DisplayMargin { get; set; }

        public enum Position
        {
            TopLeft, TopRight, BottomLeft, BottomRight,
        }
    }
}
