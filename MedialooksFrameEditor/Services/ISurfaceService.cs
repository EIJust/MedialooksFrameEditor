using System;
using System.Windows.Interop;

namespace MedialooksFrameEditor.Models
{
    public interface ISurfaceService
    {
        IntPtr SavedSurfaceIUnk { get; }

        void UpdateSurface(D3DImage previewSurface, string bsChannelID, string bsEventName, string bsEventParam, object pEventObject);
    }
}
