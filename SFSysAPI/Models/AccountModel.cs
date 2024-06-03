namespace SFSysAPI.Models
{
    public class GetAccountResponse
    {
        public string? Name { get; set; }
        public string? AccountNumber {get; set;}
        public string? Site { get; set; }  
        public string? Phone { get; set; }
    }

    public class SFAccountResponse
    {
        public int totalSize { get; set; }
        public bool done { get; set; }
        public List<AccountRecord> records { get; set; }
    }
    public class AccountRecord
    {
        public Attributes attributes { get; set; }
        public string Name { get; set; }
        public object AccountNumber { get; set; }
        public object Site { get; set; }
        public object Phone { get; set; }
    }

    public class Attributes
    {
        public string type { get; set; }
        public string url { get; set; }
    }
}