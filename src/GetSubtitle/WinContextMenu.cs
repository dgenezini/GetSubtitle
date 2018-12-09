using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GetSubtitle
{
    static class WinContextMenu
    {
        public static bool Add(ContextMenuCmdParams Params)
        {
            RegistryKey regKeyStorageKey = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    RegistryKey CurrentUserKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                    RegistryKey LocalMachineKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    RegistryKey ClassesRootKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64);

                    string CommandName = Params.LanguageCode;
                    string CommandLanguageDescription = Params.LanguageCode;
                    
                    if (!string.IsNullOrEmpty(Params.FallbackLangCode))
                    {
                        CommandName += "_" + Params.FallbackLangCode;
                        CommandLanguageDescription += "/" + Params.FallbackLangCode;
                    }

                    if (string.IsNullOrEmpty(Params.MenuDescription))
                    {
                        Params.MenuDescription = $"Download subtitles ({CommandLanguageDescription})";
                    }

                    //Add menu to CommandStore
                    regKeyStorageKey = LocalMachineKey.CreateSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\GetSubtitle.{Params.MenuDescription}");
                    regKeyStorageKey.SetValue(null, Params.MenuDescription);

                    string appPath;

                    if (Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") == "Development")
                    {
                        const string dotnetPath = @"C:\Program Files\dotnet\dotnet";

                        appPath = $"\"{dotnetPath}\" \"{System.Reflection.Assembly.GetExecutingAssembly().Location}\"";
                    }
                    else
                    {
                        appPath = $"\"{Path.ChangeExtension(System.Reflection.Assembly.GetExecutingAssembly().Location, ".exe")}\"";
                    }

                    string RegParams = $"dl -{CmdParams.PATH_SHORT} \"%v\" -{CmdParams.LANGUAGECODE_SHORT} {Params.LanguageCode}";

                    if (!string.IsNullOrEmpty(Params.FallbackLangCode))
                    {
                        RegParams += $" -{CmdParams.FALLBACKLANGCODE_SHORT} {Params.FallbackLangCode}";
                    }

                    if (Params.Overwrite)
                    {
                        RegParams += $" -{CmdParams.OVERWRITE_SHORT}";
                    }

                    if (Params.Verbose)
                    {
                        RegParams += $" -{CmdParams.VERBOSE_SHORT}";
                    }

                    if (Params.Recursive)
                    {
                        RegParams += $" -{CmdParams.RECURSIVE_SHORT}";
                    }

                    //Directory background
                    regKeyStorageKey = CurrentUserKey.CreateSubKey($@"Software\Classes\directory\Background\shell\GetSubtitle");
                    regKeyStorageKey.SetValue("MUIVerb", "GetSubtitle");

                    string SubCommands = (string)regKeyStorageKey.GetValue("SubCommands");

                    if (!string.IsNullOrEmpty(SubCommands))
                    {
                        SubCommands += ";";
                    }

                    if ((string.IsNullOrEmpty(SubCommands)) || (SubCommands?.IndexOf($"GetSubtitle.{Params.MenuDescription}") < 0))
                    {
                        SubCommands += $"GetSubtitle.{Params.MenuDescription}";
                    }

                    //Directory background
                    regKeyStorageKey.SetValue("SubCommands", SubCommands);

                    //Directory
                    regKeyStorageKey = CurrentUserKey.CreateSubKey($@"Software\Classes\directory\shell\GetSubtitle");
                    regKeyStorageKey.SetValue("MUIVerb", "GetSubtitle");
                    regKeyStorageKey.SetValue("SubCommands", SubCommands);
                    
                    foreach (var extension in Configurations.Get().FileExtensions)
                    {
                        //File
                        regKeyStorageKey = ClassesRootKey.CreateSubKey($@"SystemFileAssociations\{extension}\shell\GetSubtitle");
                        regKeyStorageKey.SetValue("MUIVerb", "GetSubtitle");
                        regKeyStorageKey.SetValue("SubCommands", SubCommands);
                    }

                    //Add command to CommandStore
                    regKeyStorageKey = LocalMachineKey.CreateSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\GetSubtitle.{Params.MenuDescription}\command");
                    regKeyStorageKey.SetValue(null, $"{appPath} {RegParams}");

                    Console.WriteLine("Context menu successfully added.");

                    return true;
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Need administrative privileges.");

                    return false;
                }
            }
            else
            {
                Console.WriteLine("Windows only.");

                return false;
            }
        }

        public static bool Remove(ContextMenuCmdParams Params)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    RegistryKey CurrentUserKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                    RegistryKey LocalMachineKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    RegistryKey ClassesRootKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64);

                    //TODO: Remove all from CommandStore

                    //Directory background
                    if (CurrentUserKey.OpenSubKey(@"Software\Classes\directory\Background\shell\GetSubtitle") != null)
                    {
                        CurrentUserKey.DeleteSubKeyTree(@"Software\Classes\directory\Background\shell\GetSubtitle");
                    }

                    //Directory
                    if (CurrentUserKey.OpenSubKey(@"Software\Classes\directory\shell\GetSubtitle") != null)
                    {
                        CurrentUserKey.DeleteSubKeyTree(@"Software\Classes\directory\shell\GetSubtitle");
                    }

                    //File
                    foreach (var extension in Configurations.Get().FileExtensions)
                    {
                        if (ClassesRootKey.OpenSubKey($@"SystemFileAssociations\{extension}\shell\GetSubtitle") != null)
                        {
                            ClassesRootKey.DeleteSubKeyTree($@"SystemFileAssociations\{extension}\shell\GetSubtitle");
                        }
                    }

                    Console.WriteLine("Context menu successfully removed.");

                    return true;
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Need administrative privileges.");

                    return false;
                }
            }
            else
            {
                Console.WriteLine("Windows only.");

                return false;
            }
        }
    }
}
