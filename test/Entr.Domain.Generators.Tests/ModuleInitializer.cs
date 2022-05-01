using System.Runtime.CompilerServices;

namespace Entr.Domain.Generators.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
