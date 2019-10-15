using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace SDLGame
{
	public class ScriptEngine
	{
		public async Task<T> EvaluateAsync<T>(string code, object globals = null)
		{
			try
			{
				return await CSharpScript.EvaluateAsync<T>(code,
				                                           ScriptOptions.Default
				                                                        .WithReferences(typeof(ScriptEngine).Assembly)
				                                                        .WithImports(nameof(SDLGame)),
				                                           globals);
			}
			catch (CompilationErrorException exception)
			{
				throw new Exception(string.Join(Environment.NewLine, exception.Diagnostics));
			}
		}
	}
}