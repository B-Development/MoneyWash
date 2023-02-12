using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Items;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace DirtyMoneyWash
{
    public class TestCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = caller as UnturnedPlayer;
            ItemManager.dropItem(new Item(45022, true), player.Position, bool.Parse(command[0]), bool.Parse(command[1]), bool.Parse(command[2]));
        }
        
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "TestCommand";
        public string Help => string.Empty;
        public string Syntax => string.Empty;
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();
    }
}
