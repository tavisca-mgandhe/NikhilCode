using CSharpSample.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSharpSample
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestMethodAttribute : Attribute
    {
    }

    public static class Assertions
    {
        public static void AreEqual<T>(T value, T expected)
        {
            if (value.Equals(expected) == false)
                throw new Exception($"{value} != {expected}");
        }

    }

    public class TypeMethodPair
    {
        public TypeMethodPair(Type type, MethodInfo method)
        {
            Type = type;
            Method = method;
        }

        public Type Type { get; }

        public MethodInfo Method { get; }
    }

    public class TestClass
    {
        [TestMethod]
        public void Simple_passing_test()
        {
            Assertions.AreEqual(1, 1);
        }

        [TestMethod]
        public void Simple_failing_test()
        {
            Assertions.AreEqual(1, 2);
        }

        public void SomeDummyMethod()
        {
        }
    }


    public class TestRunner
    {
        public void RunTests(Assembly assembly )
        {
            ///Spec:
            /// 1. A test method is 
            ///     a. Uniquely marked with a [TestMethod] attribute
            ///     b. Must not take any parameters.
            /// 2. A test class is a class which has the following features
            ///     a. It should have a parameterless constructor.
            ///     b. It should have atleast one test method.
            /// 3. Execute the test methods

            Predicate<MethodInfo> hasTestAttr = m => m.IsDefined(typeof(TestMethodAttribute), false) == true;
            Predicate<MethodInfo> isParameterless = m => m.GetParameters().Length == 0;
            Predicate<MethodInfo> isTestMethod = m => hasTestAttr(m) == true && isParameterless(m) == true;

            Predicate<Type> hasTestMethods = t => t.GetMethods().Any(m => isTestMethod(m) == true);
            Predicate<Type> hasParamlessConstr = t => t.GetConstructors().Any(c => c.GetParameters().Length == 0);
            Predicate<Type> isTestClass = t => hasTestMethods(t) == true && hasParamlessConstr(t) == true;

            assembly
                .GetTypes()
                .Where(t => isTestClass(t) == true)
                .Select(t => new
                {
                    Run = new Action(
                                () => t .GetMethods()
                                        .Where(m => isTestMethod(m) == true)
                                        .ToList()
                                        .ForEach(m => ExecuteTest(t, m)))
                })
                .ToList()
                .ForEach(f => f.Run());
            // done 

            var types = assembly.GetTypes();
            List<Type> testClasses = new List<Type>();
            foreach ( var type in types )
            {
                if (IsTestClass(type) == true)
                    testClasses.Add(type);
            }

            List<TypeMethodPair> testMethods = new List<TypeMethodPair>();
            foreach( var type in testClasses)
            {
                var methods = type.GetMethods();
                foreach( var method in methods )
                {
                    if (method.IsDefined(typeof(TestMethodAttribute), false) == true)
                    {
                        if( method.GetParameters().Length == 0 )
                            testMethods.Add(new TypeMethodPair(type, method));
                    }
                }
            }

            foreach (var pair in testMethods)
                ExecuteTest(pair.Type, pair.Method);

            Console.ReadKey(true);
        }



        private void ExecuteTest(Type type, MethodInfo method)
        {
            var testObj = Activator.CreateInstance(type);
            var testName = method.Name;
            try
            {
                method.Invoke(testObj, null);
                Console.WriteLine($"{testName} : PASSED");
            }
            catch( Exception ex )
            {
                Console.WriteLine($"{testName} : FAILED");
            }
        }

        private bool IsTestClass(Type type)
        {
            bool hasParamlessConstr = false;
            var constructors = type.GetConstructors();
            foreach( var constr in constructors )
            {
                if (constr.GetParameters().Length == 0)
                    hasParamlessConstr = true;
            }
            if (hasParamlessConstr == false)
                return false;

            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (method.IsDefined(typeof(TestMethodAttribute), false) == true)
                    return true;
            }
            return false;
        }
    }


    public static class StaticExtensions
    {
        public static T FindMatching<T>(this List<T> list, Predicate<T> condition)
        {
            foreach( var item in list)
            {
                if (condition(item) == true)
                    return item;
            }
            return default(T);
        }

    }

}
