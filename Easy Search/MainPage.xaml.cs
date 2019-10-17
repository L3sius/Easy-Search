using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Easy_Search
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            string query = entry.Text.Replace(' ', '+');
            string html = GetHtmlCode(query);
            List<string> urls = GetPictureUrls(html);
            label1.Text = getDescription(entry.Text).Split(new[] { "\n" },StringSplitOptions.None)[0];
            label2.Text = getDescription(entry.Text).Split(new[] { "\n" }, StringSplitOptions.None)[1];
            System.Diagnostics.Debug.WriteLine(urls[0]);
            Device.BeginInvokeOnMainThread(() =>
            {
                face.Source = "";
                face.Source = ImageSource.FromUri(new Uri(urls[0]));
            });
            //await Navigation.PushAsync(new SecondPage());
        }
        public string getDescription(string name)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            string url = "https://en.wikipedia.org/wiki/";
            foreach (string str in name.Split(' '))
                url += str + "_";
            string data = "";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

            var response = (HttpWebResponse)request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return "";
                using (var sr = new StreamReader(dataStream))
                {
                    data = sr.ReadToEnd();
                }
            }

            string desc = data.Split(new[] { "class=\"mw-parser-output\"" }, StringSplitOptions.None)[1];
            desc = desc.Split(new[] { "<div id=\"toc" }, StringSplitOptions.None)[0];
            desc = "<div" + desc.Substring(1);
            desc = desc.Substring(desc.IndexOf("<p><b>"));
            string regex = "(<.*?>)";
            desc = Regex.Replace(desc, regex, "");
            return desc;
        }

        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public string GetHtmlCode(string query)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            string url = "http://www.google.com/search?q=" + query + "&tbm=isch";
            string data = "";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

            var response = (HttpWebResponse)request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return "";
                using (var sr = new StreamReader(dataStream))
                {
                    data = sr.ReadToEnd();
                }
            }
            return data;
        }

        public List<string> GetPictureUrls(string html)
        {
            var urls = new List<string>();

            int ndx = html.IndexOf("\"ou\"", StringComparison.Ordinal);

            while (ndx >= 0)
            {
                ndx = html.IndexOf("\"", ndx + 4, StringComparison.Ordinal);
                ndx++;
                int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
                string url = html.Substring(ndx, ndx2 - ndx);
                urls.Add(url);
                ndx = html.IndexOf("\"ou\"", ndx2, StringComparison.Ordinal);
            }
            return urls;
        }
    }
}
