using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDLGame
{
	static class Program
	{
		static void Main()
		{
			using (var game = new Game())
			{
				game.Run();
			}
		}
	}
}
