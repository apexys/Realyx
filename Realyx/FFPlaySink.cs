using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Realyx
{
	public class FFPlaySink : StereoSink
	{
		Process ffplay_process;

		StereoBlock source;

		Stopwatch st = new Stopwatch();

		public FFPlaySink ()
		{
			ProcessStartInfo psi = new ProcessStartInfo ();
			psi.FileName = "ffplay";
			psi.Arguments = "-i - -f f32le -ar 48000 -ac 2 -acodec pcm_f32le -nodisp -infbuf";
			psi.RedirectStandardInput = true;
			psi.UseShellExecute = false;
			ffplay_process = new Process ();
			ffplay_process.StartInfo = psi;
			ffplay_process.Start();
		}

		public void configure(StereoBlock source){
			this.source = source;
		}

		public void Play(){
			byte[] bits = new byte[4];
			int written = 0;

			RawByteBuffer rbb = new RawByteBuffer (this.source, 4);

			rbb.Run (); //Let it buffer!

			while (!source.hasEnded()) {
				
				//st.Start ();

				byte[] outbuffer = rbb.get();

				//st.Stop ();
				//Console.WriteLine ("Time spent in sink: " + st.ElapsedMilliseconds + "ms");
				//st.Restart ();
				//aplay_process.StandardInput.BaseStream.Flush ();
				ffplay_process.StandardInput.BaseStream.Write (outbuffer, 0,StereoAudioFrame.MAXLEN * 4 * 2);
				//written++;
				//st.Stop();
				//Console.WriteLine ("Time spent in stream: " + st.ElapsedMilliseconds + "ms");
				//st.Reset ();

				//Console.WriteLine ("Written: " + written + ", Buffered: " + rbb.buffered());
			}
		}
	}
}

