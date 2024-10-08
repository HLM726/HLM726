using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkOrder.ViewModel
{
    public class ViewModelLocator
    {

        public MainViewModel mv { get => ServiceLocator.Current.GetInstance<MainViewModel>(); }
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
        }


        public static void Cleanup<T>() where T : ViewModelBase
        {
            if (!SimpleIoc.Default.IsRegistered<T>() || !SimpleIoc.Default.ContainsCreated<T>())
            {
                return;
            }
            IEnumerable<T> allCreatedInstances = SimpleIoc.Default.GetAllCreatedInstances<T>();
            foreach (T item in allCreatedInstances)
            {
                item.Cleanup();
            }
            SimpleIoc.Default.Unregister<T>();
            SimpleIoc.Default.Register<T>();
        }
    }
}
