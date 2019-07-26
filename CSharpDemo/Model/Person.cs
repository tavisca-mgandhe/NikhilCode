using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpSample.Model
{
    /// var
    /// anonymous types
    /// delegates
    /// extension methods
    class Person
    {
        public Person(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public interface IFruit { }

    public class Apple : IFruit { }
}
