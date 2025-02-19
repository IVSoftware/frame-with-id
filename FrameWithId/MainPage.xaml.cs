using FrameWithIdDemo.Controls;
using System.Collections;
using System.Collections.ObjectModel;

namespace FrameWithIdDemo
{
    enum FrameId { BarcodeFrame, QRCodeFrame }
    public partial class MainPage : ContentPage
    {
        public MainPage() => InitializeComponent();

        private void OnFrameDiscovered(object sender, EventArgs e)
        {
            if(sender is FrameWithId frame)
            {
                BindingContext.Frames.Add(new DataModel { FrameId = frame.FrameId });
            }
        }
        new MainPageViewModel BindingContext => (MainPageViewModel)base.BindingContext;
    }
    class MainPageViewModel
    {
        public ObservableCollection<DataModel> Frames { get; } = new();
    }
    class DataModel
    {
        public Enum? FrameId { get; set; }
        public override string ToString() => $"{FrameId}";
    }
}
