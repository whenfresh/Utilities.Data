using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

[assembly: CLSCompliant(true)]
[assembly: AssemblyDefaultAlias("Cavity.Data.dll")]
[assembly: AssemblyTitle("Cavity.Data.dll")]

#if (DEBUG)

[assembly: AssemblyDescription("Cavity : Data Library (Debug)")]

#else

[assembly: AssemblyDescription("Cavity : Data Library (Release)")]

#endif

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Cavity.Data", Justification = "This is a root namespace.")]