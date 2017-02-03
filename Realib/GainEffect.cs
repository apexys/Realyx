using System;

namespace Realib
{
	public class GainEffect : StereoEffect
	{
		public float gain;
		public GainEffect ()
		{
			gain = 1;
		}

		public void process(ref Realib.StereoAudioFrame saf){
			for (var i = 0; i < Realib.StereoAudioFrame.MAXLEN; i++) {
				saf.left_data [i] *= gain;
				saf.right_data [i] *= gain;
			}
		}
	}
}

