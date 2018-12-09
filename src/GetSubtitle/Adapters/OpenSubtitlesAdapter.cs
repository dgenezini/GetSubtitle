using GetSubtitle.Adapters.Interfaces;
using OSDBnet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GetSubtitle.Adapters
{
    public class OpenSubtitlesAdapter : ISubtitleAPIAdapter
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

                try
                {
                    subtitles = osdb.SearchSubtitlesFromFile(LanguageCode, filename).Result;
                }
                catch
                {
                    return Task.FromResult(false);
                }

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
