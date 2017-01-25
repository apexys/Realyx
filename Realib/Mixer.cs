using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Realib
{
	public class Mixer : StereoBlock
	{
		StereoAudioFrame saf;

		List<Channel> channels;
		List<StereoEffect> effects;

		Stopwatch st = new Stopwatch();

		public Mixer ()
		{
			this.saf = new StereoAudioFrame ();
			channels = new List<Channel> ();
			this.effects = new List<StereoEffect> ();
		}

		public void addEffect(StereoEffect effect){
			this.effects.Add (effect);
		}

		public int addChannel(StereoBlock source){
			channels.Add (new Channel(source));
			return channels.Count - 1;
		}

		public void setVolume(int ChannelNumber, float volume){
			channels [ChannelNumber].volume = volume;
		}

		public StereoAudioFrame get(){
			st.Start ();
			//1. Zero out saf
			Array.Clear(saf.left_data,0,StereoAudioFrame.MAXLEN);
			Array.Clear(saf.right_data,0,StereoAudioFrame.MAXLEN);

			//2. Calculate channel factor
			float channel_factor = 0;
			foreach(Channel channel in channels){
				if (!channel.hasEnded()) {
					channel_factor++;
				}
			}
			channel_factor = 1 / channel_factor;

			//3. Load new data
			StereoAudioFrame channel_frame;
			foreach(Channel channel in channels){
				if (!channel.hasEnded ()) {
					channel_frame = channel.get ();
					for (var i = 0; i < StereoAudioFrame.MAXLEN; i++) {
						saf.left_data [i] += channel_frame.left_data [i] * channel_factor * channel.volume;
						saf.right_data [i] += channel_frame.right_data [i] * channel_factor * channel.volume;
					}
				} else {
					Console.WriteLine ("Channel has ended ");
				}
			}

			foreach (var effect in effects) {
				effect.process (ref saf);
			}

			//Set Metadata to at least *something*
			saf.length = (double)StereoAudioFrame.MAXLEN / 48000;
			saf.start += saf.length;

			st.Stop ();
			Console.WriteLine ("Time spent in mixer: " + st.ElapsedMilliseconds + "ms");
			st.Reset ();

			//4. Return data
			return saf;

		}

		public bool hasEnded(){
			return false;
			for (int i = 0; i < channels.Count; i++) {
				if (channels [i].hasEnded()) {
					return true;
				}
			}
			return false;
		}

		public bool isActive(){
			return true;
		}
			


		class Channel{
			public StereoBlock source;
			public List<StereoEffect> effects;
			public float volume;

			public Channel(StereoBlock source){
				this.source = source;
				effects = new List<StereoEffect>();
				volume = 1;
			}

			public void addEffect(StereoEffect se){
				effects.Add (se);
			}

			public StereoAudioFrame get(){
				StereoAudioFrame result = source.get ();
				foreach(StereoEffect effect in effects){
					effect.process (ref result);
				}
				return result;
			}

			public bool hasEnded(){
				return source.hasEnded ();
			}
		}
	}
}

