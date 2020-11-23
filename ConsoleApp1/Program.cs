using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using quick_code_test;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BenchClass>();
        }
    }

    [ShortRunJob]
    [MemoryDiagnoser]
    public class BenchClass
    {
        
        public BenchClass()
        {
        }

        [Benchmark]
        public async Task BenchIntMaxValueIdsWith10TasksSequentialNumbers()
        {
            var _tester = new HighlyOptimizedThreadSafeDuplicateCheckService(null);
            var itemCount = int.MaxValue;//
            object duplicatedLock = new object();
            ulong totalDuplicates = 0;
            int concurrentTaksToTest = 10;

            var tasks = new List<Task>();
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < concurrentTaksToTest; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    ulong duplicatedCount = 0;
                    for (int i = 0; i < itemCount; i++)
                    {
                        var isduplicated = _tester.IsThisTheFirstTimeWeHaveSeen(i);
                        if (isduplicated)
                            duplicatedCount++;
                    }
                    lock (duplicatedLock)
                    {
                        totalDuplicates += duplicatedCount;
                    }
                }));
            }

            await Task.WhenAll(tasks);
            watch.Stop();

            ulong expectedDuplicated = (ulong)itemCount * (ulong)(concurrentTaksToTest - 1);
            ulong totalQueries = (ulong)itemCount * (ulong)concurrentTaksToTest;
            var throuput = Math.Floor(totalQueries / watch.Elapsed.TotalSeconds);
        }


        [Benchmark]
        public async Task BenchIntMaxValueIdsWith100TasksSequentialNumbers()
        {
            var _tester = new HighlyOptimizedThreadSafeDuplicateCheckService(null);
            var itemCount = int.MaxValue;//
            object duplicatedLock = new object();
            ulong totalDuplicates = 0;
            int concurrentTaksToTest = 100;

            var tasks = new List<Task>();
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < concurrentTaksToTest; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    ulong duplicatedCount = 0;
                    for (int i = 0; i < itemCount; i++)
                    {
                        var isduplicated = _tester.IsThisTheFirstTimeWeHaveSeen(i);
                        if (isduplicated)
                            duplicatedCount++;
                    }
                    lock (duplicatedLock)
                    {
                        totalDuplicates += duplicatedCount;
                    }
                }));
            }

            await Task.WhenAll(tasks);
            watch.Stop();
            ulong expectedDuplicated = (ulong)itemCount * (ulong)(concurrentTaksToTest - 1);
            ulong totalQueries = (ulong)itemCount * (ulong)concurrentTaksToTest;
            var throuput = Math.Floor(totalQueries / watch.Elapsed.TotalSeconds);
        }


        [Benchmark]
        public async Task BenchIntMaxValueIdsWith10TasksRandomNumbers()
        {
            var _tester = new HighlyOptimizedThreadSafeDuplicateCheckService(null);
            var itemCount = int.MaxValue;//
            object duplicatedLock = new object();
            ulong totalDuplicates = 0;
            int concurrentTaksToTest = 10;

            var tasks = new List<Task>();
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < concurrentTaksToTest; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var random = new Random();
                    ulong duplicatedCount = 0;
                    for (int i = 0; i < itemCount; i++)
                    {
                        var item = random.Next(0, int.MaxValue);
                        var isduplicated = _tester.IsThisTheFirstTimeWeHaveSeen(item);
                        if (isduplicated)
                            duplicatedCount++;
                    }
                    lock (duplicatedLock)
                    {
                        totalDuplicates += duplicatedCount;
                    }
                }));
            }

            await Task.WhenAll(tasks);
            watch.Stop();

            ulong expectedDuplicated = (ulong)itemCount * (ulong)(concurrentTaksToTest - 1);
            ulong totalQueries = (ulong)itemCount * (ulong)concurrentTaksToTest;
            var throuput = Math.Floor(totalQueries / watch.Elapsed.TotalSeconds);
        }


        [Benchmark]
        public async Task BenchIntMaxValueIdsWith100TasksRandomNumbers()
        {
            var _tester = new HighlyOptimizedThreadSafeDuplicateCheckService(null);
            var itemCount = int.MaxValue;//
            object duplicatedLock = new object();
            ulong totalDuplicates = 0;
            int concurrentTaksToTest = 100;

            var tasks = new List<Task>();
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < concurrentTaksToTest; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var random = new Random();
                    ulong duplicatedCount = 0;
                    for (int i = 0; i < itemCount; i++)
                    {
                        var item = random.Next(0, int.MaxValue);
                        var isduplicated = _tester.IsThisTheFirstTimeWeHaveSeen(item);
                        if (isduplicated)
                            duplicatedCount++;
                    }
                    lock (duplicatedLock)
                    {
                        totalDuplicates += duplicatedCount;
                    }
                }));
            }

            await Task.WhenAll(tasks);
            watch.Stop();

            ulong expectedDuplicated = (ulong)itemCount * (ulong)(concurrentTaksToTest - 1);
            ulong totalQueries = (ulong)itemCount * (ulong)concurrentTaksToTest;
            var throuput = Math.Floor(totalQueries / watch.Elapsed.TotalSeconds);
        }
    }
}
