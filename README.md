# ObjectMapping
对象映射程序，简化版AutoMapper
等待后续改进
Test:
'''
           var person=new Person{ID=234,Name="abcd"};
            var mapper01=new Mapper<Person,Human>();
            System.Console.WriteLine(mapper01.AutoMapper(person)); 
            var mapper02=new Mapper<Person,Human>().Property(x=>x.ID,x=>x.ID+10086);
            System.Console.WriteLine(mapper02.AutoMapper(person)); 
            var mapper03=new Mapper<Person,Human>().Property(x=>x.ID,()=>int.MaxValue);
            System.Console.WriteLine(mapper03.AutoMapper(person));
'''
result:
Person => new Human() {ID = Person.ID, Name = Person.Name}
ID:234 Name:abcd
Person => new Human() {Name = Person.Name, ID = Invoke(x => (x.ID + 10086), Person)}
ID:10320 Name:abcd
Person => new Human() {Name = Person.Name, ID = Invoke(() => 2147483647)}
ID:2147483647 Name:abcd
