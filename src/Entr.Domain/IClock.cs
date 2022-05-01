using System;

namespace Entr.Domain;

public interface IClock
{
    DateTimeOffset GetUtcNow();
}
