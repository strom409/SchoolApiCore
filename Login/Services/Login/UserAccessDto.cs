namespace Login.Services.Login
{
    public class UserAccessDto
    {
        public long ID { get; set; }
        public long UIDFK { get; set; }
        public string UserName { get; set; }
        public string MasterIDs { get; set; }
        public string PageIDs { get; set; }
    }
}
