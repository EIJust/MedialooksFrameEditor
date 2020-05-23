namespace MedialooksFrameEditor.Services
{
    public interface IDialogService
    {
        string FilePath { get; }
        bool OpenFileDialog();
        void ShowMessage(string message);
    }
}
