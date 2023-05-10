using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace lab2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        string path;
        public Window1()
        {
            path = "@C:/";
            InitializeComponent();
        }

        public Window1(string p)
        {
            path = p;
            InitializeComponent();
        }

        public void CreateNewFile(object sender, RoutedEventArgs e)
        {
            path += "\\";
            path += name.Text;
            bool? file = File1.IsChecked;
            FileAttributes rahs = 0;
            
            if ((bool)read1.IsChecked) rahs = rahs | FileAttributes.ReadOnly;
            if ((bool)archive1.IsChecked) rahs = rahs | FileAttributes.Archive;
            if ((bool)hidden1.IsChecked) rahs = rahs | FileAttributes.Hidden;
            if ((bool)system1.IsChecked) rahs = rahs | FileAttributes.System;

            if ((bool)file)
            {
                File.Create(path);
            }else
            {
                Directory.CreateDirectory(path);
            }
            File.SetAttributes(path, rahs);
            this.DialogResult = true;
            this.Close();
        }
        public void Exit(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

    }
}
