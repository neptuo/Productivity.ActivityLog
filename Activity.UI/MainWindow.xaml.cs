using Activity.Core;
using Activity.Core.Models;
using Activity.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Activity.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string activitiesFileName = System.IO.Path.Combine(System.IO.Path.GetTempPath() + "Activities.xml");

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private ActivityProvider provider = null;
        private ActivityService service = null;
        private MainViewModel viewModel = null;

        public MainWindow()
        {
            InitializeComponent();
            Window_SizeChanged(this);

            service = new ActivityService();
            service.Load(activitiesFileName);

            provider = new ActivityProvider();
            provider.WindowChanged += (sender, e) =>
            {
                DispatcherOperation op = Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new Action<ActiveWindowChangedEventArgs>(OnWindowChanged),
                    e
                );

                DispatcherOperationStatus status = op.Status;
                while (status != DispatcherOperationStatus.Completed)
                    status = op.Wait(TimeSpan.FromMilliseconds(1000));
            };
            provider.Start();

            viewModel = new MainViewModel(service);
            CreateTrayIcon(viewModel);

            DataContext = viewModel;
        }

        private void CreateTrayIcon(MainViewModel viewModel)
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = "Activity";
            notifyIcon.Visible = true;
            notifyIcon.Icon = IconHelper.GetCurrentIcon(this);
            notifyIcon.Click += new EventHandler(delegate
            {
                Show();
                Activate();
            });

            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "CurrentActivity")
                    SetTrayStatus(viewModel.CurrentActivity);
            };
            provider.WindowChanged += (sender, e) =>
            {
                SetTrayStatus(service.GetActivity(e.CurrentProcess));
            };
        }

        private void SetTrayStatus(ActivityModel model)
        {
            notifyIcon.Text = String.Format("{0} -> {1}",
                System.IO.Path.GetFileName(model.ProcessFileName),
                viewModel.CurrentActivity.UsedTime
            );
        }

        private void OnWindowChanged(ActiveWindowChangedEventArgs args)
        {
            service.ProcessActivity(args);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            provider.Stop();
            service.Save(activitiesFileName);
            notifyIcon.Visible = false;
        }

        private void lvwModels_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvwModels.SelectedItem != null)
                provider.SetCurrentWindow((lvwModels.SelectedItem as ActivityModel).WindowHandle);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e = null)
        {
            Left = SystemParameters.WorkArea.Width - (e != null ? e.NewSize.Width : Width);
            Top = SystemParameters.WorkArea.Height - (e != null ? e.NewSize.Height : Height);
        }

        private void mniShowHide_Click(object sender, RoutedEventArgs e)
        {
            ActivityModel model = lvwModels.SelectedItem as ActivityModel;
            if (model != null)
            {
                model.IsHidden = !model.IsHidden;
                viewModel.UpdateFilter(service);
            }
        }
    }
}
