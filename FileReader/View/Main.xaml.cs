using AlphaChiTech.Virtualization;
using System;
using System.Windows;
using System.Windows.Threading;

namespace FileReader.View
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            //this routine only needs to run once, so first check to make sure the
            //VirtualizationManager isn’t already initialized
            if (!VirtualizationManager.IsInitialized)
            {
               
                //set the VirtualizationManager’s UIThreadExcecuteAction. In this case
                //we’re using Dispatcher.Invoke to give the VirtualizationManager access
                //to the dispatcher thread, and using a DispatcherTimer to run the background
                //operations the VirtualizationManager needs to run to reclaim pages and manage memory.
                VirtualizationManager.Instance.UIThreadExcecuteAction = a => Dispatcher.Invoke(a);
                new DispatcherTimer(TimeSpan.FromSeconds(1),
                                     DispatcherPriority.Background,
                                     delegate
                                     {
                                         VirtualizationManager.Instance.ProcessActions();
                                     },
                                     Dispatcher).Start();
            }
            InitializeComponent();
        }
    }
}
