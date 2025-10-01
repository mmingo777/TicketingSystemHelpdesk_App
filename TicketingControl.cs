using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;

namespace IT_HelpDesk
{
    public partial class TicketingControl : UserControl
    {
            private readonly Dictionary<string, string> PriorityMap = new Dictionary<string, string>
    {
        { "Critical", "P1" },
        { "High", "P2" },
        { "Medium", "P3" },
        { "Low", "P4" },
        { "None", "P5" }
    };

            private TextBox txtTitle, txtDescription;
        private ComboBox cmbPriority;
        private Button btnSubmit;
        private TextBox txtAttachmentName;
        private Button btnAttachFile;
        private ListBox lstAttachments;
        private List<string> attachments = new List<string>();
        private DataGridView dgvTickets = new DataGridView();

        public TicketingControl()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // Initialize controls for tabs
            InitializeForm();
            InitializeTicketGrid();
            LoadTicketsIntoGrid();

            // Create tabs
            TabControl tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            TabPage tabSubmit = new TabPage("Submit Ticket") { BackColor = Color.White };
            tabSubmit.Controls.AddRange(this.Controls.Cast<Control>().ToArray());
            this.Controls.Clear();

            TabPage tabTickets = new TabPage("Ticket List") { BackColor = Color.White };
            tabTickets.Controls.Add(dgvTickets);

            tabControl.TabPages.Add(tabSubmit);
            tabControl.TabPages.Add(tabTickets);

            this.Controls.Add(tabControl);
        }

        private void InitializeForm()
        {
            this.Padding = new Padding(20);

            int startX = 300, currentY = 40, labelSpacing = 30, inputSpacing = 40;

            Label lblHeader = new Label
            {
                Text = "Submit New Ticket",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Theme.PrimaryColor,
                AutoSize = true,
                Location = new Point(startX, currentY)
            };
            currentY += 60;

            Label lblTitle = new Label { Text = "Issue Title:", Location = new Point(startX, currentY), AutoSize = true };
            currentY += labelSpacing;
            txtTitle = new TextBox { Location = new Point(startX, currentY), Width = 400, BorderStyle = BorderStyle.FixedSingle };
            currentY += inputSpacing;

            Label lblDesc = new Label { Text = "Description:", Location = new Point(startX, currentY), AutoSize = true };
            currentY += labelSpacing;
            txtDescription = new TextBox
            {
                Location = new Point(startX, currentY),
                Width = 400,
                Height = 80,
                Multiline = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            currentY += 90;

            Label lblPriority = new Label { Text = "Priority:", Location = new Point(startX, currentY), AutoSize = true };
            currentY += labelSpacing;
            cmbPriority = new ComboBox
            {
                Location = new Point(startX, currentY),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPriority.Items.AddRange(new[] { "Low", "Medium", "High", "Critical" });
            cmbPriority.SelectedIndex = 0;
            currentY += inputSpacing + 10;

            Label lblAttachment = new Label { Text = "Add Attachment:", Location = new Point(startX, currentY), AutoSize = true };
            currentY += labelSpacing;
            txtAttachmentName = new TextBox { Location = new Point(startX, currentY), Width = 250, BorderStyle = BorderStyle.FixedSingle };
            btnAttachFile = new Button
            {
                Text = "Attach File",
                Location = new Point(startX + 260, currentY - 1),
                Width = 100,
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAttachFile.FlatAppearance.BorderSize = 0;
            btnAttachFile.Click += BtnAttachFile_Click;
            currentY += inputSpacing;



            lstAttachments = new ListBox
            {
                Location = new Point(startX, currentY),
                Width = 360,
                Height = 60
            };
            currentY += 80;

            btnSubmit = new Button
            {
                Text = "Submit Ticket",
                Location = new Point(startX, currentY),
                Width = 200,
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Click += BtnSubmit_Click;

            this.Controls.AddRange(new Control[]
            {
                lblHeader, lblTitle, txtTitle, lblDesc, txtDescription,
                lblPriority, cmbPriority, lblAttachment, txtAttachmentName,
                btnAttachFile, lstAttachments, btnSubmit
            });
        }

        private void InitializeTicketGrid()
        {
            dgvTickets.Dock = DockStyle.Fill;
            dgvTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTickets.BackgroundColor = Color.White;
            dgvTickets.BorderStyle = BorderStyle.None;
        }

        private void LoadTicketsIntoGrid()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["HelpDeskDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT TKT_ID, TKT_SUBJ, STS_ID, PRI_ID, CRD_DATE, RES_DATE FROM TICKET";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvTickets.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load tickets:\n" + ex.Message, "Error");
            }
        }

        private void BtnAttachFile_Click(object sender, EventArgs e)
        {
            string fileName = txtAttachmentName.Text.Trim();
            if (!string.IsNullOrEmpty(fileName))
            {
                attachments.Add(fileName);
                lstAttachments.Items.Add(fileName);
                txtAttachmentName.Clear();
            }
            else
            {
                MessageBox.Show("Please enter a file name to attach.");
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedPriorityText = cmbPriority.SelectedItem.ToString();
            string selectedPriorityId = PriorityMap[selectedPriorityText];

            var ticket = new Ticket
            {
                Id = "T" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper(),
                Title = txtTitle.Text,
                Description = txtDescription.Text,
                Priority = selectedPriorityId, // ✅ Add this
                Status = "S1",
                AssignedTo = "U001",
                SubmittedBy = "U002",
                CreatedDate = DateTime.Now,
                Attachments = new List<string>(attachments)
            };


            try
            {
                TicketDataAccess.AddTicket(ticket);
                MessageBox.Show("Ticket submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error submitting ticket: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // ✅ Clear form
            txtTitle.Clear();
            txtDescription.Clear();
            cmbPriority.SelectedIndex = 0;
            txtAttachmentName.Clear();
            lstAttachments.Items.Clear();
            attachments.Clear();
        }
    }
}

