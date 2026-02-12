namespace Blazr.Monad;

public record Container<T>(T Value)
{
    public Container<TResult> Then<TResult>(Func<T, TResult> func)
        => new Container<TResult>(func.Invoke(Value));

    public void Write(Action<T> action)
        => action.Invoke(Value);
}

public static class Container
{
    public static Container<T> Read<T>(Func<T> func)
        => new Container<T>(func.Invoke());
}
