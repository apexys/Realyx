using System;
using Realib;

namespace PulseaudioBindings
{
	public class PulseaudioSource : StereoBlock
	{
		IntPtr pa_simple;
		StereoAudioFrame saf;
		float[] data;
		int number;
		double position;

		public PulseaudioSource ()
		{
			this.saf = new Realib.StereoAudioFrame ();
			this.data = new float[StereoAudioFrame.MAXLEN * 2];
			unsafe{
				PulseaudioSimple.pa_sample_spec ss = new  PulseaudioSimple.pa_sample_spec ();
				ss.channels = 2;
				ss.format = PulseaudioSimple.pa_sample_format.PA_SAMPLE_FLOAT32LE;
				ss.rate = 48000;

				int error;

				pa_simple = PulseaudioSimple.pa_simple_new (null, "Realyx", PulseaudioSimple.pa_stream_direction.PA_STREAM_RECORD, null, "Recording", &ss, null, null, &error);

				if (error != 0) {
					Console.WriteLine(PulseaudioSimple.getErrorString(error));
				}
			}
		}

		public PulseaudioSource (string sourceDevice)
		{
			this.saf = new Realib.StereoAudioFrame ();
			this.data = new float[StereoAudioFrame.MAXLEN * 2];
			unsafe{
				PulseaudioSimple.pa_sample_spec ss = new  PulseaudioSimple.pa_sample_spec ();
				ss.channels = 2;
				ss.format = PulseaudioSimple.pa_sample_format.PA_SAMPLE_FLOAT32LE;
				ss.rate = 48000;

				int error;

				pa_simple = PulseaudioSimple.pa_simple_new (null, "Realyx", PulseaudioSimple.pa_stream_direction.PA_STREAM_RECORD, sourceDevice, "Recording", &ss, null, null, &error);

				if (error != 0) {
					Console.WriteLine(PulseaudioSimple.getErrorString(error));
				}
			}
		}

		~PulseaudioSource(){
			PulseaudioSimple.pa_simple_free (pa_simple);
		}

		public StereoAudioFrame get(){
			unsafe{
				int error;
				PulseaudioSimple.pa_simple_read (pa_simple, this.data, StereoAudioFrame.MAXLEN * 4 * 2, &error);
				if (error != 0) {
					Console.WriteLine(PulseaudioSimple.getErrorString(error));
				}
			}

			int i = 0;
			int pos = 0;
			while(i < StereoAudioFrame.MAXLEN * 2){
				saf.left_data[pos] = this.data[i];
				i++;
				saf.right_data[pos] = this.data[i];
				i++;
				pos++;
			}


			saf.number = number;
			number++;
			this.position = saf.start;
			saf.start += saf.length;
			saf.length = (double)StereoAudioFrame.MAXLEN / (double)48000;
			return saf;
		}

		public bool hasEnded(){
			return false;
		}

		public bool isActive(){
			return true;
		}
	}
}

