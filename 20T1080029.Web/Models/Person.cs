namespace SV20T1080029.Web.Models
{
    public class Person
    {
        public int PersonId { get; set; } //public là access modìier  // private , internal 
        public string? Name { get; set; } // cho phep gia tri null neu có dấu ?
        public string? Address { get; set; }
        public string? Email { get; set; }
    }
}
