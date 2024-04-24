using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            int[] numbers = { 1, 2, 3,4};
            PrintCombinations(numbers);

            Console.Read();
        }


        static void PrintCombinations(int[] numbers)
        {
            int n = numbers.Length;

            // 输出 {*, *}, {*, 2}, {*, 3}, {1, *}, {1, 2}, {1, 3}
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    List<string> subset = new List<string>();
                    for (int k = 0; k < n; k++)
                    {
                        if (k <= i && k >= j)
                        {
                            subset.Add(numbers[k].ToString());
                        }
                        else
                        {
                            subset.Add("*");
                        }
                    }
                    Console.WriteLine("{0}", string.Join(",", subset));
                }
            }

            // 输出 {*, *, *}, {*, *, 3}, {*, 2, *}, {*, 2, 3}, {1, *, *}, {1, *, 3}, {1, 2, *}, {1, 2, 3}
            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    List<string> subset = new List<string>();
                    for (int k = 0; k < n; k++)
                    {
                        if (k <= j && k >= i)
                        {
                            subset.Add(numbers[k].ToString());
                        }
                        else
                        {
                            subset.Add("*");
                        }
                    }
                    Console.WriteLine("{0}", string.Join(",", subset));
                }
            }
        }

    }
}
