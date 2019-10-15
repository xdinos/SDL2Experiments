using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Lunatics.Mathematics;

namespace Lunatics.Framework.Graphics
{
	/// <summary>
	/// Defines sprite visual options for mirroring.
	/// </summary>
	[Flags]
	public enum SpriteEffects
	{
		/// <summary>
		/// No options specified.
		/// </summary>
		None = 0,
		/// <summary>
		/// Render the sprite reversed along the X axis.
		/// </summary>
		FlipHorizontally = 1,
		/// <summary>
		/// Render the sprite reversed along the Y axis.
		/// </summary>
		FlipVertically = 2
	}
	
	public class SpriteBatch : GraphicsResource
	{
		public SpriteBatch(GraphicsDevice graphicsDevice)
		{
			GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

			vertexInfo = new VertexPositionColorTexture4[MaxSprites];
			textureInfo = new Texture2D[MaxSprites];
			spriteInfos = new SpriteInfo[MaxSprites];
			sortedSpriteInfos = new IntPtr[MaxSprites];
			_vertexBuffer = GraphicsDevice.CreateVertexBuffer(typeof(VertexPositionColorTexture),
			                                                  MaxVertices,
			                                                  BufferUsage.WriteOnly,
			                                                  true);
			_indexBuffer = GraphicsDevice.CreateIndexBuffer(IndexElementSize.SixteenBits,
			                                                MaxIndices,
			                                                BufferUsage.WriteOnly,
			                                                false);
			_indexBuffer.SetData(indexData);

			//_spriteEffect = new SpriteEffect(graphicsDevice);

			//_spriteMatrixTransform = _spriteEffect.Parameters["MatrixTransform"].values;
			//_spriteEffectPass = _spriteEffect.CurrentTechnique.Passes[0];

			_beginCalled = false;
			_numSprites = 0;
		}

		public void Begin()
		{
			Begin(SpriteSortMode.Deferred, 
			      BlendState.AlphaBlend, 
			      SamplerState.LinearClamp, 
			      DepthStencilState.None,
			      RasterizerState.CullCounterClockwise, 
			      null, 
			      Matrix.Identity);
		}

		public void Begin(SpriteSortMode sortMode, BlendState blendState)
		{
			Begin(sortMode,
			      blendState,
			      SamplerState.LinearClamp,
			      DepthStencilState.None,
			      RasterizerState.CullCounterClockwise,
			      null,
			      Matrix.Identity);
		}

		public void Begin(SpriteSortMode sortMode,
		                  BlendState blendState,
		                  SamplerState samplerState,
						  DepthStencilState depthStencilState,
		                  RasterizerState rasterizerState,
		                  Effect effect,
		                  Matrix transformationMatrix)
		{
			if (_beginCalled)
				throw new InvalidOperationException(
					"Begin cannot be called again until End has been successfully called.");

			_sortMode = sortMode;
			_blendState = blendState ?? BlendState.AlphaBlend;
			_samplerState = samplerState ?? SamplerState.LinearClamp;
			_depthStencilState = depthStencilState ?? DepthStencilState.None;
			_rasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;

			_effect = effect;
			_transformationMatrix = transformationMatrix;

			_beginCalled = true;

			if (_sortMode == SpriteSortMode.Immediate)
				Setup();
		}

		public void End()
		{
			if (!_beginCalled)
				throw new InvalidOperationException("Begin must be called before calling End.");

			_beginCalled = false;

			if (_sortMode != SpriteSortMode.Immediate)
				Setup();

			DrawBatch();
		}

		public void Draw(Texture2D texture, Vector2 position, Color color)
		{
			CheckBegin("Draw");
			PushSprite(texture,
			           0.0f, 0.0f,
			           1.0f, 1.0f,
			           position.X, position.Y,
			           texture.Width, texture.Height,
			           color,
			           0.0f, 0.0f,
			           0.0f, 1.0f,
			           0.0f,
			           0);
		}

