using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace IT_HelpDesk
{
    public partial class TicketDetailsForm : Form
    {
        private Ticket ticket;
        private Label labelAssignedTo;
        private Label labelSubmittedBy;
        private FlowLayoutPanel pnlComments;
        private TextBox txtComment;
        private Button btnAddComment;



        public TicketDetailsForm(Ticket selectedTicket)
        {
            this.ticket = selectedTicket;
            this.Text = $"Ticket #{ticket.Id} Details";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            InitializeUI();
        }


        private void InitializeUI()
        {
            var assignedInfo = TicketDataAccess.GetUserInfo(ticket.AssignedTo);
            var submittedInfo = TicketDataAccess.GetUserInfo(ticket.SubmittedBy);

            // Layout structure with 3 rows
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                BackColor = Color.White
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // Top: ticket info
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));  // Middle: comments
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90)); // Bottom: add comment

            // ==== Top Ticket Info Section ====
            FlowLayoutPanel topPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10),
                BackColor = Color.WhiteSmoke
            };

            Label lblHeader = new Label
            {
                Text = $"Ticket: {ticket.Title}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true
            };
            topPanel.Controls.Add(lblHeader);

            Label lblDescription = new Label
            {
                Text = $"Description: {ticket.Description}",
                Font = new Font("Segoe UI", 9),
                AutoSize = true
            };
            topPanel.Controls.Add(lblDescription);

            Label lblMeta = new Label
            {
                Text = $"Priority: {ticket.Priority}, Status: {ticket.Status}, Created: {ticket.CreatedDate:g}",
                Font = new Font("Segoe UI", 9),
                AutoSize = true
            };
            topPanel.Controls.Add(lblMeta);

            labelAssignedTo = new Label
            {
                Text = $"Assigned To: {assignedInfo.FullName} ({assignedInfo.Email}) - 📞 {assignedInfo.Phone}",
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
            topPanel.Controls.Add(labelAssignedTo);

            labelSubmittedBy = new Label
            {
                Text = $"Submitted By: {submittedInfo.FullName} ({submittedInfo.Email}) - 📞 {submittedInfo.Phone}",
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
            topPanel.Controls.Add(labelSubmittedBy);

            ComboBox cmbStatus = new ComboBox
            {
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            List<string> statuses = TicketDataAccess.GetAllStatusTypes();
            cmbStatus.Items.AddRange(statuses.ToArray());
            cmbStatus.SelectedItem = ticket.Status;
            topPanel.Controls.Add(cmbStatus);

            Button btnSaveStatus = new Button
            {
                Text = "Update Status",
                Width = 120,
                BackColor = Theme.PrimaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5)
            };
            btnSaveStatus.Click += (s, e) =>
            {
                string newStatus = cmbStatus.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(newStatus) && newStatus != ticket.Status)
                {
                    TicketDataAccess.UpdateTicketStatus(ticket.Id, newStatus);
                    MessageBox.Show("Ticket status updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            topPanel.Controls.Add(btnSaveStatus);

            layout.Controls.Add(topPanel, 0, 0);

            // ==== Middle Comment Display Area ====
            pnlComments = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            foreach (var comment in ticket.Comments)
            {
                AddCommentPanel(comment);
            }

            layout.Controls.Add(pnlComments, 0, 1);

            // ==== Bottom Add Comment Box ====
            FlowLayoutPanel commentPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10),
                BackColor = Color.WhiteSmoke
            };

            txtComment = new TextBox
            {
                Width = 500,
                Height = 60,
                Multiline = true,
                Font = new Font("Segoe UI", 9)
            };

            btnAddComment = new Button
            {
                Text = "Add Comment",
                Width = 120,
                Height = 60,
                BackColor = Theme.PrimaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddComment.Click += BtnAddComment_Click;

            commentPanel.Controls.Add(txtComment);
            commentPanel.Controls.Add(btnAddComment);
            layout.Controls.Add(commentPanel, 0, 2);

            // ==== Add entire layout to form ====
            this.Controls.Add(layout);
        }




        private void BtnAddComment_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtComment.Text))
            {
                var newComment = new Comment
                {
                    Author = Session.Username,
                    Message = txtComment.Text.Trim(),
                    Timestamp = DateTime.Now
                };

                ticket.Comments.Add(newComment);

                // Save to CSV or log file (or stub method if not using files)
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TicketComments.csv");
                TicketDataAccess.SaveToFile(path);

                AddCommentPanel(newComment);
                txtComment.Clear();
            }
        }

        private void AddCommentPanel(Comment comment)
        {
            var panel = new Panel
            {
                Width = 600,
                Height = 60,
                Margin = new Padding(5),
                BackColor = Color.LightGray,
                Padding = new Padding(8)
            };

            var lblComment = new Label
            {
                Text = $"[{comment.Timestamp:MM/dd HH:mm}] {comment.Author}: {comment.Message}",
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            panel.Controls.Add(lblComment);
            pnlComments.Controls.Add(panel);
        }
    }
}

