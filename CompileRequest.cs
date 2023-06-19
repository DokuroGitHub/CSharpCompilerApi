namespace CSharpCompilerApi;

public class CompileRequest
{
    public string Code { get; init; } = string.Empty;
    public string ClassPath { get; init; } = "Program";
    public string MethodName { get; init; } = "Main";
    public object?[]? Parameters { get; init; } = Array.Empty<object>();
}
