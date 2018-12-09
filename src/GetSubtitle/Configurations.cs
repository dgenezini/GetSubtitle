using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetSubtitle
{
    class Configurations
    {
        private static Configurations _Configs { get; set; }

        public string[] FileExtensions { get; set; }

        public static Configurations Get()
        {
            if (_Configs == null)
            {
                string ConfigPath = Path.Combine(AppContext.BaseDirectory, "config.json");

                _Configs = JsonConvert.DeserializeObject<Configurations>(File.ReadAllText(ConfigPath));
            }

            return _Configs;
        }
    }
}
