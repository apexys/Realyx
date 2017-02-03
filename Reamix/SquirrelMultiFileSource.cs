using System;
using System.Net;
using Realib;

namespace Reamix
{
	public class SquirrelMultiFileSource : MultiFileSource
	{
		string squirrel_address;
		public SquirrelMultiFileSource (string squirrel_address)
		{
			this.squirrel_address = squirrel_address;
			if (!this.squirrel_address.EndsWith ("/")) {
				this.squirrel_address += "/";
			}
		}

		public void setID(int id){
			HttpWebRequest wr = WebRequest.CreateHttp(squirrel_address + "file/" + id.ToString());
			WebResponse webresp = wr.GetResponse ();
			if (((HttpWebResponse)webresp).StatusCode != HttpStatusCode.OK) {
				throw new Exception (((HttpWebResponse)webresp).StatusDescription);
			}

			fs1 = new FFMpegSource (webresp.GetResponseStream());
		}


	}
}

