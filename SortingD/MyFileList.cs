using System;
using System.IO;
namespace SortingD
{
    class MyFileList : DataList
    {
        private static readonly byte[] NULLBYTE = { (byte)0 , (byte)0 , (byte)0 , (byte)0 };

        private static readonly string tmpFile = "tmpList.dat";

        private int nextNodeLocation;

        int prevNode;
        int currentNode;
        int nextNode;

        public MyFileList(string filename)
        {
            length = 0;
            nextNodeLocation = 4;
            if (File.Exists(filename))
                File.Delete(filename);
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    writer.Write(NULLBYTE);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            fs = new FileStream(filename, FileMode.Create);
        }

        public MyFileList(string filename, int n, int seed)
        {
            length = n;
            Random rand = new Random(seed);
            if (File.Exists(filename)) File.Delete(filename);
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    writer.Write(4);
                    for (int j = 0; j < length; j++)
                    {
                        writer.Write(rand.NextDouble());
                        if (j != length - 1)
                            writer.Write((j + 1) * 12 + 4);
                        else
                            writer.Write(0);
                    }
                                      
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public FileStream fs { get; set; }
        public override double Head()
        {
            Byte[] data = new Byte[12];
            fs.Seek(0, SeekOrigin.Begin);
            fs.Read(data, 0, 4);
            currentNode = BitConverter.ToInt32(data, 0);
            prevNode = -1;
            fs.Seek(currentNode, SeekOrigin.Begin);
            fs.Read(data, 0, 12);
            double result = BitConverter.ToDouble(data, 0);
            nextNode = BitConverter.ToInt32(data, 8);
            return result;
        }
        public override double Next()
        {
            if (currentNode == 0)
                return double.NaN;

            Byte[] data = new Byte[12];
            fs.Seek(nextNode, SeekOrigin.Begin);
            fs.Read(data, 0, 12);
            prevNode = currentNode;
            currentNode = nextNode;
            double result = BitConverter.ToDouble(data, 0);
            nextNode = BitConverter.ToInt32(data, 8);
            return result;

        }
        public override void Swap(double a, double b)

        {
            Byte[] data;
            fs.Seek(prevNode, SeekOrigin.Begin);
            data = BitConverter.GetBytes(a);
            fs.Write(data, 0, 8);
            fs.Seek(currentNode, SeekOrigin.Begin);
            data = BitConverter.GetBytes(b);
            fs.Write(data, 0, 8);

        }

        public void Add(double value)
        {
            if (ReadHead() == 0)
            {
                WriteHead(nextNodeLocation);
            }
            else
            {
                WriteNextNode(nextNodeLocation - 12, nextNodeLocation);
            }

            WriteValue(nextNodeLocation, value);
            WriteNextNode(nextNodeLocation, 0);
            nextNodeLocation += 12;
            length++;
        }

        public void MergeSort()
        {
            fs.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[4];
            fs.Read(data, 0, 4);
            data = BitConverter.GetBytes(MergeSort(BitConverter.ToInt32(data, 0)));
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(data, 0, 4);
        }

        private int MergeSort(int head)
        {
            if (head == 0)
            {
                return head;
            }
            if (ReadNextNode(head) == 0)
            {
                return head;
            }
            int middle = GetMiddle(head);
            int leftHead = head;
            int rightHead = ReadNextNode(middle);
            fs.Seek(middle + 8, SeekOrigin.Begin);
            fs.Write(NULLBYTE, 0, 4);
            return Merge(MergeSort(leftHead), MergeSort(rightHead));
        }

        private int Merge(int a, int b)
        {
            int dummy;
            int current;
            if (ReadValue(a) <= ReadValue(b))
            {
                dummy = a;
                current = a;
                a = ReadNextNode(a);
            }
            else
            {
                dummy = b;
                current = b;
                b = ReadNextNode(b);
            }
            for (; a != 0 && b != 0; current = ReadNextNode(current))
            {
                if (ReadValue(a) <= ReadValue(b))
                {
                    WriteNextNode(current, a);
                    a = ReadNextNode(a);
                }
                else
                {
                    WriteNextNode(current, b);
                    b = ReadNextNode(b);
                }             
            }
            if (a == 0)
            {
                WriteNextNode(current, b);
            }
            else
            {
                WriteNextNode(current, a);
            }
            return dummy;
        }

        private int GetMiddle(int head)
        {
            if (head == 0)
                return head;
            int slow = head;
            int fast = head;
            byte[] data = new byte[4];
            fs.Seek(0, SeekOrigin.Begin);
            fs.Read(data, 0, 4);
            int next = ReadNextNode(fast);
            int nextnext = ReadNextNode(next);
            while (next != 0 && nextnext != 0)
            {
                slow = ReadNextNode(slow);
                fast = nextnext;

                next = ReadNextNode(fast);
                nextnext = ReadNextNode(next);
            }
            return slow;
        }

        public void InsertionSort()
        {
            InsertionSort(ReadHead());
        }

        private void InsertionSort(int node)
        {
            int sorted = 0;
            int current = node;
            while (current != 0)
            {
                int next = ReadNextNode(current);
                SortedInsert(ref current, ref sorted);
                current = next;
            }
            WriteHead(sorted);
        }

        private void SortedInsert(ref int newNode, ref int sorted)
        {
            if (sorted == 0 || ReadValue(sorted) >= ReadValue(newNode))
            {
                WriteNextNode(newNode, sorted);
                sorted = newNode;
            }
            else
            {
                int current = sorted;

                while (ReadNextNode(current) != 0 && ReadValue(ReadNextNode(current)) < ReadValue(newNode))
                {
                    current = ReadNextNode(current);
                }
                WriteNextNode(newNode, ReadNextNode(current));
                WriteNextNode(current, newNode);
            }
        }

        public void Close()
        {
            fs.Dispose();
        }

        public void BucketSort()
        {
            MyFileList[] buckets = new MyFileList[length];
            for (int i = 0; i < buckets.Length; i++)
            {
                string file = "tmp/tmp" + i + ".dat";
                if (File.Exists(file))
                    File.Delete(file);
                buckets[i] = new MyFileList(file);
            }

            int node = ReadHead();

            while (node != 0)
            {
                int bi = (int)((double)Length * ReadValue(node));
                buckets[bi].Add(ReadValue(node));
                node = ReadNextNode(node);
            }

            foreach (var bucket in buckets)
            {
                bucket.InsertionSort();
            }

            node = ReadHead();
            for (int i = 0; i < Length; i++)
            {
                var bucket = buckets[i];
                if (bucket.Length == 0)
                    continue;
                WriteValue(node, bucket.Head());
                node = ReadNextNode(node);
                for (int j = 1; j < bucket.Length; j++)
                {
                    WriteValue(node, bucket.Next());
                    node = ReadNextNode(node);
                }
            }

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i].Close();
            }
        }

