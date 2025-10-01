using System.Configuration;

namespace IT_HelpDesk
{
    public static class AppConfig
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["HelpDeskDB"].ConnectionString;
            }
        }
    }
}
