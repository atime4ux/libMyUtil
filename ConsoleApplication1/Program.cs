using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Main start*****************");
            Console.WriteLine(test.getValue());
            Console.WriteLine("Finished *****************Main");
            test.setA();
            Console.WriteLine(test.getA());
            Console.Read();
        }

        static void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            MessageBox.Show("완료");
        }

        static void testFunc()
        {
            Console.WriteLine("Test Function Start*****************");
            System.Threading.Thread t1 = new System.Threading.Thread(new System.Threading.ThreadStart(mainJob));
            t1.Name = "Main Job";
            t1.Start();
        }

        static void mainJob()
        {
            Console.WriteLine("Main Job*****************");
            clsTest objTest = new clsTest();
            objTest.DoIt();
            Console.WriteLine("Begin Sleep Main Job");
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine("End Sleep Main Job");
            Console.WriteLine("Finished *****************Main Job");
        }

        static void subJob()
        {
            Console.WriteLine("Sub Job*****************");
            for(int i=0; i<40; i++)
            {
                Console.WriteLine("This is Sub Job");
                System.Threading.Thread.Sleep(500);
            }
            Console.WriteLine("Finished *****************Sub Job");
        }
    }
}