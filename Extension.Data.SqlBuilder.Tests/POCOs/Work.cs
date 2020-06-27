using System.Collections.Generic;

namespace Extension.Data.SqlBuilder.Tests.POCOs
{
    public class Work
    {
        public string WorkId { get; set; }
        public string Address { get; set; }
        public ICollection<Person> Employees { get; set; }
    }
}
