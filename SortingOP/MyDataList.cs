﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingOP
{
    class MyDataList : DataList
    {
        class MyLinkedListNode
        {
            public MyLinkedListNode nextNode { get; set; }
            public double data { get; set; }
            public MyLinkedListNode(double data)
            {
                this.data = data;
            }
        }

        MyLinkedListNode headNode;
        MyLinkedListNode prevNode;
        MyLinkedListNode currentNode;

        public MyDataList()
        {
            
        }

        public MyDataList(int n, int seed)
        {
            length = n;
            Random rand = new Random(seed);
            headNode = new MyLinkedListNode(rand.NextDouble());
            currentNode = headNode;
            for (int i = 1; i < length; i++)
            {
                prevNode = currentNode;
                currentNode.nextNode = new MyLinkedListNode(rand.NextDouble());
                currentNode = currentNode.nextNode;
            }
            currentNode.nextNode = null;
        }

        public void Add(double value)
        {
            MyLinkedListNode newNode = new MyLinkedListNode(value);
            length++;
            newNode.nextNode = headNode;
            headNode = newNode;
        }

        public override double Head()
        {
            currentNode = headNode;
            prevNode = null;
            return currentNode.data;
        }

        public override double Next()
        {
            prevNode = currentNode;
            currentNode = currentNode.nextNode;
            return currentNode.data;
        }

        public override void Swap(double a, double b)
        {
            prevNode.data = a;
            currentNode.data = b;
        }

        public void MergeSort()
        {
            headNode = MergeSort(headNode);
        }

        private MyLinkedListNode MergeSort(MyLinkedListNode head)
        {
            if (head == null || head.nextNode == null)
                return head;
            MyLinkedListNode middle = GetMiddle(head);
            MyLinkedListNode leftHead = head;
            MyLinkedListNode rightHead = middle.nextNode;
            middle.nextNode = null;
            return Merge(MergeSort(leftHead), MergeSort(rightHead));
        }

        private MyLinkedListNode Merge(MyLinkedListNode a, MyLinkedListNode b)
        {
            MyLinkedListNode dummy = new MyLinkedListNode(double.NaN);
            MyLinkedListNode current = dummy;
            for ( ; a != null && b != null; current = current.nextNode)
            {
                if (a.data <= b.data)
                {
                    current.nextNode = a;
                    a = a.nextNode;
                }
                else
                {
                    current.nextNode = b;
                    b = b.nextNode;
                }
            }
            current.nextNode = (a == null) ? b : a;
            return dummy.nextNode;
        }

        private MyLinkedListNode GetMiddle(MyLinkedListNode head)
        {
            if (head == null)
                return head;
            MyLinkedListNode slow = head;
            MyLinkedListNode fast = head;
            while (fast.nextNode != null && fast.nextNode.nextNode != null)
            {
                slow = slow.nextNode;
                fast = fast.nextNode.nextNode;
            }
            return slow;
        }

        public void InsertionSort()
        {
            InsertionSort(headNode);
        }

        private void InsertionSort(MyLinkedListNode node)
        {
            MyLinkedListNode sorted = null;
            MyLinkedListNode current = node;
            while (current != null)
            {
                MyLinkedListNode next = current.nextNode;
                SortedInsert(ref current, ref sorted);
                current = next;
            }
            headNode = sorted;
        }

        private void SortedInsert(ref MyLinkedListNode newNode, ref MyLinkedListNode sorted)
        {
            if (sorted == null || sorted.data >= newNode.data)
            {
                newNode.nextNode = sorted;
                sorted = newNode;
            }
            else
            {
                MyLinkedListNode current = sorted;
                while (current.nextNode != null && current.nextNode.data < newNode.data)
                {
                    current = current.nextNode;
                }
                newNode.nextNode = current.nextNode;
                current.nextNode = newNode;
            }
        }

        public void BucketSort()
        {
            MyDataList[] buckets = new MyDataList[Length];
            for (int i = 0; i < buckets.Length; i++)
                buckets[i] = new MyDataList();
            MyLinkedListNode node = headNode;
            while (node != null)
            {
                int bi = (int)((double)Length * node.data);
                buckets[bi].Add(node.data);
                node = node.nextNode;
            }
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i].InsertionSort();
            }
            node = headNode;
            for (int i = 0; i < Length; i++)
            {
                var bucket = buckets[i];
                if (bucket.Length == 0)
                    continue;
                node.data = bucket.Head();
                node = node.nextNode;
                for (int j = 1; j < bucket.Length; j++)
                {
                    node.data = bucket.Next();
                    node = node.nextNode;
                }
            }
        }

        public bool IsSorted()
        {
            MyLinkedListNode node = headNode;
            while (node.nextNode != null)
            {
                if (node.data > node.nextNode.data)
                    return false;
                node = node.nextNode;
            }
            return true;
        }

        private void ElementCount(double element, ref int position, ref int count)
        {
            position = -1;
            count = 0;
            int i = 0;
            for (MyLinkedListNode node = headNode; node != null; node = node.nextNode, i++)
            {
                if (Math.Abs(node.data - element) < 0.00001)
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
            for (MyLinkedListNode node = headNode; node != null; node = node.nextNode, i++)
            {
                if (node.data > element)
                {
                    if (Math.Abs(node.data - larger) < 0.00001)
                        count++;
                    else if (node.data < larger)
                    {
                        position = i;
                        count = 1;
                        larger = node.data;
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
            for (MyLinkedListNode node = headNode; node != null; node = node.nextNode, i++)
            {
                if (Math.Abs(node.data - biggest) < 0.00001)
                    count++;
                else if (node.data > biggest)
                {
                    position = i;
                    count = 1;
                    biggest = node.data;
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
            for (MyLinkedListNode node = headNode; node != null && i < right; node = node.nextNode, i++)
            {
                if (i >= left)
                    Console.Write(" {0:F5} ", node.data);
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