		public void Draw(Texture2D texture,
		                 Vector2 position,
		                 Rectangle? sourceRectangle,
		                 Color color)
		{
			float sourceX, sourceY, sourceW, sourceH;
			float destW, destH;
			if (sourceRectangle.HasValue)
			{
				sourceX = sourceRectangle.Value.X / (float)texture.Width;
				sourceY = sourceRectangle.Value.Y / (float)texture.Height;
				sourceW = sourceRectangle.Value.Width / (float)texture.Width;
				sourceH = sourceRectangle.Value.Height / (float)texture.Height;
				destW = sourceRectangle.Value.Width;
				destH = sourceRectangle.Value.Height;
			}
			else
			{
				sourceX = 0.0f;
				sourceY = 0.0f;
				sourceW = 1.0f;
				sourceH = 1.0f;
				destW = texture.Width;
				destH = texture.Height;
			}
			CheckBegin("Draw");
			PushSprite(texture,
			           sourceX, sourceY, 
			           sourceW, sourceH,
			           position.X, position.Y,
			           destW, destH,
			           color,
			           0.0f, 0.0f,
			           0.0f, 1.0f,
			           0.0f,
			           0);
		}

		public void Draw(Texture2D texture,
		                 Vector2 position,
		                 Rectangle? sourceRectangle,
		                 Color color,
		                 float rotation,
		                 Vector2 origin,
		                 float scale,
		                 SpriteEffects effects,
		                 float layerDepth)
		{
			CheckBegin("Draw");
			float sourceX, sourceY, sourceW, sourceH;
			var destW = scale;
			var destH = scale;
			if (sourceRectangle.HasValue)
			{
				sourceX = sourceRectangle.Value.X / (float) texture.Width;
				sourceY = sourceRectangle.Value.Y / (float) texture.Height;
				sourceW = Math.Sign(sourceRectangle.Value.Width) *
				          Math.Max(Math.Abs(sourceRectangle.Value.Width), MathUtils.MachineEpsilonFloat) /
				          (float) texture.Width;
				sourceH = Math.Sign(sourceRectangle.Value.Height) *
				          Math.Max(Math.Abs(sourceRectangle.Value.Height), MathUtils.MachineEpsilonFloat) /
				          (float) texture.Height;
				destW *= sourceRectangle.Value.Width;
				destH *= sourceRectangle.Value.Height;
			}
			else
			{
				sourceX = 0.0f;
				sourceY = 0.0f;
				sourceW = 1.0f;
				sourceH = 1.0f;
				destW *= texture.Width;
				destH *= texture.Height;
			}

			PushSprite(texture,
			           sourceX, sourceY,
			           sourceW, sourceH,
			           position.X, position.Y,
			           destW, destH,
			           color,
			           origin.X / sourceW / (float) texture.Width, origin.Y / sourceH / (float) texture.Height,
			           (float) Math.Sin(rotation), (float) Math.Cos(rotation),
			           layerDepth,
			           (byte) (effects & (SpriteEffects) 0x03));
		}

		public void Draw(Texture2D texture,
		                 Vector2 position,
		                 Rectangle? sourceRectangle,
		                 Color color,
		                 float rotation,
		                 Vector2 origin,
		                 Vector2 scale,
		                 SpriteEffects effects,
		                 float layerDepth)
		{
			CheckBegin("Draw");
			float sourceX, sourceY, sourceW, sourceH;
			if (sourceRectangle.HasValue)
			{
				sourceX = sourceRectangle.Value.X / (float) texture.Width;
				sourceY = sourceRectangle.Value.Y / (float) texture.Height;
				sourceW = Math.Sign(sourceRectangle.Value.Width) *
				          Math.Max(Math.Abs(sourceRectangle.Value.Width), MathUtils.MachineEpsilonFloat) /
				          (float) texture.Width;
				sourceH = Math.Sign(sourceRectangle.Value.Height) *
				          Math.Max(Math.Abs(sourceRectangle.Value.Height), MathUtils.MachineEpsilonFloat) /
				          (float) texture.Height;
				scale.X *= sourceRectangle.Value.Width;
				scale.Y *= sourceRectangle.Value.Height;
			}
			else
			{
				sourceX = 0.0f;
				sourceY = 0.0f;
				sourceW = 1.0f;
				sourceH = 1.0f;
				scale.X *= texture.Width;
				scale.Y *= texture.Height;
			}
			PushSprite(
				texture,
				sourceX,
				sourceY,
				sourceW,
				sourceH,
				position.X,
				position.Y,
				scale.X,
				scale.Y,
				color,
				origin.X / sourceW / (float)texture.Width,
				origin.Y / sourceH / (float)texture.Height,
				(float)Math.Sin(rotation),
				(float)Math.Cos(rotation),
				layerDepth,
				(byte)(effects & (SpriteEffects)0x03)
			);
		}

