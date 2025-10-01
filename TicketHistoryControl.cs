using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IT_HelpDesk
{
    public partial class TicketHistoryControl : UserControl
    {
        private DataGridView dgvTickets;
        private Button btnExportAllTickets;
        private string selectedTicketId = string.Empty;


        public TicketHistoryControl()
        {
            InitializeUI();
            Theme.ApplyControlTheme(this);
            LoadTicketsFromDatabase();
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.Margin = new Padding(230, 20, 20, 20);

            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(230, 50, 10, 10),
                BackColor = this.BackColor
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 90));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            dgvTickets = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 9),
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                DefaultCellStyle = {
                    WrapMode = DataGridViewTriState.True,
                    Padding = new Padding(3, 2, 3, 2)
                },
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = {
                    BackColor = Color.LightSteelBlue,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                ColumnHeadersHeight = 35
            };
            dgvTickets.DoubleClick += OpenTicketDetailsModal;
            mainLayout.Controls.Add(dgvTickets, 0, 0);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = false,
                Height = 40,
                Padding = new Padding(10, 5, 10, 5),
                WrapContents = false
            };

            btnExportAllTickets = new Button
            {
                Text = "Export All Tickets to CSV",
                Width = 200,
                Height = 30,
                BackColor = Theme.PrimaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExportAllTickets.FlatAppearance.BorderSize = 0;
            btnExportAllTickets.Click += ExportAllTicketsToCSV;
            buttonPanel.Controls.Add(btnExportAllTickets);

            if (Session.UserRole == "Admin")
            {
                Button btnDeleteSelected = new Button
                {
                    Text = "Delete Selected Ticket",
                    Width = 200,
                    Height = 30,
                    BackColor = Theme.PrimaryColor,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnDeleteSelected.FlatAppearance.BorderSize = 0;
                btnDeleteSelected.Click += DeleteSelectedTicket;
                buttonPanel.Controls.Add(btnDeleteSelected);

            }

            mainLayout.Controls.Add(buttonPanel, 0, 1);
            this.Controls.Add(mainLayout);
        }

        private void DeleteSelectedTicket(object sender, EventArgs e)
        {
            if (dgvTickets.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a ticket to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show(
                "Are you sure you want to delete the selected ticket?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult != DialogResult.Yes) return;

            try
            {
                string ticketId = dgvTickets.SelectedRows[0].Cells["Id"].Value.ToString();
                bool success = TicketDataAccess.DeleteTicketById(ticketId);

                if (success)
                {
                    MessageBox.Show("Ticket deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadTicketsFromDatabase();
                }
                else
                {
                    MessageBox.Show("Failed to delete ticket.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while deleting the ticket:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTicketsFromDatabase()
        {
            var tickets = TicketDataAccess.GetAllTicketsFromDatabase();

            dgvTickets.DataSource = tickets.Select(t => new
            {
                t.Id,
                t.Title,
                t.Status,
                t.Priority,
                Created = t.CreatedDate,
                AssignedTo = t.AssignedTo
            }).ToList();

            foreach (DataGridViewColumn col in dgvTickets.Columns)
            {
                col.AutoSizeMode = col.Name == "Title" ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.DisplayedCells;
            }
        }

        private void ExportAllTicketsToCSV(object sender, EventArgs e)
        {
            var tickets = TicketDataAccess.GetAllTicketsFromDatabase();
            if (!tickets.Any()) return;

            StringBuilder csv = new StringBuilder();
            csv.AppendLine("Id,Title,Status,Priority,CreatedDate,AssignedTo");

            foreach (var t in tickets)
            {
                csv.AppendLine($"{t.Id},\"{t.Title}\",{t.Status},{t.Priority},{t.CreatedDate},{t.AssignedTo}");
            }

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "All_Tickets.csv");
            File.WriteAllText(path, csv.ToString());

            MessageBox.Show($"All tickets exported to:\n{path}", "Export Complete");
        }

        private void OpenTicketDetailsModal(object sender, EventArgs e)
        {
            if (dgvTickets.SelectedRows.Count == 0) return;

            // Store selected ID in the class-level variable
            selectedTicketId = dgvTickets.SelectedRows[0].Cells["Id"].Value.ToString();

            var ticket = TicketDataAccess.GetTicketById(selectedTicketId);

            if (ticket == null) return;

            var modal = new TicketDetailsForm(ticket);
            modal.ShowDialog();
        }
    }
}

