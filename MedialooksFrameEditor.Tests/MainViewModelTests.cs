using MedialooksFrameEditor.Models;
using MedialooksFrameEditor.Services;
using MedialooksFrameEditor.ViewModels;
using MFORMATSLib;
using NSubstitute;
using NUnit.Framework;
using System.Linq;

namespace MedialooksFrameEditor.Tests
{
    public class MainViewModelTests
    {
        private MainViewModel _vm;
        private IDialogService _dialogServiceStub;
        private ISurfaceService _surfaceServiceStub;
        private IFrameService _frameServiceStub;

        [SetUp]
        public void Setup()
        {
            var mfPreview = new MFPreviewClass();

            _dialogServiceStub = Substitute.For<IDialogService>();
            _surfaceServiceStub = Substitute.For<ISurfaceService>();
            _frameServiceStub = Substitute.For<IFrameService>();

            _frameServiceStub.MFPreview.Returns(mfPreview);

            _vm = new MainViewModel(_dialogServiceStub, _frameServiceStub, _surfaceServiceStub);
        }

        [Test]
        public void MouseEvents_NoneLines_AddLine()
        {
            var linesCount = _vm.DrawLines.Count();

            _vm.MouseDownCommand.Execute(null);
            _vm.MouseMoveCommand.Execute(null);
            _vm.MouseUpCommand.Execute(null);

            Assert.IsTrue(_vm.DrawLines.Count() == linesCount + 1);
        }

        [Test]
        public void ClearAll_FilledTextAndLines_TextAndLinesIsEmpty()
        {
            _vm.Text = "123";
            _vm.MouseDownCommand.Execute(null);
            _vm.MouseMoveCommand.Execute(null);
            _vm.MouseUpCommand.Execute(null);

            _vm.ClearAllCommand.Execute(null);

            Assert.IsTrue(_vm.DrawLines.Count() == 0 && _vm.Text == string.Empty);
        }

        [Test]
        public void OpenFileDialog_InitState_FileDialogCalled()
        {
            _vm.OpenFileDialogCommand.Execute(null);

            _dialogServiceStub.Received().OpenFileDialog();
        }
    }
}