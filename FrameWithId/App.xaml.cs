﻿namespace FrameWithIdDemo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            if (Application.Current is not null && DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                var window = new Window(new AppShell());
                window.Width = 540;
                window.Height = 960;

                // Ensure window resizing completes
                window.Dispatcher.DispatchAsync(() => { }).GetAwaiter().OnCompleted(() =>
                {
                    var disp = DeviceDisplay.Current.MainDisplayInfo;
                    window.X = (disp.Width / disp.Density - window.Width) / 2;
                    window.Y = (disp.Height / disp.Density - window.Height) / 2;
                });
                return window;
            }
            else
            {
                return new Window(new AppShell());
            }
        }
    }
}