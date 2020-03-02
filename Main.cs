using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace SuperColorChat
{
    public class Main : RocketPlugin<Configuration>
    {
        public static readonly Dictionary<string, string> UserList = new Dictionary<string, string>();

        public static Main Instance;
        public static Configuration Config;
        public static MySqlUtils MySqlUtils;

        public const string Version = "1.0.3";

        protected override void Load()
        {
            Instance = this;
            Config = Configuration.Instance;
            MySqlUtils = new MySqlUtils();

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerChatted += OnPlayerChatted;

            Logger.Log($"SuperColorChat by Johnanater, version: {Version}");
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerChatted -= OnPlayerChatted;
        }

        private void OnPlayerConnected(UnturnedPlayer untPlayer)
        {
            Task.Run(() => AddPlayer(untPlayer));
        }

        private void OnPlayerDisconnected(UnturnedPlayer untPlayer)
        {
            if (UserList.ContainsKey(untPlayer.Id))
            {
                UserList.Remove(untPlayer.Id);
            }
        }

        private void OnPlayerChatted(UnturnedPlayer player, ref UnityEngine.Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            if (UserList.ContainsKey(player.Id))
            {
                color = (UnityEngine.Color)UnturnedChat.GetColorFromHex(UserList[player.Id]);
            }
        }

        public async Task AddPlayer(UnturnedPlayer untPlayer)
        {
            if (await MySqlUtils.CheckExists(untPlayer.Id))
            {
                UserList.Add(untPlayer.Id, await MySqlUtils.GetColor(untPlayer.Id));
            }
        }

        public void Tell(IRocketPlayer player, string message)
        {
            TaskDispatcher.QueueOnMainThread(() => UnturnedChat.Say(player, message));
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {"not_enough_money", "You don't have enough money!"},
            {"not_whitelisted_color", "That's not a whitelisted color!"},
            {"available_colors", "Available colors: {0}"},
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
