using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using XInputium.XInput;

namespace XinputWindowsManager
{
    public class ManagerConfiguration
    {
        public const string BUTTON_A = "A";

        public const string BUTTON_B = "B";

        public const string BUTTON_X = "X";

        public const string BUTTON_Y = "Y";

        public const string BUTTON_LB = "LB";

        public const string BUTTON_RB = "RB";

        public const string BUTTON_LT = "LT";

        public const string BUTTON_RT = "RT";

        public const string BUTTON_LS = "LS";

        public const string BUTTON_RS = "RS";

        public const string BUTTON_BACK = "BACK";

        public const string BUTTON_START = "START";

        public const string BUTTON_DPAD_UP = "DPAD_UP";

        public const string BUTTON_DPAD_DOWN = "DPAD_DOWN";

        public const string BUTTON_DPAD_LEFT = "DPAD_LEFT";

        public const string BUTTON_DPAD_RIGHT = "DPAD_RIGHT";

        public bool StartEnabled { get; set; }

        public int XinputPollingDelayMsecs { get; set; }

        public double MaxCursorMovementSpeed { get; set; }

        public double LeftStickThreshold { get; set; }

        public double RightStickThreshold { get; set; }

        public double TriggerThreshold { get; set; }

        public string[] ToggleManagerCombination { get; set; }

        public ManagerConfiguration()
        {
            try
            {
                this.StartEnabled = this.ReadBoolSetting("START_ENABLED");
                this.XinputPollingDelayMsecs = this.ReadIntSetting("XINPUT_POLLING_DELAY_MSECS", minvalue: 4, maxvalue: 32);
                this.MaxCursorMovementSpeed = this.ReadDoubleSetting("MAX_CURSOR_MOVEMENT_SPEED", minvalue: 5.0, maxvalue: 50.0);
                this.LeftStickThreshold = this.ReadDoubleSetting("LEFT_STICK_THRESHOLD", minvalue: 0.0, maxvalue: 1.0);
                this.RightStickThreshold = this.ReadDoubleSetting("RIGHT_STICK_THRESHOLD", minvalue: 0.0, maxvalue: 1.0);
                this.TriggerThreshold = this.ReadDoubleSetting("TRIGGER_THRESHOLD", minvalue: 0.0, maxvalue: 1.0);
                this.ToggleManagerCombination = this.ParseToggleCombination();
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Invalid App.Config. {e.Message}");
            }
        }

        private string ReadSetting(string setting)
        {
            string value = ConfigurationManager.AppSettings[setting];

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"Setting {setting} not available or empty");
            }

            return value;
        }

        private bool ReadBoolSetting(string setting)
        {
            return bool.Parse(this.ReadSetting(setting));
        }

        private int ReadIntSetting(string setting, int minvalue, int maxvalue)
        {
            int v = int.Parse(this.ReadSetting(setting));

            if (v < minvalue || v > maxvalue)
            {
                throw new ArgumentException($"Invalid value for setting {setting}. Value must be between {minvalue} and {maxvalue}.");
            }

            return v;
        }

        private double ReadDoubleSetting(string setting, double minvalue, double maxvalue)
        {
            double v = double.Parse(this.ReadSetting(setting));

            if (v < minvalue || v > maxvalue)
            {
                throw new ArgumentException($"Invalid value for setting {setting}. Value must be between {minvalue} and {maxvalue}.");
            }

            return v;
        }

        private string[] ParseToggleCombination()
        {
            string rawValue = this.ReadSetting("TOGGLE_MANAGER_COMBINATION");

            string[] splitValues = rawValue.Split(',');

            foreach (string button in splitValues)
            {
                Debug.WriteLine($"Combination button: {button}");

                if (   button != BUTTON_A
                    && button != BUTTON_B
                    && button != BUTTON_X
                    && button != BUTTON_Y
                    && button != BUTTON_LB
                    && button != BUTTON_RB
                    && button != BUTTON_LS
                    && button != BUTTON_RS
                    && button != BUTTON_LT
                    && button != BUTTON_RT
                    && button != BUTTON_BACK
                    && button != BUTTON_START
                    && button != BUTTON_DPAD_UP
                    && button != BUTTON_DPAD_DOWN
                    && button != BUTTON_DPAD_LEFT
                    && button != BUTTON_DPAD_RIGHT)
                {
                    throw new ArgumentException($"Invalid button value in TOGGLE_MANAGER_COMBINATION: {button}");
                }
            }

            return splitValues;
        }
    }
}
