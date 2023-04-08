using Models;
using PropertyGrid.WPF.Demo.Infrastructure;
using SoftFluent.Windows;
using System.Windows;

namespace Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            SQLitePCL.Batteries.Init();
            AutoObject.PropertyStore = PropertyStore.Instance;
            Collection.Context = System.Threading.SynchronizationContext.Current;        
            base.OnStartup(e);
        }
    }
}
