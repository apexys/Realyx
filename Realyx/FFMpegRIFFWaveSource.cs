using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Realyx
{
	public class FFMpegRIFFWaveSource: StereoBlock
	{
		Process ffmpeg_process;
		FileStream fstr;
		RIFFWaveSource pcms;

		public FFMpegRIFFWaveSource (String filename)
		{
			//Make sure file exists and is mp3
			if (!File.Exists (filename)) {
				throw new ArgumentException ("File does not exist: " + filename);
			}
			if (! (new FileInfo (filename).Extension == ".mp3")) {
				throw new ArgumentException ("File does not have mp3 extension: " + filename);
			}

			fstr = new FileStream (filename, FileMode.Open);

			ProcessStartInfo psi = new ProcessStartInfo ();
			filename = filename.Replace(" ", "\\ ");
			psi.FileName = "ffmpeg";
			psi.Arguments = "-i - -ar 48000 -ac 2 -f wav -acodec pcm_f32le -";
			Console.WriteLine (psi.FileName + " " + psi.Arguments);
			psi.UseShellExecute = false;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardInput = true;

			ffmpeg_process = new Process ();
			ffmpeg_process.StartInfo = psi;
			ffmpeg_process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => {
				Console.WriteLine("Error: " + e.Data);
			};
			//ffmpeg_process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => {
			//Console.WriteLine("Data: " + e.Data);
			//};
			ffmpeg_process.EnableRaisingEvents = true;
			ffmpeg_process.Start ();
			//ffmpeg_process.BeginOutputReadLine ();
			ffmpeg_process.BeginErrorReadLine ();

			Task.Run (() => {
				int read = 0;
				byte[] buffer = new byte[4096];
				while ((read = fstr.Read (buffer, 0, 4096)) >= 4096) {
					//Console.WriteLine ("Stream: read " + read.ToString () + " bytes");
					ffmpeg_process.StandardInput.BaseStream.Write (buffer, 0, read);
					ffmpeg_process.StandardInput.BaseStream.Flush ();
					//System.Threading.Thread.Sleep (100);
				}
				//ffmpeg_process.StandardInput.BaseStream.WriteByte(0x04);
				ffmpeg_process.StandardInput.BaseStream.Close();
				fstr.Close();
				Console.WriteLine ("Finished feeding input");
			});

			System.Threading.Thread.Sleep (1500);

			pcms = new RIFFWaveSource (ffmpeg_process.StandardOutput.BaseStream, () => ffmpeg_process.StandardOutput.EndOfStream);

		}

		public StereoAudioFrame get(){
			return pcms.get ();
		}

		public bool hasEnded(){
			return ffmpeg_process.StandardOutput.EndOfStream;
		}

		public Boolean hasExited(){
			return ffmpeg_process.HasExited;
		}
	}
}

