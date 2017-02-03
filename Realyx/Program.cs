using System;
using System.IO;
using Realib;
namespace Realyx
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//var mixer = new Mixer ();
			var source = new PulseaudioBindings.PulseaudioSource (PulseaudioBindings.PulseaudioHelpers.getSourceDeviceNames()[2]);
			//var source = new FFMpegSource ("/home/apexys/Music/03. Cat Thruster.mp3");
			//var source = new FFMpegSource ("/home/apexys/Music/Arty & Andrew Bayer - Follow The Light-272737205.mp3");
			//var source2 = new FFMpegSource ("/home/apexys/Music/Jaime.mp3");
			//var source = new FFMpegRIFFWaveSource("/home/apexys/Music/Jaime.mp3");
			//FileStream fstr = new FileStream("/home/apexys/Music/CatThruster.wav",FileMode.Open);
			//var source = new RIFFWaveSource (fstr, () => fstr.Position == fstr.Length);
			//var ch1 = mixer.addChannel (source);
			//var ch2 = mixer.addChannel (source2);
			//mixer.setVolume (ch1, 0.5f);

			//mixer.addEffect (effect);
			//source.Play ();
			//source2.Play ();
			Channel input = new Channel(source);
			input.Gain = 1f;

			var sink = new PulseaudioBindings.PulseaudioSink(PulseaudioBindings.PulseaudioHelpers.getSinkDeviceNames ()[2]);
			sink.configure (input);

			//var sink = new FFPlaySink ();
			//sink.configure (mixer);
			//sink.Play ();
			//var sink = new NullSink();
			var clock = new Clock();
			clock.RegisterClockedStructure (sink);
			clock.Start ();
			//System.Threading.Thread.Sleep (40000);
			//clock.Stop ();
			Console.WriteLine ("All done!");

			var names = PulseaudioBindings.PulseaudioHelpers.getSourceDeviceNames ();
			foreach(string name in names){
				Console.WriteLine (name);
			}
		}
	}
}
