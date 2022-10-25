using FluentResults;

namespace Kerbero.Domain.Common.Interfaces;

public interface Interactor<in TRequest, TResult>
{
	public Result<TResult> Handle(TRequest request);
}

public interface InteractorAsync<in TRequest, TResult>
{
	public Task<Result<TResult>> Handle(TRequest request);
}

public interface InteractorAsync<in TRequest>
{
	public Task<Result> Handle(TRequest request);
}