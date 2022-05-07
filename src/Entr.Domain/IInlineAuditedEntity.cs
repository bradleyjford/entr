namespace Entr.Domain;

public interface IInlineAuditedEntity : IInlineAuditedEntity<object>
{
}

public interface IInlineAuditedEntity<out TUserId>
{
    TUserId CreatedByUserId { get; }
    DateTime CreatedUtcDate { get; }
    TUserId ModifiedByUserId { get; }
    DateTime ModifiedUtcDate { get; }
}
