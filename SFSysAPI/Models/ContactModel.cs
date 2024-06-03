namespace SFSysAPI.Models
{
    public class Contacts
    {
        public int totalSize { get; set; }
        public bool done { get; set; }
        public List<ContactRecord> records { get; set; }
    }
    public class ContactRecord
    {
        public Attributes attributes { get; set; }
        public string Name { get; set; }
        public string AccountId { get; set; }
        public object MobilePhone { get; set; }
        public string Email { get; set; }
    }
    public class SendContact
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class SendContacts
    {
        public List<SendContact> sed{ get; set; }
    }
}