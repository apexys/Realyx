using System;

namespace Realib
{
	public interface StereoBlock
	{
		StereoAudioFrame get();

		bool hasEnded();

		bool isActive();
	}
}

