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

            //this equals to C# expression: it => it.Name == "Pam Beesly" && it.Age > 30 && it.Age < 40
            var expression = "and(eq(it.Name, \"Pam Beesly\"), gt(it.Age, 30), lt(it.Age, 40))";

            var selected = employees.AsQueryable().Where(expression);
            foreach (var e in selected)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }
    }
}
