namespace Yaba.Infrastructure.DTO
{
    public class JwtConfig
    {
        public string SecretKey { get; set; }
    }

    public class AzureConfig
    {
        public string QueueName { get; set; }
    }

    public class ConnectionStrings
    {
        public string AzureWebJobsStorage { get; set; }
    }

    public class AuthConfig
    {
        public string[] Issuers { get; set; }
        public string[] Audiences { get; set; }
        public string Authority { get; set; }
    }
}
