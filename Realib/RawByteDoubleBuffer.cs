using System;
using System.Threading;
using System.Threading.Tasks;

namespace Realib
{
	public class RawByteDoubleBuffer : StereoSink
	{
		byte[] output1;
		byte[] output2;
		byte activeOutput = 0;
		byte activeInput = 0;
		StereoBlock source;
		SemaphoreSlim sem_write;
		SemaphoreSlim sem_read;

		public RawByteDoubleBuffer ()
		{
			output1 = new byte[2 * StereoAudioFrame.MAXLEN * 4];
			output2 = new byte[2 * StereoAudioFrame.MAXLEN * 4];
			sem_write = new SemaphoreSlim (0);
			sem_read = new SemaphoreSlim (0);
		}

		public void configure(StereoBlock source){
			this.source = source;
		}

		private void serialize(byte buffer_number){
			StereoAudioFrame saf = source.get ();
			//Console.WriteLine ("Read frame " + saf.number);
			byte[] bits = new byte[4];
			byte[] buffer;
			if (buffer_number == 1) {
				buffer = output1;
			} else {
				buffer = output2;
			}
			for (int i = 0; i < StereoAudioFrame.MAXLEN; i++) {
				bits = BitConverter.GetBytes (saf.left_data[i]);
				Array.ConstrainedCopy (bits, 0, buffer, i * 4 * 2, 4);
				bits = BitConverter.GetBytes (saf.right_data[i]);
				Array.ConstrainedCopy (bits, 0, buffer, (i * 4 * 2) + 4, 4);
			}
		}

		public void Play(){
			serialize(1);
			activeOutput = 1;
			activeInput = 2;
			sem_read.Release ();
			Task.Run (() => {
				while(true /*!source.hasEnded()*/){
					sem_write.Wait();
					serialize(activeInput);
					sem_read.Release();
				}
			});
		}

		public byte[] getBytes(){
			activeInput = activeOutput;
			activeOutput = (byte) (activeInput == 2 ? 1 : 2);
			sem_write.Release();
			sem_read.Wait ();
			if (activeOutput == 1) {
				return output1;
			} else {
				return output2;
			}
		}
	}
}

