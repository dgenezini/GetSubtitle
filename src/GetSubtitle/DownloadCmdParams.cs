﻿using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetSubtitle
{
    [Verb("dl", HelpText = "Download subtitles")]
    class DownloadCmdParams
    {
        [Option('p', "Path", HelpText = "Directory or file path to download subtitles for.", Required = true)]
        public string Path { get; set; }

        [Option('l', "LanguageCode", HelpText = "Language Code for subtitles. (en-US, pt-BR, etc)", Required = true)]
        public string LanguageCode { get; set; }

        [Option('f', "FallbackLangCode", HelpText = "Fallback Language Code for subtitles. (en-US, pt-BR, etc)")]
        public string FallbackLangCode { get; set; }

        [Option('v', "Verbose", HelpText = "Use detailed messages.")]
        public bool Verbose { get; set; }

        [Option('o', "Overwrite", HelpText = "Overwrite existing subtitles.")]
        public bool Overwrite { get; set; }

        [Option('r', "Recursive", HelpText = "Recursive.")]
        public bool Recursive { get; set; }
    }

}
