using System;

namespace PulseaudioBindings
{
	public class PulseaudioSink : Realib.StereoSink, Realib.ClockedStructure
	{
		IntPtr pa_simple;
		Realib.StereoBlock source;
		byte[] buffer;

		public PulseaudioSink ()
		{
			buffer = new byte[Realib.StereoAudioFrame.MAXLEN * 4 * 2];
			unsafe{
				PulseaudioSimple.pa_sample_spec ss = new PulseaudioSimple.pa_sample_spec ();
				ss.format = PulseaudioSimple.pa_sample_format.PA_SAMPLE_FLOAT32LE;
				ss.channels = 2;
				ss.rate = 48000;

				int error;

				pa_simple = PulseaudioSimple.pa_simple_new (null, "Testapp", PulseaudioSimple.pa_stream_direction.PA_STREAM_PLAYBACK, null, "Playback", &ss, null,null,&error);

				Console.WriteLine (PulseaudioSimple.getErrorString(error));

				Console.WriteLine(pa_simple);

				Console.WriteLine(PulseaudioSimple.pa_simple_get_latency(pa_simple,&error));
			}
		}

		public PulseaudioSink (string sinkDevice)
		{
			buffer = new byte[Realib.StereoAudioFrame.MAXLEN * 4 * 2];
			unsafe{
				PulseaudioSimple.pa_sample_spec ss = new PulseaudioSimple.pa_sample_spec ();
				ss.format = PulseaudioSimple.pa_sample_format.PA_SAMPLE_FLOAT32LE;
				ss.channels = 2;
				ss.rate = 48000;

				int error;

				pa_simple = PulseaudioSimple.pa_simple_new (null, "Testapp", PulseaudioSimple.pa_stream_direction.PA_STREAM_PLAYBACK, sinkDevice, "Playback", &ss, null,null,&error);

				Console.WriteLine (PulseaudioSimple.getErrorString(error));
		
				Console.WriteLine(pa_simple);

				Console.WriteLine(PulseaudioSimple.pa_simple_get_latency(pa_simple,&error));
			}
		}

		~PulseaudioSink(){
			PulseaudioSimple.pa_simple_free (pa_simple);
		}



		public void configure (Realib.StereoBlock source){
			this.source = source;
		}
			
		public void tick(){
			Realib.StereoAudioFrame saf = source.get();
			//Console.WriteLine ("Read frame " + saf.number);
			byte[] bits = new byte[4];
			for (int i = 0; i < Realib.StereoAudioFrame.MAXLEN; i++) {
				bits = BitConverter.GetBytes (saf.left_data[i]);
				Array.ConstrainedCopy (bits, 0, buffer, i * 4 * 2, 4);
				bits = BitConverter.GetBytes (saf.right_data[i]);
				Array.ConstrainedCopy (bits, 0, buffer, (i * 4 * 2) + 4, 4);
			}
			unsafe{
				int error;
				PulseaudioSimple.pa_simple_write (pa_simple, buffer, (uint)buffer.Length, &error);
				if (error != 0) {
					Console.WriteLine(PulseaudioSimple.getErrorString(error));
				}
				//PulseaudioSimple.pa_simple_flush (pa_simple, &error);
				if (error != 0) {
					Console.WriteLine(PulseaudioSimple.getErrorString(error));
				}
			}
		}
	}
}

