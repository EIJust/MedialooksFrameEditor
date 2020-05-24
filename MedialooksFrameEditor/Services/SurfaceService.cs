using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MedialooksFrameEditor.Models
{
    public class SurfaceService : ISurfaceService
    {
        private IntPtr _pSavedSurfaceIUnk;

        public void UpdateSurface(D3DImage previewSurface, string bsChannelID, string bsEventName, string bsEventParam, object pEventObject)
        {
            if (bsEventName == "wpf_nextframe")
            {
                IntPtr pSurfaceIUnk = Marshal.GetIUnknownForObject(pEventObject);
                if (pSurfaceIUnk != _pSavedSurfaceIUnk)
                {
                    if (_pSavedSurfaceIUnk != IntPtr.Zero)
                        Marshal.Release(_pSavedSurfaceIUnk);

                    _pSavedSurfaceIUnk = pSurfaceIUnk;
                    Marshal.AddRef(_pSavedSurfaceIUnk);

                    previewSurface.Lock();
                    previewSurface.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                    previewSurface.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _pSavedSurfaceIUnk);
                    previewSurface.Unlock();
                }

                if (pSurfaceIUnk != IntPtr.Zero)
                    Marshal.Release(pSurfaceIUnk);

                previewSurface.Lock();
                previewSurface.AddDirtyRect(new Int32Rect(0, 0, previewSurface.PixelWidth, previewSurface.PixelHeight));
                previewSurface.Unlock();
            }

            Marshal.ReleaseComObject(pEventObject);
        }
    }
}
