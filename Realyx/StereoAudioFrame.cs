using System;

namespace Realyx
{
	public class StereoAudioFrame
	{
		public const int MAXLEN = 4096;
		public float[] left_data;
		public float[] right_data;
		public double start;
		public double length;


		public StereoAudioFrame ()
		{
			left_data = new float[StereoAudioFrame.MAXLEN];
			right_data = new float[StereoAudioFrame.MAXLEN];
			start = 0;
			length = 0;
		}
	}
}

