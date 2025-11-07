namespace Cirreum.Logging.Deferred.Internal;
internal class LogScope<TState>(
	TState state,
	Stack<IDisposable> scopes)
	: ILogScope
	, IDisposable
	where TState : notnull {

	public TState State { get; } = state;

	public object GetState() => this.State;

	public void Dispose() {
		if (scopes.Count > 0) {
			scopes.Pop();
		}
	}

}

internal class CompositeDisposable : IDisposable {

	private readonly List<IDisposable> _disposables = [];

	public void Add(IDisposable disposable) => this._disposables.Add(disposable);

	void IDisposable.Dispose() {
		foreach (var disposable in this._disposables.AsEnumerable().Reverse()) {
			disposable.Dispose();
		}
	}

}