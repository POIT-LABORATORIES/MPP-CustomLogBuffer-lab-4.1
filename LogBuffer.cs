using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CustomLogBuffer
{
    public class LogBuffer
    {
        //private List<string> _buffer = new List<string>();
        //private ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        //private BlockingCollection<string> _blockingCollection = new BlockingCollection<string>();
        private BlockingCollection<string> _blockingCollection;
        private int MaxBufferSize
        {
            get;
        }

        public LogBuffer(int num)
        {
            MaxBufferSize = num;
            _blockingCollection = new BlockingCollection<string>(MaxBufferSize);
        }

        public void Add(string item)
        {
            try
            {
                if (_blockingCollection.IsCompleted) return;
                _blockingCollection.Add(item);
                if (_blockingCollection.Count != MaxBufferSize) return;
                var writeToFileTask = Task.Run(WriteToFile);
                Task.WaitAll(writeToFileTask);
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine("The blocking collection has been disposed of, cannot add any more messages!");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void WriteToFile()
        {
            Console.WriteLine("WRITING TO FILE");
            using (StreamWriter streamWriter = new StreamWriter(@"./LogFile.txt", true))
            {
                while (_blockingCollection.Count != 0)
                {
                    string item;
                    while (_blockingCollection.TryTake(out item))
                    {
                        streamWriter.WriteLine(item);
                    }
                }
            }
        }

        public void CompleteJournaling()
        {
            _blockingCollection.CompleteAdding();
            _blockingCollection.Dispose(); // МОЖЕТ ВОЗНИКНУТЬ ОШИБКА!
        }
    }
}