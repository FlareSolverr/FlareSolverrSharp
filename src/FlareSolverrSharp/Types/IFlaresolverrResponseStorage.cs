// Author: Deci | Project: FlareSolverrSharp | Name: IFlareSolverrResponseStorage.cs
// Date: 2024/10/23 @ 17:10:02

using System.Threading.Tasks;

namespace FlareSolverrSharp.Types;

public interface IFlareSolverrResponseStorage
{

	Task SaveAsync(FlareSolverrResponse result);

	Task<FlareSolverrResponse?> LoadAsync();

}

public class DefaultFlareSolverrResponseStorage : IFlareSolverrResponseStorage
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