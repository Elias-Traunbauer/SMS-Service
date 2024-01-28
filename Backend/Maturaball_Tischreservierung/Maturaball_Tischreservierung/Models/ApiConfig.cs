namespace Maturaball_Tischreservierung.Models
{
    public class ApiConfig
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public TimeSpan AccessTokenLifetime { get; set; }

        public string AccessTokenCookieIdentifier { get; set; } = string.Empty;

        public string ConnectionString { get; set; } = string.Empty;
        public string SmtpAddress { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
    }
}
