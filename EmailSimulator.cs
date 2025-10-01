using System;
using System.IO;
using System.Windows.Forms;

namespace IT_HelpDesk
{
    public static class EmailSimulator
    {
        private static readonly string LogFilePath = "simulated_emails.txt";

        public static void SendEmail(string to, string subject, string body)
        {
            string message = $"[{DateTime.Now}]\nTo: {to}\nSubject: {subject}\n{body}\n---------------------------\n";

            try
            {
                File.AppendAllText(LogFilePath, message);
                MessageBox.Show("📧 Simulated email logged!\nSubject: " + subject, "Email Simulator", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to log simulated email:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
