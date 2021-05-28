using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BudgetManager.User_controls
{
    /// <summary>
    /// Interaction logic for NavigationButtons.xaml
    /// </summary>
    public partial class NavigationButtons : UserControl
    {
        public NavigationButtons()
        {
            InitializeComponent();
            SetupButtons();
        }

        public void SetupButtons()
        {
            if (AppData.IsNotEmpty())
            {
                var min = 0;
                var current = AppData.currentPeriod;
                var max = AppData.billingPeriods.Count - 1;

                PrevButton.IsEnabled = current > min;
                NextButton.IsEnabled = current < max;
            }
            else
            {
                PrevButton.IsEnabled = NextButton.IsEnabled = false;
            }
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SaveData();
        }

        private void ButtonPrev_OnClick(object sender, RoutedEventArgs e)
        {
            AppData.currentPeriod--;
            SetupButtons();
            ((MainWindow)Window.GetWindow(this)).ChangeBillingPeriod();
        }

        private void ButtonNext_OnClick(object sender, RoutedEventArgs e)
        {
            AppData.currentPeriod++;
            SetupButtons();
            ((MainWindow)Window.GetWindow(this)).ChangeBillingPeriod();
        }

        private void ButtonRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            FilesHandler.ReadData();
            ((MainWindow)Window.GetWindow(this)).RefreshPage();
        }

        private void ScreenshotButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = ((MainWindow) Window.GetWindow(this));
            var frame = (Page)window.RootFrame.Content;
            Point relativePoint = frame.TransformToAncestor(window).Transform(new Point(0, 0));
            var dpiX = 96.0;
            var dpiY = 96.0;
            var source = PresentationSource.FromVisual(this);
            if (source != null)
            {
                dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }
            var frameRenderTargetBitmap = new RenderTargetBitmap((int)window.ActualWidth, (int)window.ActualHeight, dpiX, dpiY, PixelFormats.Pbgra32);
            frameRenderTargetBitmap.Render(window);
            
            var cropped = new CroppedBitmap(frameRenderTargetBitmap,
                new Int32Rect((int)relativePoint.X, (int)relativePoint.Y, (int)frame.ActualWidth, (int)frame.ActualHeight));

            var pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(cropped));

            using (var ms = new MemoryStream())
            {
                pngImage.Save(ms);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                Clipboard.SetImage(bi);
            }
        }
    }
}
