using System;

namespace Realyx
{
	public class PCMStereoStream
	{
		public AudioBufferStream left;
		public AudioBufferStream right;
		PCMStream raw;
		bool has_leftover;
		int output_buffer_size;
		float[] read_buffer;
		float[] left_buffer;
		float[] right_buffer;

		public PCMStereoStream (int output_buffer_size = 4096, int input_buffer_size = 8192)
		{
			this.output_buffer_size = output_buffer_size;
			left = new AudioBufferStream (output_buffer_size);
			right = new AudioBufferStream (output_buffer_size);
			read_buffer = new float[output_buffer_size];
			left_buffer = new float[output_buffer_size];
			right_buffer = new float[output_buffer_size];
			raw = new PCMStream (input_buffer_size);
			has_leftover = false;
		}

		public void write(byte[] data, int amount){
			int index = 0;
			while (index < amount) {
				index += raw.try_write (data, amount, 0);
				int floats_read = raw.read (ref read_buffer, output_buffer_size);
				int read_index = 0;
				int left_index = has_leftover ? 1: 0;
				int right_index = 0;
				while (read_index + 2 <= floats_read) {
					left_buffer[left_index] = read_buffer [read_index];
					left_index++;
					read_index++;
					right_buffer[right_index] = read_buffer [read_index];
					read_index++;
					right_index++;
				}
				left.write (left_buffer, left_index);
				right.write (right_buffer, right_index);
				if (read_index < floats_read) {
					left_buffer [0] = read_buffer [read_index];
				}
			}
		}

	}
}

