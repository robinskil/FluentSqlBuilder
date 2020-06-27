using System;
using System.Text;

namespace Extension.Data.SqlBuilder.Tests.POCOs
{
    public class Person
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public double Salary { get; set; }
        public string WorkId { get; set; }

        public Work WorksAt { get; set; }
    }
}
