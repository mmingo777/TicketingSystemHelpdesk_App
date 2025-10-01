using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace IT_HelpDesk
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblError;

        public LoginForm()
        {
            InitializeLoginUI();
        }

        private void InitializeLoginUI()
        {
            this.Text = "Login - NY Transit Helpdesk";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblUsername = new Label { Text = "Email:", Location = new Point(60, 30), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(150, 25), Width = 160 };

            Label lblPassword = new Label { Text = "Password:", Location = new Point(60, 70), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(150, 65), Width = 160, PasswordChar = '●' };

            btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(150, 110),
                Width = 100,
                BackColor = Theme.PrimaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.Click += BtnLogin_Click;

            lblError = new Label
            {
                ForeColor = Color.Red,
                AutoSize = true,
                Location = new Point(60, 150),
                Visible = false
            };

            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lblError);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Please enter both email and password.";
                lblError.Visible = true;
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(
                  System.Configuration.ConfigurationManager.ConnectionStrings["HelpDeskDB"].ConnectionString))

                {
                    conn.Open();
                    string query = @"SELECT USER_ID, USER_EML, USER_PWD, ROLE_ID FROM USERS 
                                     WHERE USER_EML = @email AND USER_PWD = @password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Success: save session data
                                Session.UserId = reader["USER_ID"].ToString();
                                Session.Username = reader["USER_EML"].ToString();
                                Session.Role = reader["ROLE_ID"].ToString();

                                MessageBox.Show("Login successful!", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                this.Hide();
                                Form1 dashboard = new Form1(Session.Role);

                                dashboard.Show();
                            }
                            else
                            {
                                lblError.Text = "Invalid login credentials.";
                                lblError.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
