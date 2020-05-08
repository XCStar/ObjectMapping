using System;
using System.Diagnostics;
using ObjectMapping.Attributes;

namespace ObjectMapping
{
    class Program
    {
        static void Main (string[] args)
        {

            Test01 ();
            Test04 ();
            //Test02();
            Test03 ();
        }
        public static void Test01 ()
        {
            var h = new Human { ID = 1, Name = "test" };
            var mapper = new Mapper<Person, Human> ();
            Func<Human,string> func= x =>
            {

                return x.ID + x.Name;
            };
            mapper.Property (x => x.Name,func);
            var time01 = Caculate (1000000, () =>
            {
                var p = mapper.AutoMapper (h);
            });
            System.Console.WriteLine (time01.TotalMilliseconds);
        }
        public static void Test04 ()
        {
            var h = new Human { ID = 1, Name = "test" };
            var mapper = new Mapper<Person, Human> ();
            var time01 = Caculate (1000000, () =>
            {
                var p = mapper.AutoMapper (h);
            });
            System.Console.WriteLine (time01.TotalMilliseconds);
        }
        public static void Test02 ()
        {
            var h = new Human { ID = 1, Name = "test" };
            var time01 = Caculate (1000000, () =>
            {
                //var p = Mapper<Human, Person>.AutoMapper(h,t=>{

                //System.Console.WriteLine(t.ID);
                //});
            });
            System.Console.WriteLine (time01.TotalMilliseconds);
        }
        public static void Test03 ()
        {
            var time01 = Caculate (1000000, () =>
            {
                var p = new Person { ID = 1, Name = "test" };
            });
            System.Console.WriteLine (time01.TotalMilliseconds);
        }
        public static TimeSpan Caculate (int executeTimes, Action action)
        {
            var sw = new Stopwatch ();
            sw.Start ();
            for (int i = 0; i < executeTimes; i++)
            {
                action ();
            }
            sw.Stop ();
            return sw.Elapsed;
        }

    }
    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class Human
    {

        public int ID { get; set; }

        public string Name { get; set; }
    }
}