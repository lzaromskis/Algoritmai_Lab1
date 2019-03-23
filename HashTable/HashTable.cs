using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTableSearch
{
    class HashTable
    {

        private KeyValueNode[] table;

        public int Size { get; private set; }
        public float LoadFactor { get; private set; }

        public HashTable()
        {
            table = new KeyValueNode[16];
            LoadFactor = 0.75f;
        }

        public HashTable(int initialCapacity, float loadFactor)
        {
            table = new KeyValueNode[initialCapacity];
            LoadFactor = loadFactor;
            Size = 0;
        }

        public void Put(string key, string value)
        {
            if (key == null)
                return;

            int index = Hash(key);

            KeyValueNode node = GetInChain(key, table[index]);

            if (node == null)
            {
                table[index] = new KeyValueNode(key, value, table[index]);
                Size++;

                if (Size > table.Length * LoadFactor)
                    Rehash();
            }
            else
            {
                node.Value = value;
            }
        }

        public string Get(string key)
        {
            int index = Hash(key);
            KeyValueNode node = GetInChain(key, table[index]);
            if (node != null)
                return node.Value;
            else
                return null;
        }

        public bool IsInSameChain(string key1, string key2, out int hash1, out int hash2)
        {
            hash1 = Hash(key1);
            hash2 = Hash(key2);

            return hash1 == hash2;
        }

        private void Rehash()
        {
            HashTable newTable = new HashTable(table.Length * 2, LoadFactor);

            for (int i = 0; i < table.Length; i++)
            {
                while (table[i] != null)
                {
                    newTable.Put(table[i].Key, table[i].Value);
                    table[i] = table[i].NextNode;
                }
            }

            table = newTable.table;
        }

        private int Hash(string key)
        {
            int hash = key.GetHashCode();
            return Math.Abs(hash) % table.Length;
        }

        private KeyValueNode GetInChain(string key, KeyValueNode node)
        {
            for (KeyValueNode n = node; n != null; n = n.NextNode)
                if (key.Equals(n.Key))
                    return n;
            return null;
        }

        public int GetNumberOfChains()
        {
            int count = 0;
            foreach (var pair in table)
            {
                if (pair != null)
                    count++;
            }
            return count;
        }


        private class KeyValueNode
        {
            public string Key;
            public string Value;

            public KeyValueNode NextNode;

            public KeyValueNode(string key, string value, KeyValueNode next)
            {
                Key = key;
                Value = value;
                NextNode = next;
            }
        }

        public string[] ToVisualizedString()
        {
            string[] result = new string[table.Length];

            for (int i = 0; i < table.Length; i++)
            {
                KeyValueNode n = table[i];
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("[{0:00}]", i);
                int length = 0;
                while (n != null)
                {
                    builder.AppendFormat("-->{0,4}", n.Key);
                    length++;
                    n = n.NextNode;
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
