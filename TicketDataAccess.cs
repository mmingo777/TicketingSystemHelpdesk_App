using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;


namespace IT_HelpDesk
{
    public static class TicketDataAccess
    {
        private static readonly string connString = ConfigurationManager.ConnectionStrings["HelpDeskDB"].ConnectionString;

        public static List<Ticket> GetAllTicketsFromDatabase()
        {
            List<Ticket> tickets = new List<Ticket>();
            string connString = ConfigurationManager.ConnectionStrings["HelpDeskDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
            SELECT 
                TKT.TKT_ID,
                TKT.TKT_SUBJ,
                STS.STS_TYP AS StatusName,
                PRI.PRI_TYP AS PriorityName,
                TKT.CRD_DATE,
                TKT.ASS_TO_USER_ID
            FROM TICKET TKT
            LEFT JOIN STATUS STS ON TKT.STS_ID = STS.STS_ID
            LEFT JOIN PRIORITY PRI ON TKT.PRI_ID = PRI.PRI_ID";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tickets.Add(new Ticket
                    {
                        Id = reader["TKT_ID"].ToString(),
                        Title = reader["TKT_SUBJ"].ToString(),
                        Status = reader["StatusName"].ToString(),
                        Priority = reader["PriorityName"].ToString(),
                        CreatedDate = Convert.ToDateTime(reader["CRD_DATE"]),
                        AssignedTo = reader["ASS_TO_USER_ID"].ToString()
                    });
                }
            }

            return tickets;
        }

        public static void AddTicket(Ticket ticket)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"INSERT INTO TICKET (TKT_ID, TKT_SUBJ, TKT_DES, STS_ID, PRI_ID, 
                                                      INC_ORI_ID, CAT_ID, SUB_CAT_ID, 
                                                      ASS_TO_USER_ID, REQ_BY_USER_ID, CRD_DATE, RES_DATE)
                                 VALUES (@TKT_ID, @TKT_SUBJ, @TKT_DES, @STS_ID, @PRI_ID, 
                                         @INC_ORI_ID, @CAT_ID, @SUB_CAT_ID,
                                         @ASS_TO_USER_ID, @REQ_BY_USER_ID, @CRD_DATE, @RES_DATE)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TKT_ID", ticket.Id);
                cmd.Parameters.AddWithValue("@TKT_SUBJ", ticket.Title);
                cmd.Parameters.AddWithValue("@TKT_DES", ticket.Description ?? "");
                cmd.Parameters.AddWithValue("@STS_ID", ticket.Status);
                cmd.Parameters.AddWithValue("@PRI_ID", ticket.Priority);
                cmd.Parameters.AddWithValue("@INC_ORI_ID", ticket.OriginId ?? "IO1");
                cmd.Parameters.AddWithValue("@CAT_ID", ticket.CategoryId ?? "CAT1");
                cmd.Parameters.AddWithValue("@SUB_CAT_ID", ticket.SubcategoryId ?? "SUBC1");
                cmd.Parameters.AddWithValue("@ASS_TO_USER_ID", ticket.AssignedTo);
                cmd.Parameters.AddWithValue("@REQ_BY_USER_ID", ticket.SubmittedBy);
                cmd.Parameters.AddWithValue("@CRD_DATE",
                    ticket.CreatedDate >= SqlDateTime.MinValue.Value
                    ? (object)ticket.CreatedDate
                    : DBNull.Value);
                cmd.Parameters.AddWithValue("@RES_DATE",
                    ticket.ResolvedDate.HasValue && ticket.ResolvedDate.Value >= SqlDateTime.MinValue.Value
                    ? (object)ticket.ResolvedDate.Value
                    : DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void ClearAll()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "DELETE FROM TICKET";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void SaveToFile(string filePath)
        {
            var tickets = GetAllTicketsFromDatabase();
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Id,Title,Description,Status,Priority,CreatedDate,AssignedTo,SubmittedBy");
                foreach (var t in tickets)
                {
                    writer.WriteLine($"{t.Id},\"{t.Title}\",\"{t.Description}\",{t.Status},{t.Priority},{t.CreatedDate},{t.AssignedTo},{t.SubmittedBy}");
                }
            }
        }
        public static bool DeleteTicketById(string id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["HelpDeskDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM TICKET WHERE TKT_ID = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        public static Ticket GetTicketById(string ticketId)
        {
            Ticket ticket = null;
            string connectionString = ConfigurationManager.ConnectionStrings["HelpDeskDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                TKT.TKT_ID,
                TKT.TKT_SUBJ,
                TKT.TKT_DES,
                TKT.STS_ID,
                STS.STS_TYP AS StatusName,
                TKT.PRI_ID,
                PRI.PRI_TYP AS PriorityName,
                TKT.INC_ORI_ID,
                TKT.CAT_ID,
                TKT.SUB_CAT_ID,
                TKT.ASS_TO_USER_ID,
                TKT.REQ_BY_USER_ID,
                TKT.CRD_DATE,
                TKT.RES_DATE
            FROM TICKET TKT
            LEFT JOIN STATUS STS ON TKT.STS_ID = STS.STS_ID
            LEFT JOIN PRIORITY PRI ON TKT.PRI_ID = PRI.PRI_ID
            WHERE TKT.TKT_ID = @TicketId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TicketId", ticketId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ticket = new Ticket
                    {
                        Id = reader["TKT_ID"].ToString(),
                        Title = reader["TKT_SUBJ"].ToString(),
                        Description = reader["TKT_DES"].ToString(),
                        Status = reader["StatusName"].ToString(),
                        Priority = reader["PriorityName"].ToString(),
                        OriginId = reader["INC_ORI_ID"].ToString(),
                        CategoryId = reader["CAT_ID"].ToString(),
                        SubcategoryId = reader["SUB_CAT_ID"].ToString(),
                        AssignedTo = reader["ASS_TO_USER_ID"].ToString(),
                        SubmittedBy = reader["REQ_BY_USER_ID"].ToString(),
                        CreatedDate = Convert.ToDateTime(reader["CRD_DATE"]),
                        ResolvedDate = reader["RES_DATE"] != DBNull.Value ? Convert.ToDateTime(reader["RES_DATE"]) : (DateTime?)null
                    };
                }
            }

            return ticket;
        }
        public static (string FullName, string Email, string Phone) GetUserInfo(string userId)
        {
            string fullName = "", email = "", phone = "";
            string connString = ConfigurationManager.ConnectionStrings["HelpDeskDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT USER_FST_NAME, USER_LST_NAME, USER_EML, USER_PHONE FROM USERS WHERE USER_ID = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    fullName = reader["USER_FST_NAME"] + " " + reader["USER_LST_NAME"];
                    email = reader["USER_EML"].ToString();
                    phone = reader["USER_PHONE"] != DBNull.Value ? reader["USER_PHONE"].ToString() : "N/A";
                }
            }

            return (fullName, email, phone);
        }
        public static List<string> GetAllStatusTypes()
        {
            List<string> statuses = new List<string>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT STS_TYP FROM STATUS", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    statuses.Add(reader["STS_TYP"].ToString());
                }
            }
            return statuses;
        }

        public static void UpdateTicketStatus(string ticketId, string newStatus)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
            UPDATE TICKET
            SET STS_ID = (SELECT STS_ID FROM STATUS WHERE STS_TYP = @NewStatus)
            WHERE TKT_ID = @TicketId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NewStatus", newStatus);
                cmd.Parameters.AddWithValue("@TicketId", ticketId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

