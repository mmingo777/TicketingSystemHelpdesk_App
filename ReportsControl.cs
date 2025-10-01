using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace IT_HelpDesk
{
    public class ReportsControl : UserControl
    {
        public ReportsControl()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            Theme.ApplyControlTheme(this);

            var tickets = TicketDataAccess.GetAllTicketsFromDatabase();

            // Count per status
            var statusCounts = tickets
                .GroupBy(t => t.Status ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            Label lblHeader = new Label
            {
                Text = "Ticket Summary Report",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(240, 60)
            };
            this.Controls.Add(lblHeader);

            int top = 110;
            int left = 220;

            // Show all statuses dynamically
            foreach (var kvp in statusCounts)
            {
                Panel panel = new Panel
                {
                    Width = 150,
                    Height = 80,
                    Location = new Point(left, top),
                    BackColor = Color.LightGray
                };

                Label lbl = new Label
                {
                    Text = $"{kvp.Key}: {kvp.Value}",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                panel.Controls.Add(lbl);
                this.Controls.Add(panel);
                left += 180;
            }

            // Line Chart: Ticket Volume Over Time
            Chart chartLine = new Chart
            {
                Size = new Size(600, 200),
                Location = new Point(220, 200)
            };
            ChartArea chartArea = new ChartArea("MainArea");
            chartLine.ChartAreas.Add(chartArea);

            Series series = new Series("Tickets Over Time")
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.Date,
                Color = Color.Blue,
                BorderWidth = 2
            };

            var groupedByDate = tickets
                .Where(t => t.CreatedDate > DateTime.MinValue)
                .GroupBy(t => t.CreatedDate.Date)
                .OrderBy(g => g.Key);

            foreach (var group in groupedByDate)
            {
                series.Points.AddXY(group.Key, group.Count());
            }

            chartLine.Series.Add(series);
            this.Controls.Add(chartLine);

            // Pie Chart: Tickets by Status
            Chart chartStatus = new Chart
            {
                Size = new Size(250, 200),
                Location = new Point(300, 420)
            };
            ChartArea pieArea1 = new ChartArea();
            chartStatus.ChartAreas.Add(pieArea1);

            Series pieSeries1 = new Series("By Status")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true
            };

            foreach (var group in tickets.GroupBy(t => t.Status ?? "Unknown"))
            {
                pieSeries1.Points.AddXY(group.Key, group.Count());
            }

            chartStatus.Series.Add(pieSeries1);
            this.Controls.Add(chartStatus);

            // Pie Chart: Tickets by Priority
            Chart chartPriority = new Chart
            {
                Size = new Size(250, 200),
                Location = new Point(600, 420)
            };
            ChartArea pieArea2 = new ChartArea();
            chartPriority.ChartAreas.Add(pieArea2);

            Series pieSeries2 = new Series("By Priority")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true
            };

            foreach (var group in tickets.GroupBy(t => t.Priority ?? "Unspecified"))
            {
                pieSeries2.Points.AddXY(group.Key, group.Count());
            }

            chartPriority.Series.Add(pieSeries2);
            this.Controls.Add(chartPriority);
        }
    }
}

