using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HashTableSearchD
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    string filename = "HashTable.dat";

        //    HashTable hash = new HashTable(30, 0.75f, filename);
        //    using (hash.fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite))
        //    {
        //        hash.Put("aaaaaaaaaa", "0000000001");
        //        hash.Put("bbbbbbbbbb", "0000000002");
        //        hash.Put("cccccccccc", "0000000003");
        //        hash.Put("dddddddddd", "0000000004");
        //        hash.Put("eeeeeeeeee", "0000000005");
        //        hash.Put("ffffffffff", "0000000006");
        //        hash.Put("gggggggggg", "0000000007");
        //        hash.Put("hhhhhhhhhh", "0000000008");
        //        hash.Put("iiiiiiiiii", "0000000009");
        //        hash.Put("jjjjjjjjjj", "0000000010");
        //        hash.Put("kkkkkkkkkk", "0000000011");
        //        hash.Put("llllllllll", "0000000012");
        //        hash.Put("mmmmmmmmmm", "0000000013");
        //        hash.Put("nnnnnnnnnn", "0000000014");
        //        hash.Put("oooooooooo", "0000000015");
        //        hash.Put("pppppppppp", "0000000016");

        //        string[] s = hash.ToVisualizedString();

        //        foreach (string ss in s)
        //        {
        //            Console.WriteLine(ss);
        //        }

        //        Console.WriteLine();
        //        Console.WriteLine(hash.Get("aaaaaaaaaa"));
        //        Console.WriteLine(hash.Get("bbbbbbbbbb"));
        //        Console.WriteLine(hash.Get("cccccccccc"));
        //        Console.WriteLine(hash.Get("dddddddddd"));
        //        Console.WriteLine(hash.Get("eeeeeeeeee"));
        //        Console.WriteLine(hash.Get("ffffffffff"));
        //        Console.WriteLine(hash.Get("gggggggggg"));
        //        Console.WriteLine(hash.Get("hhhhhhhhhh"));
        //        Console.WriteLine(hash.Get("iiiiiiiiii"));
        //        Console.WriteLine(hash.Get("jjjjjjjjjj"));
        //        Console.WriteLine(hash.Get("kkkkkkkkkk"));
        //        Console.WriteLine(hash.Get("llllllllll"));
        //        Console.WriteLine(hash.Get("mmmmmmmmmm"));
        //        Console.WriteLine(hash.Get("nnnnnnnnnn"));
        //        Console.WriteLine(hash.Get("oooooooooo"));
        //        Console.WriteLine(hash.Get("pppppppppp"));
        //    }
        //}


        static Random random;

        static void Main(string[] args)
        {
            List<string> nameList = new List<string>();
            List<string> data = new List<string>();
            string filename = "HashTable.dat";
            int[] amount = { 1000, 2000, 3000, 4000, 5000, 7500, 10000, 15000, 20000, 25000, 30000, 40000, 50000, 75000, 100000 };
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
                    HashTable hashTable = new HashTable((int)((float)n / 0.75f), 0.8f, filename);
                    using (hashTable.fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite))
                    {
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
                    }
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
