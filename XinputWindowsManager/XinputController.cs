using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XinputWindowsManager
{
    public class XinputController : IDisposable
    {
        private const double MAX_TRIGGER_VALUE = 255.0;

        private const double MAX_AXIS_VALUE = 32767.0;

        private Controller gamepad;

        private State gamepadState;

        private IDictionary<GamepadButtonFlags, (bool, bool)> buttonStates;

        private CancellationTokenSource poolingCts;
        public XinputController(UserIndex gamepadIndex = UserIndex.One)
        {
            this.gamepad = new Controller(gamepadIndex);
            this.buttonStates = new Dictionary<GamepadButtonFlags, (bool, bool)>();

            this.buttonStates.Add(GamepadButtonFlags.Start, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.Back, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.A, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.B, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.X, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.Y, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.LeftShoulder, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.RightShoulder, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.LeftThumb, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.RightThumb, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.DPadUp, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.DPadDown, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.DPadLeft, (false, false));
            this.buttonStates.Add(GamepadButtonFlags.DPadRight, (false, false));
        }

        public event EventHandler<GamepadButtonFlags> OnButtonPressed;

        public event EventHandler<GamepadButtonFlags> OnButtonReleased;

        public bool IsPooling
        {
            get
            {
                return this.poolingCts != null && !this.poolingCts.IsCancellationRequested;
            }
        }

        public void Pool()
        {
            this.gamepadState = gamepad.GetState();

            this.UpdateButtonState(GamepadButtonFlags.Back);
            this.UpdateButtonState(GamepadButtonFlags.Start);
            this.UpdateButtonState(GamepadButtonFlags.A);
            this.UpdateButtonState(GamepadButtonFlags.B);
            this.UpdateButtonState(GamepadButtonFlags.X);
            this.UpdateButtonState(GamepadButtonFlags.Y);
            this.UpdateButtonState(GamepadButtonFlags.LeftShoulder);
            this.UpdateButtonState(GamepadButtonFlags.RightShoulder);
            this.UpdateButtonState(GamepadButtonFlags.LeftThumb);
            this.UpdateButtonState(GamepadButtonFlags.RightThumb);
            this.UpdateButtonState(GamepadButtonFlags.DPadUp);
            this.UpdateButtonState(GamepadButtonFlags.DPadDown);
            this.UpdateButtonState(GamepadButtonFlags.DPadLeft);
            this.UpdateButtonState(GamepadButtonFlags.DPadRight);
        }

        public void StartPooling()
        {
            if (!this.IsPooling)
            {
                this.poolingCts = new CancellationTokenSource();

                Task.Factory.StartNew(() => {
                    while (!this.poolingCts.IsCancellationRequested)
                        this.Pool();
                });
            }
        }

        public void StopPooling()
        {
            if (this.IsPooling)
            {
                this.poolingCts.Cancel();
                this.poolingCts.Dispose();
                this.poolingCts = null;
            }
        }

        public void Dispose()
        {
            this.StopPooling();
        }

        public bool ButtonState(GamepadButtonFlags button)
        {
            return this.buttonStates[button].Item2;
        }

        public bool ButtonPressed(GamepadButtonFlags button)
        {
            return !this.buttonStates[button].Item1 && this.buttonStates[button].Item2;
        }

        public bool ButtonHolded(GamepadButtonFlags button)
        {
            return this.buttonStates[button].Item1 && this.buttonStates[button].Item2;
        }

        public bool ButtonReleased(GamepadButtonFlags button)
        {
            return this.buttonStates[button].Item1 && !this.buttonStates[button].Item2;
        }

        public double LeftTriggerState()
        {
            return this.gamepadState.Gamepad.LeftTrigger / MAX_TRIGGER_VALUE;
        }

        public double RightTriggerState()
        {
            return this.gamepadState.Gamepad.RightTrigger / MAX_TRIGGER_VALUE;
        }

        public double LeftStickXState()
        {
            return this.gamepadState.Gamepad.LeftThumbX / MAX_AXIS_VALUE;
        }

        public double LeftStickYState()
        {
            return this.gamepadState.Gamepad.LeftThumbY / MAX_AXIS_VALUE;
        }

        public double RightStickXState()
        {
            return this.gamepadState.Gamepad.RightThumbX / MAX_AXIS_VALUE;
        }

        public double RightStickYState()
        {
            return this.gamepadState.Gamepad.RightThumbY / MAX_AXIS_VALUE;
        }

        private void UpdateButtonState(GamepadButtonFlags button)
        {
            this.buttonStates[button] = (this.buttonStates[button].Item2, this.gamepadState.Gamepad.Buttons.HasFlag(button));

            if (!this.buttonStates[button].Item1 && this.buttonStates[button].Item2)
            {
                this.OnButtonPressed?.Invoke(this, button);
            }
            else if (this.buttonStates[button].Item1 && !this.buttonStates[button].Item2)
            {
                this.OnButtonReleased?.Invoke(this, button);
            }
        }
    }
}
