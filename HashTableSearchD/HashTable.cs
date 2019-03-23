using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HashTableSearchD
{
    class HashTable
    {
        private static readonly byte[] NULLBYTE = { (byte)0, (byte)0, (byte)0, (byte)0 };

        private const int StringByteLenght = sizeof(char) * 10;
        private const int AddressByteLenght = 4;



        public int Size { get; private set; }
        public int Capacity { get; private set; }
        public float LoadFactor { get; private set; }

        public FileStream fs { get; set; }

        private int nextNodeLocation;

        //public HashTable()
        //{
        //    table = new KeyNode[16];
        //    LoadFactor = 0.75f;
        //}

        public HashTable(int initialCapacity, float loadFactor, string fileName)
        {
           // table = new KeyNode[initialCapacity];
            LoadFactor = loadFactor;
            Capacity = initialCapacity;
            Size = 0;
            nextNodeLocation = AddressByteLenght * initialCapacity;
            
            if (File.Exists(fileName))
                File.Delete(fileName);

            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                {
                    for (int i = 0; i < initialCapacity; i++)
                    {
                        writer.Write(NULLBYTE);
                    }
                }
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Put(string key, string value)
        {
            if (key == null)
                return;

            int index = Hash(key);

            int node = GetInChain(key, index);

            if (node == 0)
            {

                WriteKey(nextNodeLocation, key);
                WriteValue(nextNodeLocation, value);
                WriteNextNode(nextNodeLocation, ReadFirstNode(index));
                WriteFirstNode(index, nextNodeLocation);
                nextNodeLocation += StringByteLenght * 2 + AddressByteLenght;
                Size++;
            }
        }

        public string Get(string key)
        {
            int index = Hash(key);
            int node = GetInChain(key, ReadFirstNode(index));
            if (node != 0)
                return ReadValue(node);
            else
                return null;
        }

        public bool IsInSameChain(string key1, string key2, out int hash1, out int hash2)
        {
            hash1 = Hash(key1);
            hash2 = Hash(key2);

            return hash1 == hash2;
        }

        private int Hash(string key)
        {
            int hash = key.GetHashCode();
            return (Math.Abs(hash) % Capacity) * AddressByteLenght;
        }

        private int GetInChain(string key, int node)
        {
            for (int n = node; n != 0; n = ReadNextNode(n))
                if (key.Equals(ReadKey(n)))
                    return n;
            return 0;
        }

        public int GetNumberOfChains()
        {
            int count = 0;
            fs.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < Capacity; i++)
            {
                if (ReadFirstNode(i * AddressByteLenght) != 0)
                    count++;
            }
            return count;
        }

        private int ReadFirstNode(int tableIndex)
        {
            fs.Seek(tableIndex, SeekOrigin.Begin);
            byte[] data = new byte[AddressByteLenght];
            fs.Read(data, 0, AddressByteLenght);
            return BitConverter.ToInt32(data, 0);
        }

        private void WriteFirstNode(int tableIndex, int node)
        {
            fs.Seek(tableIndex, SeekOrigin.Begin);
            byte[] data = BitConverter.GetBytes(node);
            fs.Write(data, 0, AddressByteLenght);
        }

        private int ReadNextNode(int node)
        {
            fs.Seek(node + StringByteLenght * 2, SeekOrigin.Begin);
            byte[] data = new byte[AddressByteLenght];
            fs.Read(data, 0, AddressByteLenght);
            return BitConverter.ToInt32(data, 0);
        }

        private void WriteNextNode(int current, int next)
        {
            fs.Seek(current + StringByteLenght * 2, SeekOrigin.Begin);
            byte[] data = BitConverter.GetBytes(next);
            fs.Write(data, 0, AddressByteLenght);
        }

        private string ReadKey(int node)
        {
            fs.Seek(node, SeekOrigin.Begin);
            byte[] data = new byte[StringByteLenght];
            fs.Read(data, 0, StringByteLenght);
            StringBuilder builder = new StringBuilder(10);
            for (int i = 0; i < StringByteLenght; )
            {
                byte[] ch = new byte[sizeof(char)];
                for (int j = 0; j < sizeof(char); j++)
                {
                    ch[j] = data[i++];
                }
                builder.Append(BitConverter.ToChar(ch, 0));
            }
            return builder.ToString();
        }

        private void WriteKey(int node, string key)
        {
            fs.Seek(node, SeekOrigin.Begin);
            foreach (char c in key)
            {
                byte[] data = BitConverter.GetBytes(c);
                fs.Write(data, 0, sizeof(char));
            }
        }

        private string ReadValue(int node)
        {
            fs.Seek(node + StringByteLenght, SeekOrigin.Begin);
            byte[] data = new byte[StringByteLenght];
            fs.Read(data, 0, StringByteLenght);
            StringBuilder builder = new StringBuilder(10);
            for (int i = 0; i < StringByteLenght;)
            {
                byte[] ch = new byte[sizeof(char)];
                for (int j = 0; j < sizeof(char); j++)
                {
                    ch[j] = data[i++];
                }
                builder.Append(BitConverter.ToChar(ch, 0));
            }
            return builder.ToString();
        }

        private void WriteValue(int node, string value)
        {
            fs.Seek(node + StringByteLenght, SeekOrigin.Begin);
            foreach (char c in value)
            {
                byte[] data = BitConverter.GetBytes(c);
                fs.Write(data, 0, sizeof(char));
            }
        }

        public string[] ToVisualizedString()
        {
            string[] result = new string[Capacity];

            for (int i = 0; i < Capacity; i++)
            {
                int n = ReadFirstNode(i * AddressByteLenght);
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("[{0:00}]", i);
                int length = 0;
                while (n != 0)
                {
                    builder.AppendFormat("-->{0,10}", ReadKey(n));
                    length++;
                    n = ReadNextNode(n);
                }
                result[i] = builder.ToString();
            }

            return result;
        }

        public void Print()
        {
            foreach (string ss in ToVisualizedString())
            {
                Console.WriteLine(ss);
            }
        }
    }
}
