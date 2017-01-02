using System;

namespace Realyx
{
	public interface StereoEffect
	{
		void process(ref StereoAudioFrame saf);

		void connect(StereoEffect input);
	}
}

