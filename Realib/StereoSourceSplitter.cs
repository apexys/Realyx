using System;
using Realib;

namespace Realib
{
	public class StereoSourceSplitter : ClockedStructure
	{
		StereoBlock source;
		public SplitSource Left;
		public SplitSource Right;

		public StereoSourceSplitter (StereoBlock source)
		{
			this.source = source;
			Left = new SplitSource ();
			Right = new SplitSource ();
		}

		public void tick(){
			StereoAudioFrame saf = source.get();
			Left.updateLeft(saf);
			Right.updateRight (saf);
		}

		public class SplitSource: StereoBlock{
			StereoAudioFrame saf = new StereoAudioFrame();
			public StereoAudioFrame get(){
				return saf;
			}

			public void updateLeft(StereoAudioFrame newsaf){
				Array.Copy (newsaf.left_data, this.saf.left_data, StereoAudioFrame.MAXLEN);
				Array.Copy (newsaf.left_data, this.saf.right_data, StereoAudioFrame.MAXLEN);
				this.saf.number = newsaf.number;
				this.saf.start = newsaf.start;
				this.saf.length = newsaf.length;
			}


			public void updateRight(StereoAudioFrame newsaf){
				Array.Copy (newsaf.right_data, this.saf.left_data, StereoAudioFrame.MAXLEN);
				Array.Copy (newsaf.right_data, this.saf.right_data, StereoAudioFrame.MAXLEN);
				this.saf.number = newsaf.number;
				this.saf.start = newsaf.start;
				this.saf.length = newsaf.length;
			}

			public bool hasEnded(){
				return false;
			}

			public bool isActive(){
				return false;
			}
		}
	}
}

