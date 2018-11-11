using System.Collections.Generic;
using System.Linq;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;

namespace SuperColorChat
{
    public class Main : RocketPlugin<Configuration>
    {
        public static Dictionary<string, string> userList = new Dictionary<string, string>();

        public static Main Instance;
        public static Configuration Config;
        public static MySQLUtils MySQLUtils;

        protected override void Load()
        {
            Instance = this;
            Config = Configuration.Instance;
            MySQLUtils = new MySQLUtils();

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerChatted += OnPlayerChatted;
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerChatted -= OnPlayerChatted;
        }

        void OnPlayerConnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            if (MySQLUtils.CheckExists(player.Id))
            {
                userList.Add(player.Id, MySQLUtils.GetColor(player.Id));
            }
        }

        void OnPlayerDisconnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            if (userList.ContainsKey(player.Id))
            {
                userList.Remove(player.Id);
            }
        }

        void OnPlayerChatted(Rocket.Unturned.Player.UnturnedPlayer player, ref UnityEngine.Color color, string message, SDG.Unturned.EChatMode chatMode, ref bool cancel)
        {
            if (userList.ContainsKey(player.Id))
            {
                var hex = userList.FirstOrDefault(h => h.Key.Contains(player.Id)).Value;
                color = (UnityEngine.Color)Rocket.Unturned.Chat.UnturnedChat.GetColorFromHex(hex);
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

        public string Translate(string translation, string arg)
        {
            string[] args = arg.Split(',');

            return Translations.Instance.Translate(translation, args);
        }
    }
}