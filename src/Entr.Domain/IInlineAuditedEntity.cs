using System;

namespace Entr.Domain
{
    public interface IInlineAuditedEntity
    {
        object CreatedByUserId { get; }
        object ModifiedByUserId { get; }
    }

    public interface IInlineAuditedEntity<out TUserId>
    {
        TUserId CreatedByUserId { get; }
        DateTime CreatedUtcDate { get; }
        TUserId ModifiedByUserId { get; }
        DateTime ModifiedUtcDate { get; }
    }
}
