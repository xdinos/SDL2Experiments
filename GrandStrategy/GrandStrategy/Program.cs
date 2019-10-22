using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandStrategy
{
	static class Program
	{
		static void Main()
		{

            Environment.SetEnvironmentVariable("OPENGL_FORCE_CORE_PROFILE", "1");
            Environment.SetEnvironmentVariable("GRAPHICS_ENABLE_HIGHDPI", "1");
			
			using (var game = new Game())
			{
				game.Run();
			}
		}
	}
}
