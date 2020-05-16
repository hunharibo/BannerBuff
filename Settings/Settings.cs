using ModLib.Definitions;
using ModLib.Definitions.Attributes;
using System.Xml.Serialization;

namespace BannerBuff.Settings
{
    class Settings : SettingsBase
    {
        [XmlElement]
        public override string ID { get; set; } = "mod.settings.bannerbuff";

        public override string ModuleFolderName => BannerBuffModule.ModuleDirectoryName;

        public override string ModName => BannerBuffModule.ModuleName;

        public static Settings Instance
        {
            get
            {
                return (Settings)SettingsDatabase.GetSettings<Settings>();
            }
        }

        [XmlElement]
        [SettingProperty("Some Multiplier", 1f, 3f, "This is a multiplier")]
        [SettingPropertyGroup("Group 1")]
        public float SomeMultiplier { get; set; } = 1.5f;
        [XmlElement]
        [SettingProperty("Some multiplier Enabled", "Enables SomeMultiplier")]
        [SettingPropertyGroup("Group 1")]
        public bool MultiplierEnabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("Some Other Multiplier", 1f, 3f, "This is another multiplier")]
        [SettingPropertyGroup("Group 2")]
        public float SomeOtherMultiplier { get; set; } = 1.5f;
    }
}
