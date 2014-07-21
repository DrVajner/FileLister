using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WinForms = System.Windows.Forms;

namespace FileLister
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AutoOpenCheckBox.IsChecked = true;
        }

        private void ListDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            using (var openFolderDialog = new WinForms.FolderBrowserDialog())
            {
                openFolderDialog.ShowNewFolderButton = false;
                if (openFolderDialog.ShowDialog() == WinForms.DialogResult.OK)
                {
                    ListDirectoryTextBox.Text = openFolderDialog.SelectedPath;
                }
            }
        }

        private void OutputFileButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "*.csv|CSV-files";
            if (saveFileDialog.ShowDialog() == true)
            {
                OutputFileTextBox.Text = saveFileDialog.FileName;
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateOutput();
            if (AutoOpenCheckBox.IsChecked == true)
            {
                Process.Start(OutputFileTextBox.Text);
            }
        }

        private void ListDirectoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ListDirectoryTextBox.Text) || !Directory.Exists(ListDirectoryTextBox.Text))
            {
                ActivateControls(false);
            }
            else
            {
                ActivateControls(true);
            }
        }

        private void ActivateControls(bool enabled)
        {
            GenerateButton.IsEnabled = enabled;
        }

        private void GenerateOutput()
        {
            var filesDescriptions = ListFiles(ListDirectoryTextBox.Text);
            filesDescriptions = FillData(filesDescriptions);

            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                fileStream = File.Create(OutputFileTextBox.Text);
                streamWriter = new StreamWriter(fileStream);

                foreach (var fileDescription in filesDescriptions)
                {
                    streamWriter.WriteLine(fileDescription.ToString());
                }
            }
            catch (Exception ex)
            {
                //TODO: Add some logging.
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        private List<FileDescription> ListFiles(string directory)
        {
            var files = new List<FileDescription>();

            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                files.AddRange(ListFiles(subDirectory));
            }

            foreach (var file in Directory.GetFiles(directory))
            {
                files.Add(new FileDescription
                {
                    FileName = Path.GetFileName(file),
                    Directory = GetRelativeDirectory(Path.GetFullPath(file)),
                });
            }

            return files;
        }

        private List<FileDescription> FillData(List<FileDescription> filesDescriptions)
        {
            foreach (var fileDescription in filesDescriptions)
            {
                fileDescription.Date = string.IsNullOrWhiteSpace(fileDescription.Date) ?
                    ListingDate.DisplayDate.ToString("dd.MM.yyyy.") :
                    fileDescription.Date;
                fileDescription.Description = string.IsNullOrWhiteSpace(fileDescription.Description) ?
                    DescriptionTextBox.Text :
                    fileDescription.Description;
                fileDescription.BugNumber = string.IsNullOrWhiteSpace(fileDescription.BugNumber) ?
                    BugNumberTextBox.Text :
                    fileDescription.BugNumber;
                fileDescription.Developer = string.IsNullOrWhiteSpace(fileDescription.Developer) ?
                    DeveloperTextBox.Text :
                    fileDescription.Developer;
            }

            return filesDescriptions;
        }

        private string FillProperty(string propertyValue, string value)
        {
            if (string.IsNullOrWhiteSpace(propertyValue))
            {
                return value;
            }

            return propertyValue;
        }

        private string GetRelativeDirectory(string directory)
        {
            var checkDirectory = Environment.CurrentDirectory;
            var resultDirectory = directory.Replace(checkDirectory, "");
            return resultDirectory;
        }
    }
}
