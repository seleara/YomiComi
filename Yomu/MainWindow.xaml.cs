using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using SharpCompress.Archives;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Yomu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow :  Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Bookmarks bookmarks;

        private List<IArchiveEntry> pageEntries;

        public struct YomuPage
        {
            public string FileName;
            public IArchiveEntry ArchiveEntry;
        }

        private List<YomuPage> pages;
        int currentPage = 0;

        IArchive archive;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        string currentFilename;

        public int CurrentPage
        {
            get
            {
                return currentPage + 1;
            }
            set
            {
                currentPage = value - 1;
                PageCountDisplay.CurrentPage = CurrentPage;
                LoadPages();
                OnPropertyChanged();
            }
        }

        private int pageCount;
        public int PageCount
        {
            get
            {
                return pageCount;
            }
            private set
            {
                pageCount = value;
                PageCountDisplay.PageCount = PageCount;
                OnPropertyChanged();
            }
        }

        public bool OnePageView
        {
            get
            {
                return !ImageCanvas.TwoPageView;
            }
            set
            {
                ImageCanvas.TwoPageView = !value;
                OnPropertyChanged();
                OnPropertyChanged("TwoPageView");
            }
        }

        public bool TwoPageView
        {
            get
            {
                return ImageCanvas.TwoPageView;
            }
            set
            {
                ImageCanvas.TwoPageView = value;
                OnPropertyChanged();
                OnPropertyChanged("OnePageView");
            }
        }

        public bool RightToLeft
        {
            get
            {
                return ImageCanvas.RightToLeft;
            }
            set
            {
                ImageCanvas.RightToLeft = value;
                OnPropertyChanged();
                OnPropertyChanged("LeftToRight");
            }
        }

        public bool LeftToRight
        {
            get
            {
                return !ImageCanvas.RightToLeft;
            }
            set
            {
                ImageCanvas.RightToLeft = !value;
                OnPropertyChanged();
                OnPropertyChanged("RightToLeft");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            
            DataContext = this;
            bookmarks = new Bookmarks();
        }

        private BitmapImage GetEntryAsImage(IArchiveEntry entry)
        {
            //var s = entry.OpenEntryStream();
            Console.WriteLine(entry.Key);

            //byte[] buffer = new byte[entry.Size];
            //s.Read(buffer, 0, (int)entry.Size);

            var img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = new MemoryStream();
            entry.WriteTo(img.StreamSource);
            img.StreamSource.Position = 0;
            img.EndInit();
            //s.Close();
            return img;
        }

        private BitmapImage GetPageAsImage(YomuPage page)
        {
            Console.WriteLine(page.FileName);

            BitmapImage img;
            if (archive == null)
            {
                img = new BitmapImage(new Uri(page.FileName));
            }
            else
            {
                img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                var entry = page.ArchiveEntry;
                img.StreamSource = new MemoryStream();
                entry.WriteTo(img.StreamSource);
                img.StreamSource.Position = 0;
                img.EndInit();
            }
            return img;
        }

        bool canvasMouseLeftDown = false;
        bool canvasMouseRightDown = false;
        bool dragging = false;
        Point oldMousePosition = new Point(0, 0);

        private void LoadPages()
        {
            /*if (currentPage >= PageCount)
            {
                currentPage = PageCount - 1;
                return;
            }
            var e0 = pageEntries[currentPage];//archive.Entries.ElementAt(currentPage);
            Console.WriteLine(e0.Key);
            var img1 = GetEntryAsImage(e0);
            ImageCanvas.Page1 = img1;
            if (currentPage + 1 < PageCount)
            {
                var e1 = pageEntries[currentPage + 1];//archive.Entries.ElementAt(currentPage + 1);
                Console.WriteLine(e1.Key);
                var img2 = GetEntryAsImage(e1);
                ImageCanvas.Page2 = img2;
            }
            else
            {
                //var img2 = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgra32, null, new byte[] { 255, 255, 255, 255 }, 4);
                // var tb = new TransformedBitmap(img2, new ScaleTransform(img1.PixelWidth, img1.PixelHeight));
                ImageCanvas.Page2 = null; // tb;
            }
            ScrollViewer.ScrollToTop();
            bookmarks.SetBookmarkedPage(currentFilename, currentPage);
            OnPropertyChanged("CurrentPage");*/
            if (currentPage >= PageCount)
            {
                currentPage = PageCount - 1;
                return;
            }
            var p0 = pages[currentPage];
            Console.WriteLine(p0.FileName);
            var img1 = GetPageAsImage(p0);
            ImageCanvas.Page1 = img1;
            if (currentPage + 1 < PageCount)
            {
                var p1 = pages[currentPage + 1];
                Console.WriteLine(p1.FileName);
                var img2 = GetPageAsImage(p1);
                ImageCanvas.Page2 = img2;
            }
            else
            {
                //var img2 = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgra32, null, new byte[] { 255, 255, 255, 255 }, 4);
                // var tb = new TransformedBitmap(img2, new ScaleTransform(img1.PixelWidth, img1.PixelHeight));
                ImageCanvas.Page2 = null; // tb;
            }
            ScrollViewer.ScrollToTop();
            bookmarks.SetBookmarkedPage(currentFilename, currentPage);
            OnPropertyChanged("CurrentPage");
        }

        private void ImageCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvasMouseLeftDown = true;
            oldMousePosition = Mouse.GetPosition(ScrollViewer);
            ImageCanvas.CaptureMouse();
        }

        private void TwoPageViewChecked(object sender, RoutedEventArgs e)
        {

        }

        private void OnePageViewChecked(object sender, RoutedEventArgs e)
        {
            
        }

        private void ImageCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!canvasMouseLeftDown) return;
            canvasMouseLeftDown = false;
            ImageCanvas.ReleaseMouseCapture();
            if (!dragging && PageCount != 0)
            {
                currentPage += TwoPageView ? 2 : 1;
                LoadPages();
            }
            dragging = false;
            Cursor = Cursors.Arrow;
        }

        private void ImageCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvasMouseRightDown = true;
        }

        private void ImageCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!canvasMouseRightDown) return;
            canvasMouseRightDown = false;
            if (PageCount != 0)
            {
                //if (ImageCanvas.TwoPageView)
                currentPage -= TwoPageView ? 2 : 1;
                if (currentPage < 0) currentPage = 0;
                /*var e0 = archive.Entries.ElementAt(++currentPage);
                Console.WriteLine(e0.Key);
                var img1 = GetEntryAsImage(e0);
                ImageCanvas.Page1 = img1;
                var e1 = archive.Entries.ElementAt(++currentPage);
                Console.WriteLine(e1.Key);
                var img2 = GetEntryAsImage(e1);
                ImageCanvas.Page2 = img2;
                ScrollViewer.ScrollToTop();
                CurrentPage = currentPage + 1;*/
                LoadPages();
            }
        }

        private void OpenArchive_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            bool? result = ofd.ShowDialog();
            if (result != true) return;
            string filename = ofd.FileName;
            Title = "YomiComi - " + filename;
            var stream = File.OpenRead(filename);
            archive = ArchiveFactory.Open(stream);
            pages = new List<YomuPage>();
            
            foreach (var entry in archive.Entries.OrderBy(entry => entry.Key)) {
                if (entry.IsDirectory)
                {
                    continue;
                }
                var ext = entry.Key.Substring(entry.Key.Length - 4);
                if (ext != ".jpg" && ext != ".png" && ext != ".bmp" && ext != ".gif") // Add more later?
                {
                    continue;
                }
                //pageEntries.Add(entry);
                var page = new YomuPage();
                page.FileName = entry.Key;
                page.ArchiveEntry = entry;
                pages.Add(page);
            }

            PageCount = pages.Count;
            //PageSlider.Ticks
            currentFilename = filename;
            currentPage = bookmarks.GetBookmarkedPage(currentFilename);
            LoadPages();
        }

        private void OpenFolder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var ofd = new CommonOpenFileDialog();
            ofd.IsFolderPicker = true;
            var result = ofd.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
            {
                return;
            }
            string dir = ofd.FileName;
            Title = "YomiComi - " + dir;

            archive = null;
            pages = new List<YomuPage>();

            var files = Directory.EnumerateFiles(dir);
            foreach (string file in files)
            {
                string filename = file; //.Substring(dir.Length + 1);
                var page = new YomuPage();
                page.FileName = filename;
                pages.Add(page);
            }
            PageCount = pages.Count;
            currentFilename = dir;
            currentPage = bookmarks.GetBookmarkedPage(currentFilename);

            LoadPages();
        }

        private void Quit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void Config_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var cw = new ConfigWindow();
            cw.ShowDialog();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Vector diff = Mouse.GetPosition(ScrollViewer) - oldMousePosition;
            if (dragging || (canvasMouseLeftDown && diff.Length > 4))
            {
                dragging = true;
                Cursor = Cursors.ScrollAll;
                diff = Mouse.GetPosition(ScrollViewer) - oldMousePosition;
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - diff.Y * Config.Instance.DragSpeed);
                oldMousePosition = Mouse.GetPosition(ScrollViewer);
            }
        }

        private void Maximize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void Minimize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                var border = (Border)Template.FindName("WindowBorder", this);
                border.BorderThickness = new Thickness(8);
            }
            else
            {
                var border = (Border)Template.FindName("WindowBorder", this);
                border.BorderThickness = new Thickness(1);
            }
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr mWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowProc));
        }

        private static System.IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            IntPtr lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            MONITORINFO lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
            {
                return;
            }

            IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

            MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            if (lPrimaryScreen.Equals(lCurrentScreen) == true)
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            else
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
            }

            Marshal.StructureToPtr(lMmi, lParam, true);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }


        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }
    }
}
