using System;
using Realib;

namespace Reamix
{
	public class MultiFileSource : StereoBlock
	{
		protected FFMpegSource fs1;
		public bool Paused;
		public double Position;

		public MultiFileSource ()
		{
		}

		public void setFile(string filename){
			fs1 = new FFMpegSource (filename);
		}

		public void skipTo(double target_position){
			while (Position < target_position) {
				StereoAudioFrame result = fs1.get ();
				Position = result.start;
			}
		}

		public StereoAudioFrame get(){
			if (Paused) {
				return StereoAudioFrame.Zero;
			} else {
				StereoAudioFrame result = fs1.get ();
				Position = result.start;
				return result;
			}

		}
	}
}

