using System;
using System.Threading;
using System.Threading.Tasks;

namespace Realyx
{
	public class RawByteBuffer
	{
		StereoBlock source;
		const int blockLength = StereoAudioFrame.MAXLEN * 4 * 2;
		byte[] output;
		byte[] buffer;
		int writePos;
		int readPos;
		int cycles;
		SemaphoreSlim sem_write;
		SemaphoreSlim sem_read;

		public RawByteBuffer (StereoBlock source, int cycles)
		{
			this.source = source;
			output = new byte[blockLength];
			buffer = new byte[blockLength * cycles];
			sem_read = new SemaphoreSlim (0,cycles);
			sem_write = new SemaphoreSlim (cycles,cycles);
			this.cycles = cycles;
		}

		public byte[] get(){
			sem_read.Wait ();
			Array.ConstrainedCopy (buffer, readPos * blockLength, output, 0, blockLength);
			readPos++;
			if (readPos == cycles) {
				readPos = 0;
			}
			if (sem_write.CurrentCount != cycles) {
				sem_write.Release ();
			}
			return buffer;
		}

		public int buffered(){
			return writePos > readPos ? writePos - readPos : ((cycles - readPos) + writePos);
		}

		private void serializeData (int startPos){
			StereoAudioFrame saf = source.get ();
			byte[] bits = new byte[4];
			for (int i = 0; i < StereoAudioFrame.MAXLEN; i++) {
				bits = BitConverter.GetBytes (saf.left_data[i]);
				Array.ConstrainedCopy (bits, 0, buffer, i * 4 * 2 + startPos, 4);
				bits = BitConverter.GetBytes (saf.right_data[i]);
				Array.ConstrainedCopy (bits, 0, buffer, (i * 4 * 2) + 4 + startPos, 4);
			}
		}

		public void Run(){
			sem_write.Wait ();
			serializeData(0);
			writePos++;
			Task.Run (() => {
				while(true){
					sem_write.Wait();//Wait until we have enough space to write
					while(writePos != readPos && !source.hasEnded()){
						serializeData(writePos * blockLength);
						writePos++;
						if(writePos == cycles){
							writePos = 0;
						}
						sem_read.Release();
					}
					if(source.hasEnded()){
						break;
					}
				}
			});
		}

		public bool hasEnded(){
			return source.hasEnded ();
		}
	}
}

