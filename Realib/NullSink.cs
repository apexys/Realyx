using System;
using System.Diagnostics;

namespace Realib
{
	public class NullSink : StereoSink, ClockedStructure
	{
		StereoBlock source;

		public NullSink ()
		{
		}

		public void configure (StereoBlock source)
		{
			this.source = source;
		}

		public void tick(){
			this.source.get ();
		}

		public void Play ()
		{
			while (true /*!source.hasEnded ()*/) {
				this.source.get ();
			}
		}
	}
	
}
