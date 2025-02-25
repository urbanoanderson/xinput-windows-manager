using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using XInputium;
using XInputium.XInput;

namespace XinputWindowsManager
{
    public class SystemTrayApplicationContext : ApplicationContext
    {
        private const string APPLICATION_NAME_FORMAT = "Xinput Windows Manager v{0}";

        private const string TOGGLE_LABEL_TEXT_FORMAT = "Toggle Manager {0}";

        private const string EXIT_LABEL_TEXT = "Exit";

        private NotifyIcon trayIcon;

        private ContextMenuStrip contextMenuStrip;

        private ToolStripMenuItem toggleLabel;

        private ToolStripMenuItem exitLabel;

        private Form helpForm;

        private bool desktopManagerActive = false;

        private bool prevLeftTriggerActive = false;

        private bool prevRightTriggerActive = false;

        private ManagerConfiguration managerConfiguration;

        private readonly InputSimulator inputSimulator;

        private readonly XGamepad xinputDevice;

        private DateTime LastToggleTime = DateTime.Now;

        private TimeSpan ToggleCooldown = TimeSpan.FromSeconds(1);

        public SystemTrayApplicationContext(ManagerConfiguration managerConfiguration) : base(new HelpForm())
        {
            this.managerConfiguration = managerConfiguration;
            this.inputSimulator = new InputSimulator();
            this.xinputDevice = new XGamepad();
            this.helpForm = this.MainForm;

            // SetupContextMenu
            this.toggleLabel = new ToolStripMenuItem();
            this.toggleLabel.Anchor = AnchorStyles.Right;
            this.toggleLabel.Click += (o, e) => this.ToggleDesktopManager();

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
                Text = string.Format(APPLICATION_NAME_FORMAT, Application.ProductVersion),
                ContextMenuStrip = this.contextMenuStrip,
            };

            this.UpdateToggleLabel(false);

            // Setup Xinput events
            this.xinputDevice.LeftJoystick.Updated += this.HandleLeftJoystickUpdated;
            this.xinputDevice.RightJoystick.Updated += this.HandleRightJoystickUpdated;
            this.xinputDevice.ButtonPressed += this.HandleXinputButtonPressed;
            this.xinputDevice.ButtonReleased += this.HandleXinputButtonReleased;
            this.xinputDevice.LeftTriggerMove += this.HandleLeftTriggerMove;
            this.xinputDevice.RightTriggerMove += this.HandleRightTriggerMove;

            // Start polling Xinput device
            Task.Run(() => {
                while (true)
                {
                    this.xinputDevice.Update();
                    Task.Delay(TimeSpan.FromMilliseconds(this.managerConfiguration.XinputPollingDelayMsecs)).Wait();
                }
            });
        }

