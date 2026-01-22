namespace Sentry.Uno.Internal;

internal sealed class Disposer : IDisposable
{
    private readonly List<IDisposable> _disposables = [];

    public void Register(IDisposable disposable)
    {
        ArgumentNullException.ThrowIfNull(disposable);
        _disposables.Add(disposable);
    }

    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
        _disposables.Clear();
    }
}
