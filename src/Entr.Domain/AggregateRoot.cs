using System;

namespace Entr.Domain
{
    public abstract class AggregateRoot<TId, TUserId> : Entity<TId>, IInlineAudited<TUserId>
    {
        public TUserId CreatedByUserId { get; protected set; }
        public DateTime CreatedUtcDate { get; protected set; }
        public TUserId ModifiedByUserId { get; protected set; }
        public DateTime ModifiedUtcDate { get; protected set; }

        public byte[] RowVersion { get; set; }
    }
}
