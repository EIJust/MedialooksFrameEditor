using MFORMATSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MedialooksFrameEditor.Services
{
    public class FrameService : IFrameService
    {
        private const string ERROR_OPEN_FILE = "Error open file:";

        private readonly MFReader _mfReader;

        public FrameService()
        {
            _mfReader = new MFReader();

            Overlay = new MF_RECT();
            MFPreview = new MFPreviewClass();
        }

        public MFPreviewClass MFPreview { get; }
        public MF_RECT Overlay { get; set; }

        public MFFrame GetFrame()
        {
            _mfReader.SourceFrameGetByTime(-1, -1, out MFFrame sourceFrame, "");

            sourceFrame.MFClone(out MFFrame clonedFrame, eMFrameClone.eMFC_Reference, eMFCC.eMFCC_ARGB32);

            return clonedFrame;
        }

        public MFFrame DrawLinesOnFrame(MFFrame mFrame, bool isDrawing, List<CurveLine> lines, int panelWidth, int panelHeight, int x, int y)
        {
            if (panelHeight <= 0 || panelWidth <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (lines.Any())
            {
                MFFrame mFrameDraw = CurveDraw.DrawFrame(isDrawing, mFrame, panelWidth, panelHeight, x, y, lines);
                Marshal.ReleaseComObject(mFrame);
                return mFrameDraw;
            }

            return mFrame;
        }

        public MFFrame DrawTextOnFrame(MFFrame mFrame, string text, int fontSize = 8)
        {
            if (fontSize < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (!string.IsNullOrEmpty(text))
            {
                MFFrame mFrameClone = null;
                try
                {
                    mFrame.MFClone(out mFrameClone, eMFrameClone.eMFC_Full, eMFCC.eMFCC_Default);

                    var overlay = Overlay;
                    mFrameClone.MFPrint(text, fontSize, ref overlay, eMFTextFlags.eMFT_WordBreaks, "");
                    Marshal.ReleaseComObject(mFrame);
                    return mFrameClone;
                }
                catch
                {
                    if (mFrameClone != null)
                        Marshal.ReleaseComObject(mFrameClone);

                    return mFrame;
                }
            }

            return mFrame;
        }

        public void PreviewFrame(MFFrame frame, int maxWait = -1, string hints = "")
        {
            MFPreview.ReceiverFramePut(frame, maxWait, hints);
        }

        public bool TryOpenFile(string path, out string error)
        {
            try
            {
                _mfReader.ReaderOpen(path, "loop=true");
                error = string.Empty;

                return true;
            }
            catch (Exception ex)
            {
                error = ERROR_OPEN_FILE + path + Environment.NewLine + ex.Message;

                return false;
            }
        }
    }
}
