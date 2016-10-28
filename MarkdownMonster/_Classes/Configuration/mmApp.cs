﻿using System;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Westwind.Utilities;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownMonster
{

    /// <summary>
    /// Application class for Markdown Monster that provides
    /// a global static placeholder for configuration and some
    /// utility functions
    /// </summary>
    public class mmApp
    {

        /// <summary>
        /// Holds a static instance of the application's configuration settings
        /// </summary>
        public static ApplicationConfiguration Configuration { get; set;  }


        /// <summary>
        /// The full name of the application displayed on toolbar and dialogs
        /// </summary>
        public static string ApplicationName { get; set; } = "Markdown Monster";

        public static DateTime Started { get; set;  }


        /// <summary>
        /// Static constructor to initialize configuration
        /// </summary>
        static mmApp()
        {
            Configuration = new ApplicationConfiguration();
            Configuration.Initialize();            
        }


        /// <summary>
        /// Logs exceptions in the applications
        /// </summary>
        /// <param name="ex"></param>
        public static void Log(Exception ex)
        {
            ex = ex.GetBaseException();
            var msg = ex.Message;

            Log(ex.Message,ex);
        }

        /// <summary>
        /// Logs messages to the log file
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string msg, Exception ex = null)
        {
            string exMsg = string.Empty;
            if (ex != null)
            {
                ex = ex.GetBaseException();
                exMsg = "\r\n" + ex.Message +
                        "\r\n---\r\n" + ex.Source +
                        "\r\n" + ex.StackTrace;

                SendBugReport(ex);
            }

            var text = msg +
                       exMsg +
                       "\r\n\r\n---------------------------\r\n\r\n";
            StringUtils.LogString(text, Path.Combine( Configuration.CommonFolder ,                               
                "MarkdownMonsterErrors.txt"), Encoding.UTF8);
        }

        public static void SetWorkingSet(int lnMaxSize, int lnMinSize)
        {
            try
            {
                Process loProcess = Process.GetCurrentProcess();
                loProcess.MaxWorkingSet = (IntPtr)lnMaxSize;
                loProcess.MinWorkingSet = (IntPtr)lnMinSize;
            }
            catch {}
        }


        /// <summary>
        /// Handles an Application level exception by logging the error
        /// to log, and displaying an error message to the user.
        /// Also sends the error to server if enabled.
        /// 
        /// Returns true if application should continue, false to exit.        
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool HandleApplicationException(Exception ex)
        {            
            mmApp.Log(ex);

            var msg = string.Format("Yikes! Something went wrong...\r\n\r\n{0}\r\n\r\n" +
                "The error has been recorded and written to a log file and you can\r\n" +
                "review the details or report the error via Help | Show Error Log\r\n\r\n" +
                "Do you want to continue?", ex.Message);

            var res = MessageBox.Show(msg, mmApp.ApplicationName + " Error",
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Error);
            mmApp.SendBugReport(ex);

            if (res.HasFlag(MessageBoxResult.No))
                return false;
            return true;
        }


        public static void SendBugReport(Exception ex)
        {
            var v = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            var bug = new BugReport()
            {
                TimeStamp = DateTime.UtcNow,
                Message = ex.Message,                
                Product = "Markdown Monster",
                Version = v.FileMajorPart + "." + v.FileMinorPart,
                StackTrace = (ex.Source + "\r\n\r\n" + ex.StackTrace).Trim()               
            };
            
            new TaskFactory().StartNew(
                (bg) =>
                {                    
                    try
                    {
                        var temp = HttpUtils.JsonRequest<BugReport>(new HttpRequestSettings()
                        {
                            Url = mmApp.Configuration.BugReportUrl,
                            HttpVerb = "POST",
                            Content = bg,
                            Timeout = 3000
                        });
                    }
                    catch (Exception ex2)
                    {
                        // don't log with exception otherwise we get an endless loop
                        Log("Unable to report bug: " + ex2.Message);
                    }
                },bug);            
        }

        public static void SendTelemetry(string operation, string data = null)
        {
            if (!Configuration.SendTelemetry)
                return;

            var v = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            string version = v.FileMajorPart + "." + v.FileMinorPart;
            
            var t = new Telemetry
            {
                Version = version,
                Registered = UnlockKey.IsRegistered(),
                Access = mmApp.Configuration.ApplicationUpdates.AccessCount,
                Operation = operation,
                Time = Convert.ToInt32((DateTime.UtcNow - Started).TotalSeconds),
                Data = data
            };

            try
            {
                HttpUtils.JsonRequest<string>(new HttpRequestSettings()
                {
                    Url = mmApp.Configuration.TelemetryUrl,
                    HttpVerb = "POST",
                    Content = t,
                    Timeout = 1000
                });
            }
            catch (Exception ex2)
            {
                // don't log with exception otherwise we get an endless loop
                Log("Unable to send telemetry: " + ex2.Message);
            }
        }

        /// <summary>
        /// Sets the light or dark theme for a form. Call before
        /// InitializeComponents().
        /// 
        /// We only support the dark theme now so this no longer relevant
        /// but left in place in case we decide to support other themes.
        /// </summary>
        /// <param name="theme"></param>
        /// <param name="window"></param>
        public static void SetTheme(Themes theme = Themes.Default,MetroWindow window = null)
        {
            if (theme == Themes.Default)
                theme = mmApp.Configuration.ApplicationTheme;

            //if (theme == Themes.Light)
            //{
            //    // get the current app style (theme and accent) from the application
            //    // you can then use the current theme and custom accent instead set a new theme
            //    Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);

            //    // now set the Green accent and dark theme
            //    ThemeManager.ChangeAppStyle(Application.Current,
            //        ThemeManager.GetAccent("Steel"),
            //        ThemeManager.GetAppTheme("BaseLight")); // or appStyle.Item1                
            //}
            //else
            //{
            //    // get the current app style (theme and accent) from the application
            //    // you can then use the current theme and custom accent instead set a new theme
            //    Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);

            //    // now set the highlight accent and dark theme
            //    ThemeManager.ChangeAppStyle(Application.Current,
            //        ThemeManager.GetAccent("Blue"),
            //        ThemeManager.GetAppTheme("BaseDark")); // or appStyle.Item1      
            //}

            if (window != null)
                SetThemeWindowOverride(window);            

        }

        /// <summary>
        /// Overrides specific theme colors in the window header
        /// </summary>
        /// <param name="window"></param>
        public static void SetThemeWindowOverride(MetroWindow window)
        {
            if (mmApp.Configuration.ApplicationTheme == Themes.Dark)
            {
                if (window != null)
                {
                    window.WindowTitleBrush = (SolidColorBrush) (new BrushConverter().ConvertFrom("#333333"));
                    window.NonActiveWindowTitleBrush = (Brush) window.FindResource("WhiteBrush");

                    var brush = App.Current.Resources["MenuSeparatorBorderBrush"] as SolidColorBrush;
                    App.Current.Resources["MenuSeparatorBorderBrush"] = (SolidColorBrush) new BrushConverter().ConvertFrom("#333333");
                    brush = App.Current.Resources["MenuSeparatorBorderBrush"] as SolidColorBrush;
                }
            }
            //else
            //{
            //    if (window != null)
            //    {
            //        // Need to fix this to show the accent color when switching
            //        //window.WindowTitleBrush = (Brush)window.FindResource("WhiteBrush");
            //        //window.NonActiveWindowTitleBrush = (Brush)window.FindResource("WhiteBrush");
            //    }
            //}
        }


        internal static string EncryptionMachineKey { get; } = "42331333#1Ae@rTo*dOO-002" + Environment.MachineName;
        internal static string Signature { get; } = "S3VwdWFfMTAw";

        /// <summary>
        /// The URL where new versions are downloaded from
        /// </summary>
        public static string InstallerDownloadUrl { get; internal set; } =
            "https://markdownmonster.west-wind.com/download.aspx";

        /// <summary>
        /// Url that is used to check for new version information
        /// </summary>
        public static string UpdateCheckUrl { get; internal set; }
    }


    /// <summary>
    /// Supported themes (not used any more)
    /// </summary>
    public enum Themes
    {
        Dark,
        Light,
        Default
    }


    public class BugReport
    {
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
        public string Product { get; set; }
        public string Version { get; set; }
        public string StackTrace { get; set; }
    }

    public class Telemetry
    {
        public string Version { get; set; }
        public bool Registered { get; set; }
        public string Operation { get; set; }
        public string Data { get; set; }
        public int Access { get; set; }
        public int Time { get; set; }
    }
}