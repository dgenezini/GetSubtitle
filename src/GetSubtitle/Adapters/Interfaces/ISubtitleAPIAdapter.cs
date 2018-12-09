using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace GetSubtitle.Adapters.Interfaces
{
    public interface ISubtitleAPIAdapter
    {
        string GetDisplayName();
        Task<bool> DownloadSubtitleAsync(string filename, CultureInfo cultureInfo);
    }
}
