using System;
using System.Collections.Generic;
using System.Diagnostics;  		// for the stop watch
using System.Threading.Tasks;	// for parallel for
using System.Text;              // for stringbuilder

namespace cs474lab2
{
    class Lab2
    {
        public static int CPUS = (int)Environment.ProcessorCount;

        // 1. Again define a constant SIZE and make it 20 initially for easy debugging and testing.
        // 2. Declare an array of int/short/char/bit of SIZE representing integers from 1 to SIZE.
        // 3. Initialize the array to all 1â€™s. This step can be done in parallel.
        // 4. Looping through the array to remove the multiples of primes. The loop needs to go from 2 to the ceiling of square root of SIZE. Within each iteration, if the array element representing an integer that is the multiple of the current prime, set the array element to 0. Select a suitable method to parallelize the loop body.
        // 5. Count the number of 1â€™s still in the array and print that number out.

        /// <summary>
        /// Sequentially count the primes less than {size} using a prime number sieve.
        /// </summary>
        /// <returns>The primes less than {size}</returns>
        /// <param name="size">The number under which to count the primes.</param>
        public static int SequentialCountPrimesLessThan(int size)
        {
            int primes = 0;
            byte[] array = new byte[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = 1;
            }

            // explicitly set 0 and 1 to be not prime.
            if (size > 0) array[0] = 0;
            if (size > 1) array[1] = 0;

            /*
            int min = 2;
            int max = (int) Math.Ceiling(Math.Sqrt(size));
            for(int i = min; i < max; i++)
            {
                // if i is prime, remove multiples
                if (array[i] == 1)
                {
                    for(int j = i + i; j < size; j += i)
                    {	
                        array[j] = 0;
                    }	 
                }
            }

            // 5. Count the number of 1â€™s still in the array and print that number out.
            for(int i = 0; i < size; i ++)
            {
                primes += array[i];
            }
            */
            return primes;
        }

        /// <summary>
        /// In parallel, count the primes less than {size} using a prime number sieve.
        /// </summary>
        /// <returns>The primes less than {size}</returns>
        /// <param name="size">The number under which to count the primes.</param>
        public static int ParallelCountPrimesLessThan(int size)
        {
            int primes = 0;
            object primeLock = new object();
            // 2. Declare an array of int/short/char/bit of SIZE representing integers from 1 to SIZE.
            byte[] array = new byte[size];

            // 3. Initialize the array to all 1â€™s. This step can be done in parallel.
            Parallel.For(0, size, p =>
            {
                array[p] = 1;
            });


            // explicitly set 0 and 1 to be not prime.
            if (size > 0) array[0] = 0;
            if (size > 1) array[1] = 0;

            /*
            // 4. Looping through the array to remove the multiples of primes. 
            //    The loop needs to go from 2 to the ceiling of square root of SIZE. 
            //    Within each iteration, if the array element representing an integer 
            //    that is the multiple of the current prime, set the array element to 0. 
            //    Select a suitable method to parallelize the loop body.

            int min = 2;
            int max = (int) Math.Ceiling(Math.Sqrt(size));
            int chunk = max / CPUS; 
            for(int i = min; i < max; i++)
            {
                // if i is prime, remove multiples
                if (array[i] == 1)
                {
                    // parallelize removal of multiples
                    Parallel.For(0, CPUS, p => 
                    {
                        for(int j = p * chunk; j < (p + 1) * chunk; j += i)
                        {	
                            array[j] = 0;
                        }	 
                    });
                }
            }


            // 5. Count the number of 1â€™s still in the array and print that number out.

            chunk = size / CPUS;
            Parallel.For(0, CPUS, p =>
            {
                int sum = 0;
                for(int i = p * chunk; i < (p + 1) * chunk && i < size; i ++)
                {
                    sum += array[i];
                }
                lock(primeLock)
                {
                    primes += sum;	
                }

            });

            //PrintArrayContents(array);
            */
            return primes;
        }

        /// <summary>
        /// helper method to print array contents.
        /// </summary>
        /// <param name="array">The array to iterate over.</param>
        public static void PrintArrayContents(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine("array[{0}] = {1}", i, array[i]);
            }
        }

        /// <summary>
        /// helper method to print the primes.
        /// </summary>
        /// <param name="array">The array to iterate over.</param>
        public static void PrintPrimes(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 1) Console.WriteLine(i.ToString());
            }
        }

        /// <summary>
        /// Count all primes under the given size, and print the results.
        /// </summary>
        /// <param name="size">The upper limit for counting the primes.</param>
        /// <param name="parallel">If true, run in parallel, else run in sequential.</param>
        public static void RunTest(int size, bool parallel)
        {
            Stopwatch timer = new Stopwatch();
            int count = 0;
            if (parallel)
            {
                Console.WriteLine("Count primes (parallel)");
                timer.Start();
                count = ParallelCountPrimesLessThan(size);
                timer.Stop();
            }
            else
            {
                Console.WriteLine("Count primes (sequential)");
                timer.Start();
                count = SequentialCountPrimesLessThan(size);
                timer.Stop();
            }
            Console.WriteLine("Primes less than {0}: {1}", size, count);
            Console.WriteLine("Elapsed time: {0} seconds\n", timer.ElapsedMilliseconds / 1000.0);


        }

        public static void TestParallels()
        {
            Stopwatch timer = new Stopwatch();
            StringBuilder sb = new StringBuilder();
            int max = 10000;
            int[] elements;
            double time = 0;

            for (int chunk = 1; chunk <= max; chunk += 1)
            {
                // **********************************
                // parallel
                // **********************************
                elements = new int[max];
                timer.Restart();

                Parallel.For(0, chunk, p =>
                {
                    for (int i = p * chunk; i < max; i += chunk)
                    {
                        for (int j = 0; j < 1000; j++)
                        {
                            elements[i] += i;
                        }
                    }
                });
                timer.Stop();
                time = timer.ElapsedMilliseconds / 1000.0;
                sb.AppendLine(String.Format("pStep: {0}\tTime: {1:N6})", chunk, time));
            }

            // **********************************
            // sequential
            // **********************************
            elements = new int[max];
            timer.Restart();
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    elements[i] += i;
                }
            }
            timer.Stop();
            time = timer.ElapsedMilliseconds / 1000.0;

            sb.AppendLine(String.Format("Sequential Time: {0:N3})", time));

            Console.WriteLine(sb.ToString());
        }

        public static void Main(string[] args)
        {
            		
            Console.WriteLine("CPUS: " + CPUS);
            Console.WriteLine("----------------------------");
            //RunTest(1000, true);
            RunTest(1000, false);
            Console.WriteLine("----------------------------");
            //RunTest(1000000, true);
            RunTest(1000000, false);
            Console.WriteLine("----------------------------");
            //RunTest(2000000, true);
            RunTest(2000000, false);
            

            //TestParallels();

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }
}