		public void Draw(Texture2D texture,
		                 Rectangle destinationRectangle,
		                 Color color)
		{
			CheckBegin("Draw");
			PushSprite(texture,
			           0.0f, 0.0f,
			           1.0f, 1.0f,
			           destinationRectangle.X, destinationRectangle.Y,
			           destinationRectangle.Width, destinationRectangle.Height,
			           color,
			           0.0f, 0.0f,
			           0.0f,1.0f,
			           0.0f,
			           0);
		}

		public void Draw(Texture2D texture,
		                 Rectangle destinationRectangle,
		                 Rectangle? sourceRectangle,
		                 Color color)
		{
			CheckBegin("Draw");
			float sourceX, sourceY, sourceW, sourceH;
			if (sourceRectangle.HasValue)
			{
				sourceX = sourceRectangle.Value.X / (float) texture.Width;
				sourceY = sourceRectangle.Value.Y / (float) texture.Height;
				sourceW = sourceRectangle.Value.Width / (float) texture.Width;
				sourceH = sourceRectangle.Value.Height / (float) texture.Height;
			}
			else
			{
				sourceX = 0.0f;
				sourceY = 0.0f;
				sourceW = 1.0f;
				sourceH = 1.0f;
			}

			PushSprite(texture,
			           sourceX, sourceY,
			           sourceW, sourceH,
			           destinationRectangle.X, destinationRectangle.Y,
			           destinationRectangle.Width, destinationRectangle.Height,
			           color,
			           0.0f, 0.0f,
			           0.0f, 1.0f,
			           0.0f,
			           0);
		}

		public void Draw(Texture2D texture,
		                 Rectangle destinationRectangle,
		                 Rectangle? sourceRectangle,
		                 Color color,
		                 float rotation,
		                 Vector2 origin,
		                 SpriteEffects effects,
		                 float layerDepth
		)
		{
			CheckBegin("Draw");
			float sourceX, sourceY, sourceW, sourceH;
			if (sourceRectangle.HasValue)
			{
				sourceX = sourceRectangle.Value.X / (float) texture.Width;
				sourceY = sourceRectangle.Value.Y / (float) texture.Height;
				sourceW = Math.Sign(sourceRectangle.Value.Width) *
				          Math.Max(Math.Abs(sourceRectangle.Value.Width), MathUtils.MachineEpsilonFloat) /
				          (float) texture.Width;
				sourceH = Math.Sign(sourceRectangle.Value.Height) *
				          Math.Max(Math.Abs(sourceRectangle.Value.Height), MathUtils.MachineEpsilonFloat) /
				          (float) texture.Height;
			}
			else
			{
				sourceX = 0.0f;
				sourceY = 0.0f;
				sourceW = 1.0f;
				sourceH = 1.0f;
			}

			PushSprite(texture,
			           sourceX, sourceY,
			           sourceW, sourceH,
			           destinationRectangle.X, destinationRectangle.Y,
			           destinationRectangle.Width, destinationRectangle.Height,
			           color,
			           origin.X / sourceW / (float) texture.Width, origin.Y / sourceH / (float) texture.Height,
			           (float) Math.Sin(rotation), (float) Math.Cos(rotation),
			           layerDepth,
			           (byte) (effects & (SpriteEffects) 0x03));
		}


		private void Setup()
		{
			GraphicsDevice.BlendState = _blendState;
			GraphicsDevice.SamplerStates[0] = _samplerState;
			GraphicsDevice.DepthStencilState = _depthStencilState;
			GraphicsDevice.RasterizerState = _rasterizerState;

			GraphicsDevice.SetVertexBuffer(_vertexBuffer);
			GraphicsDevice.Indices = _indexBuffer;

			//_spritePass.Apply();
		}

