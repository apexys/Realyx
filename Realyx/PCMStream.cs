using System;
using System.Threading;
using System.Collections;

namespace Realyx
{
	/// <summary>
	/// Stream-like implementation of a decoder for 32-bit float PCM into float
	/// </summary>
	public class PCMStream
	{
		SemaphoreSlim sem_write;
		SemaphoreSlim sem_read;
		byte[] input_buffer;
		float[] output_buffer;
		int output_available;
		int input_available;
		int buffer_size;
		Boolean closed;

		/// <summary>
		/// Initializes a new instance of the <see cref="Realyx.PCMStream"/> class.
		/// </summary>
		/// <param name="buffer_size">Output buffer size</param>
		public PCMStream (int buffer_size = 8192)
		{
			sem_write = new SemaphoreSlim (0);
			sem_read = new SemaphoreSlim (0);
			this.buffer_size = buffer_size;
			input_buffer = new byte[4];
			output_buffer = new float[buffer_size];
			output_available = 0;
			input_available = 0;
			closed = false;
		}

		/// <summary>
		/// Tries to write data into the stream
		/// </summary>
		/// <returns>The number of bytes that were actually written</returns>
		/// <param name="pcm_data">Pcm data.</param>
		/// <param name="count">Count.</param>
		public int try_write(byte[] pcm_data, int count, int index){
			int initial_index = index;
			if (closed) {
				throw new InvalidOperationException ("Attempted to write data to closed stream");
			}

			if (input_available > 0) { //We still have data from the last byte packet
				while(input_available < 4){//So we copy over new data until we can read a byte
					input_buffer[input_available] = pcm_data [index]; //Copy over value
					index++;//Increase pointers
					input_available++;
				}
		
				//if we can't write the value, return how many bytes we have read so far
				if (output_available == buffer_size) {
					return index - initial_index;
				}

				lock(output_buffer){
					output_buffer [output_available] = System.BitConverter.ToSingle (input_buffer, 0); //Write the decoded output
					output_available ++;//Increase output array pointer
					sem_read.Release();//Signal that there is new data
					input_available = 0;//Reset input array pointer
				}
			}

			//Decode bytes from input
			while (index + 4 < count) {//As long as we can read at least four bytes from this position
				//Wait until we can write the decoded value
				if (output_available == buffer_size) {
					return index - initial_index;
				}
				lock(output_buffer){
					output_buffer [output_available] = System.BitConverter.ToSingle (pcm_data, index); //Write the decoded output
					output_available += 4;//Increase output array pointer
					sem_read.Release();//Signal that there is new data
					index+= 4; //Increase input pointer
				}
			}

			//Store leftover bytes from input
			while (index < count) {
				input_buffer [input_available] = pcm_data [index];
				input_available++;
				index++;
			}
			return index - initial_index;
		}

		/// <summary>
		/// Writes raw byte data to the stream, decoding it to float. Might block if the internal buffer is full
		/// </summary>
		/// <param name="pcm_data">Raw PCM bytes</param>
		/// <param name="count">The number of bytes written</param>
		public void write (byte[] pcm_data, int count){
			if (closed) {
				throw new InvalidOperationException ("Attempted to write data to closed stream");
			}

			int index = 0;

			if (input_available > 0) { //We still have data from the last byte packet
				while(input_available < 4){//So we copy over new data until we can read a byte
					input_buffer [input_available] = pcm_data [index]; //Copy over value
					index++;//Increase pointers
					input_available++;
				}
				//Wait until we can write the decoded value
				while (output_available == buffer_size) {
					sem_write.Wait ();
				}
				lock(output_buffer){
					output_buffer [output_available] = System.BitConverter.ToSingle (input_buffer, 0); //Write the decoded output
					output_available ++;//Increase output array pointer
					sem_read.Release();//Signal that there is new data
					input_available = 0;//Reset input array pointer
				}
			}

			//Decode bytes from input
			while (index + 4 <= count) {//As long as we can read at least four bytes from this position
				//Wait until we can write the decoded value
				while (output_available == buffer_size) {
					sem_write.Wait ();
				}
				lock(output_buffer){
					output_buffer [output_available] = System.BitConverter.ToSingle (pcm_data, index); //Write the decoded output
					output_available += 4;//Increase output array pointer
					sem_read.Release();//Signal that there is new data
					index+= 4; //Increase input pointer
				}
			}

			//Store leftover bytes from input
			while (index < count) {
				input_buffer [input_available] = pcm_data [index];
				input_available++;
				index++;
			}
		}
			
		/// <summary>
		/// Close the stream, marks the end of the input
		/// </summary>
		public void close(){
			closed = true;
			sem_read.Release ();//There might be a read process waiting for data
		}

		/// <summary>
		/// Read the specified buffer and count.
		/// </summary>
		/// <param name="buffer">Buffer.</param>
		/// <param name="count">Count.</param>
		public int read(ref float[] buffer, int count){
			//We can at most read the number of bytes that fit into our buffer or, if the stream has ended, the number of bytes written so far
			if (count > buffer_size) {
				count = closed ? output_available : buffer_size;
			}
		

			//Wait until we can satisfy the request
			/*while (output_available < count) {
				sem_write.Release ();//Signal the input processing that it can store more data
				sem_read.Wait (); //Wait until new data is released
				if (closed) {
					count = output_available;
				}
			}*/
			//TODO: Fix the above, we'll just give out what we already have in the meantime
			count = output_available;
			while (output_available == 0) {
				sem_write.Release ();//Signal the input processing that it can store more data
				sem_read.Wait (); //Wait until new data is released
				count = output_available;
			}


			lock (output_buffer) {
				//Copy values over to output buffer	
				Array.Copy (output_buffer, buffer, count);
				//If we didn't read the complete input buffer, we need to shift the elements down
				if (count != buffer_size) {
					Array.ConstrainedCopy (output_buffer, count, output_buffer, 0, buffer_size - count);
				}
				output_available = output_available - count;//Reduce the number of available output by the amount read
				sem_write.Release ();//New data can be written now
			}
			//Return the number of bytes read
			return count;
		}
	}
}

