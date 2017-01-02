using System;
using System.Threading;
using System.Threading.Tasks;

namespace Realyx
{
	//As far as I can see this is mostly useless
	public class DoubleBufferSink : StereoSink, StereoBlock
	{
		StereoAudioFrame saf1;
		StereoAudioFrame saf2;
		SemaphoreSlim sem_write;
		SemaphoreSlim sem_read;
		int write_sem;
		int read_sem;
		StereoBlock source;
		StereoSink sink;

		public DoubleBufferSink (StereoSink sink)
		{
			saf1 = new StereoAudioFrame ();
			saf2 = new StereoAudioFrame ();
			sem_write = new SemaphoreSlim (2, 2);
			sem_read = new SemaphoreSlim (0, 2);
			write_sem = 1;
			read_sem = 1;
			this.sink = sink;
		}

		public void configure(StereoBlock source){
			this.source = source;
		}

		public StereoAudioFrame get(){
			return null;
		}

		public bool hasEnded(){
			return source.hasEnded();
		}

		public bool isActive(){
			return source.isActive ();
		}

		private void readThread(){
			StereoAudioFrame s;
			while (true) {
				sem_write.Wait ();
				s = source.get ();
				Array.Copy (s.left_data, saf1.left_data, StereoAudioFrame.MAXLEN);
				Array.Copy (s.right_data, saf1.right_data, StereoAudioFrame.MAXLEN);
				sem_read.Release ();
				sem_write.Wait ();
				s = source.get ();
				Array.Copy (s.left_data, saf2.left_data, StereoAudioFrame.MAXLEN);
				Array.Copy (s.right_data, saf2.right_data, StereoAudioFrame.MAXLEN);
				sem_read.Release ();
			}
		}

		private void writeThread(){

		}



		public void Play(){

			//1. Read thread
			Task.Run (() => readThread());

			//2. Write thread

		}
	}
}

