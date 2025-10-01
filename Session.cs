namespace IT_HelpDesk
{
    public static class Session
    {
        public static string Username { get; set; }
        public static string UserRole { get; set; }
        public static string Role { get; set; }
        public static string UserId { get; set; }
        public static string CurrentUser => Username;
        public static string CurrentRole => Role;
    }
}

  
    