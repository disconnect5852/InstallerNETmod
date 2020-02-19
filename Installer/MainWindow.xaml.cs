using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Threading;

namespace Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void UpdateDownMessage(String message);
        internal static MainWindow thiswindow{ get; set; }
        private static InstLogic insta;
        //private static int filecont { get; set; }

        public MainWindow()
        {
            thiswindow = this;
            InitializeComponent();
            choice.Items.Add("database file is unreachable. Server, or connection problem");
            insta = new InstLogic();

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            downbutton.ClearValue(System.Windows.Controls.Button.BackgroundProperty);
            gepassword.ClearValue(System.Windows.Controls.PasswordBox.BackgroundProperty);
            gemail.ClearValue(System.Windows.Controls.TextBox.BackgroundProperty);
            e.Handled = true;
            string destination = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\RailSimulator.com\\RailWorks", "Install_Path", null) as string;
            System.Console.WriteLine("Selected Destination folder: " + destination);
            if (!File.Exists(destination + "\\railworks.exe") || !Autochk.IsChecked.Value)
            {
                destination = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 24010", "InstallLocation", null) as string;
                System.Console.WriteLine("Selected Destination folder: "+destination);
                if (!File.Exists(destination + "\\railworks.exe") || !Autochk.IsChecked.Value)
                {
                    FolderBrowserDialog fbdialog = new FolderBrowserDialog();
                    fbdialog.ShowNewFolderButton = false;
                    fbdialog.Description = "Please select your Train Simulator Folder";
                    //fbdialog.RootFolder = Environment.SpecialFolder.ProgramFilesX86;
                    //fbdialog.SelectedPath="f:\\mutyi\\RW";
                    if (fbdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && File.Exists(fbdialog.SelectedPath + "\\railworks.exe"))
                    {
                        destination = fbdialog.SelectedPath;
                        System.Console.WriteLine("Selected Destination folder: " + destination);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Train Simulator not found in the selected directory", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }
            //System.Console.WriteLine(destination+Directory.Exists(destination));
            if (gemail.Text != null && gepassword.Password != null && gemail.Text.Length > 4 && gepassword.Password.Length > 10)
            {
                downbutton.Background = Brushes.Yellow;
                //var leeelist= choice.SelectedItems;
                //Task.Run(() => insta.LetsDoThis(gemail.Text, gepassword.Password,destination));
                //await Task.Run(() => { new InstLogic().LetsDoThis(gemail.Text, gepassword.Password, destination); });
                insta.email = gemail.Text;
                insta.pass = gepassword.Password;
                insta.dest = destination;
                insta.choicess = choice.SelectedItems;
                await Task.Run(() => insta.LetsDoThis());
                downbutton.Background = Brushes.LightGreen;
                //   new InstLogic().LetsDoThis(gemail.Text, gepassword.Password,destination);
                //t.Start();

            } else
            {
                System.Windows.Forms.MessageBox.Show("Email or password is wrong", "Problem?", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                gemail.Background = System.Windows.Media.Brushes.Red;
                gepassword.Background = System.Windows.Media.Brushes.Red;
            }
        }
    }
}
