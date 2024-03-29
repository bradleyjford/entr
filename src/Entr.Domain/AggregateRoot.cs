﻿namespace Entr.Domain;

public abstract class AggregateRoot<TId, TUserId> : 
    Entity<TId>, 
    IInlineAuditedEntity<TUserId>
{
    public TUserId CreatedByUserId { get; protected set; } = default!;
    public DateTime CreatedUtcDate { get; protected set; }
    public TUserId ModifiedByUserId { get; protected set; } = default!;
    public DateTime ModifiedUtcDate { get; protected set; }
    
    public byte[] RowVersion { get; set; } = default!;
}
