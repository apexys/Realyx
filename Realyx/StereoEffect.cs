using System;

namespace Realyx
{
	public interface StereoEffect
	{
		void process(ref StereoAudioFrame saf);
	}
}

