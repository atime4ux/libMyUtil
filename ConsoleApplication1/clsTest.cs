using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplication1
{
    public class clsTest
    {
        public void DoIt()
        {
            System.Threading.Thread t1 = new System.Threading.Thread(new System.Threading.ThreadStart(printChar));
            t1.Name = System.Threading.Thread.CurrentThread.Name + "'s CHILD";
            t1.Start();
        }

        public void printChar()
        {
            string threadName = System.Threading.Thread.CurrentThread.Name;

            for (int i = 0; i < 30; i++)
            {
                Console.WriteLine(threadName);
                System.Threading.Thread.Sleep(800);
            }
        }
    }

    public class test
    {
        private static string a;
        private static test the_object;

        private test()
        {
            the_object = this;
            a = "this is a";
        }

        public static string getValue()
        {
            if (the_object == null)
            {
                the_object = new test();
            }

            return a;
        }

        public static void setA()
        {
            a = "modified A";
        }

        public static string getA()
        {
            return a;
        }


    }
}
