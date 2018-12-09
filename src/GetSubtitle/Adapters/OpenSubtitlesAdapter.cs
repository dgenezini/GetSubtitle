using OSDBnet;
using GetSubtitle.Adapters.Interfaces;
using GetSubtitle.Adapters.POCO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GetSubtitle.Adapters
{
    public class OpenSubtitlesAdapter: ISubtitleAPIAdapter
    {
        private const string DISPLAYNAME = "OpenSubtitles";
        private const string USERAGENT = "TemporaryUserAgent";

        public Task<DownloadReturn> DownloadSubtitleAsync(string Filename, string LanguageCode)
        {
            using (var osdb = Osdb.Create(USERAGENT))
            {
                IList<Subtitle> subtitles = null;

                subtitles = osdb.SearchSubtitlesFromFile(LanguageCode, Filename).Result;

                int subtitlesCount = subtitles.Count;
                if (subtitlesCount == 0)
                {
                    return Task.FromResult(new DownloadReturn()
                    {
                        Found = false,
                        Message = "Sorry, no subtitle found"
                    });
                }

                var selectedSubtitle = subtitles.First();

                string SubtitleFilename = Path.ChangeExtension(Filename, "srt");

                string subtitleFile = osdb.DownloadSubtitleToPath(Path.GetDirectoryName(Filename), selectedSubtitle, SubtitleFilename).Result;

                return Task.FromResult(new DownloadReturn()
                {
                    Found = true
                });
            }
        }

        public string GetDisplayName()
        {
            return DISPLAYNAME;
        }
    }
}
