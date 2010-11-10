using System;
using System.IO;
using System.Net;

namespace FakeNews
{
	public class _Default : System.Web.UI.Page
	{
		static DateTime ExpiresDate;
		static readonly object _cacheLock = new object();
		protected System.Web.UI.WebControls.Label path;

		protected void Page_Load(object sender, EventArgs e)
		{
			lock (_cacheLock) {
				if ((DateTime.Now > ExpiresDate) || (null == Cache["homepage"])) {
					string newscomauUrl = "http://www.news.com.au";
					WebRequest req = WebRequest.Create(newscomauUrl);
					req.Method = "GET";
					using (WebResponse res = req.GetResponse()) {
						using (Stream responseStream = res.GetResponseStream()) {
							using (StreamReader sr = new StreamReader(responseStream)) {
								Cache["homepage"] = sr.ReadToEnd();
							}
						}
					}
					ExpiresDate = DateTime.Now.AddMinutes(5);
				}
			}
            
			Response.Clear();
			Response.CacheControl = "no-cache";
			Response.Write(new HomePage().Build(Cache["homepage"].ToString()));
			Response.End();
		}
	}
}