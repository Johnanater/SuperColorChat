using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace SuperColorChat
{
    public class Main : RocketPlugin<Configuration>
    {
        public static Dictionary<string, string> userList = new Dictionary<string, string>();

        public static Main Instance;
        public static Configuration Config;
        public static MySQLUtils MySQLUtils;

        public const string version = "1.0.0.1";

        protected override void Load()
        {
            Instance = this;
            Config = Configuration.Instance;
            MySQLUtils = new MySQLUtils();

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerChatted += OnPlayerChatted;

            Logger.Log(string.Format("SuperColorChat by Johnanater, version: {0}", version));
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerChatted -= OnPlayerChatted;
        }

        async void OnPlayerConnected(UnturnedPlayer player)
        {
            await Task.Run(() =>
            {
                if (MySQLUtils.CheckExists(player.Id))
                {
                    userList.Add(player.Id, MySQLUtils.GetColor(player.Id));
                }
            });
        }

        void OnPlayerDisconnected(UnturnedPlayer player)
        {
            if (userList.ContainsKey(player.Id))
            {
                userList.Remove(player.Id);
            }
        }

        void OnPlayerChatted(UnturnedPlayer player, ref UnityEngine.Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            if (userList.ContainsKey(player.Id))
            {
                color = (UnityEngine.Color)UnturnedChat.GetColorFromHex(userList[player.Id]);
            }
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {"not_enough_money", "You don't have enough money!"},
            {"not_whitelisted_color", "That's not a whitelisted color!"},
            {"avaliable_colors", "Avaliable colors: {0}"},
            {"color_updated_to", "You've updated your color to {0} for ${1}!"},
            {"log_color_change", "{0} ({1}) has changed their color to {2}!"},
            {"color_reset", "Your color has been reset!"},
            {"color_reset_by_staff", "Your color has been reset by a staff member!"},
            {"player_color_reset", "{0}'s color has been reset!"},
            {"player_color_not_found", "{0} doesn't have a color selected!"},
            {"your_color_not_found", "You don't have a color selected!"}
        };
    }
}