		private unsafe void PushSprite(Texture2D texture,
		                               float sourceX,
		                               float sourceY,
		                               float sourceW,
		                               float sourceH,
		                               float destinationX,
		                               float destinationY,
		                               float destinationW,
		                               float destinationH,
		                               Color color,
		                               float originX,
		                               float originY,
		                               float rotationSin,
		                               float rotationCos,
		                               float depth,
		                               byte effects)
		{

			if (_numSprites >= MaxSprites)
			{
				// Oh crap, we're out of space, flush!
				DrawBatch();
			}

			if (_sortMode == SpriteSortMode.Immediate)
			{
				fixed (VertexPositionColorTexture4* sprite = &vertexInfo[0])
				{
					GenerateVertexInfo(sprite,
					                   sourceX,
					                   sourceY,
					                   sourceW,
					                   sourceH,
					                   destinationX,
					                   destinationY,
					                   destinationW,
					                   destinationH,
					                   color,
					                   originX,
					                   originY,
					                   rotationSin,
					                   rotationCos,
					                   depth,
					                   effects);

					/* We do NOT use Discard here because
					 * it would be stupid to reallocate the
					 * whole buffer just for one sprite.
					 *
					 * Unless you're using this to blit a
					 * target, stop using Immediate ya donut
					 * -flibit
					 */
					_vertexBuffer.SetDataPointer(0,
					                             (IntPtr) sprite,
					                             VertexPositionColorTexture4.RealStride,
					                             SetDataOptions.None);
				}

				DrawPrimitives(texture, 0, 1);
			}
			else if (_sortMode == SpriteSortMode.Deferred)
			{
				fixed (VertexPositionColorTexture4* sprite = &vertexInfo[_numSprites])
				{
					GenerateVertexInfo(
						sprite,
						sourceX,
						sourceY,
						sourceW,
						sourceH,
						destinationX,
						destinationY,
						destinationW,
						destinationH,
						color,
						originX,
						originY,
						rotationSin,
						rotationCos,
						depth,
						effects
					);
				}

				textureInfo[_numSprites] = texture;
				_numSprites += 1;
			}
			else
			{
				fixed (SpriteInfo* spriteInfo = &spriteInfos[_numSprites])
				{
					spriteInfo->textureHash = texture.GetHashCode();
					spriteInfo->sourceX = sourceX;
					spriteInfo->sourceY = sourceY;
					spriteInfo->sourceW = sourceW;
					spriteInfo->sourceH = sourceH;
					spriteInfo->destinationX = destinationX;
					spriteInfo->destinationY = destinationY;
					spriteInfo->destinationW = destinationW;
					spriteInfo->destinationH = destinationH;
					spriteInfo->color = color;
					spriteInfo->originX = originX;
					spriteInfo->originY = originY;
					spriteInfo->rotationSin = rotationSin;
					spriteInfo->rotationCos = rotationCos;
					spriteInfo->depth = depth;
					spriteInfo->effects = effects;
				}

				textureInfo[_numSprites] = texture;
				_numSprites += 1;
			}
		}

		private unsafe void DrawBatch()
		{
			var offset = 0;
			Texture2D curTexture = null;

			Setup();

			// Nothing to do.
			if (_numSprites == 0) return;
			

			if (_sortMode != SpriteSortMode.Deferred)
			{
				IComparer<IntPtr> comparer;
				if (_sortMode == SpriteSortMode.Texture)
				{
					comparer = TextureCompare;
				}
				else if (_sortMode == SpriteSortMode.BackToFront)
				{
					comparer = BackToFrontCompare;
				}
				else
				{
					comparer = FrontToBackCompare;
				}

				fixed (SpriteInfo* spriteInfo = &spriteInfos[0])
				{
					fixed (IntPtr* sortedSpriteInfo = &sortedSpriteInfos[0])
					{
						fixed (VertexPositionColorTexture4* sprites = &vertexInfo[0])
						{
							for (var i = 0; i < _numSprites; i += 1)
							{
								sortedSpriteInfo[i] = (IntPtr)(&spriteInfo[i]);
							}

							Array.Sort(sortedSpriteInfos, textureInfo, 0, _numSprites, comparer);

							for (var i = 0; i < _numSprites; i += 1)
							{
								var info = (SpriteInfo*)sortedSpriteInfo[i];
								GenerateVertexInfo(&sprites[i],
								                   info->sourceX,
								                   info->sourceY,
								                   info->sourceW,
								                   info->sourceH,
								                   info->destinationX,
								                   info->destinationY,
								                   info->destinationW,
								                   info->destinationH,
								                   info->color,
								                   info->originX,
								                   info->originY,
								                   info->rotationSin,
								                   info->rotationCos,
								                   info->depth,
								                   info->effects);
							}
						}
					}
				}
			}

			fixed (VertexPositionColorTexture4* p = &vertexInfo[0])
			{
				/* We use Discard here because the last batch
				 * may still be executing, and we can't always
				 * trust the driver to use a staging buffer for
				 * buffer uploads that overlap between commands.
				 *
				 * If you aren't using the whole vertex buffer,
				 * that's your own fault. Use the whole buffer!
				 * -flibit
				 */
				_vertexBuffer.SetDataPointer(0,
				                             (IntPtr) p,
				                             _numSprites * VertexPositionColorTexture4.RealStride,
				                             SetDataOptions.Discard);
			}

			curTexture = textureInfo[0];
			for (var i = 1; i < _numSprites; i += 1)
			{
				if (textureInfo[i] != curTexture)
				{
					DrawPrimitives(curTexture, offset, i - offset);
					curTexture = textureInfo[i];
					offset = i;
				}
			}
			DrawPrimitives(curTexture, offset, _numSprites - offset);

			_numSprites = 0;
		}

