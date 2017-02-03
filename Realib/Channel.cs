using System;
using System.Collections.Generic;
using Realib;

namespace Realib
{
	public class Channel : StereoBlock
	{
		StereoBlock source;
		/// <summary>
		/// The channel gain, applied LAST
		/// </summary>
		public float Gain = 1f;

		private float _maxvol;

		/// <summary>
		/// Gets the maximum volume 
		/// </summary>
		/// <value>The maximum volume.</value>
		public float MaximumVolume{
			get{
				return _maxvol;
			}
		}

		public List<StereoEffect> ChannelEffects;

		public Channel (StereoBlock input)
		{
			this.source = input;
			ChannelEffects = new List<StereoEffect> ();
		}

		public StereoAudioFrame get(){
			StereoAudioFrame saf = source.get ();
			foreach (StereoEffect se in ChannelEffects) {
				se.process (ref saf);
			}
			for (var i = 0; i < StereoAudioFrame.MAXLEN; i++) {
				saf.left_data[i] *= Gain;
				saf.right_data[i] *= Gain;
				if (saf.left_data [i] > _maxvol) {
					_maxvol = saf.left_data [i];
				}
				if (saf.right_data [i] > _maxvol) {
					_maxvol = saf.right_data [i];
				}
			}
			return saf;
		}

	}
}

