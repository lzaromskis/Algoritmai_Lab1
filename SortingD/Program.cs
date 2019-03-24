using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace SortingD
{
    class Program
    {
        static readonly string tmpFile = "tmp.dat";

        static void Main(string[] args)
        {
            int seed = (int)DateTime.Now.Ticks & 0x0000FFFF;

            // Antras etapas
            if (!Directory.Exists("tmp"))
                Directory.CreateDirectory("tmp");
            //Test_Array_List(seed);
            TextUserInterface(seed);
        }

        static MyFileArray GlobalArray = null;
        static MyFileList GlobalList = null;

        static void TextUserInterface(int seed)
        {
            bool isWorking = true;
            char key;
            while (isWorking)
            {
                PrintActionList();
                key = Console.ReadKey().KeyChar;
                switch (key)
                {
                    case '1':
                        Generate(seed);
                        break;
                    case '2':
                        Print();
                        break;
                    case '3':
                        Sort();
                        break;
                    case '4':
                        GetInfoAboutElement();
                        break;
                    case '0':
                        isWorking = false;
                        break;
                }

            }

        }

        static void PrintActionList()
        {
            Console.Write("\n1 - Sugeneruoti naują;  2 - Išvesti;\n3 - Rikiuoti;  4 - Surasti elementą;\n0 - Baigti darbą;\n");
        }

        static char GetAction(bool notNull)
        {
            char key;
            while (true)
            {
                Console.WriteLine("\n1 - Masyvas;  2 - Sąrašas;  3 - Abu;  0 - Atšaukti");
                key = Console.ReadKey().KeyChar;
                if (key == '1' || key == '2' || key == '3' || key == '0')
                {
                    if (notNull)
                    {
                        if (GlobalArray == null && (key == '1' || key == '3'))
                            Console.WriteLine("\nMasyvas turi būti sugeneruotas!");
                        else if (GlobalList == null && (key == '2' || key == '3'))
                            Console.WriteLine("\nSąrašas turi būti sigeneruotas");
                        else
                            return key;
                    }
                    else
                        return key;
                }
            }
        }

        static void Generate(int seed)
        {
            char key = GetAction(false);
            if (key == '0')
                return;
            Console.WriteLine("\nĮveskite dydį:");
            int size = ReadPositiveInteger();
            if (key == '1' || key == '3')
            {
                if (GlobalArray != null)
                {
                    GlobalArray.fs.Dispose();
                }
                GlobalArray = new MyFileArray("array.dat", size, seed);
                GlobalArray.fs = new FileStream("array.dat", FileMode.Open, FileAccess.ReadWrite);
            }
            if (key == '2' || key == '3')
            {
                if (GlobalList != null)
                {
                    GlobalList.fs.Dispose();
                }
                GlobalList = new MyFileList("list.dat", size, seed);
                GlobalList.fs = new FileStream("list.dat", FileMode.Open, FileAccess.ReadWrite);
            }
        }

        static void Print()
        {
            char key = GetAction(true);
            if (key == '0')
                return;
            char print;
            while (true)
            {
                Console.WriteLine("\n1 - Visą;  2 - Dalį;");
                print = Console.ReadKey().KeyChar;
                if (print == '1' || print == '2')
                    break;
            }
            if (print == '1')
            {
                Console.WriteLine();
                if (key == '1' || key == '3')
                    GlobalArray.PrintAll();
                if (key == '2' || key == '3')
                    GlobalList.PrintAll();
            }
            if (print == '2')
            {
                Console.WriteLine("\nĮveskite kairinę ribą (įskaitant):");
                int left = ReadPositiveInteger();
                Console.WriteLine("\nĮveskite dešininę ribą (neįskaitant):");
                int right = ReadPositiveInteger();
                Console.WriteLine();
                if (key == '1' || key == '3')
                    GlobalArray.PrintRange(left, right);
                if (key == '2' || key == '3')
                    GlobalList.PrintRange(left, right);
            }
        }

        static int ReadPositiveInteger()
        {
            while (true)
            {
                string line = Console.ReadLine();
                int size;
                if (int.TryParse(line, out size))
                {
                    if (size > 0)
                    {
                        return size;
                    }
                }
                Console.WriteLine("\nĮveskite teigiamą sveikąjį skaičių.");
            }
        }

        static void Sort()
        {
            char key = GetAction(true);
            if (key == '0')
                return;
            char sort;
            while (true)
            {
                Console.WriteLine("\n1 - MergeSort;  2 - BucketSort;");
                sort = Console.ReadKey().KeyChar;
                if (sort == '1' || sort == '2')
                    break;
            }
            if (sort == '1')
            {
                if (key == '1' || key == '3')
                    MergeSort(GlobalArray);
                if (key == '2' || key == '3')
                    GlobalList.MergeSort();
            }
            if (sort == '2')
            {
                if (key == '1' || key == '3')
                    BucketSort(GlobalArray);
                if (key == '2' || key == '3')
                    GlobalList.BucketSort();
            }
        }

        static void GetInfoAboutElement()
        {
            char key = GetAction(true);
            if (key == '0')
                return;
            double element;
            Console.WriteLine("\nĮveskitę elementą:");
            while (true)
            {
                string line = Console.ReadLine();
                if (double.TryParse(line, out element))
                {
                    break;
                }
                Console.WriteLine("\nĮveskitę tinkąmą realujį skaičių.");
            }
            if (key == '1' || key == '3')
                GlobalArray.PrintElement(element);
            if (key == '2' || key == '3')
                GlobalList.PrintElement(element);
        }

        public static void MergeSort(MyFileArray input)
        {
            MergeSort(input, 0, input.Length - 1);
        }

        private static void MergeSort(MyFileArray input, int low, int high)
        {
            if (low < high)
            {
                int middle = (low / 2) + (high / 2);
                MergeSort(input, low, middle);
                MergeSort(input, middle + 1, high);
                Merge(input, low, middle, high);
            }
        }

        private static void Merge(MyFileArray input, int low, int middle, int high)
        {
            int left = low;
            int right = middle + 1;
            MyFileArray tmp = new MyFileArray(tmpFile);
            int tmpIndex = 0;
            using (tmp.fs = new FileStream(tmpFile, FileMode.Open, FileAccess.ReadWrite))
            {
                while ((left <= middle) && (right <= high))
                {
                    if (input[left] < input[right])
                    {
                        tmp[tmpIndex] = input[left];
                        left = left + 1;
                    }
                    else
                    {
                        tmp[tmpIndex] = input[right];
                        right = right + 1;
                    }
                    tmpIndex = tmpIndex + 1;
                }
                if (left <= middle)
                {
                    while (left <= middle)
                    {
                        tmp[tmpIndex] = input[left];
                        left = left + 1;
                        tmpIndex = tmpIndex + 1;
                    }
                }
                if (right <= high)
                {
                    while (right <= high)
                    {
                        tmp[tmpIndex] = input[right];
                        right = right + 1;
                        tmpIndex = tmpIndex + 1;
                    }
                }
                for (int i = 0; i < tmpIndex; i++)
                {
                    input[low + i] = tmp[i];
                }
            }
        }
        
        public static void BucketSort(MyFileArray input)
        {
            MyFileList[] buckets = new MyFileList[input.Length];
            for (int i = 0; i < buckets.Length; i++)
            {
                string file = "tmp/tmp" + i + ".dat";
                if (File.Exists(file))
                    File.Delete(file);
                buckets[i] = new MyFileList(file);
            }
            for (int i = 0; i < input.Length; i++)
            {
                int bi = (int)((double)input.Length * input[i]);
                buckets[bi].Add(input[i]);
            }
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i].InsertionSort();
            }
            int index = 0;
            for (int i = 0; i < input.Length; i++)
            {
                var bucket = buckets[i];
                if (bucket.Length == 0)
                    continue;
                input[index++] = bucket.Head();
                for (int j = 1; j < bucket.Length; j++)
                    input[index++] = bucket.Next();
            }
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i].Close();
            }
        }

        public static void Test_File_Array_List(int seed)
        {
            if (!Directory.Exists("tmp"))
                Directory.CreateDirectory("tmp");
            bool print = false;
            long startTime, endTime;
            int n = 120;
            string filename;
            filename = @"mydataarray.dat";
            MyFileArray myfilearray = new MyFileArray(filename, n, seed);
            using (myfilearray.fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite))
            {
                Console.WriteLine("\n FILE ARRAY \n"); 
                if (print)
                    myfilearray.Print(n);

                startTime = DateTime.Now.Ticks;
                BucketSort(myfilearray);
                endTime = DateTime.Now.Ticks;

                if (print)
                  myfilearray.Print(n);      

                Console.WriteLine("Is sorted: " + myfilearray.IsSorted());
                Console.WriteLine("Elapsed time in ticks: " + (endTime - startTime));
            }
            filename = @"mydatalist.dat";
            MyFileList myfilelist = new MyFileList(filename, n, seed);
            using (myfilelist.fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite))
            {
                Console.WriteLine("\n FILE LIST \n");
                if (print)
                    myfilelist.Print(n);

                startTime = DateTime.Now.Ticks;
                myfilelist.BucketSort();
                endTime = DateTime.Now.Ticks;

                if (print)
                   myfilelist.Print(n);

                Console.WriteLine("Is sorted: " + myfilelist.IsSorted());
                Console.WriteLine("Elapsed time in ticks: " + (endTime - startTime));
            }
        }

        public static void Test_Array_List(int seed)
        {
            bool merge = false;
            bool bucket = false;
            Console.WriteLine("Press M to MergeSort\nPress B to BucketSort\nPress Q to quit\n");
            bool input = true;
            while (input)
            {
                var key = Console.ReadKey();
                switch (char.ToUpper(key.KeyChar))
                {
                    case 'M':
                        merge = true;
                        input = false;
                        Console.WriteLine("\n\nPerforming MergeSort...\n");
                        break;
                    case 'B':
                        bucket = true;
                        input = false;
                        Console.WriteLine("\n\nPerforming BucketSort...\n");
                        break;
                    case 'Q':
                        return;
                }
            }

            System.Threading.Thread.Sleep(1000);

            if (merge && bucket)
            {
                Console.WriteLine("Cannot perform merge sort and bucket sort at the same time");
                return;
            }

            List<string> data = new List<string>();
            int[] amount = { 100, 250, 500, 1000, 1500, 2000, 2500, 3000};
            int repeat = 10;
            bool print = false;
            long startTime = 0;
            long endTime = 0;
            // int n = 100000;
            data.Add("ARRAY\n");
            Console.WriteLine("\n ARRAY \n");
            foreach (int n in amount)
            {
                Console.WriteLine("n = " + n);
                for (int i = 0; i < repeat; i++)
                {
                    MyFileArray myarray = new MyFileArray("mydataarray.dat", n, seed);
                    using (myarray.fs = new FileStream("mydataarray.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        if (print)
                            myarray.Print(n);
                        GC.Collect();
                        if (merge)
                        {
                            startTime = DateTime.Now.Ticks;
                            MergeSort(myarray);
                            endTime = DateTime.Now.Ticks;
                        }
                        if (bucket)
                        {
                            startTime = DateTime.Now.Ticks;
                            BucketSort(myarray);
                            endTime = DateTime.Now.Ticks;
                        }
                        if (print)
                            myarray.Print(n);
                        Console.WriteLine("Is sorted: " + myarray.IsSorted());
                    }
                    Console.WriteLine("Elapsed time in ticks: " + (endTime - startTime));

                    data.Add($"{n};{(float)TimeSpan.FromTicks(endTime - startTime).TotalMilliseconds / 1000.0f}");
                }
                data.Add("");
            }

            int line = 2 + amount.Length + (repeat * amount.Length);
            int l = 3;
            foreach (int n in amount)
            {
                data.Add($"{n};=AVERAGE(B{l}:B{l + repeat - 1})");
                l += repeat + 1;
            }

            data.Add("\n\n\nLIST\n");
            Console.WriteLine("\n LIST \n");
            foreach (int n in amount)
            {
                Console.WriteLine("n = " + n);
                for (int i = 0; i < repeat; i++)
                {
                    MyFileList mylist = new MyFileList("mydatalist.dat", n, seed);
                    using (mylist.fs = new FileStream("mydatalist.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        if (print)
                            mylist.Print(n);
                        GC.Collect();
                        if (merge)
                        {
                            startTime = DateTime.Now.Ticks;
                            mylist.MergeSort();
                            endTime = DateTime.Now.Ticks;
                        }
                        if (bucket)
                        {
                            startTime = DateTime.Now.Ticks;
                            mylist.BucketSort();
                            endTime = DateTime.Now.Ticks;
                        }
                        if (print)
                            mylist.Print(n);
                        Console.WriteLine("Is sorted: " + mylist.IsSorted());
                    }
                    Console.WriteLine("Elapsed time in ticks: " + (endTime - startTime));
                    data.Add($"{n};{(float)TimeSpan.FromTicks(endTime - startTime).TotalMilliseconds / 1000.0f}");
                }
                data.Add("");
            }
            l += 5 + amount.Length;
            foreach (int n in amount)
            {
                data.Add($"{n};=AVERAGE(B{l}:B{l + repeat - 1})");
                l += repeat + 1;
            }
            if (merge)
            {
                File.WriteAllLines("Merge_Times.csv", data);
            }
            if (bucket)
            {
                File.WriteAllLines("Bucket_Times.csv", data);
            }
        }
    }
    abstract class DataArray
    {
        protected int length;
        public int Length { get { return length; } }
        public abstract double this[int index] { get; set; }
        public abstract void Swap(int j, double a, double b);
        public void Print(int n)
        {
            for (int i = 0; i < n; i++)
                Console.Write(" {0:F5} ", this[i]);
            Console.WriteLine();
        }
    }
    abstract class DataList
    {
        protected int length;
        public int Length { get { return length; } }
        public abstract double Head();
        public abstract double Next();
        public abstract void Swap(double a, double b);
        public void Print(int n)
        {
            Console.Write(" {0:F5} ", Head());
            for (int i = 1; i < n; i++)
                Console.Write(" {0:F5} ", Next());
            Console.WriteLine();

        }

    }
}