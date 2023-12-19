namespace SV20T1080029.Web.Models
{
    public class Student
    {
        public string? StudentId { get; set; }
        public string? StudentName { get; set;}


    }

    public class StudentDal
    {
        public List<Student> List()
        {
            List<Student> list = new List<Student>();

            list.Add(new Student
            {
                StudentId = "1",

                StudentName = "Kieth2"
            });

            list.Add(new Student
            {
                StudentId = "12",

                StudentName = "Kieth"
            });



            return list;
        }


    }
}
