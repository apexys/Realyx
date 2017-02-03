using System;
using Realib;
using PulseaudioBindings;

namespace Reamix
{
	public class Switchboard : ClockedStructure
	{
		PulseaudioSource microphonesource;
		StereoSourceSplitter microphonesplitter;
		Channel microphone1;
		Channel microphone2;
		Channel song1;
		Channel song2;
		Channel jingle;
		Mixer mixer1;
		Mixer mixer2;

		Channel monitors;

		PulseaudioSink main_sink;
		NullSink monitor_sink;

		MultiFileSource song1source;
		MultiFileSource song2source;
		MultiFileSource jinglesource;

		public Switchboard ()
		{
			//1. Set up microphones
			microphonesource = new PulseaudioSource();
			microphonesplitter = new StereoSourceSplitter (microphonesource);

			microphone1 = new Channel (microphonesplitter.Left);
			microphone2 = new Channel (microphonesplitter.Right);

			//2. Set up Songinputs
			song1source = new MultiFileSource();
			song2source = new MultiFileSource();
			jinglesource = new MultiFileSource();

			song1 = new Channel (song1source);
			song2 = new Channel (song2source);
			jingle = new Channel (jingle);

			//3. Set up mixers
			mixer1 = new Mixer();
			mixer1.addChannel (song1);
			mixer1.addChannel (song2);
			mixer1.addChannel (jingle);

			var output = new Channel(mixer1.createSink ());

			monitors = new Channel (mixer1.createSink ());

			mixer2 = new Mixer ();
			mixer2.addChannel (microphone1);
			mixer2.addChannel (microphone2);
			mixer2.addChannel (output);

			main_sink = new PulseaudioSink();
			main_sink.configure (mixer2.createSink ());

			monitor_sink = new NullSink ();
			monitor_sink.configure (monitors);

		}

		public void tick(){
			//1. Live inputs
			microphonesplitter.tick();

			//3. Mixers
			mixer1.tick();
			mixer2.tick ();

			//4. Sinks
			main_sink.tick();
			monitor_sink.tick();


		}


		public string getAttribute(string name){
			return null;
		}
	}
}

