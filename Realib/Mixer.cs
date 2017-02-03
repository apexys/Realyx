using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Realib
{
	public class Mixer : ClockedStructure
	{
		StereoAudioFrame saf;

		List<Channel> channels;
		List<StereoBlock> sinks;

		Stopwatch st = new Stopwatch();

		public Mixer ()
		{
			this.saf = new StereoAudioFrame ();
			channels = new List<Channel> ();
			this.sinks = new List<StereoBlock> ();
		}
			
		public int addChannel(Channel source){
			channels.Add (new Channel(source));
			return channels.Count - 1;
		}

		public MixerSink createSink(){
			var sink = new MixerSink ();
			sinks.Add (sink);
			return sink;
		}

		public class MixerSink : StereoBlock{
			StereoAudioFrame saf = new StereoAudioFrame();
			public StereoAudioFrame get(){
				return saf;
			}

			public void update(StereoAudioFrame newsaf){
				Array.Copy (newsaf.left_data, this.saf.left_data, StereoAudioFrame.MAXLEN);
				Array.Copy (newsaf.right_data, this.saf.right_data, StereoAudioFrame.MAXLEN);
				this.saf.number = newsaf.number;
				this.saf.start = newsaf.start;
				this.saf.length = newsaf.length;
			}

			public bool hasEnded(){
				return false;
			}

			public bool isActive(){
				return false;
			}
		}

		public void tick(){
			st.Start ();
			//1. Zero out saf
			Array.Clear(saf.left_data,0,StereoAudioFrame.MAXLEN);
			Array.Clear(saf.right_data,0,StereoAudioFrame.MAXLEN);

			//2. Calculate channel factor
			float channel_factor = 1 / (float) channels.Count;

			//3. Load new data
			StereoAudioFrame channel_frame;
			foreach(Channel channel in channels){
				channel_frame = channel.get ();
				for (var i = 0; i < StereoAudioFrame.MAXLEN; i++) {
					saf.left_data [i] += channel_frame.left_data [i] * channel_factor;
					saf.right_data [i] += channel_frame.right_data [i] * channel_factor;
				}
			}
				

			//Set Metadata to at least *something*
			saf.length = (double)StereoAudioFrame.MAXLEN / 48000;
			saf.start += saf.length;

			st.Stop ();
			Console.WriteLine ("Time spent in mixer: " + st.ElapsedMilliseconds + "ms");
			st.Reset ();

			foreach (MixerSink sink in sinks) {
				sink.update (saf);
			}
		}

	}
}

