using System;

using System.IO;

namespace Realyx
{
	public class PCMSource : StereoBlock
	{
		Stream sourceStream;
		Func<bool> EndOfStream;
		StereoAudioFrame saf;
		byte[] buffer;
		int overlap;
		int number = 0;
		const int fullBufferLen = StereoAudioFrame.MAXLEN * 4 * 2;//Maximum buffer length * 4 Bytes in a float * 2 Channels
		public PCMSource (Stream sourceStream, Func<bool> EndOfStream)
		{
			this.sourceStream = sourceStream;
			this.EndOfStream = EndOfStream;
			saf = new StereoAudioFrame ();
			buffer = new byte[fullBufferLen * 2];//full Buffer length  * enough space to read twice
			overlap = 0;
			br = new BinaryReader (sourceStream);
		}

		public bool hasEnded(){
			return EndOfStream();
		}

		public bool isActive(){
			return hasEnded();
		}

		BinaryReader br;
		public StereoAudioFrame get(){
			for (int i = 0; i < StereoAudioFrame.MAXLEN; i++) {
				saf.left_data [i] = br.ReadSingle ();
				saf.right_data [i] = br.ReadSingle ();
				if (EndOfStream ()) {
					break;
				}
			}
			saf.number = number;
			number++;
			return saf;
		}

		public StereoAudioFrame get2(){
			//Read until we have at least SAF.MAXLEN * 4 * 2 bytes in our buffer
			int bytesRead = overlap;
			while (bytesRead < fullBufferLen && (! EndOfStream())) { //Read until buffer is at least half full
				bytesRead += sourceStream.Read(buffer, bytesRead, fullBufferLen);
			}



			int index = 0;
			float value = 0;
			int leftindex = 0, rightindex = 0;
			int float_index = 0;
			bool leftNext = true;

			while (((index + 8) <= bytesRead) && (float_index < StereoAudioFrame.MAXLEN)) { //Read a sample from both channels
				value = BitConverter.ToSingle (buffer, index);
				saf.left_data [float_index] = value;
				index += 4;
				value = BitConverter.ToSingle (buffer, index);
				saf.right_data [float_index] = value;
				index += 4;
				float_index++;
			}
		
			//And now for possible overlap
			if (index < bytesRead) {
				Array.ConstrainedCopy (buffer, index, buffer, 0, bytesRead - index);
				overlap = bytesRead - index;
			}

			/*
			while ((((!leftNext) && index + 4 < bytesRead) || (leftNext && index + 8 < bytesRead)) && leftindex < StereoAudioFrame.MAXLEN && rightindex < StereoAudioFrame.MAXLEN) {
				value = BitConverter.ToSingle (buffer, index);
				if (leftNext) {
					saf.left_data [leftindex] = value;
					leftindex++;
					leftNext = false;
				} else {
					saf.right_data [rightindex] = value;
					rightindex ++;
					leftNext = true;
				}
				index += 4;
			}

			saf.length = rightindex;


			overlap = 0;
			while (index < bytesRead) {
				buffer [overlap] = buffer [index];
				overlap++;
				index++;
			}*/

			saf.start += saf.length;
			saf.length = (double)StereoAudioFrame.MAXLEN / (double)48000;

			return saf;
		}


	}
}

