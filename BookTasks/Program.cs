using System;
using System.Collections;

/*
namespace Example
{
    interface ITest
    {
        string Str
        {
            get;
            set;
        }
    }

    class MyClass : ITest
    {
        string myStr;
        public string Str
        {
            get
            {
                return myStr;
            }
            set
            {
                myStr = value;
            }
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Вывод программы: {Str}");
        }
    }

    class Program
    {
        static void Main()
        {
            MyClass myClass = new MyClass();
            myClass.Str = "test";
            myClass.DisplayInfo();
        }
    }
}
*/
/*
namespace Example
{
    class Program
    {
        struct CarInfo
        {
            public string Brand;
            public string Model;

            public CarInfo(string brand, string model)
            {
                this.Brand = brand;
                this.Model = model;
            }

            public void GetCarInfo()
            {
                Console.WriteLine($"Бренд: {Brand}. Модель: {Model}");
            }

            static void Main()
            {
                CarInfo carOne = new CarInfo("Audi", "A6");
                CarInfo carTwo = new CarInfo("BMW", "X5");

                Console.Write($"Car1: .");
                carOne.GetCarInfo();
                Console.Write($"Car2: .");
                carTwo.GetCarInfo();


                carOne = carTwo; // BMW, X5
                Console.Write("Car1: ");
                carOne.GetCarInfo();

                Console.Write("Car2: ");
                carTwo.Brand = "Toyota";
                carTwo.Model = "Camry";
                carTwo.GetCarInfo();

            }
        }
    }

}
*/
/*
namespace Example
{
    enum car : long {Brand, Model, Year, Engine}

    class Program
    {
        static void Main()
        {
            car car = new car();

            for (car = car.Brand; car <= car.Engine; car++)
            {
                Console.WriteLine($"Ключ: \"{car}\", значение {(int)car} ");
            }
        }
    }
}
*/
/*
namespace Program
{
    class Program
    {
        public static void Main(string[] args)
        {
            int x = 10, y = 0, z;
            try
            {
                z = x / y;
            }
            catch (Exception ex)
            {
                Console.Write("Ошибка: ");
                Console.Write(ex.Message + "\n\n");
                Console.Write("Метод: ");
                Console.Write(ex.TargetSite + "\n\n");
                Console.Write("Стек: ");
                Console.Write(ex.StackTrace + "\n\n");
                Console.Write("Подробности: ");
                Console.Write(ex.HelpLink + "\n\n");
                
                if (ex.Data != null)
                {
                    Console.Write("Подробности: ");
                    foreach (DictionaryEntry entry in ex.Data)
                    {
                        Console.WriteLine($"-> {entry.Key}, {entry.Value}");
                    }
                }
            }
        }
    }
}
*/
/*
 * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
namespace Exapmle
{
    class IntCollection
    {
        public static ArrayList NewCollection(int i)
        {
            Random random = new Random();
            ArrayList list = new ArrayList();

            for (int j = 0; j < i; j++)
            {
                list.Add(random.Next(1, 100));
            }

            return list;
        }

        public static void RemoveElement(int i, int j, ref ArrayList list)
        {
            list.RemoveRange(i, j);
        }

        public static void AddElement(int i, ref ArrayList list)
        {
            Random random = new Random();
            
            for (int j = 0; j < i; j++)
            {
                list.Add(random.Next(1, 100));
            }
        }

        public static void Write(ArrayList list)
        {
            foreach (int a in list)
            {
                Console.Write($"{a}\t");
            }
            Console.WriteLine("");
        }
    }

    class Program
    {
        public static void Main()
        {
            ArrayList list = IntCollection.NewCollection(4);
            Console.WriteLine("Моя коллекция: ");
            IntCollection.Write(list);

            IntCollection.AddElement(4, ref list);
            Console.WriteLine("После добавления элементов: ");
            IntCollection.Write(list);

            IntCollection.RemoveElement(3, 2, ref list);
            Console.WriteLine("После удаления элементов: ");
            IntCollection.Write(list);

            list.Sort();
            Console.WriteLine("После сортировки: ");
            IntCollection.Write(list);
        }
    }
}
*/
/*
 * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Hashtable ht = new Hashtable();

            ht.Add("Misha", 5732937459);
            ht.Add("Denis", 4853485743);
            ht.Add("Aboba", 5348753839);

            ICollection keys = ht.Keys;

            foreach (string key in keys)
            {
                Console.WriteLine($"{key}: {ht[key]}");
            }
        }
    }
}
*/
/*
namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var MyStack = new Stack<int>();
            MyStack.Push(10);
            MyStack.Push(20);
            MyStack.Push(30);

            Console.Write("Содержимое стека: ");

            foreach (int i in MyStack)
            {
                Console.Write($"{i} - ");
            }
            Console.WriteLine("\n");

            while (MyStack.Count > 0)
            {
                Console.WriteLine(MyStack.Pop());
            }

            if (MyStack.Count == 0)
            {
                Console.WriteLine("Стек пуст!");
            }

        }
    }
}
*/

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Queue<int> queue = new Queue<int>();
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                queue.Enqueue(random.Next(1, 100));
            }

            Console.WriteLine("Моя очередь: ");

            foreach (int i in queue)
            {
                for (int j = 0; j < 1; j++)
                {
                    Console.WriteLine($"{j}) - {i}");
                }
            }

            queue.Dequeue();
            queue.Clear();
        }
    }
}