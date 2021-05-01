using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Client
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            int length = 0;
            try
            {
                var tasks = new ConcurrentBag<Task>();
                Parallel.ForEach(Enumerable.Range(0, 100), async (x) =>
                {
                    //Console.Write(x + " ");
                    tasks.Add(
                        Task.Run(async () =>
                            {
                                var response = await client.GetStringAsync("https://localhost:5001/api/test");
                                if (x == 0) length = (int)response?.Length;
                                Console.Write(response?.Length != length ? " Failed" : ".");
                            }
                        )
                    );
                });
                await Task.WhenAll(tasks);

                Console.WriteLine(" finish ...");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
