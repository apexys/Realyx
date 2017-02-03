using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Realib
{
	public class FFMpegSource : StereoBlock, ControllableSource
	{
		Process ffmpeg_process;
		FileStream fstr;
		PCMSource pcms;
		bool stopped;
		bool paused;
		Task fillTask;

		public enum FFMpegSourceState
		{
			Loading,
			Playing,
			Finished
		}

		public FFMpegSource (String filename)
		{
			//Set options
			paused = true;
			stopped = false;

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
			psi.Arguments = "-i - -f f32le -ar 48000 -ac 2 -acodec pcm_f32le -";
			Console.WriteLine (psi.FileName + " " + psi.Arguments);
			psi.UseShellExecute = false;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardInput = true;

			ffmpeg_process = new Process ();
			ffmpeg_process.StartInfo = psi;
			/*ffmpeg_process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => {
				Console.WriteLine("Error: " + e.Data); //TODO: Debug
			};*/
			//ffmpeg_process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => {
			//Console.WriteLine("Data: " + e.Data);
			//};
			ffmpeg_process.EnableRaisingEvents = true;
			ffmpeg_process.Start ();
			//ffmpeg_process.BeginOutputReadLine ();
			ffmpeg_process.BeginErrorReadLine ();

			fillTask = Task.Run (() => {
				int read = 0;
				byte[] buffer = new byte[4096];
				while ((read = fstr.Read (buffer, 0, 4096)) >= 4096) {
					//Console.WriteLine ("Stream: read " + read.ToString () + " bytes");
					ffmpeg_process.StandardInput.BaseStream.Write (buffer, 0, read);
					//ffmpeg_process.StandardInput.BaseStream.Flush ();
					//System.Threading.Thread.Sleep (100);
				}
				//ffmpeg_process.StandardInput.BaseStream.WriteByte(0x04);
				ffmpeg_process.StandardInput.BaseStream.Close ();
				fstr.Close ();
				Console.WriteLine ("Finished feeding input");
			});

			pcms = new PCMSource (ffmpeg_process.StandardOutput.BaseStream, () => ffmpeg_process.StandardOutput.EndOfStream);

		}

		public FFMpegSource (Stream input)
		{
			//Set options
			paused = true;
			stopped = false;

			ProcessStartInfo psi = new ProcessStartInfo ();
			psi.FileName = "ffmpeg";
			psi.Arguments = "-i - -f f32le -ar 48000 -ac 2 -acodec pcm_f32le -";
			Console.WriteLine (psi.FileName + " " + psi.Arguments);
			psi.UseShellExecute = false;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardInput = true;

			ffmpeg_process = new Process ();
			ffmpeg_process.StartInfo = psi;
			/*ffmpeg_process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => {
				Console.WriteLine("Error: " + e.Data); //TODO: Debug
			};*/
			//ffmpeg_process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => {
			//Console.WriteLine("Data: " + e.Data);
			//};
			ffmpeg_process.EnableRaisingEvents = true;
			ffmpeg_process.Start ();
			//ffmpeg_process.BeginOutputReadLine ();
			ffmpeg_process.BeginErrorReadLine ();

			fillTask = Task.Run (() => {
				int read = 0;
				byte[] buffer = new byte[4096];
				while ((read = input.Read (buffer, 0, 4096)) >= 4096) {
					//Console.WriteLine ("Stream: read " + read.ToString () + " bytes");
					ffmpeg_process.StandardInput.BaseStream.Write (buffer, 0, read);
					//ffmpeg_process.StandardInput.BaseStream.Flush ();
					//System.Threading.Thread.Sleep (100);
				}
				//ffmpeg_process.StandardInput.BaseStream.WriteByte(0x04);
				ffmpeg_process.StandardInput.BaseStream.Close ();
				input.Close();
				Console.WriteLine ("Finished feeding input");
			});

			pcms = new PCMSource (ffmpeg_process.StandardOutput.BaseStream, () => ffmpeg_process.StandardOutput.EndOfStream);

		}

		~FFMpegSource(){
			//ffmpeg_process.Kill ();
		}

		public StereoAudioFrame get(){
			return pcms.get ();
		}

		public bool hasEnded(){
			return pcms.hasEnded ();
		}




		public Boolean hasExited(){
			return ffmpeg_process.HasExited || stopped;
		}

		public bool isActive(){
			if (hasExited()) {
				return false;
			} else {
				return !paused;
			}
		}

		public void Play(){
			paused = false;
		}

		public void Pause (){
			paused = true;
		}

		public void Stop(){
			stopped = true;
		}
			
	}
}

