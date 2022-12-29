using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Forms;

namespace ClipboardImageHelper
{

    public enum FileType
    {
        Jpg,
        Png
    }

    public partial class MainWindow : Window
    {

        // Inputs
        private string targetDirectory = "";
        private string fileNamePrefix = "";
        private string fileNameSuffix = "";
        private FileType saveFileType = FileType.Jpg; // default is Jpg

        public MainWindow()
        {
            InitializeComponent();
            selectTargetDirectoryButton.Click += handleSelectTargetDirectory;
            saveButton.Click += handleSaveButton;
        }

        private void handleSaveButton(object sender, EventArgs args)
        {
            readInputs();

            if (!System.Windows.Clipboard.ContainsImage())
            {
                displayMessage("ERROR: No image in clipboard");
                return;
            }

            BitmapSource image = System.Windows.Clipboard.GetImage();
            string target = getFileName();
            //Debug.WriteLine(target);
            if (target.Trim().Length > 0)
            {
                bool result = saveImage(image, target);
                if (result == true)
                {
                    System.Windows.Clipboard.SetText(target);
                }
            }
        }

        private void handleSelectTargetDirectory(object sender, EventArgs args)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (fbd.SelectedPath != null)
                    {
                        string selectedPath = fbd.SelectedPath;
                        targetDirectoryTextBox.Text = selectedPath;
                    }
                }
            }
        }

        private void readInputs()
        {
            this.targetDirectory = targetDirectoryTextBox.Text.Trim();
            this.fileNamePrefix = fileNamePrefixTextBox.Text.Trim();
            this.fileNameSuffix = fileNameSuffixTextBox.Text.Trim();
            this.saveFileType = imageFileTypePng.IsChecked == true ? FileType.Png : FileType.Jpg;
        }

        private bool saveImage(BitmapSource image, string target)
        {
            try
            {
                using (var fileStream = new FileStream(target, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(fileStream);
                    displayMessage("Image saved");
                }
            }
            catch (Exception e)
            {
                displayMessage("ERROR occured when saving image");
                return false;
            }
            return true;
        }

        private string getFileName()
        {
            string fileName = this.targetDirectory;
            if (fileName.EndsWith("\\"))
            {
                fileName = fileName.Substring(0, fileName.Length - 1);
            }
            if (!Directory.Exists(fileName))
            {
                displayMessage("ERROR: Target directory does not exist");
                return "";
            }
            fileName = string.Concat(fileName, "\\", this.fileNamePrefix, getTimestamp(), this.fileNameSuffix, getFileExtension(this.saveFileType));
            return fileName;
        }

        private void displayMessage(string message)
        {
            System.Windows.MessageBox.Show(message);
        }

        private string getTimestamp()
        {
            return DateTime.Now.ToFileTimeUtc().ToString();
        }

        private string getFileExtension(FileType fileType)
        {
            if (fileType == FileType.Png)
            {
                return ".png";
            }
            return ".jpg";
        }
    }
}
