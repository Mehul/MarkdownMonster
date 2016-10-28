﻿using System;
using System.Diagnostics;
using System.Reflection;
using MahApps.Metro.Controls;
using Westwind.Utilities;

namespace MarkdownMonster.Windows
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : MetroWindow
    {
        public About()
        {           
            InitializeComponent();
            
            mmApp.SetThemeWindowOverride(this);
            
            string bitNess = Environment.Is64BitProcess ? "64 bit" : "32 bit";
            var v = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            VersionLabel.Content = "Version " + v.FileMajorPart + "." + v.FileMinorPart + 
                (v.FileBuildPart > 0 ? "." + v.FileBuildPart : "") +
                " • " + bitNess;

            if (UnlockKey.IsRegistered())
            {
                PanelFreeNotice.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void WestWindIcon_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ShellUtils.GoUrl("http://weblog.west-wind.com");
        }

        private void Register_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ShellUtils.GoUrl("https://store.west-wind.com/product/markdown_monster");
        }
    }
}
