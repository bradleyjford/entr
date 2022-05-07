namespace Entr.Domain;

public class SystemClock : IClock
{
    public DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }
}