		private static unsafe void GenerateVertexInfo(VertexPositionColorTexture4* sprite,
		                                              float sourceX,
		                                              float sourceY,
		                                              float sourceW,
		                                              float sourceH,
		                                              float destinationX,
		                                              float destinationY,
		                                              float destinationW,
		                                              float destinationH,
		                                              Color color,
		                                              float originX,
		                                              float originY,
		                                              float rotationSin,
		                                              float rotationCos,
		                                              float depth,
		                                              byte effects)
		{
			var cornerX = -originX * destinationW;
			var cornerY = -originY * destinationH;
			sprite->Position0.X = -rotationSin * cornerY + rotationCos * cornerX + destinationX;
			sprite->Position0.Y = rotationCos * cornerY + rotationSin * cornerX + destinationY;
			cornerX = (1.0f - originX) * destinationW;
			cornerY = -originY * destinationH;
			sprite->Position1.X = -rotationSin * cornerY + rotationCos * cornerX + destinationX;
			sprite->Position1.Y = rotationCos * cornerY + rotationSin * cornerX + destinationY;
			cornerX = -originX * destinationW;
			cornerY = (1.0f - originY) * destinationH;
			sprite->Position2.X = -rotationSin * cornerY + rotationCos * cornerX + destinationX;
			sprite->Position2.Y = rotationCos * cornerY + rotationSin * cornerX + destinationY;
			cornerX = (1.0f - originX) * destinationW;
			cornerY = (1.0f - originY) * destinationH;
			sprite->Position3.X = -rotationSin * cornerY + rotationCos * cornerX + destinationX;
			sprite->Position3.Y = rotationCos * cornerY + rotationSin * cornerX + destinationY;
			fixed (float* flipX = &CornerOffsetX[0])
			{
				fixed (float* flipY = &CornerOffsetY[0])
				{
					sprite->TextureCoordinate0.X = flipX[0 ^ effects] * sourceW + sourceX;
					sprite->TextureCoordinate0.Y = flipY[0 ^ effects] * sourceH + sourceY;
					sprite->TextureCoordinate1.X = flipX[1 ^ effects] * sourceW + sourceX;
					sprite->TextureCoordinate1.Y = flipY[1 ^ effects] * sourceH + sourceY;
					sprite->TextureCoordinate2.X = flipX[2 ^ effects] * sourceW + sourceX;
					sprite->TextureCoordinate2.Y = flipY[2 ^ effects] * sourceH + sourceY;
					sprite->TextureCoordinate3.X = flipX[3 ^ effects] * sourceW + sourceX;
					sprite->TextureCoordinate3.Y = flipY[3 ^ effects] * sourceH + sourceY;
				}
			}

			sprite->Position0.Z = depth;
			sprite->Position1.Z = depth;
			sprite->Position2.Z = depth;
			sprite->Position3.Z = depth;
			sprite->Color0 = color;
			sprite->Color1 = color;
			sprite->Color2 = color;
			sprite->Color3 = color;
		}

