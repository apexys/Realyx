using System;

namespace Realib
{
	public class AudioMetaData
	{
		public string[] artists;
		public string album_artist;
		public string album;
		public string title;
		public string track;
		public string path;
		public string genre;
		public string date;
		public float length;
		public float position;


		public void validate(){
			if(this.album_artist == null){
				this.album_artist = this.artists[0];
			}
		}

		public AudioMetaData ()
		{
		}
	}
}

