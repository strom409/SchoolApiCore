namespace Login.Services.Users
{
    public class UsersModel
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhoto { get; set; }
        public string UserAddress { get; set; }
        public string UserPhoneNo { get; set; }
        public long UserTypeID { get; set; }
        public string ControlId { get; set; }
        public string UserRemarks { get; set; }
        public string UserLogoPath { get; set; }
        public string Current_Session { get; set; }
        public long? SessionID { get; set; }
        public bool Activation { get; set; }
        public string Dashboard { get; set; }
        public string Control1Id { get; set; }
        public string ClassIDS { get; set; }
        public int Allows { get; set; }
    }
}
