using System;
using Xunit;

namespace Entr.Domain.Tests;

public class EntityIdTests
{
    [Fact]
    public void Test1()
    {
        var id1 = new BradId(3);
        var id2 = new BradId(4);

        Assert.True(id1 == id2);
    }
}

[EntrEntityId<int>]
public class BradId { }

