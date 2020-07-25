namespace Yaba.Infrastructure.DTO
{
    public class BankAccountResponseDTO
    {
        public int Id { get; set; }
        public short BankCode { get; set; }
        public string BankName { get; set; }
        public string Agency { get; set; }
        public string AccountNumber { get; set; }
    }
}
