using System;

namespace TNVS.SimpleInjector.Modularization
{
    public sealed class AssemblyLoadException : Exception
    {
        public AssemblyLoadException(string assemblyPath, Exception innerException) : base($"Could not load assembly from path '{assemblyPath}'.", innerException)
        {
            AssemblyPath = assemblyPath;
        }

        public string AssemblyPath { get; }
    }
}
