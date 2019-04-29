using Manager_Plikow.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace Manager_Plikow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum PanelSelected { Left, Right }
        private string currentPathLeft;
        private string currentPathRight;
        private PanelSelected focusPanel;
        public MainWindow()
        {
            InitializeComponent();

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo currentDrive in allDrives)
            {
                leftDrive.Items.Add(currentDrive.Name);
                rightDrive.Items.Add(currentDrive.Name);
            }
            currentPathLeft = allDrives[0].ToString();
            currentPathRight = allDrives[0].ToString();
            RefreshPanel(currentPathLeft, leftDirectory, leftDirectoryPathTextBox);
            RefreshPanel(currentPathRight, rightDirectory, rightDirectoryPathTextBox);

        }
        //Refresh Panel logic
        private void RefreshPanel(string path, ListView listView, TextBox textBox)
        {
            List<Item> itemList = new List<Item>();
            itemList.Clear();
            textBox.Text = path;
            MyDirectory el = new MyDirectory(path);
            List<DiscElement> myElements = el.GetSubElements();
            foreach (DiscElement myElement in myElements)
            {
                try
                {
                    itemList.Add(new Item(myElement));
                }
                catch(UnauthorizedAccessException)
                {
                    continue;
                }
            }
            listView.ItemsSource = itemList;
        }
        //Focus of current panel
        private void leftDirectory_GotFocus(object sender, RoutedEventArgs e)
        {
            focusPanel = PanelSelected.Left;
        }
        private void rightDirectory_GotFocus(object sender, RoutedEventArgs e)
        {
            focusPanel = PanelSelected.Right;
        }
        //Back Button logic
        private void BackButton(string currentPath, ListView listView, TextBox textBox, out string newPath)
        {
            DirectoryInfo parentDirectory = Directory.GetParent(currentPath);
            if (parentDirectory != null)
            {
                string parentPath = parentDirectory.FullName;
                RefreshPanel(parentPath, listView, textBox);
                newPath = parentPath;

            }
            else
            {
                newPath = currentPath;
            }
        }
        private void LeftBackButton_Click(object sender, RoutedEventArgs e)
        {
            BackButton(currentPathLeft, leftDirectory, leftDirectoryPathTextBox, out currentPathLeft);
        }
        private void RightBackButton_Click(object sender, RoutedEventArgs e)
        {
            BackButton(currentPathRight, rightDirectory, rightDirectoryPathTextBox, out currentPathRight);
        }

        //Changing drives from DriveList
        private void ChangeDrive(ComboBox drive,ListView panel, TextBox panelTextBox, string currentPath)
        {
            try
            {
                RefreshPanel(drive.SelectedItem.ToString(), panel, panelTextBox);
                currentPath = drive.SelectedItem.ToString();
            }
            catch
            {
                MessageBox.Show(Strings.changeDriveErrorText, Strings.changeDriveErrorHeader, MessageBoxButton.OK, MessageBoxImage.Error);
                panelTextBox.Text = "";
            }
        }
        private void leftDrive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeDrive(leftDrive, leftDirectory, leftDirectoryPathTextBox, currentPathLeft);
        }
        private void rightDrive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeDrive(rightDrive, rightDirectory, rightDirectoryPathTextBox, currentPathRight);
        }


        //logic for Refresh Button
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPanel(currentPathLeft, leftDirectory, leftDirectoryPathTextBox);
            RefreshPanel(currentPathRight, rightDirectory, rightDirectoryPathTextBox);
            searchTextBox.Clear();
        }


        //logic for Delete Button
        private void DeleteItem(ListView panel)
        {
            Item itemToDelete = (Item)panel.SelectedValue;
            try
            {
                if (itemToDelete.IsDirectory())
                {
                    Directory.Delete(itemToDelete.Path);
                }
                else
                {
                    File.Delete(itemToDelete.Path);
                }
                RefreshPanel(currentPathLeft, leftDirectory, leftDirectoryPathTextBox);
                RefreshPanel(currentPathRight, rightDirectory, rightDirectoryPathTextBox);
            }
            catch (Exception)
            {
                MessageBox.Show(Strings.fileToDeleteNotSelectedText, Strings.fileNotSelectedHeader, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (focusPanel == PanelSelected.Left)
            {
                DeleteItem(leftDirectory);
            }
            else
            {
                DeleteItem(rightDirectory);
            }
        }


        //logic for creating Directory connected with NewFileView View
        public static void Create(string targetPath)
        {
            NewFileView newItemWindow = new NewFileView();
            if (newItemWindow.ShowDialog() == true)
            {
                try
                {
                    if (Directory.Exists(System.IO.Path.Combine(targetPath, newItemWindow.itemNameTB)))
                    {
                        MessageBoxResult result = MessageBox.Show(Strings.fileExistsErrorText, Strings.fileExistsErrorHeader, MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                            Directory.Delete(System.IO.Path.Combine(targetPath, newItemWindow.itemNameTB), true);
                            Directory.CreateDirectory(System.IO.Path.Combine(targetPath, newItemWindow.itemNameTB));
                    }
                    else
                    {
                        Directory.CreateDirectory(System.IO.Path.Combine(targetPath, newItemWindow.itemNameTB));
                    }

                }
                catch(UnauthorizedAccessException)
                {
                    MessageBox.Show(Strings.moveFileOrDirectoryErrorText, Strings.moveFileOrDirectoryErrorHeader, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void NewDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (focusPanel == PanelSelected.Left)
            {
                    Create(currentPathLeft);
                    RefreshPanel(currentPathLeft, leftDirectory, leftDirectoryPathTextBox);
            }
            else if(focusPanel == PanelSelected.Right)
            {
                Create(currentPathRight);
                RefreshPanel(currentPathRight, rightDirectory, rightDirectoryPathTextBox);
            }

        }


        //Copies from right panel to left panel
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Item itemToCopy = (Item)rightDirectory.SelectedValue;
            if (itemToCopy == null)
            {
                MessageBox.Show(Strings.fileToCopyNotSelectedText, Strings.fileNotSelectedHeader, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string sourceItemPath = itemToCopy.Path;
                try
                {
                    if (itemToCopy.IsDirectory())
                    {
                        string destinationPath = currentPathRight + "\\" + itemToCopy.Name;
                    }
                    else
                    {
                        try
                        {
                            string destinationPath = currentPathLeft + "\\" + itemToCopy.Name + "." + itemToCopy.Extension;
                            File.Copy(sourceItemPath, destinationPath);
                        }
                        catch(Exception)
                        {
                        }
                    }
                    RefreshPanel(currentPathLeft, leftDirectory, leftDirectoryPathTextBox);
                    RefreshPanel(currentPathRight, rightDirectory, rightDirectoryPathTextBox);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(Strings.moveFileOrDirectoryErrorText, Strings.moveFileOrDirectoryErrorHeader, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //checks if istext type
        public static bool HasTextExtension(string source)
        {
            return (source.EndsWith(".txt") || source.EndsWith(".tex"));
        }
        //Checks if is photo type
        public static bool HasImageExtension(string source)
        {
            return (source.EndsWith(".png") || source.EndsWith(".jpg") || source.EndsWith(".jpeg") || source.EndsWith(".bmp") || source.EndsWith(".gif"));
        }
        //Going into directories by double click Opens in viewer if is text file or photo
        private void DoubleClick(string currentPath, ListView currentPanel, TextBox textBox, out string newPath)
        {
            if (currentPanel.SelectedIndex != -1)
            {
                Item itemElement = (Item)currentPanel.SelectedValue;
                if (itemElement.IsDirectory())
                {
                    newPath = itemElement.Path;
                    RefreshPanel(newPath, currentPanel, textBox);
                }
                else if (HasImageExtension(itemElement.Path.ToLower()))
                {
                    newPath = currentPath;
                    ImageViewer img = new ImageViewer();
                    img.imageHolder.Source = new BitmapImage(new Uri(itemElement.Path));
                    img.ShowDialog();
                }
                else if (HasTextExtension(itemElement.Path.ToLower()))
                {
                    newPath = currentPath;
                    TextViewer txt = new TextViewer();
                    txt.textHolder.Text = File.ReadAllText(itemElement.Path);
                    txt.textHolder.IsReadOnly = true;
                    txt.ShowDialog();
                }
                else
                {
                    newPath = currentPath;
                    Process.Start(currentPathLeft);
                }
            }
            else
            {
                newPath = currentPath;
            }
        }
        private void leftDirectory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DoubleClick(currentPathLeft, leftDirectory, leftDirectoryPathTextBox, out currentPathLeft);
        }
        private void rightDirectory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DoubleClick(currentPathRight, rightDirectory, rightDirectoryPathTextBox, out currentPathRight);
        }

        //Filter ListView via TextBox
        private void SearchFiltering(string path, ListView listView)
        {
            string keyWord = searchTextBox.Text.ToLower();
            List<Item> itemList = new List<Item>();
            itemList.Clear();
            MyDirectory el = new MyDirectory(path);
            List<DiscElement> myElements = el.GetSubElements();
            foreach (DiscElement myElement in myElements)
            {
                try
                {
                    if (myElement.GetName().ToLower().Contains(keyWord))
                    {
                        itemList.Add(new Item(myElement));
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
            }
            listView.ItemsSource = itemList;
        }
        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            if (focusPanel == PanelSelected.Left)
            {
                SearchFiltering(currentPathLeft, leftDirectory);
            }
            else if (focusPanel == PanelSelected.Right)
            {
                SearchFiltering(currentPathRight, rightDirectory);
            }
        }
        //Sort ListView by currently clicked column
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header  
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
            
        }
        private void Sort(string sortBy, ListSortDirection direction)
        {
            if (focusPanel == PanelSelected.Left)
            {
                ICollectionView dataView = CollectionViewSource.GetDefaultView(leftDirectory.ItemsSource);
                dataView.SortDescriptions.Clear();
                SortDescription sd = new SortDescription(sortBy, direction);
                dataView.SortDescriptions.Add(sd);
                dataView.Refresh();
            }
            else if (focusPanel == PanelSelected.Right)
            {
                ICollectionView dataView = CollectionViewSource.GetDefaultView(rightDirectory.ItemsSource);
                dataView.SortDescriptions.Clear();
                SortDescription sd = new SortDescription(sortBy, direction);
                dataView.SortDescriptions.Add(sd);
                dataView.Refresh();
            }
        }
    }
}
