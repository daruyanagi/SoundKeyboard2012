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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Interop;
using System.Media;
using Microsoft.VisualBasic.ApplicationServices;
using System.Reflection;

namespace SoundKeyboard2012
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool mForceClose = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            panelSoundEngine.DataContext = App.SoundEngine;
            groupBoxSoundEngineSettings.DataContext = App.SoundEngine;
            groupBoxDisplayInputSettings.DataContext = App.DisplayInput;
            listBoxSoundPacks.ItemsSource = App.SoundPacks;
            comboBoxDisplayPosition.ItemsSource = Enum.GetValues(typeof(DisplayInputWindow.Position));
            tabItemAbout.DataContext = new AssemblyInfo(Assembly.GetExecutingAssembly());

            GlobalKeybordMonitor.KeyDown += (_sender, _e) => { Title = _e.KeyData.ToString(); };

            Visibility = Visibility.Hidden;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mForceClose)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                Visibility = Visibility.Hidden;
                App.ShowBaloonTip("メインウィンドウを最小化しました。");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void SelectSoundPack(object sender, RoutedEventArgs e)
        {
            try
            {
                App.SoundEngine.Location = App.SoundPacks[listBoxSoundPacks.SelectedIndex].Location;
            }
            catch (Exception exception)
            {
                App.ShowBaloonTip(exception.Message);
            }
        }

        private void AddSoundPack(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    App.SoundPacks.Add(new SoundPack(dialog.SelectedPath));
                }
            }
        }

        private void DeleteSoundPack(object sender, RoutedEventArgs e)
        {
            try
            {
                App.SoundPacks.RemoveAt(listBoxSoundPacks.SelectedIndex);
            }
            catch (Exception exception)
            {
                App.ShowBaloonTip(exception.Message);
            }
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ForceClose(object sender, RoutedEventArgs e)
        {
            mForceClose = true; Close();
        }
    }
}
