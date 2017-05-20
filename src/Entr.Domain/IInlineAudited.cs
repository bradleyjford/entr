using System;

namespace Entr.Domain
{
    public interface IInlineAudited<out TUserId>
    {
        TUserId CreatedByUserId { get; }
        DateTime CreatedUtcDate { get; }
        TUserId ModifiedByUserId { get; }
        DateTime ModifiedUtcDate { get; }
    }
}