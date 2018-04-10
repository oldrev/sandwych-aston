using System;
using System.Linq;
using System.Linq.Expressions;
using Sandwych.Aston.Linq;

namespace Sandwych.Aston.ExampleApp
{
    public class Employee
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public override string ToString() =>
            $"Employee(Name='{Name}', Age={Age})";
    }

    class Program
    {
        static void Main(string[] args)
        {
            var employees = new Employee[]
            {
                new Employee{ Name = "Michael Scott", Age = 46},
                new Employee{ Name = "Pam Beesly", Age = 32},
                new Employee{ Name = "Jim Halpert", Age = 33},
                new Employee{ Name = "Dwight Schrute", Age = 34},
            };

            var selected = employees.AsQueryable().Where("==(it.Name, \"Pam Beesly\")");
            foreach (var e in selected)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }
    }
}
