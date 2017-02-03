using System;
using Realib;

namespace Reamix
{
	public class MultiFileSource : StereoBlock
	{
		public MultiFileSource ()
		{
		}

		public StereoAudioFrame get(){
			return StereoAudioFrame.Zero;
		}
	}
}

