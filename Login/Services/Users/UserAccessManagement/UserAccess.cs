namespace Login.Services.Users.UserAccessManagement
{
    public class UserAccess
    {
        public string ID { get; set; }
        public string UIDFK { get; set; }
        public string UserName { get; set; }
        public string MasterIDs { get; set; }
        public string PageIDs { get; set; }
    }
}
