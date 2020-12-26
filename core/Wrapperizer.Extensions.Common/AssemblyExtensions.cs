
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Wrapperizer.Extensions.Common
{
    public static class AssemblyExtensions
    {
        public static Assembly[] WithReferencedAssemblies(this Assembly assembly)
        {
            var listOfAssemblies = new List<Assembly>();
            listOfAssemblies.Add(assembly);

            listOfAssemblies.AddRange(assembly.GetReferencedAssemblies().Select<AssemblyName, Assembly>(Assembly.Load));
            return listOfAssemblies.ToArray();
        }

        public static Assembly[] WithEntryReferences(this Assembly assembly)
        {
            var listOfAssemblies = new List<Assembly>();

            var directory = Path.GetDirectoryName(assembly.Location);

            var dlls = Directory.EnumerateFiles(directory, "*.dll")
                .Select(Path.GetFileName)
                .Where(file => file.StartsWith("Sample") && file != "Sample.MessageRelay.dll");


            foreach (var dll in dlls)
            {
                var x = Path.Combine(directory, dll);
                var ass = Assembly.LoadFrom(x);
                listOfAssemblies.Add(ass);
            }

            return listOfAssemblies.ToArray();
        }

    }
}
