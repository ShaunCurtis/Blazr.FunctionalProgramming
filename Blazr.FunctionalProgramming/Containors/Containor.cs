/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Containors;

public readonly record struct Containor<T>
{
    private readonly T Value;

    public Containor(T value)
        => this.Value = value;

    public static Containor<T> Read(T value)
        => new Containor<T>(value);

    public T Write()
        => this.Value;

    public static Containor<T> Read(Func<T> func)
        => new Containor<T>(func.Invoke());

    public void Write<TOut>(Action<T> action)
        => action.Invoke(this.Value);

    public Containor<TOut> Map<TOut>(Func<T, TOut> func)
        => new Containor<TOut>(func.Invoke(this.Value));

    public Containor<TOut> Bind<TOut>(Func<T, Containor<TOut>> func)
        => func.Invoke(this.Value);
}

public static class Containor
{
    public static Containor<T> Read<T>(T value)
        => Containor<T>.Read(value);
    public static Containor<T> Read<T>(Func<T> func)
        => Containor<T>.Read(func);
}
