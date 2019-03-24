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

        static HashTable GlobalTable = null;

        static void TextUserInterface()
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
                        Generate();
                        break;
                    case '2':
                        Print();
                        break;
                    case '3':
                        ChainCount();
                        break;
                    case '4':
                        InSameChain();
                        break;
                    case '0':
                        isWorking = false;
                        break;
                }

            }
        }

        static void PrintActionList()
        {
            Console.WriteLine("\n1 - Sugeneruoti maišos lentelę;  2 - Išvesti maišos lentelę;\n3 - Išvesti grandinėlių skaičių;  4 - Patikrinti ar du įrašai yra vienoje grandinėlėje;\n0 - Baigti darbą");
        }

        static void Generate()
        {
            Console.WriteLine("\nĮveskie elementų kiekį:");
            int n = ReadPositiveInteger();
            GlobalTable = new HashTable((int)((float)n / 0.75f), 0.8f);
            for (int j = 0; j < n; j++)
            {
                string s = RandomName(10);
                GlobalTable.Put(s, s);
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

        static bool CheckIfNull()
        {
            if (GlobalTable == null)
            {
                Console.WriteLine("\nMaišos lentelė turi būti sugeneruota!");
                return true;
            }
            return false;
        }

        static void Print()
        {
            if (CheckIfNull())
                return;
            Console.WriteLine();
            GlobalTable.Print();
        }

        static void ChainCount()
        {
            if (CheckIfNull())
                return;
            int count = GlobalTable.GetNumberOfChains();
            Console.WriteLine($"\nMaišos lentelėje iš viso yra {count} grandinėlių.");
        }

        static void InSameChain()
        {
            if (CheckIfNull())
                return;
            Console.WriteLine("\nĮveskite pirmąjį raktą:");
            string key1 = ReadString();
            Console.WriteLine("\nĮveskite antrąjį raktą:");
            string key2 = ReadString();
            int hash1, hash2;
            GlobalTable.IsInSameChain(key1, key2, out hash1, out hash2);
            if (GlobalTable.IsInSameChain(key1, key2, out hash1, out hash2))
               Console.WriteLine($"Raktai yra toje pačioje grandinėlėje, nes hash1 = {hash1}, o hash2 = {hash2}.");
            else
               Console.WriteLine($"Raktai nėra toje pačioje grandinėlėje, nes hash1 = {hash1}, o hash2 = {hash2}.");
        }

        static string ReadString()
        {
            string line;
            while (true)
            {
                line = Console.ReadLine();
                if (line.Length != 10)
                    Console.WriteLine("\nRaktas sudarytas iš 10 simbolių.");
                else
                    return line;
            }
        }

        static Random random;
        static void Main(string[] args)
        {
            int seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
            random = new Random(seed);
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            TextUserInterface();
            return;

            List<string> nameList = new List<string>();
            List<string> data = new List<string>();
            int[] amount = { 100000, 150000, 200000, 250000, 300000, 400000, 500000, 750000, 1000000, 1250000 };
            int repeat = 10;
            
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
