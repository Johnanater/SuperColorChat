using System;
using System.Collections.Generic;
using System.Linq;
using fr34kyn01535.Uconomy;
using Rocket.API;
using Rocket.Unturned.Chat;

namespace SuperColorChat
{
    public class CommandColor : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "color";

        public string Help => "Buy colors!";

        public string Syntax => "<name of color>";

        // For those communists
        public List<string> Aliases => new List<string>() { "colour" };

        public List<string> Permissions => new List<string>() { "supercolorchat.color" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 0)
            {
                if (Main.Config.UseMoney)
                {
                    if (Uconomy.Instance.Database.GetBalance(caller.Id) - Main.Config.Cost < 0)
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

                if (MySQLUtils.CheckExists(caller.Id))
                {
                    MySQLUtils.UpdateColor(caller.Id, color.Hex);
                }
                else
                {
                    MySQLUtils.SetColor(caller.Id, color.Hex);
                }
                Main.userList[caller.Id] = color.Hex;

                if (Main.Config.UseMoney)
                {
                    Uconomy.Instance.Database.IncreaseBalance(caller.Id, -Main.Config.Cost);
                }

                UnturnedChat.Say(caller, Main.Instance.Translate("color_updated_to", color.Name, Main.Config.Cost));
                Rocket.Core.Logging.Logger.Log(Main.Instance.Translate("log_color_change", caller.DisplayName, caller.Id, color.Name));
            }
            else
            {
                UnturnedChat.Say(caller, Main.Instance.Translate("avaliable_colors", string.Join(", ", Main.Config.Colors.Select(c => c.Name).ToArray())));
            }
        }
    }
}