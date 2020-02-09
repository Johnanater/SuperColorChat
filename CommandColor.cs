using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fr34kyn01535.Uconomy;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;

namespace SuperColorChat
{
    public class CommandColor : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "color";

        public string Help => "Buy colors!";

        public string Syntax => "/color <name of color>";

        // For those communists
        public List<string> Aliases => new List<string> { "colour" };

        public List<string> Permissions => new List<string> { "supercolorchat.color" };

        public async void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 0)
            {
                if (Main.Config.UseMoney)
                {
                    var uconomyMoney = await Task.Run(() => Uconomy.Instance.Database.GetBalance(caller.Id));
                    
                    if (uconomyMoney - Main.Config.Cost < 0)
                    {
                        UnturnedChat.Say(caller, Main.Instance.Translate("not_enough_money"));
                        return;
                    }
                }

                var color = Main.Config.Colors.FirstOrDefault(c => command[0].Equals(c.Name, StringComparison.OrdinalIgnoreCase));

                if (color == null)
                {
                    UnturnedChat.Say(caller, Main.Instance.Translate("not_whitelisted_color"));
                    return;
                }

                if (await Main.MySqlUtils.CheckExists(caller.Id))
                {
                    await Main.MySqlUtils.UpdateColor(caller.Id, color.Hex);
                }
                else
                {
                    await Main.MySqlUtils.SetColor(caller.Id, color.Hex);
                }
                Main.UserList[caller.Id] = color.Hex;

                if (Main.Config.UseMoney)
                {
                    await Task.Run(() => Uconomy.Instance.Database.IncreaseBalance(caller.Id, -Main.Config.Cost));
                }

                UnturnedChat.Say(caller, Main.Instance.Translate("color_updated_to", color.Name, Main.Config.Cost));
                Logger.Log(Main.Instance.Translate("log_color_change", caller.DisplayName, caller.Id, color.Name));
            }
            else
            {
                UnturnedChat.Say(caller, Main.Instance.Translate("available_colors", string.Join(", ", Main.Config.Colors.Select(c => c.Name).ToArray())));
            }
        }
    }
}
