using System;
using System.Collections.Generic;

namespace Realyx
{
	public class AverageVolumeAnalyzer :StereoEffect
	{
		float leftvol;
		float rightvol;
		float time;
		int samplesNeeded;
		int sampleLength;
		bool started = false;
		List<Action<float,float>> listeners;
		public AverageVolumeAnalyzer (float time)
		{
			this.time = time;
			this.sampleLength =Convert.ToInt32( 48000f * time);
			this.samplesNeeded = sampleLength;
			listeners = new List<Action<float, float>> ();
		}

		public void addListener(Action<float,float> listener){
			listeners.Add (listener);
		}

		public void process(ref StereoAudioFrame data){
			int counter = 0;
			if (!started) {
				leftvol = Math.Abs(data.left_data [0]);
				rightvol = Math.Abs(data.right_data [0]);
				counter++;
				samplesNeeded--;
				started = true;
			}
			for (var i = counter; counter < StereoAudioFrame.MAXLEN; counter++) {
				leftvol += (Math.Abs(data.left_data [counter]) -leftvol) / (float)sampleLength;
				rightvol += (Math.Abs(data.right_data [counter]) -rightvol) / (float) sampleLength;
				counter++;
				samplesNeeded--;
				if (samplesNeeded == 0) {
					foreach (Action<float,float> listener in listeners) {
						listener (leftvol, rightvol);
					}
					samplesNeeded = sampleLength;
				}
			}
		}
	}
}

