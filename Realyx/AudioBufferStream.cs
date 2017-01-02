using System;
using System.Threading;

namespace Realyx
{
	public class AudioBufferStream
	{
		float[] buffer;
		int available;
		int position_write;
		int position_read;
		int buffer_size;
		SemaphoreSlim sem_read;
		SemaphoreSlim sem_write;

		public AudioBufferStream (int buffer_size = 2048)
		{
			this.buffer_size = buffer_size;
			buffer = new float[buffer_size];
			available = 0;
			position_write = 0;
			position_read = 0;
			sem_read = new SemaphoreSlim(0);
			sem_write = new SemaphoreSlim(0);
		}

		public void write(float[] data, int count){
			int index = 0;
			Boolean blocked = false;
			while (index < count) {
				//Wait until there's space to write data
				//[----]
				// R W
				lock (this) {
					if (available == buffer_size) {
						blocked = true;
					}
				}
				while (blocked) {
					sem_write.Wait ();
					lock (this) {
						blocked = (available == buffer_size) ? true : false;
					}
				}
				lock(this){
					//Calculate how much data we can copy in one go:
					//If we have more data to write than fits between the current position, we write it in two chunks
					//We can at most write (buffer_size - available) bytes
					int space_available = (buffer_size - available > buffer_size - position_write) ? buffer_size - position_write : buffer_size - available; 
					Array.ConstrainedCopy (data, index, buffer, position_write, space_available);
					position_write += space_available;//Move the pointers forwards in both arrays
					index += space_available;
					available += space_available;//there are more bytes to be read now
					if (position_write == buffer_size) { //Wrap the pointer of the ringbuffer around
						position_write = 0;
					}
				}
				sem_read.Release (); //There are bytes to be read now

			}
		}

		public void read(ref float[] data, int count){
			int index = 0;
			Boolean blocked = false;
			while (index < count) {
				lock (this) {
					blocked = (available == 0) ? true : false;
				}
				while (blocked) {
					sem_read.Wait ();
					lock (this) {
						blocked = (available == 0) ? true : false;
					}
				}
				lock (this) {
					//Calculate how much data we can copy in one go:
					//If we have more data to write than fits between the current position, we write it in two chunks
					//We can at most write (buffer_size - available) bytes
					int to_read = (available >= count - index) ? count-index : available;
					to_read = (buffer_size - position_read > to_read) ? to_read : buffer_size - position_read; //We can't read over array bounds
					Array.ConstrainedCopy(buffer,position_read,data,index,to_read);
					position_read += to_read;//Move the pointers forwards in both arrays
					index += to_read;
					available -= to_read;
					if (position_read == buffer_size) { //Wrap the pointer of the ringbuffer around
						position_read = 0;
					}
				}
				sem_write.Release ();
			}
		}
	}
}

