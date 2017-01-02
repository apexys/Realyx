using System;

namespace Realyx
{
	public interface StereoSink
	{
		void configure (StereoBlock source);
		void Play();

	}
}

