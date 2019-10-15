namespace Lunatics.Framework.Graphics
{
	public class EffectTechnique
	{
		public EffectPassCollection Passes { get; private set; }

		public EffectAnnotationCollection Annotations { get; private set; }

		public string Name { get; private set; }

		internal EffectTechnique(Effect effect, EffectTechnique cloneSource)
		{
			// Share all the immutable types.
			Name = cloneSource.Name;
			Annotations = cloneSource.Annotations;

			// Clone the mutable types.
			// TODO:.. Passes = cloneSource.Passes.Clone(effect);
			Passes = cloneSource.Passes;
		}

		internal EffectTechnique(Effect effect,
		                         string name,
		                         EffectPassCollection passes,
		                         EffectAnnotationCollection annotations)
		{
			Name = name;
			Passes = passes;
			Annotations = annotations;
		}
	}


	public abstract class Effect : GraphicsResource
	{
		public EffectParameterCollection Parameters { get; private set; }

		public EffectTechniqueCollection Techniques { get; private set; }

		public EffectTechnique CurrentTechnique
		{
			get;
			//{
			//	return INTERNAL_currentTechnique;
			//}
			set;
			//{
			//	MojoShader.MOJOSHADER_effectSetTechnique(glEffect.EffectData,value.TechniquePointer);
			//	INTERNAL_currentTechnique = value;
			//}
		}

		protected Effect(GraphicsDevice graphicsDevice, byte[] effectCode)
		{
			GraphicsDevice = graphicsDevice;

			//glEffect = graphicsDevice.GLDevice.CreateEffect(effectCode);

			//// This is where it gets ugly...
			//INTERNAL_parseEffectStruct();

			//// The default technique is the first technique.
			//CurrentTechnique = Techniques[0];

			//// Use native memory for changes, .NET loves moving this around
			//unsafe
			//{
			//	stateChangesPtr = Marshal.AllocHGlobal(sizeof(MojoShader.MOJOSHADER_effectStateChanges));
			//	MojoShader.MOJOSHADER_effectStateChanges* stateChanges = (MojoShader.MOJOSHADER_effectStateChanges*)stateChangesPtr;
			//	stateChanges->render_state_change_count = 0;
			//	stateChanges->sampler_state_change_count = 0;
			//	stateChanges->vertex_sampler_state_change_count = 0;
			//}
		}

		protected Effect(Effect cloneSource)
		{
			GraphicsDevice = cloneSource.GraphicsDevice;

			//// Send the parsed data to be cloned and recompiled by MojoShader
			//glEffect = GraphicsDevice.GLDevice.CloneEffect(cloneSource.glEffect);

			//// Double the ugly, double the fun!
			//INTERNAL_parseEffectStruct();

			//// Copy texture parameters, if applicable
			//for (int i = 0; i < cloneSource.Parameters.Count; i += 1)
			//{
			//	Parameters[i].texture = cloneSource.Parameters[i].texture;
			//}

			//// The default technique is whatever the current technique was.
			//for (int i = 0; i < cloneSource.Techniques.Count; i += 1)
			//{
			//	if (cloneSource.Techniques[i] == cloneSource.CurrentTechnique)
			//	{
			//		CurrentTechnique = Techniques[i];
			//	}
			//}

			//// Use native memory for changes, .NET loves moving this around
			//unsafe
			//{
			//	stateChangesPtr = Marshal.AllocHGlobal(sizeof(MojoShader.MOJOSHADER_effectStateChanges));
			//	MojoShader.MOJOSHADER_effectStateChanges* stateChanges = (MojoShader.MOJOSHADER_effectStateChanges*)stateChangesPtr;
			//	stateChanges->render_state_change_count = 0;
			//	stateChanges->sampler_state_change_count = 0;
			//	stateChanges->vertex_sampler_state_change_count = 0;
			//}
		}

		public abstract Effect Clone();

		protected internal virtual void OnApply()
		{
		}
	}
}