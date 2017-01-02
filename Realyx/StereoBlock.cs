using System;

namespace Realyx
{
	public interface StereoBlock
	{
		StereoAudioFrame get();

		bool hasEnded();
	}
}

