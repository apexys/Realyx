using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Realyx
{
	public class AplaySink
	{
		Process aplay_process;

		StereoBlock source;

		Stopwatch st = new Stopwatch();

		public AplaySink (StereoBlock source)
		{
			ProcessStartInfo psi = new ProcessStartInfo ();
			//psi.FileName = "aplay";
			//psi.Arguments = "-c 2 -f FLOAT_LE -r 48000 -B 10000 -";
			psi.FileName = "cat";
			psi.Arguments = "-";
			psi.RedirectStandardInput = true;
			psi.UseShellExecute = false;
			aplay_process = new Process ();
			aplay_process.StartInfo = psi;
			aplay_process.Start();

			this.source = source;
		}

		public void Play(){
			byte[] bits = new byte[4];
			byte[] outbuffer = new byte[StereoAudioFrame.MAXLEN * 4 * 2];
			int written = 0;
			while (!source.hasEnded()) {
				StereoAudioFrame saf = source.get ();

				st.Start ();
				for (int i = 0; i < StereoAudioFrame.MAXLEN; i++) {
					bits = BitConverter.GetBytes (saf.left_data[i]);
					Array.ConstrainedCopy (bits, 0, outbuffer, i * 4 * 2, 4);
					bits = BitConverter.GetBytes (saf.right_data[i]);
					Array.ConstrainedCopy (bits, 0, outbuffer, (i * 4 * 2) + 4, 4);
				}

				written += 8 * StereoAudioFrame.MAXLEN;

				st.Stop ();
				Console.WriteLine ("Time spent in sink: " + st.ElapsedMilliseconds + "ms");
				st.Restart ();
				aplay_process.StandardInput.BaseStream.Write (outbuffer, 0,StereoAudioFrame.MAXLEN * 4 * 2);
				//aplay_process.StandardInput.BaseStream.Flush ();
				st.Stop();
				Console.WriteLine ("Time spent in stream: " + st.ElapsedMilliseconds + "ms");
				st.Reset ();

				Console.WriteLine ("Written: " + written + ", Position: " + saf.start.ToString());
			}
		}
	}
}

