using System;

namespace Entr.Domain
{
    public abstract class AggregateRoot<TId, TUserId> : 
        Entity<TId>, 
        IInlineAuditedEntity<TUserId>,
        IInlineAuditedEntity
    {
        public TUserId CreatedByUserId { get; protected set; }
        public DateTime CreatedUtcDate { get; protected set; }
        public TUserId ModifiedByUserId { get; protected set; }
        public DateTime ModifiedUtcDate { get; protected set; }

        object IInlineAuditedEntity.CreatedByUserId => CreatedByUserId;
        object IInlineAuditedEntity.ModifiedByUserId => ModifiedByUserId;

        public byte[] RowVersion { get; set; }
    }
}
