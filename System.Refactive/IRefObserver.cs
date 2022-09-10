namespace System.Refactive;

public interface IRefObserver<T>
{
    void OnNext(ref T value);
    void OnError(Exception error);
    void OnCompleted();
}