using System.Threading.Tasks;
using FlareSolverrSharp.Types;

namespace FlareSolverrSharp
{
    public interface IFlaresolverrResponseStorage
    {
        Task SaveAsync(FlareSolverrResponse result);
        Task<FlareSolverrResponse> LoadAsync();
    }

    public class DefaultFlaresolverrResponseStorage : IFlaresolverrResponseStorage
    {
        public Task<FlareSolverrResponse> LoadAsync()
        {
            return Task.FromResult<FlareSolverrResponse>(null);
        }

        public Task SaveAsync(FlareSolverrResponse result)
        {
            return Task.CompletedTask;
        }
    }
}
