using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace quick_code_test
{

    //Please fill in the implementation of the service defined below. This service is to keep track
    //of ids to return if they have been seen before. No 3rd party packages can be used and the method
    //must be thread safe to call.

    //create the implementation as efficiently as possible in both locking, memory usage, and cpu usage

    public interface IDuplicateCheckService
    {
        //checks the given id and returns if it is the first time we have seen it
        //IT IS CRITICAL that duplicates are not allowed through this system but false
        //positives can be tolerated at a maximum error rate of less than 1%
        bool IsThisTheFirstTimeWeHaveSeen(int id);

    }

    public class HighlyOptimizedThreadSafeDuplicateCheckService : IDuplicateCheckService
    {
        private readonly ITestOutputHelper output;
        private SpinLockedBitArray64[] _iDuplicateCheckService;
        public HighlyOptimizedThreadSafeDuplicateCheckService(ITestOutputHelper output)
        {
            this.output = output;
            var partitions = (Int32.MaxValue / 64) + 1;
            //Used bitmaps to check if I already have the id, using a bit representing each number. Fast and smallest representation
            //it's using SpinLock to synchronize
            _iDuplicateCheckService = new SpinLockedBitArray64[partitions];
        }


        [Fact]
        public void IsThisTheFirstTimeWeHaveSeenTest()
        {
            int itemCount = 100_000_000;//
            var rand = new Random();

            object duplicatedLock = new object();
            ulong totalDuplicates = 0;
            int concurrentTaksToTest = 4;

            var tasks = new List<Task>();
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < concurrentTaksToTest; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    ulong duplicatedCount = 0;
                    for (int i = 0; i < itemCount; i++)
                    {
                        var isduplicated = IsThisTheFirstTimeWeHaveSeen(i);
                        if (isduplicated)
                            duplicatedCount++;
                    }
                    lock (duplicatedLock)
                    {
                        totalDuplicates += duplicatedCount;
                    }
                }));
            }

            Task.WhenAll(tasks).Wait();
            watch.Stop();

            ulong expectedDuplicated = (ulong)itemCount * (ulong)(concurrentTaksToTest - 1);
            Assert.Equal(expectedDuplicated, totalDuplicates);
            ulong totalQueries = (ulong)itemCount * (ulong)concurrentTaksToTest;
            var throuput = Math.Floor(totalQueries / watch.Elapsed.TotalSeconds);

            if (output != null)
            {
                //Performed 400.000.000 calls from 4 concurrent Tasks with 35.489.308 request per second for 100.000.000 unique ids
                var detail = $"Performed {totalQueries:N0} calls from {concurrentTaksToTest:N0} concurrent Tasks with {throuput:N0} request per second for {itemCount:N0} unique ids";

                output.WriteLine(detail);
            }
        }

        [Fact]
        public async Task Test4TasksSequentialNumbers()
        {
            var _tester = this;
            var itemCount = int.MaxValue;//
            object duplicatedLock = new object();
            ulong totalDuplicates = 0;
            int concurrentTaksToTest = 4;

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
            if (output != null)
            {
                //Performed 8.589.934.588 calls from 4 concurrent Tasks with 34.191.768 request per second for 2.147.483.647 unique ids
                var detail = $"Performed {totalQueries:N0} calls from {concurrentTaksToTest:N0} concurrent Tasks with {throuput:N0} request per second for {itemCount:N0} unique ids";
                output.WriteLine(detail);
            }
            Assert.Equal(expectedDuplicated, totalDuplicates);
        }

        [Fact]
        public async Task Test4TasksRandomNumbers()
        {
            var _tester = this;
            var itemCount = int.MaxValue;
            object duplicatedLock = new object();
            ulong totalDuplicates = 0;
            int concurrentTaksToTest = 4;

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
            if (output != null)
            {
                //Performed 8.589.934.588 calls from 4 concurrent Tasks with 10.802.236 request per second for 2.147.483.647 unique ids
                var detail = $"Performed {totalQueries:N0} calls from {concurrentTaksToTest:N0} concurrent Tasks with {throuput:N0} request per second for {itemCount:N0} unique ids";

                output.WriteLine(detail);
            }
        }
        public bool IsThisTheFirstTimeWeHaveSeen(int id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException("id");
            }

            //with this i have O(5) + locking and unlocking as max complexity
            var partitionToUse = id / 64;
            var item = id % 64;

            return _iDuplicateCheckService[partitionToUse].IsThisTheFirstTimeWeHaveSeen(item);
        }
    }

    public struct SpinLockedBitArray64 : IDuplicateCheckService
    {
        private BitArray64 _bitVector;
        private SpinLock _spinlock;
        

        public bool IsThisTheFirstTimeWeHaveSeen(int id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException("id");
            }
            //Since the critical sections is really small and fast, I used SpinLock which doesn't put the thread to sleep, 
            //so no expensive context switching is needed.
            //By locking here I get fine granulated sync so performs better for parallel
            bool lockTaken = false;
            _spinlock.Enter(ref lockTaken);

            var found = _bitVector[id];
            if (!found)
            {
                _bitVector[id] = true;
            }

            if (lockTaken) _spinlock.Exit(false);
            return found;
        }
    }
}