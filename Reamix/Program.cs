using System;
using Realib;

namespace Reamix
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Switchboard switchboard = new Switchboard();
			Clock clock = new Clock ();
			clock.RegisterClockedStructure (switchboard);
			clock.Start ();
		}
	}
}
