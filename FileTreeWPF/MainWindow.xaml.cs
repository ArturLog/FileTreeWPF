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
using System.Printing;
using System.Reflection.Emit;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using System.Xml.Linq;
using System.Diagnostics;
using System.Drawing.Printing;

namespace lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public TreeViewItem roott;
        public MainWindow()
        {
            roott = new TreeViewItem();
            InitializeComponent();
        }

        /// <summary>
        /// Choose folder and show tree
        /// </summary>
        private void ChooseFolder(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory to open" };
            dlg.ShowDialog();
            roott.Header = System.IO.Path.GetFileNameWithoutExtension(dlg.SelectedPath);
            roott.Tag = dlg.SelectedPath;
            ProcessDirectory(dlg.SelectedPath, ref roott);

            TreeView1.Items.Add(roott);
            
        }

        /// <summary>
        /// Processing directory to find and print all elements and their attirbutes, if element is a folder,
        /// recursive getting inside folder.
        /// </summary>
        public void ProcessDirectory(string targetDirectory, ref TreeViewItem root)
        {
            //set tables of files and directories
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);


            // out all of files
            foreach (string fileName in fileEntries)
                ProcessFile(fileName, ref root);

            // searching subdirectories
            foreach (string subdirectoryName in subdirectoryEntries)
            {
                var next = new TreeViewItem
                {
                    Header = System.IO.Path.GetFileNameWithoutExtension(subdirectoryName),
                    Tag = subdirectoryName
                };
                AddContextMenuFolder(ref next);
                root.Items.Add(next);
                ProcessDirectory(subdirectoryName, ref next);
            }
        }

        /// <summary>
        /// Taking information about file (length, attributes) and printing it
        /// </summary>
        /// <param name="fileName">The path of file (string)</param>
        public void ProcessFile(string fileName, ref TreeViewItem root)
        {
            var next = new TreeViewItem
            {
                Header = System.IO.Path.GetFileName(fileName),
                Tag = fileName
            };
            AddContextMenuFile(ref next);
            root.Items.Add(next);
        }

        private void MainWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }


        public void AddContextMenuFolder(ref TreeViewItem item)
        {
            var contextmenu = new ContextMenu();
            item.ContextMenu = contextmenu;
            var mi = new MenuItem();
            mi.Header = "Create";
            mi.Click += new RoutedEventHandler(this.CreateItem);
            contextmenu.Items.Add(mi);
            var mi2 = new MenuItem();
            mi2.Header = "Delete";
            mi2.Click += new RoutedEventHandler(this.DeleteFolder);
            contextmenu.Items.Add(mi2);
        }

        public void AddContextMenuFile(ref TreeViewItem item)
        {
            var contextmenu = new ContextMenu();
            item.ContextMenu = contextmenu;
            var mi = new MenuItem();
            mi.Header = "Open";
            mi.Click += new RoutedEventHandler(this.OpenItem);
            contextmenu.Items.Add(mi);
            var mi2 = new MenuItem();
            mi2.Header = "Delete";
            mi2.Click += new RoutedEventHandler(this.DeleteFile);
            contextmenu.Items.Add(mi2);
        }

        public void DeleteFile(object sender, RoutedEventArgs e)
        {
            TreeViewItem selected = (TreeViewItem)TreeView1.SelectedItem;
            File.Delete((string)selected.Tag);
            var parent = selected.Parent as TreeViewItem;
            if (parent != null) parent.Items.Remove(selected);
        }

        public void DeleteFolder(object sender, RoutedEventArgs e)
        {
            TreeViewItem selected = (TreeViewItem)TreeView1.SelectedItem;
            DeepDeleteElements(selected);
        }

        private void DeepDeleteElements(TreeViewItem item)
        {
            while(item.Items.Count > 0)
            {
                TreeViewItem tmp = (TreeViewItem)item.Items[0];
                FileAttributes attr = File.GetAttributes((string)tmp.Tag);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    DeepDeleteElements((TreeViewItem)item.Items[0]);
                }
                else
                {
                    File.Delete((string)tmp.Tag);
                    item.Items.Remove((TreeViewItem)item.Items[0]);
                }
            }
           
            var parent = item.Parent as TreeViewItem;
            if (parent != null)
            {
                Directory.Delete((string)item.Tag);
                parent.Items.Remove(item);
            }

        }

        private void RefreshTree(TreeViewItem item)
        {
            while (item.Items.Count > 0)
            {
                TreeViewItem tmp = (TreeViewItem)item.Items[0];
                FileAttributes attr = File.GetAttributes((string)tmp.Tag);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    RefreshTree((TreeViewItem)item.Items[0]);
                }
                else
                {
                    item.Items.Remove((TreeViewItem)item.Items[0]);
                }
            }

            if(item.Tag != roott.Tag)
            {
                var parent = item.Parent as TreeViewItem;
                if (parent != null)
                {
                    parent.Items.Remove(item);
                }
            }
        }
        public void OpenItem(object sender, RoutedEventArgs e)
        {
            TreeViewItem selected = (TreeViewItem)TreeView1.SelectedItem;
            TextBlock1.Text = File.ReadAllText((string)selected.Tag);
        }

        public void CreateItem(object sender, RoutedEventArgs e)
        {
            TreeViewItem selected = (TreeViewItem)TreeView1.SelectedItem;

            Window1 createForm = new Window1((string)selected.Tag);
            bool isCreated = (bool)createForm.ShowDialog();

            if(isCreated)
            {
                RefreshTree(roott);
                ProcessDirectory((string)roott.Tag, ref roott);
                TreeView1.Items.Clear();
                TreeView1.Items.Add(roott);
            }
        }

        private void ChangeRAHS(object sender, RoutedEventArgs e)
        {
            TreeViewItem selected = (TreeViewItem)TreeView1.SelectedItem;
            string rahs = "";
            FileAttributes attributes = File.GetAttributes((string)selected.Tag);
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) rahs += "r";
            else rahs += "-";
            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive) rahs += "a";
            else rahs += "-";
            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden) rahs += "h";
            else rahs += "-";
            if ((attributes & FileAttributes.System) == FileAttributes.System) rahs += "s";
            else rahs += "-";


            rahstext.Text = rahs;
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
            Exit();
        }

        private void Exit()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Show dialog window about
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void About(object sender, RoutedEventArgs e)
        {
            WindowAbout wa = new WindowAbout();
            wa.ShowDialog();
        }
    }
}
