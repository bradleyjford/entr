using System.Runtime.CompilerServices;

namespace Entr.Data.EntityFramework.Generators.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
