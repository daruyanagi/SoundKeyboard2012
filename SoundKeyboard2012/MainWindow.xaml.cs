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
using System.Deployment.Application;

namespace SoundKeyboard2012
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedUICommand AddSoundPackCommand = new RoutedUICommand(
            "追加(_A)", "AddSoundPack", typeof(MainWindow)
        );
        public static RoutedUICommand SelectSoundPackCommand = new RoutedUICommand(
            "選択(_D)", "SelectSoundPack", typeof(MainWindow)
        );
        public static RoutedUICommand RemoveSoundPackCommand = new RoutedUICommand(
            "削除(_D)", "RemoveSoundPack", typeof(MainWindow)
        );

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
            
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                Version v = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                textBlockVersion.Text = string.Format(
                    "ネットワークインストールバージョン {0}.{1}.{2}.{3}",
                    v.Major, v.Minor, v.Build, v.Revision);
            }

            GlobalKeybordMonitor.KeyDown += (_sender, _e) => { Title = _e.KeyData.ToString(); };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            WindowState = WindowState.Minimized;

            App.ShowBaloonTip("メインウィンドウを最小化しました");
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ForceClose(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void SlectSoundPackCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute =
                0 <= listBoxSoundPacks.SelectedIndex &&
                listBoxSoundPacks.SelectedIndex < App.SoundPacks.Count;
        }

        private void AddSoundPackCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RemoveBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute =
                0 <= listBoxSoundPacks.SelectedIndex &&
                listBoxSoundPacks.SelectedIndex < App.SoundPacks.Count;
        }

        private void SlectSoundPackCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App.SoundEngine.Location = App.SoundPacks[listBoxSoundPacks.SelectedIndex].Location;
        }

        private void AddSoundPackCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    App.SoundPacks.Add(new SoundPack(dialog.SelectedPath));
                }
            }
        }

        private void RemoveSoundPackCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App.SoundPacks.RemoveAt(listBoxSoundPacks.SelectedIndex);
        }
    }
}
