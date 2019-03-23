using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingOP
{
    class MyDataArray : DataArray

    {
        double[] data;

        public MyDataArray(int n, int seed)
        {
            data = new double[n];
            length = n;
            Random rand = new Random(seed);
            for (int i = 0; i < length; i++)
            {
                data[i] = rand.NextDouble();
            }
        }

        public override double this [int index]
        {
            get { return data[index]; }
            set { data[index] = value; }
        }

        public override void Swap(int j, double a, double b)
        {
            data[j - 1] = a;
            data[j] = b;

        }

        public bool IsSorted()
        {
            for(int i = 0; i < data.Length - 1; i++)
            {
                if (data[i] > data[i + 1])
                    return false;
            }
            return true;
        }

    }
}
