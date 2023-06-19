namespace CSharpCompilerApi;

#pragma warning disable 
public class CompileFileRequest
{
    public IFormFile File { get; init; }
    public string ClassPath { get; init; }
    public string MethodName { get; init; }
    public object?[]? Parameters { get; init; }
}
