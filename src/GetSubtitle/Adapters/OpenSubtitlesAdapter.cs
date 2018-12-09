using OSDBnet;
using GetSubtitle.Adapters.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace GetSubtitle.Adapters
{
    public class OpenSubtitlesAdapter: ISubtitleAPIAdapter
    {
        private const string DISPLAYNAME = "OpenSubtitles";
        private const string USERAGENT = "GetSubtitle";

        public Task<bool> DownloadSubtitleAsync(string filename, CultureInfo cultureInfo)
        {
            string LanguageCode = cultureInfo.ThreeLetterISOLanguageName;

            if (LanguageCode == "por")
            {
                LanguageCode = "pob";
            }

            using (var osdb = Osdb.Create(USERAGENT))
            {
                IList<Subtitle> subtitles = null;

                subtitles = osdb.SearchSubtitlesFromFile(LanguageCode, filename).Result;

                //int subtitlesCount = subtitles.Count;
                var selectedSubtitle = subtitles.FirstOrDefault();

                if ((selectedSubtitle == null) || 
                    (selectedSubtitle.LanguageId != LanguageCode))
                {
                    return Task.FromResult(false);
                }
                
                string SubtitleFilename = Path.ChangeExtension(filename, "srt");

                string subtitleFile = osdb.DownloadSubtitleToPath(Path.GetDirectoryName(filename), selectedSubtitle, SubtitleFilename).Result;

                return Task.FromResult(true);
            }
        }

        public string GetDisplayName()
        {
            return DISPLAYNAME;
        }
    }
}
