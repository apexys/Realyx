using System;

namespace Realyx
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var mixer = new Mixer ();
			var source = new FFMpegSource("/home/apexys/Music/Jaime.mp3");
			mixer.addChannel (source);
			var sink = new FFPlaySink ();
			sink.configure (source);
			sink.Play ();
		}
	}
}
