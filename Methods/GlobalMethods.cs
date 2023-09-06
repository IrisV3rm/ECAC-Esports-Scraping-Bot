using System.Drawing;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Xml;
using System.Windows.Documents;
using Image = System.Windows.Controls.Image;

namespace ECAC_eSports_Scraper.Methods
{
    public static class GlobalMethods
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);


        public static async void SetElementText(Run element, string text)
        {
            await element.Dispatcher.InvokeAsync(() =>
            {
                element.Text = text;
            });
        }

        public static async void SetImageSource(Image image, BitmapImage source)
        {
            await image.Dispatcher.InvokeAsync(() =>
            {
                source.Dispatcher.Invoke(() =>
                {
                    image.Source = source;
                });
            });
        }

        public static void SetRunText(Run element, string text) => SetElementText(element, text);

        public static void AddRunText(Run element, string text)
        {
            element.Dispatcher.Invoke(() =>
            {
                element.Text += text;
            });
        }

        public static void SetIconVisible(UIElement icon, bool visible)
        {
            icon.Dispatcher.Invoke(() =>
            {
                icon.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            });
        }

        public static T CloneControl<T>(this T source) where T : FrameworkElement
        {
            using StringReader stringReader = new (XamlWriter.Save(source));
            using XmlReader xmlReader = XmlReader.Create(stringReader);
            return (T)XamlReader.Load(xmlReader);
        }


        public static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            IntPtr handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
    }
}
