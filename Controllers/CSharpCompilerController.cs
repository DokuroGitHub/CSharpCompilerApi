using Microsoft.AspNetCore.Mvc;

namespace CSharpCompilerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CSharpCompilerController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult Test()
    => Ok(CSharpCompiler.RunCSharpCode(CSharpCompiler.codeExapmple));

    [HttpGet("example")]
    public IActionResult Example()
    => Ok(CSharpCompiler.codeExapmple);

    [HttpPost("compile")]
    public IActionResult Compile([FromBody] CompileRequest request)
    => Ok(CSharpCompiler.RunCSharpCode(request));

    [HttpPost("compile-file")]
    public IActionResult CompileFile([FromForm] CompileFileRequest request)
    => Ok(CSharpCompiler.RunCSharpCodeFromFile(request));
}