		private void DrawPrimitives(ITexture texture, int baseSprite, int batchSize)
		{
			GraphicsDevice.Textures[0] = texture;
			if (_effect != null)
			{
				foreach (var pass in _effect.CurrentTechnique.Passes)
				{
					pass.Apply();
					GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
					                                     baseSprite * 4,
					                                     0,
					                                     batchSize * 4,
					                                     0,
					                                     batchSize * 2);
				}
			}
			else
			{
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
				                                     baseSprite * 4,
				                                     0,
				                                     batchSize * 4,
				                                     0,
				                                     batchSize * 2);
			}
		}
		[System.Diagnostics.Conditional("DEBUG")]
		private void CheckBegin(string method)
		{
			if (!_beginCalled)
			{
				throw new InvalidOperationException(
					$"{method} was called, but Begin has not yet been called. Begin must be called successfully before you can call {method}.");
			}
		}

		private static short[] GenerateIndexArray()
		{
			var result = new short[MaxIndices];
			for (int i = 0, j = 0; i < MaxIndices; i += 6, j += 4)
			{
				result[i] = (short)(j);
				result[i + 1] = (short)(j + 1);
				result[i + 2] = (short)(j + 2);
				result[i + 3] = (short)(j + 3);
				result[i + 4] = (short)(j + 2);
				result[i + 5] = (short)(j + 1);
			}
			return result;
		}

		#region Types

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VertexPositionColorTexture4 : IVertexType
		{
			public const int RealStride = 96;

			VertexDeclaration IVertexType.VertexDeclaration
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public Vector3 Position0;
			public Color Color0;
			public Vector2 TextureCoordinate0;

			public Vector3 Position1;
			public Color Color1;
			public Vector2 TextureCoordinate1;

			public Vector3 Position2;
			public Color Color2;
			public Vector2 TextureCoordinate2;

			public Vector3 Position3;
			public Color Color3;
			public Vector2 TextureCoordinate3;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct SpriteInfo
		{
			/* We store the hash instead of the Texture2D because
			 * it allows this to stay an unmanaged type and prevents
			 * us from constantly calling GetHashCode during sorts.
			 */
			public int textureHash;
			public float sourceX;
			public float sourceY;
			public float sourceW;
			public float sourceH;
			public float destinationX;
			public float destinationY;
			public float destinationW;
			public float destinationH;
			public Color color;
			public float originX;
			public float originY;
			public float rotationSin;
			public float rotationCos;
			public float depth;
			public byte effects;
		}

		#endregion

		#region Private Sprite Comparison Classes

		private class TextureComparer : IComparer<IntPtr>
		{
			public unsafe int Compare(IntPtr i1, IntPtr i2)
			{
				var p1 = (SpriteInfo*)i1;
				var p2 = (SpriteInfo*)i2;
				return p1->textureHash.CompareTo(p2->textureHash);
			}
		}

		private class BackToFrontComparer : IComparer<IntPtr>
		{
			public unsafe int Compare(IntPtr i1, IntPtr i2)
			{
				var p1 = (SpriteInfo*)i1;
				var p2 = (SpriteInfo*)i2;
				return p2->depth.CompareTo(p1->depth);
			}
		}

		private class FrontToBackComparer : IComparer<IntPtr>
		{
			public unsafe int Compare(IntPtr i1, IntPtr i2)
			{
				var p1 = (SpriteInfo*)i1;
				var p2 = (SpriteInfo*)i2;
				return p1->depth.CompareTo(p2->depth);
			}
		}

		#endregion

		#region Fields

		// How many sprites are in the current batch?
		private int _numSprites;

		private bool _beginCalled;
		private BlendState _blendState;
		private SamplerState _samplerState;
		private DepthStencilState _depthStencilState;
		private RasterizerState _rasterizerState;
		private SpriteSortMode _sortMode;

		private SpriteInfo[] spriteInfos;
		private IntPtr[] sortedSpriteInfos; // SpriteInfo*[]
		private Texture2D[] textureInfo;
		private VertexPositionColorTexture4[] vertexInfo;
		
		// Buffer objects used for actual drawing
		private readonly IndexBuffer _indexBuffer;
		private readonly /*DynamicVertexBuffer*/VertexBuffer _vertexBuffer;

		// Default SpriteBatch Effect
		private Effect _spriteEffect;
		private IntPtr _spriteMatrixTransform;
		private EffectPass _spriteEffectPass;

		// User-provided Effect, if applicable
		private Effect _effect;

		// Matrix to be used when creating the projection matrix
		private Matrix _transformationMatrix;

		private const int MaxSprites = 2048;
		private const int MaxVertices = MaxSprites * 4;
		private const int MaxIndices = MaxSprites * 6;

		private static readonly short[] indexData = GenerateIndexArray();

		private static readonly float[] CornerOffsetX = {0.0f, 1.0f, 0.0f, 1.0f};
		private static readonly float[] CornerOffsetY = {0.0f, 0.0f, 1.0f, 1.0f};

		private static readonly TextureComparer TextureCompare = new TextureComparer();
		private static readonly BackToFrontComparer BackToFrontCompare = new BackToFrontComparer();
		private static readonly FrontToBackComparer FrontToBackCompare = new FrontToBackComparer();


		#endregion
	}
}