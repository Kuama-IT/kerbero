using FluentResults;

namespace Kerbero.Common.Interfaces;

public interface Interactor<in TRequest, TResult>
{
	public Result<TResult> Handle(TRequest request);
}

public interface InteractorAsync<in TRequest, TResult>
{
	public Task<Result<TResult>> Handle(TRequest request);
}
