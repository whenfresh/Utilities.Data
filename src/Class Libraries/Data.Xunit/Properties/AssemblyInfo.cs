using System;
using System.Reflection;

[assembly: CLSCompliant(false)]
[assembly: AssemblyDefaultAlias("Cavity.Data.Xunit.dll")]
[assembly: AssemblyTitle("Cavity.Data.Xunit.dll")]

#if (DEBUG)

[assembly: AssemblyDescription("Cavity : xUnit Data Library (Debug)")]

#else

[assembly: AssemblyDescription("Cavity : xUnit Data Library (Release)")]

#endif