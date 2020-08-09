﻿using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XinputWindowsManager
{
    public class SystemTrayApplicationContext : ApplicationContext
    {
        private const string APPLICATION_NAME = "Xinput Windows Manager";

        private const string TOGGLE_LABEL_TEXT_FORMAT = "Toggle Mouse Mode {0}";

        private const string EXIT_LABEL_TEXT = "Exit";

        private readonly DesktopManagerService desktopManagerService;

        private NotifyIcon trayIcon;

        private ContextMenuStrip contextMenuStrip;

        private ToolStripMenuItem toggleLabel;

        private ToolStripMenuItem exitLabel;

        public SystemTrayApplicationContext()
        {
            this.SetupContextMenu();

            this.desktopManagerService = new DesktopManagerService();
            this.desktopManagerService.OnMouseModeToggled += this.HandleMouseModeToggle;

            this.UpdateToggleLabel(false);
            Task.Factory.StartNew(() => this.desktopManagerService.Start());
        }

        public void HandleExit(object sender, EventArgs e)
        {
            this.desktopManagerService.Stop();
            trayIcon.Visible = false; // Hide tray icon, otherwise it will remain shown until user mouses over it
            Application.Exit();
        }

        private void HandleMouseModeToggle(object o, bool mouseModeOn)
        {
            this.contextMenuStrip.BeginInvoke((Action)(() =>
            {
                this.UpdateToggleLabel(mouseModeOn);
            }));
        }

        private void SetupContextMenu()
        {
            this.toggleLabel = new ToolStripMenuItem();
            this.toggleLabel.Anchor = AnchorStyles.Right;
            this.toggleLabel.Click += (o, e) => this.desktopManagerService.ToggleMouseMode();
            
            this.exitLabel = new ToolStripMenuItem(EXIT_LABEL_TEXT);
            this.exitLabel.Anchor = AnchorStyles.Right;
            this.exitLabel.Click += this.HandleExit;

            this.contextMenuStrip = new ContextMenuStrip();
            this.contextMenuStrip.ShowImageMargin = false;
            this.contextMenuStrip.Items.Add(toggleLabel);
            this.contextMenuStrip.Items.Add(exitLabel);

            this.trayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.AppIcon,
                Visible = true,
                Text = APPLICATION_NAME,
                ContextMenuStrip = this.contextMenuStrip,
            };
        }

        private void UpdateToggleLabel(bool mouseModeOn)
        {
            this.toggleLabel.Text = string.Format(TOGGLE_LABEL_TEXT_FORMAT, mouseModeOn ? "Off" : "On");
        }
    }
}