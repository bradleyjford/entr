using System;

namespace Entr.Domain;

public interface IClock
{
    DateTime GetUtcNow();
}
