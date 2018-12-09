using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetSubtitle
{
    [Verb("c", HelpText = "Add context menu entries on Windows.")]
    class ContextMenuCmdParams
    {
        [Option('u', "RemoveContextMenu", HelpText = "Removes context menu entries of Windows.")]
        public bool RemoveContextMenu { get; set; }
        
        [Option('l', "LanguageCode", HelpText = "Language Code for subtitles.", Required = true)]
        public string LanguageCode { get; set; }

        [Option('f', "FallbackLangCode", HelpText = "Fallback Language Code for subtitles.")]
        public string FallbackLangCode { get; set; }

        [Option('v', "Verbose", HelpText = "Use detailed messages.")]
        public bool Verbose { get; set; }

        [Option('o', "Overwrite", HelpText = "Overwrite existing subtitles.")]
        public bool Overwrite { get; set; }

        [Option('r', "Recursive", HelpText = "Recursive.")]
        public bool Recursive { get; set; }

        [Option('m', "MenuDescription", HelpText = "Menu Description.")]
        public string MenuDescription { get; set; }
    }

}
