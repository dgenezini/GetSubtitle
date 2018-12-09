using GetSubtitle.Adapters.POCO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GetSubtitle.Adapters.Interfaces
{
    public interface ISubtitleAPIAdapter
    {
        string GetDisplayName();
        Task<DownloadReturn> DownloadSubtitleAsync(string Filename, string LanguageCode);
    }
}
