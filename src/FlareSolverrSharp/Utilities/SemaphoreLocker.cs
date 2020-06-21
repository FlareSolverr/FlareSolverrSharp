using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlareSolverrSharp.Utilities
{
    public class SemaphoreLocker
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task LockAsync<T>(Func<T> worker)
            where T : Task
        {
            await _semaphore.WaitAsync();
            try
            {
                await worker();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}