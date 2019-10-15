using System;
using System.Collections.Generic;
using Lunatics.Framework.Graphics;
using Lunatics.Mathematics;

namespace Lunatics.Framework.DesktopGL.Graphics
{
	internal sealed class OpenGLShader : Shader
	{
		public override void SetMatrix4(string name, ref Matrix matrix)
		{
			((OpenGLGraphicsDevice) GraphicsDevice).ActivateShaderProgram();
			var matrixId = OpenGL.GL.GetUniformLocation(_programHandle, name);
			OpenGL.GL.UniformMatrix4(matrixId, 1, true, ref matrix);
		}

		internal OpenGLShader(GraphicsDevice graphicsDevice, 
		                      ShaderStage stage,
		                      string glslCode) 
			: base(graphicsDevice, stage)
		{
			_glslCode = glslCode;
		}
		
		internal int GetShaderHandle()
		{
			if (_shaderHandle != -1)
				return _shaderHandle;

			_shaderHandle = OpenGL.GL.CreateShader(Stage == ShaderStage.Vertex
				                                       ? OpenGL.ShaderType.VertexShader
				                                       : OpenGL.ShaderType.FragmentShader);
			OpenGL.GL.CheckError();

			OpenGL.GL.ShaderSource(_shaderHandle, _glslCode);
			OpenGL.GL.CheckError();

			OpenGL.GL.CompileShader(_shaderHandle);
			OpenGL.GL.CheckError();

			OpenGL.GL.GetShader(_shaderHandle, OpenGL.ShaderParameter.CompileStatus, out var compiled);
			OpenGL.GL.CheckError();
			
			if (compiled != 1/*(int)Bool.True*/)
			{
				var log = OpenGL.GL.GetShaderInfoLog(_shaderHandle);
				System.Diagnostics.Debug.WriteLine(log);

				// TODO: GraphicsDevice.DisposeShader(_shaderHandle);
				OpenGL.GL.DeleteShader(_shaderHandle);
				_shaderHandle = -1;

				throw new InvalidOperationException("Shader Compilation Failed");
			}

			return _shaderHandle;
		}

		internal void GetVertexAttributeLocations(int program)
		{
			_programHandle = program;

			//for (int i = 0; i < Attributes.Length; ++i)
			//{
			//	Attributes[i].location = OpenGL.GL.GetAttribLocation(program, Attributes[i].name);
			//	OpenGL.GL.CheckError();
			//}
		}

		internal int GetAttribLocation(VertexElementUsage usage, int index)
		{
			//for (int i = 0; i < Attributes.Length; ++i)
			//{
			//	if ((Attributes[i].usage == usage) && (Attributes[i].index == index))
			//		return Attributes[i].location;
			//}

			return -1;
		}

		internal void ApplySamplerTextureUnits(int program)
		{
			//// Assign the texture unit index to the sampler uniforms.
			//foreach (var sampler in Samplers)
			//{
			//	var loc = OpenGL.GL.GetUniformLocation(program, sampler.name);
			//	OpenGL.GL.CheckError();
			//	if (loc != -1)
			//	{
			//		OpenGL.GL.Uniform1(loc, sampler.textureSlot);
			//		OpenGL.GL.CheckError();
			//	}
			//}
		}

		
		private string _glslCode;
		private int _shaderHandle = -1;
		private int _programHandle = -1;
	}

	internal class ShaderProgram
	{
		public readonly int Program;

		private readonly Dictionary<string, int> _uniformLocations = new Dictionary<string, int>();

		public ShaderProgram(int program)
		{
			Program = program;
		}

		public int GetUniformLocation(string name)
		{
			if (_uniformLocations.ContainsKey(name))
				return _uniformLocations[name];

			var location = OpenGL.GL.GetUniformLocation(Program, name);
			OpenGL.GL.CheckError();
			_uniformLocations[name] = location;
			return location;
		}
	}

	internal class ShaderProgramCache : IDisposable
	{
		private readonly Dictionary<int, ShaderProgram> _programCache = new Dictionary<int, ShaderProgram>();
		readonly GraphicsDevice _graphicsDevice;
		bool disposed;

		public ShaderProgramCache(GraphicsDevice graphicsDevice)
		{
			_graphicsDevice = graphicsDevice;
		}

		~ShaderProgramCache()
		{
			Dispose(false);
		}

		/// <summary>
		/// Clear the program cache releasing all shader programs.
		/// </summary>
		public void Clear()
		{
			// TODO: ...
			//foreach (var pair in _programCache)
			//{
			//	_graphicsDevice.DisposeProgram(pair.Value.Program);
			//}
			_programCache.Clear();
		}

		public ShaderProgram GetProgram(Shader vertexShader, Shader pixelShader)
		{
			// TODO: We should be hashing in the mix of constant 
			// buffers here as well.  This would allow us to optimize
			// setting uniforms to only when a constant buffer changes.

			var key = 1; // TODO: vertexShader.HashKey | pixelShader.HashKey;
			if (!_programCache.ContainsKey(key))
			{
				// the key does not exist so we need to link the programs
				_programCache.Add(key, Link((OpenGLShader)vertexShader, (OpenGLShader)pixelShader));
			}

			return _programCache[key];
		}

		private ShaderProgram Link(OpenGLShader vertexShader, OpenGLShader pixelShader)
		{
			// NOTE: No need to worry about background threads here
			// as this is only called at draw time when we're in the
			// main drawing thread.
			var program = OpenGL.GL.CreateProgram();
			OpenGL.GL.CheckError();

			OpenGL.GL.AttachShader(program, vertexShader.GetShaderHandle());
			OpenGL.GL.CheckError();

			OpenGL.GL.AttachShader(program, pixelShader.GetShaderHandle());
			OpenGL.GL.CheckError();

			//vertexShader.BindVertexAttributes(program);

			OpenGL.GL.LinkProgram(program);
			OpenGL.GL.CheckError();

			OpenGL.GL.UseProgram(program);
			OpenGL.GL.CheckError();

			vertexShader.GetVertexAttributeLocations(program);
			pixelShader.ApplySamplerTextureUnits(program);

			OpenGL.GL.GetProgram(program, OpenGL.GetProgramParameterName.LinkStatus, out var linked);
			// TODO: GraphicsExtensions.LogGLError("VertexShaderCache.Link(), GL.GetProgram");
			if (linked == 0/*(int)Bool.False*/)
			{
				var log = OpenGL.GL.GetProgramInfoLog(program);
				Console.WriteLine(log);
				OpenGL.GL.DetachShader(program, vertexShader.GetShaderHandle());
				OpenGL.GL.DetachShader(program, pixelShader.GetShaderHandle());
				
				// TODO: _graphicsDevice.DisposeProgram(program);

				throw new InvalidOperationException("Unable to link effect program");
			}

			OpenGL.GL.GetProgram(program, OpenGL.GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

			return new ShaderProgram(program);
		}


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
					Clear();
				disposed = true;
			}
		}
	}
}