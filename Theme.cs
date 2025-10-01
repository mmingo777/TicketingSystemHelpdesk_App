using System.Drawing;
using System.Windows.Forms;

namespace IT_HelpDesk
{
    public static class Theme
    {
        // 🌐 Global Font
        public static readonly Font GlobalFont = new Font("Segoe UI", 10F);

        // 🎨 Colors
        public static readonly Color BackgroundColor = ColorTranslator.FromHtml("#F4F6F7");
        public static readonly Color PrimaryColor = ColorTranslator.FromHtml("#2980B9");
        public static readonly Color AccentColor = ColorTranslator.FromHtml("#AED6F1");
        public static readonly Color HoverColor = ColorTranslator.FromHtml("#3498DB");
        public static readonly Color TextColor = Color.Black;
        public static readonly Color NavButtonColor = Color.FromArgb(52, 73, 94);
        public static readonly Color NavHoverColor = Color.FromArgb(41, 128, 185);

        // 📦 Apply to a Form
        public static void ApplyFormTheme(Form form)
        {
            form.Font = GlobalFont;
            form.BackColor = BackgroundColor;
            form.ForeColor = TextColor;
        }

        // 📦 Apply to a UserControl
        public static void ApplyControlTheme(Control control)
        {
            control.Font = GlobalFont;
            control.BackColor = BackgroundColor;
            control.ForeColor = TextColor;
        }

        // 🧩 Style a nav button
        public static void StyleNavButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = GlobalFont;
            btn.ForeColor = Color.White;
            btn.BackColor = NavButtonColor;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(10, 0, 0, 0);
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) => btn.BackColor = NavHoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = NavButtonColor;
        }
    }
}

