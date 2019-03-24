using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingOP
{
    class Program
    {

        static void Main(string[] args)
        {
            int seed = (int)DateTime.Now.Ticks & 0x0000FFFF;

            // Pirmas etapas
            Test_Array_List(seed);
        }

        public static void MergeSort(DataArray input)
        {
            MergeSort(input, 0, input.Length - 1);
        }

        private static void MergeSort(DataArray input, int low, int high)
        {
            if (low < high)
            {
                int middle = (low / 2) + (high / 2);
                MergeSort(input, low, middle);
                MergeSort(input, middle + 1, high);
                Merge(input, low, middle, high);
            }
        } 

        private static void Merge(DataArray input, int low, int middle, int high)
        {
            int left = low;
            int right = middle + 1;
            double[] tmp = new double[(high - low) + 1];
            int tmpIndex = 0;
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
            for (int i = 0; i < tmp.Length; i++)
            {
                input[low + i] = tmp[i];
            }
        }

        public static void BucketSort(DataArray input)
        {
            MyDataList[] buckets = new MyDataList[input.Length];
            for (int i = 0; i < buckets.Length; i++)
                buckets[i] = new MyDataList();
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
        }

        public static ulong BucketSortOperations(ulong n)
        {
            ulong tAdd = 4;
            ulong tHead = 3;
            ulong tNext = 3;
            ulong tSortedInsert = 9;
            ulong tInsertion = 7 + tSortedInsert;
            ulong count = 1 + n+1 + n + n+1 + n*tAdd + n+1 + n*tInsertion + 1 + n+1 + n + n + n*tHead + n + n*tNext;
            return count;
        }

        private static ulong MergeOps;

        public static ulong MergeSortOperations(ulong n)
        {
            MergeOps = 0;
            MergeSortOperations(0, n);
            return MergeOps;
        }

        private static void MergeSortOperations(ulong low, ulong high)
        {
            if (low < high)
            {
                ulong middle = (low / 2) + (high / 2);
                MergeOps += 1;
                MergeSortOperations(low, middle);
                MergeSortOperations(middle + 1, high);
                MergeOps += 1 + 1 + 1 + 1 + high - low + 1 + 6 * (high - low) + 1 + middle - low + 1 + 3 * (middle - low) + 1 + high - middle + 1 + 3 * (high - middle) + high - low + 1 + high - low;
            }
        }

        public static void Test_Array_List(int seed)
        {
            bool merge = false;
            bool bucket = false;
            Console.WriteLine("Press M to MergeSort\nPress B to BucketSort\nPress Q to quit\n");
            bool input = true;
            while(input)
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

            List<string> dataTime = new List<string>();
            List<string> dataOps = new List<string>();
            int[] amount = { 100000, 150000, 200000, 250000, 300000, 400000, 500000, 750000, 1000000, 1250000, 1500000, 2000000, 3000000, 4000000, 5000000, 6000000, 7000000, 8000000, 9000000, 10000000 };
            int repeat = 10;
            bool print = false;
            long startTime = 0;
            long endTime = 0;
            // int n = 100000;
            dataTime.Add("ARRAY\n");
            dataOps.Add("ARRAY\n");
            Console.WriteLine("\n ARRAY \n");
            foreach (int n in amount)
            {
                Console.WriteLine("n = " + n);
                for (int i = 0; i < repeat; i++)
                {
                    MyDataArray myarray = new MyDataArray(n, seed);
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
                    Console.WriteLine("Elapsed time in ticks: " + (endTime - startTime));
                    dataTime.Add($"{n};{(float)TimeSpan.FromTicks(endTime - startTime).TotalMilliseconds / 1000.0f}");
                }
                if (merge)
                {
                    ulong operations = MergeSortOperations((ulong)n);
                    dataOps.Add($"{n};{operations}");
                }
                if (bucket)
                {
                    ulong operations = BucketSortOperations((ulong)n);
                    dataOps.Add($"{n};{operations}");
                }
                dataTime.Add("");
            }

            int line = 2 + amount.Length + (repeat * amount.Length);
            int l = 3;
            foreach (int n in amount)
            {
                dataTime.Add($"{n};=AVERAGE(B{l}:B{l + repeat - 1})");
                l += repeat + 1;
            }

            dataTime.Add("\n\n\nLIST\n");
            dataOps.Add("\n\n\nLIST\n");
            Console.WriteLine("\n LIST \n");
            foreach (int n in amount)
            {
                Console.WriteLine("n = " + n);
                for (int i = 0; i < repeat; i++)
                {
                    MyDataList mylist = new MyDataList(n, seed);
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
                    Console.WriteLine("Elapsed time in ticks: " + (endTime - startTime));
                    dataTime.Add($"{n};{(float)TimeSpan.FromTicks(endTime - startTime).TotalMilliseconds / 1000.0f}");
                }
                if (merge)
                {

                }
                if (bucket)
                {
                    ulong operations = BucketSortOperations((ulong)n);
                    dataOps.Add($"{n};{operations}");
                }
                dataTime.Add("");
            }
            l += 5 + amount.Length;
            foreach (int n in amount)
            {
                dataTime.Add($"{n};=AVERAGE(B{l}:B{l + repeat - 1})");
                l += repeat + 1;
            }
            if (merge)
            {
                File.WriteAllLines("Merge_Times.csv", dataTime);
                File.WriteAllLines("Merge_Operations.csv", dataOps);
            }
            if (bucket)
            {
                File.WriteAllLines("Bucket_Times.csv", dataTime);
                File.WriteAllLines("Bucket_Operations.csv", dataOps);
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
