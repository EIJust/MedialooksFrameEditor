using MFORMATSLib;
using System;

namespace MedialooksFrameEditor.Services
{
    public class FrameLoader : IFrameLoader
    {
        private const string ERROR_OPEN_FILE = "Error open file:";

        private readonly MFReader _mfReader;

        public FrameLoader()
        {
            _mfReader = new MFReader();
        }

        public MFFrame GetFrame()
        {
            _mfReader.SourceFrameGetByTime(-1, -1, out MFFrame sourceFrame, "");

            sourceFrame.MFClone(out MFFrame clonedFrame, eMFrameClone.eMFC_Reference, eMFCC.eMFCC_ARGB32);

            //clonedFrame.MFVideoGetBytes(out _, out long framePointer);
            //clonedFrame.MFAVPropsGet(out M_AV_PROPS mediaProperties, out _);

            //int frameWidth = mediaProperties.vidProps.nWidth;
            //int frameHeight = Math.Abs(mediaProperties.vidProps.nHeight);
            //int frameRowBytes = mediaProperties.vidProps.nRowBytes;

            //PixelFormat framePixelFormat = PixelFormat.Format32bppRgb;
            //Bitmap frameBitmap = new Bitmap(frameWidth, frameHeight, frameRowBytes, framePixelFormat, new IntPtr(framePointer));

            return clonedFrame;
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
