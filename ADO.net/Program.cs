using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using static System.Console;
using System.Diagnostics;
using System.Linq;
using ADO.net.EF;

namespace ADO.net
{
    class Program
    {
        static void Main(string[] args)
        {
            //Benchmark(500);
            Out.WriteLine($"Finished execution in {MeasureAction(Start)} mS...");
            ReadLine();
        }

        static void Start()
        {
            ADODBContext database = new ADODBContext();
            using (database)
            {
                database.Database.CreateIfNotExists();
            }

            List<IExecutableTask> executableTasks = new List<IExecutableTask>();

            var currentAssembly = Assembly.GetExecutingAssembly();
            List<Task> tasks = new List<Task>();

            currentAssembly.GetTypes()
                    .Where(t => t.Namespace == "ADO.net.Services" && t.GetInterface(nameof(IExecutableTask)) != null)
                    .ToList()
                    .ForEach(type => executableTasks.Add((IExecutableTask)currentAssembly.CreateInstance(type.FullName)));

            executableTasks
                .ForEach(s => Task.Run(() =>
                    {
                        Type type = s.GetType();
                        foreach (MethodInfo method in type.GetMethods())
                        {
                            if (method.DeclaringType.Name == type.Name)
                            {
                                ResetColor();
                                Out.Write($"\nExecuting");

                                ForegroundColor = ConsoleColor.Yellow;
                                Out.Write($" {method.Name} ");

                                ResetColor();
                                Out.Write($"from");

                                ForegroundColor = ConsoleColor.Yellow;
                                Out.WriteLine($" {type.Name}");

                                ResetColor();

                                method.Invoke(s, null);
                            }
                        }
                    }).Wait()
                );
        }


        static void Benchmark(int times)
        {
            long totalMilliSeconds = 0;
            for (int i = 0; i < times; i++) totalMilliSeconds += MeasureAction(Start);

            Clear();
            Out.WriteLineAsync($"Ran services {times} times in {totalMilliSeconds} ms with AVG of {totalMilliSeconds / (times)} ms / itteration").ConfigureAwait(false);
        }

        static long MeasureAction(Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action.Invoke();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }


    }
}
