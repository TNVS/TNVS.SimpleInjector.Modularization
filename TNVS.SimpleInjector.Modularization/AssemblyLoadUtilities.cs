using System;
using System.IO;
using System.Reflection;

namespace TNVS.SimpleInjector.Modularization
{
    public static class AssemblyLoadUtilities
    {
        public static void LoadAssembliesFromFolder(string path, bool recurse = false)
        {
            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
                throw new ArgumentException($"The path '{path}' does not exist.");

            foreach (var assemblyFile in directory.GetFiles("*.dll", recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                try
                {
                    Assembly.LoadFile(assemblyFile.FullName);
                }
                catch (Exception ex)
                {
                    throw new AssemblyLoadException(assemblyFile.FullName, ex);
                }
            }
        }
    }
}
