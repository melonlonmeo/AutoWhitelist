using System;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using System.IO;

namespace AutoWhitelist
{
    [ApiVersion(2, 1)]
    public class AutoWhitelist : TerrariaPlugin
    {
        public override string Name => "AutoWhitelist";
        public override Version Version => new Version(1, 0);
        public override string Author => "GILX_TERRARIA_VUI-DEV";
        public override string Description => "Automatically adds new players to whitelist and notifies them";
        private bool _enabled = true;

        public AutoWhitelist(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            Commands.ChatCommands.Add(new Command("autowhitelist.toggle", ToggleAutoWhitelist, "autowhitelist")
            {
                HelpText = "Toggles automatic whitelisting of new players"
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
            }
            base.Dispose(disposing);
        }

        private void ToggleAutoWhitelist(CommandArgs args)
        {
            _enabled = !_enabled;
            args.Player.SendSuccessMessage($"AutoWhitelist is now {(_enabled ? "enabled" : "disabled")}");
        }

        private void OnJoin(JoinEventArgs args)
        {
            if (!_enabled)
                return;

            var player = TShock.Players[args.Who];
            if (player != null)
            {
                // Kiểm tra xem người chơi đã có trong whitelist chưa
                string checkCommand = $"/wl list {player.Name}";
                TShockAPI.Commands.HandleCommand(TSPlayer.Server, checkCommand);

                // Thêm người chơi vào whitelist nếu chưa có
                string addCommand = $"/wl add {player.Name}";
                TShockAPI.Commands.HandleCommand(TSPlayer.Server, addCommand);

                TShock.Log.ConsoleInfo($"Automatically checked and added {player.Name} to whitelist if needed");
                player.SendSuccessMessage("You have been automatically added to the whitelist if needed!");
            }
        }
    }
} 