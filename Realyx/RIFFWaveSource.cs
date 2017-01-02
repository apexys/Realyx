using System;
using System.IO;

namespace Realyx
{
	public class RIFFWaveSource : StereoBlock
	{
		Stream sourceStream;
		Func<bool> EndOfStream;
		StereoAudioFrame saf;
		byte[] buffer;
		int overlap;
		bool initalized = false;
		const int fullBufferLen = StereoAudioFrame.MAXLEN * 4 * 2;//Maximum buffer length * 4 Bytes in a float * 2 Channels
		public RIFFWaveSource (Stream sourceStream, Func<bool> EndOfStream)
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

		BinaryReader br;


		uint framelen;
		int framepos = -1;
		uint chunks = 0;

		Func<int,char,int> transform = (s, a) => {
			switch (a) {
			case 'd':
				if (s == 0)
					return 1;
				else
					return 0;
				break;
			case 'a':
				switch (s) {
				case 1:
					return 2;
				case 3:
					return 4;
				default:
					return 0;
				}
				break;
			case 't':
				if (s == 2)
					return 3;
				else
					return 0;
				break;
			default: 
				return 0;
			}
		};

		public StereoAudioFrame get(){
			if (!initalized) {
				//1. Read header
				char[] RIFF = br.ReadChars (4);
				uint filesize = br.ReadUInt32 ();
				char[] WAVE = br.ReadChars (4);
				//2. Read fmt
				char[] fmt_ = br.ReadChars (4);
				uint fmtsize = br.ReadUInt32 ();
				ushort format = br.ReadUInt16 ();
				ushort channels = br.ReadUInt16 ();
				uint samplerate = br.ReadUInt32 ();
				uint bitspersecond = br.ReadUInt32 ();
				ushort blockalign = br.ReadUInt16 ();
				ushort bitspersample = br.ReadUInt16 ();
			}
			//3. Find data chunks
			int counter = 0;
			while (counter < StereoAudioFrame.MAXLEN) {
				if (framepos == -1) {
					//Statemachines!
					var state = 0; //0->d, 1->a, 2->t, 3->a

						//a) find chunk
						while (state != 4) {
							if (!EndOfStream ()) {
								char c = (char)br.ReadByte ();
								state = transform (state, c);
							} else {
								while (counter < StereoAudioFrame.MAXLEN) {
									saf.left_data [counter] = 0;
									saf.right_data [counter] = 0;
									counter++;
								}
								return saf;
							}
						}
						chunks++;
						framepos = 0;
						framelen = br.ReadUInt32 ();
				}
				saf.left_data [counter] = br.ReadSingle ();
				saf.right_data [counter] = br.ReadSingle ();
				framepos += 8;
				counter++;
				if (framepos == framelen) {
					framepos = -1;
				}
			}
			return saf;
		}




	}
}

