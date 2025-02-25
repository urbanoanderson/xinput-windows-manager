using System;
using System.Windows.Forms;

namespace XinputWindowsManager
{
    public class HelpForm : Form
    {
        public HelpForm()
        {
            // Setup Help Form
            Label label = new Label();
            label.Text = "--- XINPUT WINDOWS MANAGER CONTROLS ---" + Environment.NewLine + Environment.NewLine
                + "Left Stick => Mouse Movement" + Environment.NewLine
                + "Left Stick Click => Launch Virtual Keyboard" + Environment.NewLine
                + "A => Mouse Left Click" + Environment.NewLine
                + "X => Mouse Right Click" + Environment.NewLine
                + "Y => Launch Task Manager" + Environment.NewLine
                + "B => Press ESC" + Environment.NewLine
                + "BACK => Press ALT+F4" + Environment.NewLine
                + "START => Press ENTER" + Environment.NewLine
                + "LB => Press Alt" + Environment.NewLine
                + "RB => Press Tab" + Environment.NewLine
                + "LT => Press Windows Key" + Environment.NewLine
                + "RT => Show Help Screen" + Environment.NewLine
                + "Right Stick UP/DOWN => Control System Volume" + Environment.NewLine
                + "Right Stick LEFT/RIGHT => Prev/Next Song" + Environment.NewLine
                + "Right Stick Click => Mute System Sounds" + Environment.NewLine
            ;
            label.Size = new Size(300, 300);
            label.Anchor = AnchorStyles.Left;
            label.TextAlign = ContentAlignment.TopCenter;

            this.Size = label.Size;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Controls.Add(label);
            this.SetVisibleCore(false);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Visible = false;
            this.Opacity = 0;
            this.ShowInTaskbar = false;

            base.OnLoad(e);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
    }
}
