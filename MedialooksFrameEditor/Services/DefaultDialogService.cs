using Microsoft.Win32;
using System.Windows;

namespace MedialooksFrameEditor.Services
{
    public class DefaultDialogService : IDialogService
    {
        public string FilePath { get; private set; }

        public bool OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();

            //add avaliable video formats
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }

            return false;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
