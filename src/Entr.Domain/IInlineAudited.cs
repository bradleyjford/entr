using System;

namespace Entr.Domain
{
    public interface IInlineAuditedEntity<out TUserId>
    {
        TUserId CreatedByUserId { get; }
        DateTime CreatedUtcDate { get; }
        TUserId ModifiedByUserId { get; }
        DateTime ModifiedUtcDate { get; }
    }
}
