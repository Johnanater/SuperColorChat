using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;

namespace SuperColorChat
{
    public class CommandRemoveColor : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "removecolor";

        public string Help => "Remove your color (no refunds!)";

        public string Syntax => "/removecolor [<player>]";

        public List<string> Aliases => new List<string> { "removecolour", "remcolor", "remcolour" };

        public List<string> Permissions => new List<string> { "supercolorchat.removecolor" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Task.Run(() => RemoveColor(caller, command));
        }

        public async Task RemoveColor(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 1 && caller.HasPermission("supercolorchat.removecolor.other"))
            {
                var target = UnturnedPlayer.FromName(command[0]);

                if (await Main.MySqlUtils.CheckExists(target.Id) && Main.UserList.ContainsKey(target.Id))
                {
                    await Main.MySqlUtils.RemoveColor(target.Id);
                    Main.UserList.Remove(target.Id);

                    Main.Instance.Tell(caller, Main.Instance.Translate("player_color_reset", target.CharacterName));
                    Main.Instance.Tell(target, Main.Instance.Translate("color_reset_by_staff"));
                }
                else
                {
                    Main.Instance.Tell(caller, Main.Instance.Translate("player_color_not_found", target.CharacterName));
                }
            }
            else
            {
                if (await Main.MySqlUtils.CheckExists(caller.Id) && Main.UserList.ContainsKey(caller.Id))
                {
                    await Main.MySqlUtils.RemoveColor(caller.Id);
                    Main.UserList.Remove(caller.Id);
                    Main.Instance.Tell(caller, Main.Instance.Translate("color_reset"));
                }
                else
                {
                    Main.Instance.Tell(caller, Main.Instance.Translate("your_color_not_found"));
                }
            }
        }
    }
}
