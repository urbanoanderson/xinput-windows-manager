using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using SharpDX.XInput;

namespace XinputWindowsManager
{
    public class DesktopManagerService
    {
        private const double MAX_CURSOR_MOVEMENT_SPEED = 25.0;

        private const double THUMBSTICK_MOVEMENT_THRESHOLD = 0.3;

        private static TimeSpan FRAME_TIME = TimeSpan.FromSeconds(1 / 120.0);

        private readonly XinputController gamepad;

        private readonly Stopwatch frameCounter;

        public DesktopManagerService()
        {
            this.gamepad = new XinputController(UserIndex.One);
            this.frameCounter = new Stopwatch();
            this.ServiceRunning = false;
            this.MouseModeOn = false;
        }

        public event EventHandler<bool> OnMouseModeToggled;

        public bool ServiceRunning { get; private set; }

        public bool MouseModeOn { get; private set; }

        public void Stop()
        {
            this.MouseModeOn = false;
            this.ServiceRunning = false;
        }

        public void Start()
        {
            this.ServiceRunning = true;
            this.frameCounter.Start();

            while (this.ServiceRunning)
            {
                if (this.gamepad.ButtonCombinationPressed(
                    GamepadButtonFlags.Back, GamepadButtonFlags.A, GamepadButtonFlags.X))
                    this.ToggleMouseMode();

                if (MouseModeOn)
                    this.ProcessMouseMode();

                this.PoolXinputController();
            }
        }

        public void ToggleMouseMode()
        {
            this.MouseModeOn = !this.MouseModeOn;
            Debug.WriteLine($"Switching mouse mode to '{this.MouseModeOn}'");
            Console.Beep();
            this.OnMouseModeToggled?.Invoke(this, this.MouseModeOn);
        }

        private void PoolXinputController()
        {
            this.gamepad.Pool();
            TimeSpan elapsedTime = this.frameCounter.Elapsed;
            this.frameCounter.Reset();

            if (FRAME_TIME > elapsedTime)
                Task.Delay(FRAME_TIME - elapsedTime).Wait();
        }

        private void ProcessMouseMode()
        {
            // Move mouse cursor with left thumb stick
            double leftStickX = gamepad.LeftStickXState();
            double leftStickY = gamepad.LeftStickYState();

            if (leftStickX > THUMBSTICK_MOVEMENT_THRESHOLD || leftStickX < -THUMBSTICK_MOVEMENT_THRESHOLD
            || leftStickY > THUMBSTICK_MOVEMENT_THRESHOLD || leftStickY < -THUMBSTICK_MOVEMENT_THRESHOLD)
            {
                Point mousePos = WindowsMouseCursor.GetPosition();
                int newX = (int)(mousePos.X + (leftStickX * MAX_CURSOR_MOVEMENT_SPEED));
                int newY = (int)(mousePos.Y - (leftStickY * MAX_CURSOR_MOVEMENT_SPEED));
                Debug.WriteLine($"New cursor pos: x={mousePos.X}, y={mousePos.Y}");
                WindowsMouseCursor.SetPosition(newX, newY);
            }

            // Left click with mouse cursor if A button is pressed
            if (gamepad.ButtonPressed(GamepadButtonFlags.A))
            {
                Debug.WriteLine("Sending left click input...");
                WindowsMouseCursor.SendLeftClick();
            }
            if (gamepad.ButtonPressed(GamepadButtonFlags.X))
            {
                Debug.WriteLine("Sending right click input...");
                WindowsMouseCursor.SendRightClick();
            }
        }
    }
}
