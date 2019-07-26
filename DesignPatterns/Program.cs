using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace DesignPatterns
{
    public class Program
    {
        static void Main(string[] args)
        {
            var numbers = GetNumbers();
            Console.WriteLine("Called GetNumber()");
            foreach( var item in numbers)
            {
                Console.WriteLine(item);
            }
        }

        public static IEnumerable<int> GetNumbers()
        {
            var randomValue = Guid.NewGuid().ToString();
            Console.WriteLine($"[{randomValue}] Returning 1");
            yield return 1;
            Console.WriteLine($"[{randomValue}] Returning 2");
            yield return 2;
            Console.WriteLine($"[{randomValue}] Returning 3");
            yield return 3;
            Console.WriteLine($"[{randomValue}] Returning 4");
            yield return 4;
            Console.WriteLine($"[{randomValue}] Returning 5");
            yield return 5;
        }

        static void DecoratorSample()
        {
            //var script = new Script() { UserDb = new SimpleUserDb(), Cache = new UserCache() };
            var script = new ScriptWithPatterns()
            {
                //UserDb = new CachedUserDb( new UserCache(), new SimpleUserDb())
                UserDb = new SimpleUserDb().WithCaching()
            };
            script.Run();
            script.Run();
            Console.ReadKey(true);

            List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
            foreach( var item in numbers )
            {
                Console.WriteLine(item);
            }

            var iter = numbers.GetEnumerator();
            while( iter.MoveNext() == true )
            {
                Console.WriteLine(iter.Current);
            }
        }

        public static void IteratorExample()
        {
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
            foreach (var item in numbers)
            {
                Console.WriteLine(item);
            }
        }
    }

    public class MyCollection
    {
        public IIterator<int> GetIterator()
        {
            return null;
        }
    }

    public interface IIterator<T>
    {
        bool MoveNext();

        T Current { get; }
    }

    public class SqlUserDb : IUserDb
    {
        public User Get(int id)
        {
            throw new NotImplementedException();
        }
    }

    public class UserCache
    {
        private static Dictionary<int, User> _cache = new Dictionary<int, User>();

        public bool TryGetUser(int id, out User user)
        {
            return _cache.TryGetValue(id, out user);
        }

        public void AddUser(User user)
        {
            _cache[user.Id] = user;
        }
    }

    public class Script
    {
        public IUserDb UserDb { get; set; }

        public UserCache Cache { get; set; }
        public void Run()
        {
            User user = null;
            var elapsedMs = Measure.TimingFor(() =>
               {
                   var isFound = Cache.TryGetUser(10, out user);
                   if (isFound == false)
                   {
                       user = UserDb.Get(10);
                       Cache.AddUser(user);
                   }
               });
            Console.WriteLine($"Fetched user: {user.Id} in {elapsedMs} ms.");
        }
    }

    public class ScriptWithPatterns
    {
        public IUserDb UserDb { get; set; }

        public void Run()
        {
            User user = null;
            var elapsedMs = Measure.TimingFor(() =>
            {
                user = UserDb.Get(10);
            });
            Console.WriteLine($"Fetched user: {user.Id} in {elapsedMs} ms.");
        }
    }

    public static class Measure
    {
        public static long TimingFor(Action action)
        {
            var timer = Stopwatch.StartNew();
            action();
            return (long)(timer.ElapsedTicks) / Stopwatch.Frequency * 1000;

        }
    }


    public static class UserDbExtensions
    {
        public static IUserDb WithCaching(this IUserDb db)
        {
            return new CachedUserDb(new UserCache(), db);
        }
    }

    public class CachedUserDb : IUserDb
    {
        public CachedUserDb(UserCache cache, IUserDb innerDb)
        {
            Cache = cache;
            InnerDb = innerDb;
        }

        public UserCache Cache { get; }

        public IUserDb InnerDb { get; }

        public User Get(int id)
        {
            var isFound = Cache.TryGetUser(10, out User user);
            if (isFound == false)
            {
                user = InnerDb.Get(10);
                Cache.AddUser(user);
            }
            return user;
        }
    }


    public interface IUserDb
    {
        User Get(int id);
    }

    public class SimpleUserDb : IUserDb
    {
        public User Get(int id)
        {
            Thread.Sleep(2000);
            return new User(id);
        }
    }


    public class User
    {
        public User(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }


}
