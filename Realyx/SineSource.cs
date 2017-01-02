using System;

namespace Realyx
{
	public class SineSource : StereoBlock
	{
		StereoAudioFrame saf;
		const int fullBufferLen = StereoAudioFrame.MAXLEN * 4 * 2;
//Maximum buffer length * 4 Bytes in a float * 2 Channels
		public SineSource ()
		{
			saf = new StereoAudioFrame ();
		}

		public bool hasEnded ()
		{
			return false;
		}


		public StereoAudioFrame get ()
		{
			for (int i = 0; i < StereoAudioFrame.MAXLEN; i++) {
				double phase = ((double)i / 128d) * (Math.PI * 2);
				saf.left_data [i] = (float)Math.Sin (phase);
				saf.right_data [i] = (float)Math.Sin (phase);
			}



			saf.start += saf.length;
			saf.length = (double)StereoAudioFrame.MAXLEN / (double)48000;

			return saf;
		}


	}
}
	
