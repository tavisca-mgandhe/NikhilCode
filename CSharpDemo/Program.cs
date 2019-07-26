using CSharpSample.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSharpSample
{
    public class FunctionalDelegate
    {
        public FunctionalDelegate(Type returnType, params Type[] argTypes) : this(null, returnType, argTypes)
        {
        }

        public FunctionalDelegate(object instance, Type returnType, params Type[] argTypes)
        {
        }

        public object Target { get; set; }

        public MethodInfo Method { get; set; }

        public object Invoke(params object[] args)
        {
            return Method.Invoke(Target, args);
        }
    }

    public class ActionDelegate
    {
        public static ActionDelegate Create<Type, TArg>(string name)
        {
            return new ActionDelegate(typeof(Type), name, typeof(TArg));
        }

        public ActionDelegate(Type type, string name, params Type[] argTypes) : this(null, type, name, argTypes)
        {
        }

        public ActionDelegate(object instance, Type type, string name, params Type[] argTypes)
        {
            Target = instance;
            TargetType = type;
            Method = GetMatchingMethod(type, name, argTypes);
        }

        private MethodInfo GetMatchingMethod(Type type, string name, Type[] argTypes)
        {
            return type.GetMethod(name, argTypes);
        }

        public Type TargetType { get; set;}

        public object Target { get; set; }

        public MethodInfo Method { get; set; }

        public void Invoke(params object[] args)
        {
            Method.Invoke(Target, args);
        }
    }


    class Program
    {


        static void Main(string[] args)
        {
            var runner = new TestRunner();
            var del = new ActionDelegate(typeof(Program), "Print", typeof(string));
            var del2 = ActionDelegate.Create<Program, string>("Print");
            Action<string> del3 = Print;
            del3("Nikhil");
            del.Invoke("Nikhil");
        }

        public static void Print(string name)
        {
            Console.WriteLine($"Hello {name}");
        }
    }
}
