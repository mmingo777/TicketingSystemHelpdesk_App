using System;
using System.Drawing;
using System.Windows.Forms;

namespace IT_HelpDesk
{
    public partial class UserProfileControl : UserControl
    {
        public UserProfileControl()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Label header = new Label
            {
                Text = "User Profile",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true
            };
            this.Controls.Add(header);

            AddInfoRow("Name:", "John Doe", 70);
            AddInfoRow("Role:", "IT Support Specialist", 120);
            AddInfoRow("Department:", "IT Services", 170);
            AddInfoRow("Email:", "john.doe@nytransit.org", 220);

            Button btnEdit = new Button
            {
                Text = "Edit Profile",
                Location = new Point(30, 280),
                Width = 120,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };
            btnEdit.Click += (s, e) => MessageBox.Show("Edit functionality coming soon!");
            this.Controls.Add(btnEdit);
        }

        private void AddInfoRow(string label, string value, int top)
        {
            this.Controls.Add(new Label
            {
                Text = label,
                Location = new Point(30, top),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            });

            this.Controls.Add(new Label
            {
                Text = value,
                Location = new Point(150, top),
                Font = new Font("Segoe UI", 10),
                AutoSize = true
            });
        }
    }
}

