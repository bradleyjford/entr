namespace Entr.Domain;

public abstract class AggregateRoot<TId, TUserId> :
    Entity<TId>,
    IInlineAuditedEntity<TUserId>
{
    public TUserId CreatedByUserId { get; } = default!;
    public DateTime CreatedUtcDate { get; }
    public TUserId ModifiedByUserId { get; } = default!;
    public DateTime ModifiedUtcDate { get; }
}
