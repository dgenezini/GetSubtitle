using GetSubtitle.Adapters.Interfaces;
using SubDBSharp;
using SubDBSharp.Http;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

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

                try
                {
                    Response response = Client.DownloadSubtitleAsync(Hash, LanguageCode).Result;

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return Task.FromResult(false);
                    }

                    string path = Path.ChangeExtension(filename, "srt");

                    File.WriteAllText(path, response.Body, System.Text.Encoding.UTF8);

                    return Task.FromResult(true);
                }
                catch
                {
                    return Task.FromResult(false);
                }
            }
        }

        public string GetDisplayName()
        {
            return DISPLAYNAME;
        }
    }
}
