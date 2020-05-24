using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using MedialooksFrameEditor.Models;
using MedialooksFrameEditor.Services;

namespace MedialooksFrameEditor.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();

            SimpleIoc.Default.Register<IDialogService, DefaultDialogService>();
            SimpleIoc.Default.Register<IFrameService, FrameService>();
            SimpleIoc.Default.Register<ISurfaceService, SurfaceService>();
        }

        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
    }
}
