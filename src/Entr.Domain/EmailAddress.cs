using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Entr.Domain;

[TypeConverter(typeof(TypedStringTypeConverter<EmailAddress>))]
[JsonConverter(typeof(TypedStringJsonConverter<EmailAddress>))]
public class EmailAddress : TypedString<EmailAddress>
{
    public EmailAddress(string value)
        : base(value, 254, 3, StringComparison.OrdinalIgnoreCase)
    {
        var pos = value.IndexOf('@');

        if (pos < 1 || pos - 1 == value.Length)
        {
            throw new ArgumentException($"{nameof(EmailAddress)} must contain an '@'", nameof(value));
        }
    }
}
