using System;
using System.Drawing;
using System.Windows.Forms;

namespace IT_HelpDesk
{
    public static class NotificationManager
    {
        private static Form mainForm;

        public static void Initialize(Form form)
        {
            mainForm = form;
        }

        public static void ShowNotification(string message)
        {
            if (mainForm == null) return;

            Label notification = new Label
            {
                Text = message,
                AutoSize = false,
                Width = 300,
                Height = 40,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(mainForm.Width - 320, 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            mainForm.Controls.Add(notification);
            notification.BringToFront();

            var timer = new System.Windows.Forms.Timer
            {
                Interval = 4000,
                Enabled = true
            };

            timer.Tick += (s, e) =>
            {
                mainForm.Controls.Remove(notification);
                timer.Stop();
                timer.Dispose();
            };
        }
    }
}
