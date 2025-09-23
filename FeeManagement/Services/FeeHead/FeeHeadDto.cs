namespace FeeManagement.Services.FeeHead
{
    public class FeeHeadDto
    {
        public long FHID { get; set; }       
        public string FHName { get; set; }    
        public int FHType { get; set; }       
        public string UserName { get; set; }  
        public bool IsDeleted { get; set; }    
        public DateTime? UpdatedOn { get; set; } 
        public string UpdatedBy { get; set; }  
        public decimal Frate { get; set; }     
        public int IsPrimary { get; set; }
    }
}
