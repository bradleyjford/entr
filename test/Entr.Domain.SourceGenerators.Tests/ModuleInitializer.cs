using System;
using System.Runtime.CompilerServices;
using VerifyTests;

namespace Entr.Domain.SourceGenerators.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
