using System.Diagnostics.CodeAnalysis;

namespace Maturaball_Tischreservierung.Models;

public interface IServiceResult<T> : IServiceResult
{
    public T? Value { get; }

    [MemberNotNullWhen(false, nameof(Value))]
    public new bool Failed();
}

public interface IServiceResult
{
    public InternalStatusCode Status { get; }

    public dynamic GetErrors();

    public bool Failed();
}