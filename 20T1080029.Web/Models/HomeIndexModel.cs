using SV20T1080029.Web.Models;
namespace SV20T1080029.Web.Models
{
    public class HomeIndexModel
    {

        public string? TitleMessage { get; set; }
        public List<Person>? ListOfPersons { get; set; }
        public List<Student>? ListOfStudents { get; set; }
    }
}
