using SubDBSharp;
using SubDBSharp.Http;
using GetSubtitle.Adapters.Interfaces;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;

namespace GetSubtitle.Adapters
{
    public class SubDBAdapter : ISubtitleAPIAdapter
    {
        private const string DISPLAYNAME = "SubDB";
        private const string USERAGENT = "GetSubtitle";

        public Task<bool> DownloadSubtitleAsync(string filename, CultureInfo cultureInfo)
        {
            string LanguageCode = cultureInfo.TwoLetterISOLanguageName;

            using (var Client = new SubDBClient(new System.Net.Http.Headers.ProductHeaderValue(USERAGENT, "1.0")))
            {
                var Hash = Utils.GetMovieHash(filename);

                Response response = Client.DownloadSubtitleAsync(Hash, LanguageCode).Result;

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return Task.FromResult(false);
                }

                string path = Path.ChangeExtension(filename, "srt");

                File.WriteAllText(path, response.Body, System.Text.Encoding.UTF8);
                
                return Task.FromResult(true);
            }
        }

        public string GetDisplayName()
        {
            return DISPLAYNAME;
        }
    }
}
