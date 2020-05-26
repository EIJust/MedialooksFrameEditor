using MedialooksFrameEditor.Services;
using MFORMATSLib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace MedialooksFrameEditor.Tests
{
    public class FrameServiceTests
    {
        private FrameService _frameService;

        [SetUp]
        public void Setup()
        {
            _frameService = new FrameService();

            var videoPath = TestContext.CurrentContext.TestDirectory + "\\test.avi";
            _frameService.TryOpenFile(videoPath, out string error);
        }

        [Test]
        public void DrawLinesOnFrame_NoneLines_EqualFrames()
        {
            var mFrame = _frameService.GetFrame();
            var isDrawing = true;
            var lines = new List<CurveLine>();
            var panelWidth = 100;
            var panelHeight = 100;
            var x = 0;
            var y = 0;

            var newFrame = _frameService.DrawLinesOnFrame(mFrame, isDrawing, lines, panelWidth, panelHeight, x, y);

            Assert.AreEqual(newFrame, mFrame);
        }

        [Test]
        public void DrawLinesOnFrame_WithLines_NotEqualFrames()
        {
            var mFrame = _frameService.GetFrame();
            var isDrawing = true;
            var lines = new List<CurveLine>()
            {
                new CurveLine()
                {
                    PenPath = new List<Point>(){ new Point() }
                }
            };
            var panelWidth = 100;
            var panelHeight = 100;
            var x = 0;
            var y = 0;

            var newFrame = _frameService.DrawLinesOnFrame(mFrame, isDrawing, lines, panelWidth, panelHeight, x, y);

            Assert.AreNotEqual(newFrame, mFrame);
        }

        [Test]
        public void DrawLinesOnFrame_ZeroWidthAndHeight_ThrowException()
        {
            var mFrame = _frameService.GetFrame();
            var isDrawing = true;
            var lines = new List<CurveLine>()
            {
                new CurveLine()
                {
                    PenPath = new List<Point>(){ new Point() }
                }
            };
            var panelWidth = 0;
            var panelHeight = 0;
            var x = 0;
            var y = 0;

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => _frameService.DrawLinesOnFrame(mFrame, isDrawing, lines, panelWidth, panelHeight, x, y));
        }

        [Test]
        public void DrawLinesOnFrame_BelowZeroWidthAndHeight_ThrowException()
        {
            var mFrame = _frameService.GetFrame();
            var isDrawing = true;
            var lines = new List<CurveLine>()
            {
                new CurveLine()
                {
                    PenPath = new List<Point>(){ new Point() }
                }
            };
            var panelWidth = -100;
            var panelHeight = -100;
            var x = 0;
            var y = 0;

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => _frameService.DrawLinesOnFrame(mFrame, isDrawing, lines, panelWidth, panelHeight, x, y));
        }

        [Test]
        public void DrawTextOnFrame_EmptyText_EqualFrames()
        {
            var mFrame = _frameService.GetFrame();
            var text = string.Empty;

            var newFrame = _frameService.DrawTextOnFrame(mFrame, text);

            Assert.AreEqual(newFrame, mFrame);
        }

        [Test]
        public void DrawTextOnFrame_LittleText_NotEqualFrames()
        {
            var mFrame = _frameService.GetFrame();
            var text = "little text";

            var newFrame = _frameService.DrawTextOnFrame(mFrame, text);

            Assert.AreNotEqual(newFrame, mFrame);
        }

        [Test]
        public void DrawTextOnFrame_FontSizeBelowZero_ThrowException()
        {
            var mFrame = _frameService.GetFrame();
            var text = "little text";
            var fontSize = -8;

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => _frameService.DrawTextOnFrame(mFrame, text, fontSize));
        }
    }
}
