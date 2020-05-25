using MFORMATSLib;
using System.Collections.Generic;

namespace MedialooksFrameEditor.Services
{
    public interface IFrameService
    {
        MFPreviewClass MFPreview { get; }
        MF_RECT Overlay { get; set; }

        bool TryOpenFile(string path, out string error);
        MFFrame GetFrame();
        MFFrame DrawLinesOnFrame(MFFrame mFrame, bool isDrawing, List<CurveLine> lines, int panelWidth, int panelHeight, int x, int y);
        MFFrame DrawTextOnFrame(MFFrame mFrame, string text, int fontSize = 8);
        void PreviewFrame(MFFrame frame, int maxWait = -1, string hints = "");
    }
}
