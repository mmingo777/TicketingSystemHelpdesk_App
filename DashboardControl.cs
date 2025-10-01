using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace IT_HelpDesk
{
    public partial class DashboardControl : UserControl
    {
        public DashboardControl()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Label welcomeLabel = new Label
            {
                Text = $"Welcome, {Session.Username} ({Session.UserRole})",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(welcomeLabel);

            Label lblFilter = new Label
            {
                Text = "Filters:",
                Location = new Point(220, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            ComboBox cmbTimeRange = new ComboBox
            {
                Location = new Point(300, 15),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTimeRange.Items.AddRange(new string[] { "All", "Last 7 Days", "This Month", "Custom Range" });
            cmbTimeRange.SelectedIndex = 0;

            DateTimePicker dtStart = new DateTimePicker
            {
                Location = new Point(470, 15),
                Width = 120,
                Visible = false
            };

            DateTimePicker dtEnd = new DateTimePicker
            {
                Location = new Point(600, 15),
                Width = 120,
                Visible = false
            };

            ComboBox cmbCategory = new ComboBox
            {
                Location = new Point(740, 15),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            ComboBox cmbStaff = new ComboBox
            {
                Location = new Point(870, 15),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Button btnApply = new Button
            {
                Text = "Apply",
                Location = new Point(1000, 15),
                Width = 80
            };

            cmbTimeRange.SelectedIndexChanged += (s, e) =>
            {
                bool isCustom = cmbTimeRange.SelectedItem.ToString() == "Custom Range";
                dtStart.Visible = isCustom;
                dtEnd.Visible = isCustom;
            };

            this.Controls.AddRange(new Control[] {
                lblFilter, cmbTimeRange, dtStart, dtEnd,
                cmbCategory, cmbStaff, btnApply
            });

            Label header = new Label
            {
                Text = "Ticket Status Overview",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(220, 60),
                AutoSize = true
            };
            this.Controls.Add(header);

            Dictionary<string, Label> statusLabels = new Dictionary<string, Label>();
            Dictionary<string, Color> statusColors = new Dictionary<string, Color>
            {
                { "Open", Color.LightSkyBlue },
                { "In Progress", Color.Khaki },
                { "Resolved", Color.LightGreen },
                { "Closed", Color.Silver }
            };

            int left = 220;
            foreach (var status in statusColors)
            {
                Panel card = new Panel
                {
                    Size = new Size(180, 90),
                    Location = new Point(left, 120),
                    BackColor = status.Value,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(10)
                };

                Label lblTitle = new Label
                {
                    Text = status.Key,
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(10, 10)
                };

                Label lblCount = new Label
                {
                    Text = "Count: 0",
                    Font = new Font("Segoe UI", 12),
                    AutoSize = true,
                    Location = new Point(10, 50)
                };

                card.Controls.Add(lblTitle);
                card.Controls.Add(lblCount);
                this.Controls.Add(card);

                statusLabels[status.Key] = lblCount;
                left += 200;
            }

            Chart chart = new Chart
            {
                Location = new Point(220, 230),
                Size = new Size(600, 250)
            };
            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);
            Series series = new Series
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                Color = Color.SteelBlue
            };
            series.Points.AddXY("Mon", 2);
            series.Points.AddXY("Tue", 3);
            series.Points.AddXY("Wed", 1);
            series.Points.AddXY("Thu", 5);
            series.Points.AddXY("Fri", 0);
            chart.Series.Add(series);
            this.Controls.Add(chart);

            DataGridView dgvAttention = new DataGridView
            {
                Location = new Point(850, 230),
                Size = new Size(320, 250),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvAttention.Columns.Add("ID", "ID");
            dgvAttention.Columns.Add("Subject", "Subject");
            dgvAttention.Columns.Add("Priority", "Priority");

            this.Controls.Add(dgvAttention);

            btnApply.Click += (s, e) =>
            {
                var tickets = TicketDataAccess.GetAllTicketsFromDatabase();

                string timeSelection = cmbTimeRange.SelectedItem.ToString();
                if (timeSelection == "Last 7 Days")
                {
                    DateTime last7 = DateTime.Now.AddDays(-7);
                    tickets = tickets.Where(t => t.CreatedDate >= last7).ToList();
                }
                else if (timeSelection == "This Month")
                {
                    DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    tickets = tickets.Where(t => t.CreatedDate >= start).ToList();
                }
                else if (timeSelection == "Custom Range")
                {
                    tickets = tickets.Where(t => t.CreatedDate >= dtStart.Value && t.CreatedDate <= dtEnd.Value).ToList();
                }

                string selectedStatus = cmbCategory.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All")
                {
                    tickets = tickets.Where(t => t.Status == selectedStatus).ToList();
                }

                string selectedStaff = cmbStaff.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(selectedStaff) && selectedStaff != "All")
                {
                    tickets = tickets.Where(t => t.AssignedTo == selectedStaff).ToList();
                }

                foreach (var key in statusLabels.Keys)
                {
                    int count = tickets.Count(t => t.Status == key);
                    statusLabels[key].Text = $"Count: {count}";
                }

                dgvAttention.Rows.Clear();
                foreach (var ticket in tickets.Where(t => t.Status == "Open" || t.Priority == "High"))
                {
                    dgvAttention.Rows.Add(ticket.Id, ticket.Title, ticket.Priority);
                }

            };

            var allTickets = TicketDataAccess.GetAllTicketsFromDatabase();
            var uniqueStatuses = allTickets.Select(t => t.Status).Distinct().Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            cmbCategory.Items.Add("All");
            cmbCategory.Items.AddRange(uniqueStatuses.ToArray());
            cmbCategory.SelectedIndex = 0;

            var uniqueStaff = allTickets.Select(t => t.AssignedTo).Distinct().Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            cmbStaff.Items.Add("All");
            cmbStaff.Items.AddRange(uniqueStaff.ToArray());
            cmbStaff.SelectedIndex = 0;

            btnApply.PerformClick();
        }
    }
}





