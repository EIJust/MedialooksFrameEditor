using MFORMATSLib;
using System.Collections.Generic;

namespace MedialooksFrameEditor.Services
{
    public interface IFrameService
    {
        bool TryOpenFile(string path, out string error);
        MFFrame GetFrame();
        MFFrame DrawLinesOnFrame(MFFrame mFrame, bool isDrawing, List<CurveLine> lines, int panelWidth, int panelHeight, int x, int y);
        MFFrame DrawTextOnFrame(MFFrame mFrame, string text);

    }
}
