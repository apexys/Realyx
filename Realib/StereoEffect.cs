using System;

namespace Realib
{
	public interface StereoEffect
	{
		void process(ref StereoAudioFrame saf);
	}
}

