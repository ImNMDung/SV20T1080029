namespace SV20T1080029.Web.Models
{
    public class PersonDAL
    { 
        public List<Person> List()
        {
            List<Person> list = new List<Person>(); //camelCase // tên biến sử dụng camelCase

            list.Add(new Person()
            {
                PersonId = 1,
                Name = "Nguyễn Dũng ",
                Address = "QB", 
                Email = "2@husc.edu.vn"
            }
            );
            list.Add(new Person()
            {
                PersonId = 2,
                Name = "Nguyễn Dũng2 ",
                Address = "QB2",
                Email = "2@husc.edu.vn2"
            }
            );
            list.Add(new Person()
            {
                PersonId = 3,
                Name = "Nguyễn Dũng3 ",
                Address = "QB3",
                Email = "2@husc.edu.vn3"
            }
            );

            return list;
        }
    }
}
