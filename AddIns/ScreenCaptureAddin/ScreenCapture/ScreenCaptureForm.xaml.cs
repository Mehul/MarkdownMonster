﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Gma.System.MouseKeyHook;
using MahApps.Metro.Controls;
using MarkdownMonster;
using MarkdownMonster.Windows;
using ScreenCaptureAddin;
using SnagItAddin.Annotations;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Point = System.Windows.Point;
using Timer = System.Threading.Timer;

namespace SnagItAddin
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class ScreenCaptureForm : MetroWindow, INotifyPropertyChanged

    {
        #region Externally accessible capture interface

        public ScreenCaptureConfiguration Configuration { get; set; }

        /// <summary>
        /// Determines whether the capture was cancelled
        /// </summary>
        public bool Cancelled { get; set; } = true;

        /// <summary>
        /// The file name of the actual file written disk
        /// </summary>
        public string SavedImageFile { get; set; }


        /// <summary>
        /// A Window that should be hidden when the capture
        /// is performed.
        /// </summary>
        public Window ExternalWindow { get; set; }

        /// <summary>
        /// If set this window's handle is minimized
        /// (not implemented at the moment)
        /// </summary>
        public IntPtr ExternalWindowHandleToHide { get; set; } = IntPtr.Zero;


        /// <summary>
        /// A filename to a file that is written with the 
        /// output file or the string "Cancelled"
        /// </summary>
        public string ResultFilePath { get; set; }

        /// <summary>
        /// The folder where files should be saved. Input
        /// parameter only.
        /// </summary>
        public string SaveFolder { get; set; }

        /// <summary>
        /// Determines if the Save Operation closes the dialog
        /// </summary>
        public bool AutoClose { get; set; } = true;

        public int CaptureDelaySeconds
        {
            get { return _captureDelaySeconds; }
            set
            {
                if (value == _captureDelaySeconds) return;
                _captureDelaySeconds = value;
                OnPropertyChanged();
            }
        }
        private int _captureDelaySeconds;



        public bool IncludeCursor
        {
            get { return _includeCursor; }
            set
            {
                if (value == _includeCursor) return;
                _includeCursor = value;
                OnPropertyChanged();
            }
        }
        private bool _includeCursor;

        
        

        #endregion

        #region internal Interface

        private Bitmap CapturedBitmap
        {
            get { return _capturedBitmap; }
            set
            {
                _capturedBitmap = value;
                bool b = this.IsBitmap;
            }
        }
        private Bitmap _capturedBitmap;


        private bool IsBitmap
        {
            get
            {
                bool isBitmap = CapturedBitmap != null;
                if (ToolButtonSaveImage != null)
                {
                    ToolButtonSaveImage.IsEnabled = isBitmap;
                    ToolButtonSaveAndEdit.IsEnabled = isBitmap;
                }
                return isBitmap;
            }
        }

        ScreenClickOverlay Overlay;
        ScreenOverlayDesktop Desktop;

        bool IsMouseClickCapturing = false;
        bool IsPreviewCapturing = false;


        private IntPtr WindowHandle = IntPtr.Zero;
        Timer CaptureTimer = null;

        // Keep track of captured window that the mouse is over
        WindowInfo LastWindow = null;
        WindowInfo CurWindow = null;
        
        private IKeyboardMouseEvents GlobalMouseHandler;

        #endregion


        #region Startup and Shutdown

        public ScreenCaptureForm()
        {
            mmApp.SetTheme(mmApp.Configuration.ApplicationTheme);
            
            InitializeComponent();
            
            Loaded += ScreenCaptureForm_Loaded;
            Unloaded += ScreenCaptureForm_Unloaded;
            SizeChanged += ScreenCaptureForm_SizeChanged;
            KeyDown += ScreenCaptureForm_KeyDown;

            DataContext = this;            
        }

        private void ScreenCaptureForm_Loaded(object sender, RoutedEventArgs e)
        {
            CaptureDelaySeconds = ScreenCaptureConfiguration.Current.CaptureDelaySeconds;
            IncludeCursor = ScreenCaptureConfiguration.Current.IncludeCursor;

            GlobalMouseHandler = Hook.GlobalEvents();
            GlobalMouseHandler.MouseClick += GlobalMouseHandlerMouseDown;
            GlobalMouseHandler.KeyDown += GlobalKeyHandlerKeyDown;

            CapturedBitmap = null;
            WindowHandle = new WindowInteropHelper(this).Handle;
        }


        private void ScreenCaptureForm_Unloaded(object sender, RoutedEventArgs e)
        {
            Overlay?.Close();
            CapturedBitmap?.Dispose();
            CaptureTimer?.Dispose();

            GlobalMouseHandler.MouseClick -= GlobalMouseHandlerMouseDown;
            GlobalMouseHandler.KeyDown -= GlobalKeyHandlerKeyDown;
            GlobalMouseHandler.Dispose();
            GlobalMouseHandler = null;

            if (WindowState == WindowState.Normal)
            {
                ScreenCaptureConfiguration.Current.WindowHeight = Height;
                ScreenCaptureConfiguration.Current.WindowWidth = Width;
                ScreenCaptureConfiguration.Current.Write();
            }
        }

        private void ScreenCaptureForm_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var image = ImageCaptured.Source as BitmapSource;
            if (image == null)
                return;

            if (image.Width < Width - 20 && image.Height < PageGrid.RowDefinitions[2].ActualHeight)
                ImageCaptured.Stretch = Stretch.None;
            else
                ImageCaptured.Stretch = Stretch.Uniform;

        }

        #endregion

        #region Capture Operations     
        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        
        private double SavedTop;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        internal static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }



        private void ButtonCapture_Click(object sender, EventArgs e)
        {
            StartCapture();
        }


        void StartCapture()
        {
            Hide();
            ExternalWindow?.Hide();

            StatusImageSize.Text = "";

            // make sure windows actually hides before we wait
            WindowUtilities.DoEvents();

            // Display counter
            if (CaptureDelaySeconds > 0)
            {
                IsPreviewCapturing = true;
                Cancelled = false;

                var counterForm = new ScreenOverlayCounter();

                try
                {
                    counterForm.Show();
                    counterForm.Topmost = true;
                    counterForm.SetWindowText("1");

                    for (int i = CaptureDelaySeconds; i > 0; i--)
                    {
                        counterForm.SetWindowText(i.ToString());
                        WindowUtilities.DoEvents();

                        for (int j = 0; j < 100; j++)
                        {
                            Thread.Sleep(10);
                            WindowUtilities.DoEvents();
                        }
                        if (Cancelled)
                        {
                            CancelCapture();
                            return;
                        }
                    }
                }
                finally
                {
                    counterForm.Close();
                    IsPreviewCapturing = false;
                    Cancelled = true;
                }
            }
            
            IsMouseClickCapturing = true;
            
            
            Desktop = new ScreenOverlayDesktop(this);
            Desktop.SetDesktop(IncludeCursor);
            Desktop.Show();

            WindowUtilities.DoEvents();

            Overlay = new ScreenClickOverlay
            {
                Width = 0,
                Height = 0
            };
            Overlay.Show();

            LastWindow = null;
            CaptureTimer = new Timer(Capture, null, 0, 100);
        }

        

        private void GlobalMouseHandlerMouseDown(object sender, MouseEventArgs e)
        {                        
            if (IsMouseClickCapturing)
                StopCapture();
        }

        private void GlobalKeyHandlerKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            bool cancel = e.KeyCode == Keys.Escape;
            if (IsPreviewCapturing)
            {                
                Cancelled = cancel;
                return;
            }
            
            StopCapture(cancel);
        }

        internal void StopCapture(bool cancelCapture = false)
        {
            if (!IsMouseClickCapturing)
                return;

            IsMouseClickCapturing = false;

            Overlay?.Close();
            WindowUtilities.DoEvents();

            Desktop.Topmost = true;
            Desktop.Activate();            
            WindowUtilities.DoEvents();
            
            if (LastWindow != null)
            {
                CapturedBitmap = ScreenCapture.CaptureWindowBitmap(CurWindow.Handle);
                ImageCaptured.Source = ScreenCapture.BitmapToBitmapSource(CapturedBitmap);
                StatusText.Text = "Image capture from Screen: " + $"{CapturedBitmap.Width}x{CapturedBitmap.Height}";
                ScreenCaptureForm_SizeChanged(this, null);
            }

            //Desktop.Topmost = false;
            Desktop?.Close();

            if (ExternalWindow != null)
            {
                ExternalWindow.Show();
                ExternalWindow.Activate();
            }


            if (cancelCapture)
                CancelCapture();
            else
            {
                Show();
                Activate();
            }
        }




        void Capture(object obj)
        {
            Point pt = GetMousePosition();
            CurWindow = new WindowInfo(ScreenCapture.WindowFromPoint(new System.Drawing.Point((int) pt.X, (int) pt.Y)));

            this.Invoke(() =>
            {
                if (LastWindow == null || !CurWindow.Handle.Equals(LastWindow.Handle))
                {
                    if (CurWindow.Handle != WindowHandle &&
                        CurWindow.Rect.Width <= Screen.FromHandle(CurWindow.Handle).Bounds.Width &&
                        // don't capture dual window desktop
                        Overlay != null)
                    {
                        Overlay.Left = CurWindow.Rect.X;
                        Overlay.Top = CurWindow.Rect.Y;
                        Overlay.Width = CurWindow.Rect.Width;
                        Overlay.Height = CurWindow.Rect.Height;
                        Overlay.SetWindowText($"{Overlay.Width}x{Overlay.Height}");
                    }
                }

                LastWindow = CurWindow;
            });
        }

        private void Hide()
        {
            SavedTop = Top;
            Top = -100000;
        }

        private void Show()
        {
            Top = SavedTop;
        }

        private void CancelCapture()
        {
            if (!string.IsNullOrEmpty(ResultFilePath))
                File.WriteAllText(ResultFilePath, "Cancelled");

            Cancelled = true;
            Close();

            ExternalWindow?.Activate();
        }

        #endregion

        #region ButtonHandlers

        private void tbCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelCapture();
        }

        private void tbSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SaveFolder))
                SaveFolder = Path.GetTempPath();

            SaveFileDialog sd = new SaveFileDialog
            {
                Filter = "png files (*.png)|*.png|jpg files (*.jpg)|*.jpg",
                FilterIndex = 1,
                FileName = "",
                CheckFileExists = false,
                OverwritePrompt = false,
                AutoUpgradeEnabled = true,
                CheckPathExists = true,
                InitialDirectory = SaveFolder,
                RestoreDirectory = true
            };
            var result = sd.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK)
                return;

            SavedImageFile = sd.FileName;
            try
            {

                CapturedBitmap?.Save(SavedImageFile);
            }
            catch (Exception ex)
            {
                Cancelled = true;
                StatusText.Text = "Error saving image: " + ex.Message;
                return;
            }

            if (!string.IsNullOrEmpty(ResultFilePath))
                File.WriteAllText(ResultFilePath, SavedImageFile);

            Cancelled = false;

            if (AutoClose)
                Close();

            ExternalWindow?.Activate();

            if (WindowState != WindowState.Maximized && WindowState != WindowState.Minimized)
            {
                ScreenCaptureConfiguration.Current.WindowWidth = this.Width;
                ScreenCaptureConfiguration.Current.WindowHeight = this.Width;
            }
        }

        private void tbSaveAndEdit_Click(object sender, RoutedEventArgs e)
        {
            tbSave_Click(sender, e);
            if (!Cancelled)
            {
                var exe = ScreenCaptureConfiguration.Current.ImageEditorPath;
                if (!File.Exists(exe))
                    exe = Path.Combine(Environment.SystemDirectory, "mspaint.exe");
                var process = Process.Start(new ProcessStartInfo(exe, SavedImageFile));
            }


        }


        private void tbCaptureDesktop_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            ExternalWindow?.Hide();

            WindowUtilities.DoEvents();

            var screen = Screen.FromHandle(WindowHandle);
            var img = ScreenCapture.CaptureWindow(screen.Bounds);
            ImageCaptured.Source = ScreenCapture.ImageToBitmapSource(img);

            CapturedBitmap?.Dispose();
            CapturedBitmap = new Bitmap(img);

            StatusText.Text = $"Desktop captured Image: {CapturedBitmap.Width}x{CapturedBitmap.Height}";
            ExternalWindow?.Show();

            this.Topmost = true;
            this.Show();
            WindowUtilities.DoEvents();
        }


        private void tbPasteImage_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Clipboard.ContainsImage())
            {
                CapturedBitmap?.Dispose();
                CapturedBitmap = new Bitmap(System.Windows.Forms.Clipboard.GetImage());
                ImageCaptured.Source = ScreenCapture.BitmapToBitmapSource(CapturedBitmap);
                StatusText.Text = $"Pasted Image from Clipboard: {CapturedBitmap.Width}x{CapturedBitmap.Height}";

                ScreenCaptureForm_SizeChanged(this, null);
            }
        }

        private void
            tbClearImage_Click(object sender, RoutedEventArgs e)
        {
            CapturedBitmap?.Dispose();
            ImageCaptured.Source = null;
        }


        private void ScreenCaptureForm_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.V &&
                (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                tbPasteImage_Click(this, null);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

