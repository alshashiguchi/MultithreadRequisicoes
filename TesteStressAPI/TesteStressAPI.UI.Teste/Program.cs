using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TesteStressAPI.UI.Teste
{
    public class Program
    {
        private static string _url;
        public static void Main()
        {
            Console.WriteLine("Informe URL");
            _url = Console.ReadLine();
            Console.WriteLine("Quantidade Thread");
            var n = Convert.ToInt32(Console.ReadLine());
            var parent = Task.Run(() =>
            {

                for(var i = 0; i < n; i++)
                {                    
                    new Task(() => DownloadContent(Task.CurrentId.Value), TaskCreationOptions.AttachedToParent).Start();
                }
            });
            
            var finalTask = parent.ContinueWith(
                parentTask => {                    
                    Console.WriteLine("Inicio");
                });

            Console.ReadKey();

        }


        public static void DownloadContent(int taskId)
        {
            for(var i = 0; i < 50; i++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var request = WebRequest.Create(_url);

                request.Method = "GET";

                request.Timeout = 300000;

                try
                {
                    var response = request.GetResponse();
                }
                catch
                {
                    Console.WriteLine("Erro");
                }

                stopwatch.Stop();

                Process myProcess = null;
                long memoryLength = 0;                
                try
                {

                    // Display the process statistics until// the user closes the program.do 
                    {
                        // Start the process.
                        var test = Process.GetProcessesByName("w3wp");
                        //var test = Process.GetProcessesByName("iisexpress");
                        //var test = Process.GetProcessesByName("node");
                        myProcess = test.First();

                        if (!myProcess.HasExited)
                        {
                            // Refresh the current process property values.
                            myProcess.Refresh();
                            memoryLength = myProcess.WorkingSet64;
                        }
                    }
                }
                finally
                {
                    if (myProcess != null)
                    {
                        myProcess.Close();
                    }
                }

                Console.WriteLine(taskId + "," + stopwatch.Elapsed.Milliseconds + "," + (memoryLength / 1000000));

            }
            
        }
    }
}