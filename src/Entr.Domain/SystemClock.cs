using System;

namespace Entr.Domain;

public class SystemClock : IClock
{
    public DateTimeOffset GetUtcNow()
    {
        return DateTime.UtcNow;
    }
}
