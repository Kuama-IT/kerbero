namespace Kerbero.Common.Interfaces;

public interface Interactor<in TRequest, out TResult>
{
	public TResult Handle(TRequest command);
}
