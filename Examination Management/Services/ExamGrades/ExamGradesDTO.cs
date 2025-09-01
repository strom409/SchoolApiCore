namespace Examination_Management.Services.ExamGrades
{
    public class ExamGradesDTO
    {
        public string? GradeId { get; set; }                
        public decimal From { get; set; }               
        public decimal To { get; set; }                 
        public string Grade { get; set; }              
        public string? TeacherRemarks { get; set; }    
        public string? PrincipalRemarks { get; set; }   
        public string? user { get; set; }               
        public DateTime? UpdatedOn { get; set; }        
        public string? UpdatedBy { get; set; }         
        public decimal? GradePoint { get; set; }        
        public decimal? ranks { get; set; }
    }
}
