using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;

namespace Reveal_Last_Alive.Config
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RangeAttribute : Attribute
    {
        public int Min { get; }
        public int Max { get; }
        public int Default { get; }
        public string Message { get; }

        public RangeAttribute(int min, int max, int defaultValue, string message)
        {
            Min = min;
            Max = max;
            Default = defaultValue;
            Message = message;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CommentAttribute : Attribute
    {
        public string Comment { get; }

        public CommentAttribute(string comment)
        {
            Comment = comment;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class BreakLineAttribute : Attribute
    {
        public string BreakLine { get; }

        public BreakLineAttribute(string breakLine)
        {
            BreakLine = breakLine;
        }
    }
    public static class Configs
    {
        private static readonly string ConfigDirectoryName = "config";
        private static readonly string ConfigFileName = "config.json";
        private static readonly string PrecacheResources = "ServerPrecacheResources.txt";
        private static string? _PrecacheResources;

        private static string? _configFilePath;
        private static ConfigData? _configData;

        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter()
            },
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static bool IsLoaded()
        {
            return _configData is not null;
        }

        public static ConfigData GetConfigData()
        {
            if (_configData is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            
            return _configData;
        }

        public static ConfigData Load(string modulePath)
        {
            var configFileDirectory = Path.Combine(modulePath, ConfigDirectoryName);
            if(!Directory.Exists(configFileDirectory))
            {
                Directory.CreateDirectory(configFileDirectory);
            }

            _PrecacheResources = Path.Combine(configFileDirectory, PrecacheResources);
            Helper.CreateResource(_PrecacheResources);

            _configFilePath = Path.Combine(configFileDirectory, ConfigFileName);
            if (File.Exists(_configFilePath))
            {
                _configData = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(_configFilePath), SerializationOptions);
                _configData!.Validate();
            }
            else
            {
                _configData = new ConfigData();
                _configData.Validate();
            }

            if (_configData is null)
            {
                throw new Exception("Failed to load configs.");
            }

            SaveConfigData(_configData);
            
            return _configData;
        }

        private static void SaveConfigData(ConfigData configData)
        {
            if (_configFilePath is null)
                throw new Exception("Config not yet loaded.");

            string json = JsonSerializer.Serialize(configData, SerializationOptions);
            json = Regex.Unescape(json);

            var lines = json.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var newLines = new List<string>();

            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"^\s*""(\w+)""\s*:.*");
                bool isPropertyLine = false;
                PropertyInfo? propInfo = null;

                if (match.Success)
                {
                    string propName = match.Groups[1].Value;
                    propInfo = typeof(ConfigData).GetProperty(propName);

                    var breakLineAttr = propInfo?.GetCustomAttribute<BreakLineAttribute>();
                    if (breakLineAttr != null)
                    {
                        string breakLine = breakLineAttr.BreakLine;

                        if (breakLine.Contains("{space}"))
                        {
                            breakLine = breakLine.Replace("{space}", "").Trim();

                            if (breakLineAttr.BreakLine.StartsWith("{space}"))
                            {
                                newLines.Add("");
                            }

                            newLines.Add("// " + breakLine);
                            newLines.Add("");
                        }
                        else
                        {
                            newLines.Add("// " + breakLine);
                        }
                    }

                    var commentAttr = propInfo?.GetCustomAttribute<CommentAttribute>();
                    if (commentAttr != null)
                    {
                        var commentLines = commentAttr.Comment.Split('\n');
                        foreach (var commentLine in commentLines)
                        {
                            newLines.Add("// " + commentLine.Trim());
                        }
                    }

                    isPropertyLine = true;
                }

                newLines.Add(line);

                if (isPropertyLine && propInfo?.GetCustomAttribute<CommentAttribute>() != null)
                {
                    newLines.Add("");
                }
            }

            var adjustedLines = new List<string>();
            foreach (var line in newLines)
            {
                adjustedLines.Add(line);
                if (Regex.IsMatch(line, @"^\s*\],?\s*$"))
                {
                    adjustedLines.Add("");
                }
            }

            File.WriteAllText(_configFilePath, string.Join(Environment.NewLine, adjustedLines), Encoding.UTF8);
        }

        public class ConfigData
        {
            private string? _Version;
            private string? _Link;
            [BreakLine("----------------------------[ ↓ Plugin Info ↓ ]----------------------------{space}")]
            public string Version
            {
                get => _Version!;
                set
                {
                    _Version = value;
                    if (_Version != MainPlugin.Instance.ModuleVersion)
                    {
                        Version = MainPlugin.Instance.ModuleVersion;
                    }
                }
            }

            public string Link
            {
                get => _Link!;
                set
                {
                    _Link = value;
                    if (_Link != "https://github.com/oqyh/cs2-Reveal-Last-Alive-GoldKingZ")
                    {
                        Link = "https://github.com/oqyh/cs2-Reveal-Last-Alive-GoldKingZ";
                    }
                }
            }

            [BreakLine("{space}----------------------------[ ↓ Main Config ↓ ]----------------------------{space}")]

            [Comment("Reveal Last Player Alive On Team:\n1 = CT\n2 = T")]
            [Range(1, 2, 1, "[Last Alive] RevealLastPlayerOnTeam: is invalid, setting to default value (1) Please Choose From 1 To 2.\n[Last Alive] 1 = CT\n[Last Alive] 2 = T")]
            public int RevealLastPlayerOnTeam { get; set; }

            [Comment("Play Sound On Reveal\nExample:\n sounds/ui/competitive_accept_beep.vsnd (Will Not Support Volume)\n UIPanorama.popup_accept_match_beep (Support Volume)\n\"\" = Disable")]
            public string Play_Sound { get; set; }

            [Comment("Required [Play_Sound Not Start With sounds/]\n Sound Volume Of Play_Sound From 0 to 100")]
            public string Sound_Volume { get; set; }


            [BreakLine("{space}----------------------------[ ↓ Chicken Config ↓ ]----------------------------{space}")]
            [Comment("Enable Chicken On Last Player Alive?\ntrue = Yes\nfalse = No")]
            public bool Chicken_Enable { get; set; }

            [Comment("Required [Chicken_Enable = true]\nGlow Only When Crosshair Near To Chicken?\ntrue = Yes\nfalse = No (Show All The Time)")]
            public bool Chicken_GlowType { get; set; }

            [Comment("Required [Chicken_Enable = true]\nWhats Max Range To Show Chicken Glow")]
            public int Chicken_GlowRange { get; set; }


            [Comment("Required [Chicken_Enable = true]\nChicken Size")]
            public int Chicken_Size { get; set; }

            [Comment("Required [Chicken_Enable = true]\nHow Would You Like Chicken Glow Color To Be Use This Site (https://htmlcolorcodes.com/color-picker) For Color Pick")]
            public string Chicken_GlowColor { get; set; }            



            [BreakLine("{space}----------------------------[ ↓ Player Glow Config ↓ ]----------------------------{space}")]
            [Comment("Enable Player Glow On Last Player Alive?\ntrue = Yes\nfalse = No")]
            public bool Player_Enable { get; set; }

            [Comment("Required [Player_Enable = true]\nGlow Only When Crosshair Near To Player Glow?\ntrue = Yes\nfalse = No (Show All The Time)")]
            public bool Player_GlowType { get; set; }

            [Comment("Required [Player_Enable = true]\nWhats Max Range To Show Player Glow")]
            public int Player_GlowRange { get; set; }

            [Comment("Required [Player_Enable = true]\nHow Would You Like Player Glow Color To Be Use This Site (https://htmlcolorcodes.com/color-picker) For Color Pick")]
            public string Player_GlowColor { get; set; }     

            [BreakLine("{space}----------------------------[ ↓ Utilities  ↓ ]----------------------------{space}")]

            [Comment("Auto Update Signatures (In ../Reveal-Last-Alive-GoldKingZ/gamedata/)?\ntrue = Yes\nfalse = No")]
            public bool AutoUpdateSignatures { get; set; }

            [Comment("Enable Debug Plugin In Server Console (Helps You To Debug Issues You Facing)?\ntrue = Yes\nfalse = No")]
            public bool EnableDebug { get; set; }

            public ConfigData()
            {
                Version = MainPlugin.Instance.ModuleVersion;
                Link = "https://github.com/oqyh/cs2-Reveal-Last-Alive-GoldKingZ";

                RevealLastPlayerOnTeam = 1;
                Play_Sound = "UIPanorama.popup_accept_match_beep";
                Sound_Volume = "10%";

                Chicken_Enable = true;
                Chicken_GlowType = false;
                Chicken_GlowRange = 5000;
                Chicken_Size = 10;
                Chicken_GlowColor = "#14ff00";

                Player_Enable = true;
                Player_GlowType = false;
                Player_GlowRange = 5000;
                Player_GlowColor = "#14ff00";

                AutoUpdateSignatures = true;
                EnableDebug = false;
            }
            public void Validate()
            {
                foreach (var prop in GetType().GetProperties())
                {
                    var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
                    if (rangeAttr != null && prop.PropertyType == typeof(int))
                    {
                        int value = (int)prop.GetValue(this)!;
                        if (value < rangeAttr.Min || value > rangeAttr.Max)
                        {
                            prop.SetValue(this, rangeAttr.Default);
                            Helper.DebugMessage(rangeAttr.Message,false);
                        }
                    }
                }
            }
        }
    }
}
