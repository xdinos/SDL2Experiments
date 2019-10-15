using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Lunatics.Framework.Graphics;

namespace Lunatics.Framework
{
	public abstract class GamePlatform : IDisposable
	{
		#region Events

		public event EventHandler<EventArgs> Activated;
		public event EventHandler<EventArgs> Deactivated;

		#endregion

		public Game Game { get; }

		public GameWindow Window { get; protected set; }
		
		public bool IsActive
		{
			get => _isActive;
			protected internal set
			{
				if (_isActive == value) return;

				_isActive = value;
					
				if (_isActive) 
					Activated?.Invoke(this, EventArgs.Empty);
				else 
					Deactivated?.Invoke(this, EventArgs.Empty);
			}
		}

		public abstract IReadOnlyCollection<GraphicsAdapter> GetGraphicsAdapters();

		protected GamePlatform(Game game)
		{
			Game = game;
		}

		protected internal virtual void BeforeInitialize()
		{
			IsActive = true;
		}

		protected internal abstract void RunLoop();

		protected internal abstract void Present();

		public static string GetDefaultWindowTitle()
		{
			// Set the window title.
			string windowTitle = string.Empty;

			// When running unit tests this can return null.
			var assembly = Assembly.GetEntryAssembly();
			if (assembly != null)
			{
				// Use the Title attribute of the Assembly if possible.
				try
				{
					var assemblyTitleAtt = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute)));
					if (assemblyTitleAtt != null)
						windowTitle = assemblyTitleAtt.Title;
				}
				catch
				{
					// Nope, wasn't possible :/
				}

				// Otherwise, fallback to the Name of the assembly.
				if (string.IsNullOrEmpty(windowTitle))
					windowTitle = assembly.GetName().Name;
			}

			return windowTitle;
		}

		#region IDisposable

		protected bool IsDisposed { get; private set; }

		~GamePlatform()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				IsDisposed = true;
			}
		}

		#endregion

		private bool _isActive;
	}
}