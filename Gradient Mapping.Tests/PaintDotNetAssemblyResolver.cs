using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace pyrochild.effects.gradientmapping.tests
{
    internal static class PaintDotNetAssemblyResolver
    {
        [ModuleInitializer]
        internal static void Initialize()
        {
            string dir = typeof(PaintDotNetAssemblyResolver).Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .First(a => a.Key == "PdnDir").Value!;

            AssemblyLoadContext.Default.Resolving += (context, name) =>
            {
                string path = Path.Combine(dir, name.Name + ".dll");
                return File.Exists(path) ? context.LoadFromAssemblyPath(path) : null;
            };
        }
    }
}
