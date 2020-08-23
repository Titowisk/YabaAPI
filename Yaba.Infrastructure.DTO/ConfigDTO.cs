namespace Yaba.Infrastructure.DTO
{
    public class JwtConfig
    {
        public string SecretKey { get; set; }
    }

    public class AzureConfig
    {
        public string AzureWebJobsStorage { get; set; }
        public string QueueName { get; set; }
    }

}
