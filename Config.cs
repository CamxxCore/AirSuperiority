using System.Windows.Forms;
using AirSuperiority.Data;
using GTA;

namespace AirSuperiority
{
    public static class Config
    {
        public static readonly Keys ActivationKey = INIHelper.GetConfigSetting("KeyBinds", "ActivateScript", Keys.K),
            Ability1 = INIHelper.GetConfigSetting("KeyBinds", "EngineRepair", Keys.Z),
            Ability2 = INIHelper.GetConfigSetting("KeyBinds", "Countermeasures", Keys.X);

        public static readonly int MaxTeams = INIHelper.GetConfigSetting("General", "MaxTeams", 4),
            MaxPlayers = INIHelper.GetConfigSetting<int>("General", "MaxPlayers", 50);

        public static readonly GTA.Control GP_Ability1 = INIHelper.GetConfigSetting("ControllerBinds", "EngineRepair", GTA.Control.ScriptLB),
            GP_Ability2 = INIHelper.GetConfigSetting("ControllerBinds", "Countermeasures", GTA.Control.ScriptRB);
    }
}
