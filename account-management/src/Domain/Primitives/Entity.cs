using System.Diagnostics;

namespace PlatformPlatform.AccountManagement.Domain.Primitives;

public interface IEntity
{
}

public abstract class Entity : Entity<long>
{
    protected Entity() : base(IdGenerator.NewId())
    {
    }
}

[DebuggerDisplay("Identity = {" + nameof(Id) + "}")]
public abstract class Entity<T> : IEntity, IEquatable<Entity<T>> where T : IComparable
{
    protected Entity(T id)
    {
        Id = id;
    }

    public required T Id { get; init; }

    public bool Equals(Entity<T>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<T>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Entity<T>) obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Id);
    }

    public static bool operator ==(Entity<T>? a, Entity<T>? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(Entity<T>? a, Entity<T>? b)
    {
        return !(a == b);
    }
}