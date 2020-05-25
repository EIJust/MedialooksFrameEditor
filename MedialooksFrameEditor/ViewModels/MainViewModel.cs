﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MedialooksFrameEditor.Models;
using MedialooksFrameEditor.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;

namespace MedialooksFrameEditor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IFrameService _frameService;
        private readonly ISurfaceService _surfaceService;

        private readonly BackgroundWorker _backgroundWorker;

        private string _filePath;
        public D3DImage _previewSurface;
        private List<CurveLine> _drawLines;

        private bool _isMouseMoving;
        private bool _drawingLine;
        private System.Windows.Media.Color _penColor;
        private int _penSize;
        private int _panelWidth;
        private int _panelHeight;
        private string _text;
        private int _textX;
        private int _textY;

        public MainViewModel(IDialogService dialogService, IFrameService frameService, ISurfaceService surfaceService)
        {
            _dialogService = dialogService;
            _frameService = frameService;
            _surfaceService = surfaceService;

            OpenFileDialogCommand = new RelayCommand(OpenFileDialog);
            MouseDownCommand = new RelayCommand(OnMouseDown);
            MouseUpCommand = new RelayCommand(OnMouseUp);
            MouseMoveCommand = new RelayCommand(OnMouseMove);
            ClearAllCommand = new RelayCommand(ClearAll);

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += WorkerDoWork;

            _drawLines = new List<CurveLine>();

            PenSize = 1;
            FontSize = 8;
            Width = 100;
            Height = 100;
            PenColor = Color.FromRgb(0, 0, 0);
            AvailablePenSizes = new int[] { 1, 2, 4, 8, 16 };

            _frameService.MFPreview.PreviewEnable("", 0, 1);
            _frameService.MFPreview.PropsSet("wpf_preview", "true");
            _frameService.MFPreview.PropsSet("wpf_preview.update", "0");
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                RaisePropertyChanged();
            }
        }
        public D3DImage PreviewSurface
        {
            get => _previewSurface;
            set
            {
                _previewSurface = value;
                RaisePropertyChanged();
            }
        }
        public Color PenColor
        {
            get => _penColor; set
            {
                _penColor = value;
                RaisePropertyChanged();
            }
        }
        public int PenSize
        {
            get => _penSize;
            set
            {
                _penSize = value;
                RaisePropertyChanged();
            }
        }
        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                RaisePropertyChanged();
            }
        }
        public IEnumerable<int> AvailablePenSizes { get; }

        public RelayCommand OpenFileDialogCommand { get; }
        public RelayCommand MouseUpCommand { get; }
        public RelayCommand MouseDownCommand { get; }
        public RelayCommand MouseMoveCommand { get; }
        public RelayCommand ClearAllCommand { get; }

        public int MouseX { get; set; }
        public int MouseY { get; set; }

        public int FontSize { get; set; }

        public int TextX
        {
            get => _textX;
            set
            {
                _textX = value;
                RaisePropertyChanged();
                UpdateOverlay();
            } 
        }

        public int TextY
        {
            get => _textY;
            set
            {
                _textY = value;
                RaisePropertyChanged();
                UpdateOverlay();
            }
        }

        public int Width
        {
            get => _panelWidth;
            set
            {
                _panelWidth = value;
                RaisePropertyChanged();
                UpdateOverlay();
            }
        }
        public int Height
        {
            get => _panelHeight;
            set
            {
                _panelHeight = value;
                RaisePropertyChanged();
                UpdateOverlay();
            }
        }

        private void UpdateOverlay()
        {
            var overlay = new MFORMATSLib.MF_RECT();
            overlay.dblWidth = Width - TextX;
            overlay.dblHeight = Height - TextY;
            overlay.dblPosX = TextX;
            overlay.dblPosY = TextY;

            overlay.eRectType = MFORMATSLib.eMFRectType.eMFRT_Absolute;

            _frameService.Overlay = overlay;
        }

        private void ClearAll()
        {
            _drawLines.Clear();
            Text = string.Empty;
        }

        private void StartPreview()
        {
            _frameService.MFPreview.OnEventSafe -= HandlePreviewEvent;
            PreviewSurface = new D3DImage();
            _frameService.MFPreview.OnEventSafe += HandlePreviewEvent;
        }

        private void HandlePreviewEvent(string bsChannelID, string bsEventName, string bsEventParam, object pEventObject)
        {
            _surfaceService.UpdateSurface(PreviewSurface, bsChannelID, bsEventName, bsEventParam, pEventObject);
            RaisePropertyChanged(nameof(PreviewSurface));
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            while (!bw.CancellationPending)
            {
                var frame = _frameService.GetFrame();

                frame = _frameService.DrawLinesOnFrame(frame, _isMouseMoving, _drawLines, Width, Height, MouseX, MouseY);

                if (Text != null)
                {
                    frame = _frameService.DrawTextOnFrame(frame, Text, FontSize);
                }

                _frameService.PreviewFrame(frame);
            }
        }

        private void OnMouseDown()
        {
            _drawingLine = true;
            _drawLines.Add(new CurveLine { PenColor = System.Drawing.Color.FromArgb(PenColor.A, PenColor.R, PenColor.G, PenColor.B), PenSize = PenSize });
        }

        private void OnMouseUp()
        {
            _drawingLine = false;
        }

        private void OnMouseMove()
        {
            if (_drawingLine)
            {
                _isMouseMoving = true;
            }
            else
            {
                _isMouseMoving = false;
            }
        }

        private void OpenFileDialog()
        {
            if (_dialogService.OpenFileDialog())
            {
                FilePath = _dialogService.FilePath;
                TryOpenFile(FilePath);
            }
        }

        private void TryOpenFile(string path)
        {
            if (_frameService.TryOpenFile(path, out string error))
            {
                StartPreview();
                if (!_backgroundWorker.IsBusy)
                {
                    _backgroundWorker.RunWorkerAsync();
                }
            }
            else
            {
                _dialogService.ShowMessage(error);
            }
        }
    }
}
