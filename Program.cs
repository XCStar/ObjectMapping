using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using ObjectMapping.Attributes;

namespace ObjectMapping
{
    class Program
    {
        static void Main (string[] args)
        {
            Test06 ();
            Test01 ();
            Test04 ();
            //Test02();
            Test03 ();
            //Test05 ();
        }
        public static void Test01 ()
        {
            var h = new Human { ID = 1, Name = "test" };
            var mapper = new Mapper<Human, Person> ();
            mapper.Property (x => x.Name,x => x.ID + x.Name);

            var time01 = Caculate (1000000, () =>
            {
                var p = mapper.AutoMapper (h);
            });
            System.Console.WriteLine (time01.TotalMilliseconds);
        }
        public static void Test04 ()
        {
            var h = new Human { ID = 1, Name = "test" };
            var mapper = new Mapper<Human, Person> ();
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
        public static void Test05 ()
        {
            var person = new Person { ID = 234, Name = "abcd" };
            // var mapper01 = new Mapper<Person, Human> ();
            // System.Console.WriteLine (mapper01.AutoMapper (person));
            // var mapper02 = new Mapper<Person, Human> ().Property (x => x.ID, x => x.ID + 10086);
            // System.Console.WriteLine (mapper02.AutoMapper (person));
            // var mapper03 = new Mapper<Person, Human> ().Property (x => x.ID, () => int.MaxValue);
            // System.Console.WriteLine (mapper03.AutoMapper (person));
            // var mapper04 = new Mapper<Person, Human> ().Property (x => x.Name, x =>x.Name=="abcd"?"test":"vin");
            // System.Console.WriteLine (mapper04.AutoMapper (person));
            var mapper05 = new Mapper<Person, Human> ().PropertyMap (x => x.Name).Map(p=>
            {
                switch (p.Name)
                {
                    case "abcd":return "vipabc";
                    case "test": return "haha";
                    default:
                    return "are you ok";
                }
            });
            System.Console.WriteLine (mapper05.AutoMapper (person));

        }
       public static void Test06 ()
        {
            var h = new Human { ID = 1, Name = "test" };
            var mapper = new Mapper<Human, Person> ();
            mapper.PropertyMap(x=>x.Name).Map(x=>{
                return x.ID + x.Name;
            });

            var time01 = Caculate (1000000, () =>
            {
                var p = mapper.AutoMapper (h);
            });
            System.Console.WriteLine (time01.TotalMilliseconds);
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
        public override string ToString ()
        {
            return $"ID:{ID} Name:{Name}";
        }
    }
}