using SubDBSharp;
using SubDBSharp.Http;
using GetSubtitle.Adapters.Interfaces;
using GetSubtitle.Adapters.POCO;
using System.IO;
using System.Threading.Tasks;

namespace GetSubtitle.Adapters
{
    public class SubDBAdapter : ISubtitleAPIAdapter
    {
        private const string DISPLAYNAME = "SubDB";
        private const string USERAGENT = "TemporaryUserAgent";

        public async Task<DownloadReturn> DownloadSubtitleAsync(string Filename, string LanguageCode)
        {
            using (var Client = new SubDBClient(new System.Net.Http.Headers.ProductHeaderValue(USERAGENT, "1.0")))
            {
                var Hash = Utils.GetMovieHash(Filename);

                Response response = Client.DownloadSubtitleAsync(Hash, LanguageCode).Result;

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new DownloadReturn()
                    {
                        Found = false,
                        Message = "Sorry, no subtitle found"
                    };
                }

                string path = Path.ChangeExtension(Filename, "srt");

                File.WriteAllText(path, response.Body, System.Text.Encoding.UTF8);
                
                return new DownloadReturn()
                {
                    Found = true
                };
            }
        }

        public string GetDisplayName()
        {
            return DISPLAYNAME;
        }
    }
}
