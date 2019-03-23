using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTableSearch
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    HashTable hash = new HashTable();
        //    hash.Put("aaaa");
        //    hash.Put("bbbb");
        //    hash.Put("cccc");
        //    hash.Put("dddd");
        //    hash.Put("eeee");
        //    hash.Put("ffff");
        //    hash.Put("gggg");
        //    hash.Put("hhhh");
        //    hash.Put("iiii");
        //    hash.Put("jjjj");
        //    hash.Put("kkkk");
        //    hash.Put("llll");
        //    hash.Put("mmmm");
        //    hash.Put("nnnn");
        //    hash.Put("oooo");
        //    hash.Put("pppp");

        //    string[] s = hash.ToVisualizedString();

        //    foreach (string ss in s)
        //    {
        //        Console.WriteLine(ss);
        //    }

        //    Console.WriteLine();
        //    Console.WriteLine(hash.Get("aaaa"));
        //    Console.WriteLine(hash.Get("bbbb"));
        //    Console.WriteLine(hash.Get("cccc"));
        //    Console.WriteLine(hash.Get("dddd"));
        //    Console.WriteLine(hash.Get("eeee"));
        //    Console.WriteLine(hash.Get("ffff"));
        //    Console.WriteLine(hash.Get("gggg"));
        //    Console.WriteLine(hash.Get("hhhh"));
        //    Console.WriteLine(hash.Get("iiii"));
        //    Console.WriteLine(hash.Get("jjjj"));
        //    Console.WriteLine(hash.Get("kkkk"));
        //    Console.WriteLine(hash.Get("llll"));
        //    Console.WriteLine(hash.Get("mmmm"));
        //    Console.WriteLine(hash.Get("nnnn"));
        //    Console.WriteLine(hash.Get("oooo"));
        //    Console.WriteLine(hash.Get("pppp"));
        //}
        static Random random;
        static void Main(string[] args)
        {
            List<string> nameList = new List<string>();
            List<string> data = new List<string>();
            int[] amount = { 100000, 150000, 200000, 250000, 300000, 400000, 500000, 750000, 1000000, 1250000 };
            int repeat = 10;
            int seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
            random = new Random(seed);
            long startTime = 0;
            long endTime = 0;
            data.Add("HASH TABLE\n");
            foreach (int n in amount)
            {
                Console.WriteLine($"n = {n}");
                for (int i = 0; i < repeat; i++)
                {
                    HashTable hashTable = new HashTable((int)((float)n / 0.75f), 0.8f);
                    for (int j = 0; j < n; j++)
                    {
                        string s = RandomName(10);
                        nameList.Add(s);
                        hashTable.Put(s, s);
                    }
                    nameList.Add(RandomName(10));
                    GC.Collect();
                    startTime = DateTime.Now.Ticks;
                    foreach (var s in nameList)
                    {
                        var entry = hashTable.Get(s);
                    }
                    endTime = DateTime.Now.Ticks;
                    Console.WriteLine("Elapsed time in ticks: " + (endTime - startTime));
                    data.Add($"{n};{(float)TimeSpan.FromTicks(endTime - startTime).TotalMilliseconds / 1000.0f}");
                }
                data.Add("");
            }
            int l = 3;
            foreach (int n in amount)
            {
                data.Add($"{n};=AVERAGE(B{l}:B{l + repeat - 1})");
                l += repeat + 1;
            }
            File.WriteAllLines("HashTableTimes.csv", data);
        }
        static string RandomName(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 *
            random.NextDouble() + 65)));
            builder.Append(ch);
            for (int i = 1; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 *
                random.NextDouble() + 97)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}
