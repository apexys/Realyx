using System;

using System.IO;

namespace Realib
{
	public class PCMSource : StereoBlock
	{
		Stream sourceStream;
		Func<bool> EndOfStream;
		StereoAudioFrame saf;
		byte[] buffer;
		int overlap;
		int number = 0;
		bool ended = false;

		public double position;

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
			return EndOfStream() || ended;
		}

		public bool isActive(){
			return hasEnded();
		}

		BinaryReader br;
		public StereoAudioFrame get(){
			for (int i = 0; i < StereoAudioFrame.MAXLEN; i++) {
				try{
				saf.left_data [i] = br.ReadSingle ();
				saf.right_data [i] = br.ReadSingle ();
				}catch{
					ended = true;
					break;
				}
				if (EndOfStream ()) {
					break;
				}
			}
			saf.number = number;
			number++;
			this.position = saf.start;
			saf.start += saf.length;
			saf.length = (double)StereoAudioFrame.MAXLEN / (double)48000;
			return saf;
		}

	}
}

