using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Windows.Forms.DataVisualization.Charting;

namespace IT_HelpDesk
{
    public partial class Form1 : Form
    {
        private Panel mainPanel;
        private Panel panelNav;
        private string userRole;

        public Form1(string role)
        {
            userRole = role;
            this.Text = "NY Transit Operations Dashboard";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            Theme.ApplyFormTheme(this);

            InitializeNavigationPanel();
            InitializeTopBar();
            InitializeMainPanel();
            LoadDashboard();
            TestDatabaseConnection();
            NotificationManager.Initialize(this);
        }
        private void InitializeNavigationPanel()
        {
            panelNav = new Panel
            {
                Name = "panelNav",
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.FromArgb(45, 62, 80) // dark slate blue/gray tone
            };
            this.Controls.Add(panelNav);

            AddNavButton("🏠 Dashboard", (s, e) => LoadDashboard(), 0);
            AddNavButton("📝 Submit Ticket", (s, e) => LoadTicketing(), 1);

            if (userRole == "R1" || userRole == "R3")  // Admin or Support
                AddNavButton("📁 Ticket History", (s, e) => LoadTicketHistory(), 2);

            if (userRole == "R1")  // Admin only
                AddNavButton("📊 Reports", (s, e) => LoadReports(), 3);

            AddNavButton("🚪 Logout", Logout, 10);
        }

        private void AddNavButton(string text, EventHandler action, int positionIndex)
        {
            Button btn = new Button
            {
                Text = text,
                Width = 180,
                Height = 45,
                Location = new Point(10, 20 + positionIndex * 55)
            };

            Theme.StyleNavButton(btn);

            btn.Click += action;
            panelNav.Controls.Add(btn);
        }


        private void InitializeTopBar()
        {
            Panel topBar = new Panel
            {
                Name = "topBar",
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(41, 128, 185) // steel blue
            };

            string roleDisplayName;
            if (userRole == "R1")
                roleDisplayName = "Admin";
            else if (userRole == "R2")
                roleDisplayName = "Staff";
            else if (userRole == "R3")
                roleDisplayName = "Support";
            else
                roleDisplayName = "User";


            Label title = new Label
            {
                Text = $"Welcome, {Session.Username} ({roleDisplayName})",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };


            Button btnTopLogout = new Button
            {
                Text = "Logout",
                Width = 100,
                Height = 30,
                Location = new Point(this.Width - 120, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(192, 57, 43), // red tone
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnTopLogout.FlatAppearance.BorderSize = 0;
            btnTopLogout.Click += Logout;

            topBar.Controls.Add(title);
            topBar.Controls.Add(btnTopLogout);
            this.Controls.Add(topBar);
        }

        private void InitializeMainPanel()
        {
            mainPanel = new Panel
            {
                Name = "mainPanel",
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            this.Controls.Add(mainPanel);
        }

        private void LoadTicketing()
        {
            mainPanel.Controls.Clear();
            var ticketing = new TicketingControl(); // Assumes parameterless constructor
            ticketing.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(ticketing);
        }

        private void LoadTicketHistory()
        {
            mainPanel.Controls.Clear();
            Session.Username = "mingo";
            Session.UserRole = "Admin";
            TicketHistoryControl history = new TicketHistoryControl();
            history.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(history);
        }

        private void LoadReports()
        {
            mainPanel.Controls.Clear();
            var reports = new ReportsControl();
            reports.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(reports);
        }
  

        private void Logout(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
        }
        private void LoadDashboard()
        {
            mainPanel.Controls.Clear();
            DashboardControl dashboard = new DashboardControl();
            dashboard.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(dashboard);
        }
        private void TestDatabaseConnection()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["HelpDeskDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    
                    string query = "SELECT TOP 3 TKT_ID, TKT_SUBJ FROM TICKET";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        return;
                    }

                    while (reader.Read())
                    {
                        string id = reader["TKT_ID"].ToString();
                        string subj = reader["TKT_SUBJ"].ToString();
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Connection failed:\n" + ex.Message, "Error");
            }
        }
    }
}
