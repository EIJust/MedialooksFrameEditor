using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MedialooksFrameEditor.Services;
using MFORMATSLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MedialooksFrameEditor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly BackgroundWorker _backgroundWorker;
        private readonly IDialogService _dialogService;
        private readonly IFrameLoader _frameLoader;
        private readonly MFPreviewClass _mfPreview;

        private string _filePath;
        private bool _enablePlayPause;
        public D3DImage _previewSurface;
        private IntPtr _pSavedSurfaceIUnk;
        private List<CurveLine> m_listDrawLines;
        private CurveDraw m_thePainter;
        private MF_RECT m_rcOverlay;

        private bool m_bMouseMoving;
        private int m_nMousePosX;
        private int m_nMousePosY;
        private bool m_bDraw;

        public MainViewModel(IDialogService dialogService, IFrameLoader frameLoader)
        {
            _dialogService = dialogService;
            _frameLoader = frameLoader;

            OpenFileDialogCommand = new RelayCommand(OpenFileDialog);
            MouseDownCommand = new RelayCommand(panel1_MouseDown);
            MouseUpCommand = new RelayCommand(panel1_MouseUp);
            MouseMoveCommand = new RelayCommand(panel1_MouseMove);

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += WorkerDoWork;

            _mfPreview = new MFPreviewClass();

            m_listDrawLines = new List<CurveLine>();

            m_thePainter = new CurveDraw(m_listDrawLines);

            _mfPreview.PreviewEnable("", 0, 1);
            _mfPreview.PropsSet("wpf_preview", "true");
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

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                RaisePropertyChanged();
            }
        }

        public bool EnablePlayPause
        {
            get => _enablePlayPause;
            set
            {
                _enablePlayPause = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OpenFileDialogCommand { get; }
        public RelayCommand MouseUpCommand { get; }
        public RelayCommand MouseDownCommand { get; }
        public RelayCommand MouseMoveCommand { get; }
        
        public int PanelX { get; set; }
        public int PanelY { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        private void StartLiveObject()
        {
            _mfPreview.OnEventSafe -= ObjPreview_OnEvent;
            PreviewSurface = new D3DImage();
            _mfPreview.OnEventSafe += ObjPreview_OnEvent;
        }

        private void ObjPreview_OnEvent(string bsChannelID, string bsEventName, string bsEventParam, object pEventObject)
        {
            // specific name for event is "wpf_nextframe"
            if (bsEventName == "wpf_nextframe")
            {
                // it is necessary to keep a pointer in memory cause in case of format or source changes the pointer can be changed too
                IntPtr pSurfaceIUnk = Marshal.GetIUnknownForObject(pEventObject);
                if (pSurfaceIUnk != IntPtr.Zero)
                {
                    if (pSurfaceIUnk != _pSavedSurfaceIUnk)
                    {

                        // Release prev object
                        if (_pSavedSurfaceIUnk != IntPtr.Zero)
                        {
                            Marshal.Release(_pSavedSurfaceIUnk);
                            _pSavedSurfaceIUnk = IntPtr.Zero;
                        }

                        // here we change back buffer of the surface (only in case of the pointer is changed)
                        _pSavedSurfaceIUnk = pSurfaceIUnk;
                        Marshal.AddRef(_pSavedSurfaceIUnk);

                        // Lock and Unlock are required for update of the surface
                        PreviewSurface.Lock();
                        PreviewSurface.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                        PreviewSurface.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _pSavedSurfaceIUnk);
                        PreviewSurface.Unlock();

                        // use this 3D surface as source for preview control
                        RaisePropertyChanged(nameof(PreviewSurface));
                        //GC.Collect();
                    }
                    //else {

                    //    PreviewSurface.Lock();
                    //    PreviewSurface.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                    //    PreviewSurface.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _pSavedSurfaceIUnk);
                    //    PreviewSurface.Unlock();
                    //}

                    Marshal.Release(pSurfaceIUnk);
                }

                Marshal.ReleaseComObject(pEventObject);

                // PreviewSurface of preview rectangle
                PreviewSurface.Lock();
                try
                {
                    PreviewSurface.AddDirtyRect(new Int32Rect(0, 0, PreviewSurface.PixelWidth, PreviewSurface.PixelHeight));
                }
                catch (Exception)
                {
                    PreviewSurface.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _pSavedSurfaceIUnk);
                }
                PreviewSurface.Unlock();
            }
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            while (!bw.CancellationPending)
            {
                var frame = _frameLoader.GetFrame();
                var rcOverlay = m_rcOverlay;

                if (!String.IsNullOrEmpty("test"))
                {
                    // Clone frame before overlay 
                    MFFrame mFrameClone = null;
                    try
                    {
                        frame.MFClone(out mFrameClone, eMFrameClone.eMFC_Full, eMFCC.eMFCC_Default);

                        mFrameClone.MFPrint("test", 8, ref rcOverlay, eMFTextFlags.eMFT_WordBreaks, "");
                        Marshal.ReleaseComObject(frame);
                        frame = mFrameClone;
                    }
                    catch (System.Exception)
                    {
                        if (mFrameClone != null)
                            Marshal.ReleaseComObject(mFrameClone);
                    }
                }


                if (m_listDrawLines.Any())
                {
                    MFFrame mFrameDraw = m_thePainter.DrawFrame(m_bMouseMoving, frame, Width, Height, m_nMousePosX, m_nMousePosY);
                    Marshal.ReleaseComObject(frame);
                    frame = mFrameDraw;
                }

                _mfPreview.ReceiverFramePut(frame, -1, "");
            }
        }

        private void panel1_MouseDown()
        {
            m_bDraw = true;
            m_listDrawLines.Add(new CurveLine { PenColor = Color.Red, PenSize = 1 });
        }

        private void panel1_MouseUp()
        {
            m_bDraw = false;
            if (m_listDrawLines.Any())
            {
                //ClearAll.Enabled = true;
                //UndoButton.Enabled = true;
            }
        }

        private void panel1_MouseMove()
        {
            if (m_bDraw)
            {
                m_nMousePosX = PanelX;
                m_nMousePosY = PanelY;
                m_bMouseMoving = true;
            }
            else
            {
                m_bMouseMoving = false;
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
            if (_frameLoader.TryOpenFile(path, out string error))
            {
                EnablePlayPause = true;

                StartLiveObject();
                _backgroundWorker.RunWorkerAsync();
            }
            else
            {
                _dialogService.ShowMessage(error);
            }
        }
    }
}
