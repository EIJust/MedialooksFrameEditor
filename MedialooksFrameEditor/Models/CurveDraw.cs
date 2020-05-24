using MFORMATSLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MedialooksFrameEditor.Services
{
    public class CurveDraw
    {
        public static Bitmap MFrame2Bitmap(ref MFFrame _mFrame, out M_VID_PROPS _vidProps)
        {
            // Clone frame to RGB32
            MFFrame mFrame = _mFrame;
            _mFrame.MFClone(out mFrame, eMFrameClone.eMFC_Reference, eMFCC.eMFCC_ARGB32);

            // FrameVideoGetBytes(out cbPicture, out pbPicture);
            int cbSize;
            long pbVideo;
            mFrame.MFVideoGetBytes(out cbSize, out pbVideo);

            // Get Props from frame
            int audioSample;
            M_AV_PROPS avProps;
            mFrame.MFAVPropsGet(out avProps, out audioSample);

            // Create a bitmap from frame
            Bitmap bmpPicture = new Bitmap(avProps.vidProps.nWidth, Math.Abs(avProps.vidProps.nHeight),
                avProps.vidProps.nRowBytes,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb,
                new IntPtr(pbVideo));

            _vidProps = avProps.vidProps;
            _mFrame = mFrame;

            return bmpPicture;
        }

        public static MFFrame DrawFrame(bool _draw, MFFrame _mFrame, int _panelWidth, int _panelHeight, int _x, int _y, List<CurveLine> _linesToDraw)
        {
            if (_mFrame != null)
            {
                M_VID_PROPS vidProps;
                Bitmap bmpPicture = MFrame2Bitmap(ref _mFrame, out vidProps);

                // Calculate mouse position
                if (_draw)
                {
                    int x = Math.Abs(vidProps.nWidth * _x / _panelWidth);
                    int y = Math.Abs(vidProps.nHeight * _y / _panelHeight);

                    _linesToDraw.Last().PenPath.Add(new Point(x, y));
                }

                // Draw lines
                var graphic = Graphics.FromImage(bmpPicture);

                for (int index = 0; index < _linesToDraw.Count; index++)
                {
                    CurveLine curveLine = _linesToDraw[index];
                    if (curveLine.PenPath.Count > 1)
                        graphic.DrawLines(new Pen(curveLine.PenColor, curveLine.PenSize), curveLine.PenPath.ToArray());
                }

                bmpPicture.Dispose();
                graphic.Dispose();

                return _mFrame;
            }

            return null;
        }
    }
}
