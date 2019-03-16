using CommandLine;
using GetSubtitle.Adapters;
using GetSubtitle.Adapters.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GetSubtitle
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ContextMenuCmdParams, DownloadCmdParams>(args)
                .WithParsed<ContextMenuCmdParams>(options => ContextMenu(options))
                .WithParsed<DownloadCmdParams>(async options => await Download(options))
                .WithNotParsed(error => { });
        }

        private static void ContextMenu(ContextMenuCmdParams options)
        {
            if (options.RemoveContextMenu)
            {
                WinContextMenu.Remove(options);
            }
            else
            {
                WinContextMenu.Add(options);
            }

            Console.WriteLine("Finished.");
            Console.ReadKey();
        }

        private static async Task Download(DownloadCmdParams options)
        {
            List<ISubtitleAPIAdapter> adapters = new List<ISubtitleAPIAdapter>();
            adapters.Add(new OpenSubtitlesAdapter());
            adapters.Add(new SubDBAdapter());

            if (Directory.Exists(options.Path))
            {
                Console.WriteLine($"Searching subtitles for:");
                Console.WriteLine(options.Path);

                SearchOption searchOption;

                if (options.Recursive)
                {
                    searchOption = SearchOption.AllDirectories;
                }
                else
                {
                    searchOption = SearchOption.TopDirectoryOnly;
                }

                List<string> files = Directory.GetFiles(options.Path, "*.*", searchOption).ToList();
                files = files.Where(a => Configurations.Get().FileExtensions.Contains(Path.GetExtension(a))).ToList();

                foreach (var file in files)
                {
                    bool dowloaded = await DownloadSubtitle(adapters, file, options.Overwrite,
                       options.LanguageCode);

                    if ((!dowloaded) && (!string.IsNullOrEmpty(options.FallbackLangCode)))
                    {
                        dowloaded = await DownloadSubtitle(adapters, file, options.Overwrite,
                            options.FallbackLangCode);
                    }

                    if (!dowloaded)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{Path.GetFileName(file)}: Subtitle not found ({options.LanguageCode}).");
                    }
                }

                Console.ResetColor();
                Console.WriteLine("Finished.");
                Console.ReadKey();
            }
            else if (File.Exists(options.Path))
            {
                Console.WriteLine($"Searching subtitles for:");
                Console.WriteLine(options.Path);

                bool dowloaded = await DownloadSubtitle(adapters, options.Path, options.Overwrite,
                    options.LanguageCode);

                if ((!dowloaded) && (!string.IsNullOrEmpty(options.FallbackLangCode)))
                {
                    dowloaded = await DownloadSubtitle(adapters, options.Path, options.Overwrite,
                        options.FallbackLangCode);
                }

                if (!dowloaded)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{Path.GetFileName(options.Path)}: Subtitle not found ({options.LanguageCode}).");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid path: {options.Path}.");
                Console.ReadKey();
            }
        }

        private async static Task<bool> DownloadSubtitle(List<ISubtitleAPIAdapter> adapters, string Filename,
            bool Overwrite, string LanguageCode)
        {
            var cultureInfo = new CultureInfo(LanguageCode);
            string SubtitlePath = Path.ChangeExtension(Filename, ".srt");

            if ((Overwrite) || (!File.Exists(SubtitlePath)))
            {
                foreach (var adapter in adapters)
                {
                    bool Found = await adapter.DownloadSubtitleAsync(Filename, cultureInfo);

                    if (Found)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"{Path.GetFileName(Filename)}: Subtitle downloaded ({adapter.GetDisplayName()}/{LanguageCode}).");

                        return true;
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{Path.GetFileName(Filename)}: Subtitle found in path.");

                return true;
            }

            return false;
        }
    }
}