using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace CSharpCompilerApi;

public class CSharpCompiler
{
    public static CompileRequest codeExapmple = new CompileRequest()
    {
        Code = @"
            using System;

            namespace GG
            {
                public class Program
                {
                    public void Main(string[] args)
                    {

                    }

                    public string MyFunc(object message)
                    {
                        return $""you said {message}"";
                    }
                }
            }",
        ClassPath = "GG.Program",
        MethodName = "MyFunc",
        Parameters = new object[] { "Hello World" }
    };

    public static RunResult RunCSharpCode(CompileRequest request)
    {
        var runResult = new RunResult();
        try
        {
            Console.WriteLine("Let's compile!");
            Console.WriteLine("Parsing the code into the SyntaxTree");
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(request.Code);

            string assemblyName = Path.GetRandomFileName();
            var refPaths = new[] {
                typeof(System.Object).GetTypeInfo().Assembly.Location,
                typeof(Console).GetTypeInfo().Assembly.Location,
                Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location)??"", "System.Runtime.dll")
            };
            MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();

            Console.WriteLine("Adding the following references");
            foreach (var r in refPaths)
                Console.WriteLine(r);

            Console.WriteLine("Compiling ...");
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    Console.WriteLine("Compilation failed!");
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                        runResult.Errors.Add($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                    }
                }
                else
                {
                    Console.WriteLine("Compilation successful! Now instantiating and executing the code ...");
                    ms.Seek(0, SeekOrigin.Begin);

                    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    var type = assembly.GetType(request.ClassPath);
                    var instance = assembly.CreateInstance(request.ClassPath);
                    var meth = type?.GetMember(request.MethodName).First() as MethodInfo;
                    runResult.Output = meth?.Invoke(instance, request.Parameters);
                    runResult.Success = true;
                }
            }
            return runResult;
        }
        catch (Exception e)
        {
            runResult.Errors.Add(e.Message);
            return runResult;
        }
    }

    public static List<RunResult> RunCSharpCodeFromFile(CompileFileRequest request)
    {
        var runResults = new List<RunResult>();
        var code = new StreamReader(request.File.OpenReadStream()).ReadToEnd();
        runResults.Add(RunCSharpCode(new CompileRequest()
        {
            Code = code,
            ClassPath = request.ClassPath,
            MethodName = request.MethodName,
            Parameters = request.Parameters
        }));
        return runResults;
    }
}
