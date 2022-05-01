﻿using VerifyXunit;
using Xunit;

namespace Entr.Domain.SourceGenerators.Tests;

[UsesVerify]
public class EntrEntityIdGeneratorTests
{
    [Fact]
    public Task GeneratesEntityIdClassCorrectly()
    {
        // The source code to test
        var source = @"
using System;
using Entr.Domain;

namespace Entr.Domain.Tests
{
    [EntrEntityId<int>]
    public class UserId {}

    [EntrEntityId<Guid>]
    public class RoleId {}
}
";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }
}