        private int ReadHead()
        {
            fs.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[4];
            fs.Read(data, 0, 4);
            return BitConverter.ToInt32(data, 0);
        }

        private void WriteHead(int address)
        {
            fs.Seek(0, SeekOrigin.Begin);
            byte[] data = BitConverter.GetBytes(address);
            fs.Write(data, 0, 4);
        }

        private int ReadNextNode(int node)
        {
            fs.Seek(node + 8, SeekOrigin.Begin);
            byte[] data = new byte[4];
            fs.Read(data, 0, 4);
            return BitConverter.ToInt32(data, 0);
        }

        private double ReadValue(int node)
        {
            fs.Seek(node, SeekOrigin.Begin);
            byte[] data = new byte[8];
            fs.Read(data, 0, 8);
            return BitConverter.ToDouble(data, 0);
        }

        private void WriteValue(int node, double value)
        {
            fs.Seek(node, SeekOrigin.Begin);
            fs.Write(BitConverter.GetBytes(value), 0, 8);
        }

        private void WriteNextNode(int node, int next)
        {
            fs.Seek(node + 8, SeekOrigin.Begin);
            fs.Write(BitConverter.GetBytes(next), 0, 4);
        }

        private byte[] ReadNode(int node)
        {
            byte[] data = new byte[12];
            fs.Seek(node, SeekOrigin.Begin);
            fs.Read(data, 0, 12);
            return data;
        }

        public bool IsSorted()
        {
            double prevVal = Head();
            for (int i = 1; i < length; i++)
            {
                double currVal = Next();
                if (prevVal > currVal)
                    return false;
                prevVal = currVal;
            }
            return true;
        }

        private void ElementCount(double element, ref int position, ref int count)
        {
            position = -1;
            count = 0;
            int i = 0;
            for (int node = ReadHead(); node != 0; node = node = ReadNextNode(node), i++)
            {
                if (Math.Abs(ReadValue(node) - element) < 0.00001)
                {
                    if (position == -1)
                        position = i;
                    count++;
                }
            }
        }

        private double ElementCountBigger(double element, ref int position, ref int count)
        {
            position = -1;
            count = 0;
            double larger = double.PositiveInfinity;
            int i = 0;
            for (int node = ReadHead(); node != 0; node = node = ReadNextNode(node), i++)
            {
                if (ReadValue(node) > element)
                {
                    if (Math.Abs(ReadValue(node) - larger) < 0.00001)
                        count++;
                    else if (ReadValue(node) < larger)
                    {
                        position = i;
                        count = 1;
                        larger = ReadValue(node);
                    }
                }
            }
            return larger;
        }

        private double ElementCountBiggest(ref int position, ref int count)
        {
            position = -1;
            count = 0;
            double biggest = double.MinValue;
            int i = 0;
            for (int node = ReadHead(); node != 0; node = node = ReadNextNode(node), i++)
            {
                if (Math.Abs(ReadValue(node) - biggest) < 0.00001)
                    count++;
                else if (ReadValue(node) > biggest)
                {
                    position = i;
                    count = 1;
                    biggest = ReadValue(node);
                }
            }
            return biggest;
        }

        public void PrintRange(int left, int right)
        {
            if (left > right)
            {
                int tmp = left;
                left = right;
                right = tmp;
            }
            if (right > length)
                right = length;
            if (left < 0)
                left = 0;

            int i = 0;
            for (int node = ReadHead(); node != 0 && i < right; node = ReadNextNode(node), i++)
            {
                if (i >= left)
                    Console.Write(" {0:F5} ", ReadValue(node));
            }

            Console.WriteLine();
        }

        public void PrintElement(double element)
        {
            int position = 0, count = 0;
            ElementCount(element, ref position, ref count);
            if (position == -1)
            {
                element = ElementCountBigger(element, ref position, ref count);
                if (position == -1)
                    element = ElementCountBiggest(ref position, ref count);
            }
            Console.WriteLine($"Element {element} is first found at index {position} and can be seen {count} times in the array.");
        }

        public void PrintAll()
        {
            Print(length);
        }

    }
}