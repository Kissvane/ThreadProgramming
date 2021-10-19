using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadProgramming
{
    class Program
    {
        #region global variables
        private static int Gold_1 = 0;
        private static int Bags_1 = 0;

        private static int Gold_2 = 0;
        private static int Bags_2 = 0;

        private static int Gold_3 = 0;
        private static int Bags_3 = 0;
        //private static int quotaBag = 10000;
        static int workerNumber = 10000;
        static int totalTargetBag = 100000000;
        private static Object _lock = new Object();
        private static List<Thread> pool = new List<Thread>();
        static Stopwatch stopwatch = new Stopwatch();
        static bool simple = false;

        static List<string> toWrite = new List<string>();

        #endregion

        /*static int cumul = 0;
        private static Object _lock2 = new Object();
        private static int threadNumber = 10000;
        private static int loopNumber = 10000;

        static void Main(string[] args)
        {
            Thread test = new Thread(GlobalThreadTest);
            test.Start();

            test.Join();

            Console.WriteLine("Je compte jusqu'à " + cumul + " au lieu de " + (threadNumber * loopNumber));
        }

        static void GlobalThreadTest()
        {
            List<Thread> temp = new List<Thread>();

            for (int i = 0; i < threadNumber; i++)
            {
                Thread t = new Thread(ThreadTest);
                t.Name = "Thread" + i;
                t.Start();

            }

            foreach (Thread t in temp)
            {
                t.Join();
            }
        }

        static void ThreadTest()
        {
            //Console.WriteLine("Starting thread : " + Thread.CurrentThread.Name);
            lock (_lock2)
            {
                for (int i = 0; i < loopNumber; i++)
                {
                    cumul++;
                }
            }
            //Console.WriteLine("Ending thread : " + Thread.CurrentThread.Name);
        }*/

        #region Mains and tools

        //Main mine 1 
        static async Task Main(string[] args)
        {
            Thread mine1;
            Thread mine2;
            Task mine3;

            stopwatch.Start();
            mine1 = new Thread(Mine1);
            mine1.Name = "mine1";
            mine1.Start();
            mine1.Join();
            stopwatch.Stop();
            toWrite.Add("WORK IS OVER");
            toWrite.Add("FINAL " + Bags_1 + " bags / " + Gold_1 + " gold");
            toWrite.Add("Mine1 time : " + (stopwatch.ElapsedMilliseconds / 1000f));
            stopwatch.Reset();

            WriteImportantInfos();

            stopwatch.Start();
            mine2 = new Thread(Mine2);
            mine2.Name = "mine2";
            mine2.Start();
            mine2.Join();
            stopwatch.Stop();
            toWrite.Add("WORK IS OVER");
            toWrite.Add("FINAL " + Bags_2 + " bags / " + Gold_2 + " gold");
            toWrite.Add("Mine2 time : " + (stopwatch.ElapsedMilliseconds / 1000f));
            stopwatch.Reset();

            WriteImportantInfos();

            stopwatch.Start();
            mine3 = Mine3();
            await mine3;
            stopwatch.Stop();
            toWrite.Add("WORK IS OVER");
            toWrite.Add("FINAL " + Bags_3 + " bags / " + Gold_3 + " gold");
            toWrite.Add("Mine3 time : " + (stopwatch.ElapsedMilliseconds / 1000f));
            stopwatch.Reset();

            WriteImportantInfos();

            Gold_1 = 0;
            Bags_1 = 0;
            Gold_2 = 0;
            Bags_2 = 0;
            Gold_3 = 0;
            Bags_3 = 0;
            simple = true;


            stopwatch.Start();
            mine1 = new Thread(Mine1);
            mine1.Name = "mine1";
            mine1.Start();
            mine1.Join();
            stopwatch.Stop();
            toWrite.Add("WORK IS OVER");
            toWrite.Add("FINAL " + Bags_1 + " bags / " + Gold_1 + " gold");
            toWrite.Add("Mine1 time : " + (stopwatch.ElapsedMilliseconds / 1000f));
            stopwatch.Reset();

            WriteImportantInfos();

            stopwatch.Start();
            mine2 = new Thread(Mine2);
            mine2.Name = "mine2";
            mine2.Start();
            mine2.Join();
            stopwatch.Stop();
            toWrite.Add("WORK IS OVER");
            toWrite.Add("FINAL " + Bags_2 + " bags / " + Gold_2 + " gold");
            toWrite.Add("Mine2 time : " + (stopwatch.ElapsedMilliseconds / 1000f));
            stopwatch.Reset();

            WriteImportantInfos();

            stopwatch.Start();
            mine3 = Mine3();
            await mine3;
            stopwatch.Stop();
            toWrite.Add("WORK IS OVER");
            toWrite.Add("FINAL " + Bags_3 + " bags / " + Gold_3 + " gold");
            toWrite.Add("Mine3 time : " + (stopwatch.ElapsedMilliseconds / 1000f));
            stopwatch.Reset();

            WriteImportantInfos();

        }

        static void WriteImportantInfos()
        {
            Console.Clear();
            foreach (string s in toWrite)
            {
                Console.WriteLine(s);
            }
        }

        static int NameToInt(string name)
        {
            int result = 0;
            foreach (Char c in name)
            {
                result += c;
            }

            return result;
        }

        #endregion

        #region mine 1

        static void Mine1()
        {
            Thread miner;

            for (int i = 0; i < workerNumber; i++)
            {
                if (!simple)
                {
                    miner = new Thread(Miner);
                }
                else
                {
                    miner = new Thread(SimpleMiner);
                }

                miner.Name = "miner" + i;
                pool.Add(miner);
                miner.Priority = ThreadPriority.Lowest;
                miner.Start();
            }

            foreach (Thread t in pool)
            {
                t.Join();
            }
        }

        static void Miner()
        {
            Random r = new Random((int)DateTime.Now.Ticks + NameToInt(Thread.CurrentThread.Name));

            int HarvestedGold = 0;
            int recoltedBag = 0;

            lock (_lock)
            {
                while (Bags_1 < totalTargetBag)
                {
                    recoltedBag++;
                    HarvestedGold = r.Next(1, 5);

                    Gold_1 += HarvestedGold;
                    Bags_1 += 1;
                }
            }
        }

        static public void SimpleMiner()
        {
            lock (_lock)
            {
                while (Bags_1 < totalTargetBag)
                {
                    Bags_1++;
                }
            }
        }

        #endregion

        #region Mine 2

        struct Container
        {
            public ManualResetEvent[] events;
            public int i;
        }

        static void Mine2()
        {
            ManualResetEvent[] events = new ManualResetEvent[workerNumber];
            for (int i = 0; i < workerNumber; i++)
            {
                events[i] = new ManualResetEvent(false);
            }

            for (int i = 0; i < workerNumber; i++)
            {
                Container c = new Container();
                c.events = events;
                c.i = i;

                if (simple)
                {
                    ThreadPool.QueueUserWorkItem(SimpleMiner2, c);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(Miner2, c);
                }
            }

            //solution 1
            Thread watcher = new Thread(Watcher2);
            watcher.Priority = ThreadPriority.Highest;
            watcher.Start();
            watcher.Join();
            //end of solution 1

            //solution2
            //work only if there is less than 65 workers
            //WaitHandle.WaitAll(events);
        }

        static void Watcher2(object callback)
        {
            while (Bags_2 < totalTargetBag)
            {
                Thread.Sleep(1);
            }
        }

        static void Miner2(object callback)
        {
            Random r = new Random((int)DateTime.Now.Ticks);

            int HarvestedGold = 0;
            int recoltedBag = 0;
            lock (_lock)
            {
                while (Bags_2 < totalTargetBag)
                {
                    recoltedBag++;
                    HarvestedGold = r.Next(1, 5);

                    Gold_2 += HarvestedGold;
                    Bags_2 += 1;
                }
            }


            Container c = ((Container)callback);
            c.events[c.i].Set();
        }

        static void SimpleMiner2(object callback)
        {
            lock (_lock)
            {
                while (Bags_2 < totalTargetBag)
                {
                    Bags_2 ++;
                }
                //Bags_2 += quotaBag;
            }

            Container c = ((Container)callback);
            c.events[c.i].Set();
        }

        #endregion

        #region mine 3

        static async Task Mine3()
        {
            List<Task> tasks = new List<Task>();
            Task t;
            for (int i = 0; i < workerNumber; i++)
            {
                if (simple)
                {
                    t = Task.Run(() => SimpleMiner3());
                }
                else
                {
                    t = Task.Run(() => Miner3());
                }
                tasks.Add(t);
            }

            await Task.WhenAll(tasks);
        }

        static void Miner3()
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            int HarvestedGold = 0;
            int recoltedBag = 0;

            while (Bags_3 < totalTargetBag)
            {
                recoltedBag++;
                HarvestedGold = r.Next(1, 5);
                lock (_lock)
                {
                    Gold_3 += HarvestedGold;
                    Bags_3 += 1;
                }
            }
        }

        static void SimpleMiner3()
        {
            lock (_lock)
            {
                while (Bags_3 < totalTargetBag)
                {
                    Bags_3++;
                }
            }
        }

        #endregion
    }
}
