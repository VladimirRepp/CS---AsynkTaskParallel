using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sample_8
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
            await SampleAsync_1();

            Console.ReadLine();
        }

        #region === Examples of Asynk and Task ===
        private static async Task PrintAsync()
        {
            Console.WriteLine("Начало PrintAsync()");
            // Здесь будет асинхронность
            await Task.Run(() => Console.WriteLine("В работе PrintAsync() ...")); // ~ Task.Run(Print);
            await Task.Delay(1000); // эмитация долгой работы
            Console.WriteLine("Конец PrintAsync()");
        }

        /// <summary>
        /// Пример простого асинхронного метода.
        /// - для await нужен спецификатор async;
        /// - метод async может быть синхронным.
        /// </summary>
        private async static Task SampleAsync_0()
        {
            Console.WriteLine("Начало Main потока");
            await PrintAsync(); // ЖДИ !!!
            Console.WriteLine("Конец Main потока");

            Console.ReadLine();
        }

        /// <summary>
        /// Пример 1 инициализации Task и ожидания
        /// Task - данный класс описывает отдельную задачу, которая запускается асинхронно в потоке из пула потоков. 
        /// </summary>
        /// <returns></returns>
        private async static Task SampleAsync_1()
        {
            // 1 способ
            Task task = new Task(() => {
                Console.WriteLine("Пример запуска Task!");
                Thread.Sleep(1000);
                Console.WriteLine("Пример окончания Task!");
            });
            task.Start();

            // 2 способ инициализировать и запустить 
            Task task1 = Task.Factory.StartNew(() => Console.WriteLine("Пример запуска Task 1!"));

            // 3 способ инициализировать и запустить
            Task task2 = Task.Run(() => Console.WriteLine("Пример запуска Task 2!"));

            Console.WriteLine($"Информация о task ДО ожидания завершения!");
            Console.WriteLine($"task Id: {task.Id}");
            Console.WriteLine($"task IsCompleted: {task.IsCompleted}");
            Console.WriteLine($"task Status: {task.Status}");

            // НЕОБЯЗАТЕЛЬНО главный поток дождется завершения работы задач
            // Для этого используем:
            task.Wait(); // Wait блокирует вызывающий поток
            task1.Wait();
            task2.Wait();

            Console.WriteLine($"Информация о task ПОСЛЕ ожидания завершения!");
            Console.WriteLine($"task Id: {task.Id}");
            Console.WriteLine($"task IsCompleted: {task.IsCompleted}");
            Console.WriteLine($"task Status: {task.Status}");
        }
    
        /// <summary>
        /// Пример 2 инициализации Task и ожидания
        /// </summary>
        /// <returns></returns>
        private async static Task SampleAsync_2()
        {
            var tomTask = PrintNameAsync("Tom");
            Task bobTask = PrintNameAsync("Bob");
            var samTask = PrintNameAsync("Sam");

            // Без ожидания главный поток не дождется выполнения задач
            await tomTask;
            await bobTask;
            await samTask;
        }

        private async static Task PrintNameAsync(string name)
        {
            await Task.Delay(3000);
            Console.WriteLine($"Имя: {name}");
        }
        #endregion

        #region === Examples of Parallel === 
        private static void Print()
        {
            Random rand = new Random();
            Thread.Sleep(rand.Next(1000, 3000));

            Console.WriteLine($"Выполнилась задача: {Task.CurrentId}\nPrint: Hellow Parallel!");
        }

        private static void Square(int a)
        {
            Random rand = new Random();
            Thread.Sleep(rand.Next(1000, 3000));

            Console.WriteLine($"Выполнилась задача: {Task.CurrentId}\n{a} * {a} = {a * a}");
        }

        private static void SquareWithBreak(int a, ParallelLoopState state)
        {
            // Пример выхода из цикла
            if (a == 5)
                state.Break();

            Random rand = new Random();
            Thread.Sleep(rand.Next(1000, 3000));

            Console.WriteLine($"Выполнилась задача: {Task.CurrentId}\n{a} * {a} = {a * a}");
        }

        /// <summary>
        /// Метод Parallel.Invoke принимает в качестве параметров массив объектов Action.
        /// Каждый объект Action запускается в своем потоке.
        /// </summary>
        private static async void SampleParallel_1()
        {
            Parallel.Invoke(
                Print,
                () => {
                    Random rand = new Random();
                    Thread.Sleep(rand.Next(1000, 3000));

                    Console.WriteLine($"Выполнилась задача: {Task.CurrentId}\nSample_1: Hellow Parallel!");
                },
                () => Square(5)
                );

            // ~ !!! //
            /*
            Task task = Task.Run(Print);
            Task task1 = Task.Run(() => {
                Random rand = new Random();
                Thread.Sleep(rand.Next(1000, 3000));

                Console.WriteLine($"Выполнилась задача: {Task.CurrentId}\nSample_1: Hellow Parallel!");
            });
            Task task2 = Task.Run(() => Square(5));

            await task;
            await task1;
            await task2;
            */
        }

        /// <summary>
        /// Parallel.For(int, int, Action<int>) - выполняет итерации цикла паралельно. 
        /// Parallel.ForEach(IEnumerable<T>, Action<T>) - подобно циклу foreach, только перебор выполняет параллельно.
        /// </summary>
        private static void SampleParallel_2()
        {
            Parallel.For(1, 6, Square); // если нужны цифры по порядку

            // Если нужна конкретная последовательность
            ParallelLoopResult result = Parallel.ForEach(
                new List<int> { 1, 2, 9 },
                Square
                );

            // Выход из Parallel цикла
            ParallelLoopResult resultWithBreak = Parallel.For(1, 10, SquareWithBreak);
            if (!resultWithBreak.IsCompleted)
                Console.WriteLine($"Выполение цикла завершено на итерации: {resultWithBreak.LowestBreakIteration}");
        }
        #endregion
    }
}
