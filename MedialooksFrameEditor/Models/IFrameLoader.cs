using MFORMATSLib;

namespace MedialooksFrameEditor.Services
{
    public interface IFrameLoader
    {
        bool TryOpenFile(string path, out string error);
        MFFrame GetFrame();
    }
}
