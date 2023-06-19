namespace CSharpCompilerApi;

public class RunResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public object? Output { get; set; }
}
