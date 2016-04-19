using System;
using System.Reflection;

[assembly: CLSCompliant(false)]
[assembly: AssemblyDefaultAlias("Cavity.Data.Xunit.Facts.dll")]
[assembly: AssemblyTitle("Cavity.Data.Xunit.Facts.dll")]

#if (DEBUG)

[assembly: AssemblyDescription("Cavity : xUnit Data Facts Library (Debug)")]

#else

[assembly: AssemblyDescription("Cavity : xUnit Data Facts Library (Release)")]

#endif