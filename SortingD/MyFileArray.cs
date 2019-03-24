using System;
using System.IO;
namespace SortingD
{
    class MyFileArray : DataArray
    {
        public MyFileArray(string filename, int n, int seed)
        {
            double[] data = new double[n];
            length = n;
            Random rand = new Random(seed);
            for (int i = 0; i < length; i++)
            {
                data[i] = rand.NextDouble();
            }
            if (File.Exists(filename)) File.Delete(filename);
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filename,
               FileMode.Create)))
                {
                    for (int j = 0; j < length; j++)
                        writer.Write(data[j]);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public MyFileArray(string filename)
        {
            if (File.Exists(filename)) File.Delete(filename);
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filename,
               FileMode.Create)))
                {

                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public FileStream fs { get; set; }
        public override double this[int index]
        {
            get
            {
                Byte[] data = new Byte[8];
                fs.Seek(8 * index, SeekOrigin.Begin);
                fs.Read(data, 0, 8);
                double result = BitConverter.ToDouble(data, 0);
                return result;
            }
            set
            {
                Byte[] data = BitConverter.GetBytes(value);
                fs.Seek(8 * index, SeekOrigin.Begin);
                fs.Write(data, 0, 8);
            }
        }

        public override void Swap(int j, double a, double b)
        {
            Byte[] data = new Byte[16];
            BitConverter.GetBytes(a).CopyTo(data, 0);
            BitConverter.GetBytes(b).CopyTo(data, 8);
            fs.Seek(8 * (j - 1), SeekOrigin.Begin);
            fs.Write(data, 0, 16);
        }

        public bool IsSorted()
        {
            for (int i = 0; i < this.Length - 1; i++)
            {
                if (this[i] > this[i + 1])
                    return false;
            }
            return true;
        }

        private void ElementCount(double element, ref int position, ref int count)
        {
            position = -1;
            count = 0;
            for (int i = 0; i < length; i++)
            {
                if (Math.Abs(this[i] - element) < 0.00001)
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
            for (int i = 0; i < length; i++)
            {
                if (this[i] > element)
                {
                    if (Math.Abs(this[i] - larger) < 0.00001)
                        count++;
                    else if (this[i] < larger)
                    {
                        larger = this[i];
                        position = i;
                        count = 1;
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
            for (int i = 0; i < length; i++)
            {
                if (Math.Abs(this[i] - biggest) < 0.00001)
                    count++;
                else if (this[i] > biggest)
                {
                    biggest = this[i];
                    position = i;
                    count = 1;
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
            for (int i = left; i < right; i++)
                Console.Write(" {0:F5} ", this[i]);
            Console.WriteLine();
        }

        public void PrintElement(double element)
        {
            int position = -1, count = 0;
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