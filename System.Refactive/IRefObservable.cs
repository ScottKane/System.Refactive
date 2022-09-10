namespace System.Refactive;

public interface IRefObservable<T>
{
    IDisposable Subscribe(IRefObserver<T> observer);
}