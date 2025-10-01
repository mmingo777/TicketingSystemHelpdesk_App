using System;
using System.Drawing;
using System.Windows.Forms;

namespace IT_HelpDesk
{
    public partial class AdminToolsControl : UserControl
    {
        private ListBox lstLogs;
        private Button btnSimulateLog;

        public AdminToolsControl()
        {
            InitializeAdminPanel();
        }

        private void InitializeAdminPanel()
        {
            this.BackColor = Color.White;

            Label lblHeader = new Label
            {
                Text = "Admin Tools",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            lstLogs = new ListBox
            {
                Location = new Point(20, 70),
                Size = new Size(400, 200)
            };

            btnSimulateLog = new Button
            {
                Text = "Simulate System Log",
                Location = new Point(20, 280),
                Width = 180,
                BackColor = Color.LightGray
            };
            btnSimulateLog.Click += (s, e) =>
            {
                string log = $"[{DateTime.Now}] Simulated log entry.";
                lstLogs.Items.Add(log);
            };

            this.Controls.Add(lblHeader);
            this.Controls.Add(lstLogs);
            this.Controls.Add(btnSimulateLog);
        }
    }
}
