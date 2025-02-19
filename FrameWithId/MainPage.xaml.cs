namespace FrameWithId
{
    enum FrameId { BarcodeFrame, QRCodeFrame }
    public partial class MainPage : ContentPage
    {
        public MainPage() => InitializeComponent();

        private void OnFrameDiscovered(object sender, EventArgs e)
        {

        }
    }
}