        public void HandleExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false; // Hide tray icon, otherwise it will remain shown until user mouses over it
            Application.Exit();
        }

        public void ToggleDesktopManager()
        {
            this.LastToggleTime = DateTime.Now;
            this.desktopManagerActive = !this.desktopManagerActive;

            if (this.contextMenuStrip.InvokeRequired)
            {
                this.contextMenuStrip.Invoke((Action)(() => this.UpdateToggleLabel(this.desktopManagerActive)));
            }
            else
            {
                this.UpdateToggleLabel(this.desktopManagerActive);
            }

            Debug.WriteLine($"Switching desktop manager active mode to '{this.desktopManagerActive}'");

            // Play different beep sound when toggling on or off
            if (this.desktopManagerActive)
                Console.Beep();
            else
                Console.Beep(600, 400);
        }

        private bool IsToggleCombinationPressed()
        {
            if (DateTime.Now - this.LastToggleTime < this.ToggleCooldown)
            {
                return false;
            }

            foreach (string button in this.managerConfiguration.ToggleManagerCombination)
            {
                if (button == ManagerConfiguration.BUTTON_A && !this.xinputDevice.Buttons.A.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_B && !this.xinputDevice.Buttons.B.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_X && !this.xinputDevice.Buttons.X.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_Y && !this.xinputDevice.Buttons.Y.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_LB && !this.xinputDevice.Buttons.LB.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_RB && !this.xinputDevice.Buttons.RB.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_LS && !this.xinputDevice.Buttons.LS.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_RS && !this.xinputDevice.Buttons.RS.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_START && !this.xinputDevice.Buttons.Start.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_BACK && !this.xinputDevice.Buttons.Back.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_DPAD_UP && !this.xinputDevice.Buttons.DPadUp.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_DPAD_DOWN && !this.xinputDevice.Buttons.DPadDown.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_DPAD_LEFT && !this.xinputDevice.Buttons.DPadLeft.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_DPAD_RIGHT && !this.xinputDevice.Buttons.DPadRight.IsPressed)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_LT && this.xinputDevice.LeftTrigger.Value < this.managerConfiguration.TriggerThreshold)
                    return false;
                else if (button == ManagerConfiguration.BUTTON_RT && this.xinputDevice.RightTrigger.Value < this.managerConfiguration.TriggerThreshold)
                    return false;
            }

            Debug.WriteLine("Toggle Combination pressed.");

            return true;
        }

        private void HandleLeftJoystickUpdated(object sender, EventArgs e)
        {
            if (!this.desktopManagerActive || !this.xinputDevice.LeftJoystick.IsPushed) { return; }

            if (this.xinputDevice.LeftJoystick.X > this.managerConfiguration.LeftStickThreshold || this.xinputDevice.LeftJoystick.X < -this.managerConfiguration.LeftStickThreshold
            || this.xinputDevice.LeftJoystick.Y > this.managerConfiguration.LeftStickThreshold || this.xinputDevice.LeftJoystick.Y < -this.managerConfiguration.LeftStickThreshold)
            {
                Debug.WriteLine($"Left Stick Move => Moving Mouse...");
                this.inputSimulator.Mouse.MoveMouseBy(
                    (int)(this.xinputDevice.LeftJoystick.X * this.managerConfiguration.MaxCursorMovementSpeed),
                    (int)(-this.xinputDevice.LeftJoystick.Y * this.managerConfiguration.MaxCursorMovementSpeed)
                );
            }
        }

        private void HandleRightJoystickUpdated(object sender, EventArgs e)
        {
            if (!this.desktopManagerActive || !this.xinputDevice.RightJoystick.IsPushed) { return; }

            if (this.xinputDevice.RightJoystick.Y > this.managerConfiguration.RightStickThreshold)
            {
                Debug.WriteLine("Right Stick UP => Sending volume up...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_UP);
            }
            else if (this.xinputDevice.RightJoystick.Y < -this.managerConfiguration.RightStickThreshold)
            {
                Debug.WriteLine("Right Stick DOWN => Sending volume down...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_DOWN);
            }
            else if (this.xinputDevice.RightJoystick.X < -this.managerConfiguration.RightStickThreshold)
            {
                Debug.WriteLine("Right Stick LEFT => Sending prev song command...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PREV_TRACK);
            }
            else if (this.xinputDevice.RightJoystick.X > this.managerConfiguration.RightStickThreshold)
            {
                Debug.WriteLine("Right Stick RIGHT => Sending next song command...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_NEXT_TRACK);
            }
        }

        private void HandleLeftTriggerMove(object? sender, EventArgs e)
        {
            if (!this.desktopManagerActive) { return; }

            bool newTriggerActiveState = this.xinputDevice.LeftTrigger.Value >= this.managerConfiguration.TriggerThreshold;
            Debug.WriteLine($"Left Trigger States => Prev:{this.prevLeftTriggerActive} | Current:{newTriggerActiveState}");

            if (this.IsToggleCombinationPressed())
            {
                this.ToggleDesktopManager();
                return;
            }

            if (this.prevLeftTriggerActive == false && newTriggerActiveState == true)
            {
                Debug.WriteLine("LT => Sending Windows Key...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.LWIN);
            }

            this.prevLeftTriggerActive = newTriggerActiveState;
        }

        private void HandleRightTriggerMove(object? sender, EventArgs e)
        {
            if (!this.desktopManagerActive) { return; }

            bool newTriggerActiveState = this.xinputDevice.RightTrigger.Value >= this.managerConfiguration.TriggerThreshold;
            Debug.WriteLine($"Right Trigger States => Prev:{this.prevRightTriggerActive} | Current:{newTriggerActiveState}");

            if (this.IsToggleCombinationPressed())
            {
                this.ToggleDesktopManager();
                return;
            }

            // Press Right trigger
            if (this.prevRightTriggerActive == false && newTriggerActiveState == true)
            {
                Debug.WriteLine("RT Pressed => Showing help screen...");

                this.helpForm.Invoke(() => {
                    this.helpForm.Opacity = 1;
                    this.helpForm.Show();
                    this.helpForm.Activate();
                });
            }

            // Release Right trigger
            if (this.prevRightTriggerActive == true && newTriggerActiveState == false)
            {
                Debug.WriteLine("RT Released => Hiding help screen...");

                this.helpForm.Invoke(() => {
                    this.helpForm.Opacity = 0;
                    this.helpForm.Hide();
                    this.helpForm.Visible = false;
                });
            }

            this.prevRightTriggerActive = newTriggerActiveState;
        }

        private void HandleXinputButtonPressed(object sender, DigitalButtonEventArgs<XInputButton> e)
        {
            Debug.WriteLine($"Button '{e.Button.Button}' pressed.");

            if (this.IsToggleCombinationPressed())
            {
                this.ToggleDesktopManager();
                return;
            }

            if (!this.desktopManagerActive) { return; }

            if (e.Button.Button == XButtons.A)
            {
                Debug.WriteLine("A => Sending left click input...");
                this.inputSimulator.Mouse.LeftButtonDown();
            }
            else if (e.Button.Button == XButtons.X)
            {
                Debug.WriteLine("X => Sending right click input...");
                this.inputSimulator.Mouse.RightButtonClick();
            }
            else if (e.Button.Button == XButtons.B)
            {
                Debug.WriteLine("B => Sending ESC...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
            }
            else if (e.Button.Button == XButtons.Y)
            {
                Debug.WriteLine("Y => Opening Task Manager (CTRL+SHIFT+ESC)...");
                this.inputSimulator.Keyboard.ModifiedKeyStroke(new VirtualKeyCode[] { VirtualKeyCode.LCONTROL, VirtualKeyCode.LSHIFT }, VirtualKeyCode.ESCAPE);
            }
            else if (e.Button.Button == XButtons.Start)
            {
                Debug.WriteLine("START => Sending ENTER...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.ACCEPT);
            }
            else if (e.Button.Button == XButtons.Back)
            {
                Debug.WriteLine("SELECT => Closing Window (ALT+F4)...");
                this.inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LMENU, VirtualKeyCode.F4);
            }
            else if (e.Button.Button == XButtons.DPadUp)
            {
                Debug.WriteLine("DPAD UP => Sending UP Arrow...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.UP);
            }
            else if (e.Button.Button == XButtons.DPadDown)
            {
                Debug.WriteLine("DPAD DOWN => Sending DOWN Arrow...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.DOWN);
            }
            else if (e.Button.Button == XButtons.DPadLeft)
            {
                Debug.WriteLine("DPAD LEFT => Sending LEFT Arrow...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.LEFT);
            }
            else if (e.Button.Button == XButtons.DPadRight)
            {
                Debug.WriteLine("DPAD RIGHT => Sending RIGHT Arrow...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RIGHT);
            }
            else if (e.Button.Button == XButtons.LS)
            {
                Debug.WriteLine("LS => Toggling OnScreen Keyboard...");
                this.inputSimulator.Keyboard.ModifiedKeyStroke(new VirtualKeyCode[] { VirtualKeyCode.LCONTROL, VirtualKeyCode.LWIN }, VirtualKeyCode.VK_O);
            }
            else if (e.Button.Button == XButtons.RS)
            {
                Debug.WriteLine("RS => Sending Mute command...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_MUTE);
            }
            else if (e.Button.Button == XButtons.LB)
            {
                Debug.WriteLine("LB => Holding Alt...");
                this.inputSimulator.Keyboard.KeyDown(VirtualKeyCode.MENU);
            }
            else if (e.Button.Button == XButtons.RB)
            {
                Debug.WriteLine("RB => Sending Tab...");
                this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
            }
        }

        private void HandleXinputButtonReleased(object? sender, DigitalButtonEventArgs<XInputButton> e)
        {
            Debug.WriteLine($"Button '{e.Button.Button}' released.");

            if (!this.desktopManagerActive) { return; }

            if (e.Button.Button == XButtons.A)
            {
                Debug.WriteLine("A => Sending left click input...");
                this.inputSimulator.Mouse.LeftButtonUp();
            }
            else if (e.Button.Button == XButtons.LB)
            {
                Debug.WriteLine("LB => Releasing Alt...");
                this.inputSimulator.Keyboard.KeyUp(VirtualKeyCode.MENU);
            }
        }

        private void UpdateToggleLabel(bool newState)
        {
            this.toggleLabel.Text = string.Format(TOGGLE_LABEL_TEXT_FORMAT, newState ? "Off" : "On");
        }
    }
}
