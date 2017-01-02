﻿using System;
using System.IO;
namespace Realyx
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var mixer = new Mixer ();
			var source = new FFMpegSource ("/home/apexys/Music/Arty & Andrew Bayer - Follow The Light-272737205.mp3");
			var source2 = new FFMpegSource ("/home/apexys/Music/Jaime.mp3");
			//var source = new FFMpegRIFFWaveSource("/home/apexys/Music/Jaime.mp3");
			//FileStream fstr = new FileStream("/home/apexys/Music/CatThruster.wav",FileMode.Open);
			//var source = new RIFFWaveSource (fstr, () => fstr.Position == fstr.Length);
			var ch1 = mixer.addChannel (source);
			var ch2 = mixer.addChannel (source2);
			mixer.setVolume (ch1, 0.3f);
			var sink = new FFPlaySink ();
			sink.configure (mixer);
			sink.Play ();
		}
	}
}
