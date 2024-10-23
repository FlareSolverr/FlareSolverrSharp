// Author: Deci | Project: FlareSolverrSharp | Name: IFlaresolverrResponseStorage.cs
// Date: 2024/10/23 @ 17:10:02

using System.Threading.Tasks;

namespace FlareSolverrSharp.Types;

public interface IFlaresolverrResponseStorage
{

	Task SaveAsync(FlareSolverrResponse result);

	Task<FlareSolverrResponse?> LoadAsync();

}

public class DefaultFlaresolverrResponseStorage : IFlaresolverrResponseStorage
{

	public Task<FlareSolverrResponse?> LoadAsync()
	{
		return Task.FromResult<FlareSolverrResponse?>(null);
	}

	public Task SaveAsync(FlareSolverrResponse result)
	{
		return Task.CompletedTask;
	}

}