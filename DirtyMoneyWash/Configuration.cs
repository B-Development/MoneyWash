using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DirtyMoneyWash.Models;
using Rocket.API;
using SDG.Framework.Debug;

namespace DirtyMoneyWash
{
    public class Configuration : IRocketPluginConfiguration
    {
        public int DryTime { get; set; }
        public int DestroyChance { get; set; }
        public ushort WashedEffect { get; set; }
        public int WaterToFill { get; set; }

        [XmlArray("MoneyItems")]
        [XmlArrayItem("MoneyItem")]
        public List<MoneyItem> MoneyItems { get; set; }

        [XmlArray("ToolItems")]
        [XmlArrayItem("ToolItem")]
        public List<ToolItem> ToolItems { get; set; }

        public void LoadDefaults()
        {
            DryTime = 5;
            DestroyChance = 0;
            WashedEffect = 76;
            WaterToFill = 6;

            MoneyItems = new List<MoneyItem>
            {
                new MoneyItem(30562, 30586, 45006), // 1$ Bill
                new MoneyItem(30563, 30587, 45010), // 5$ Bill
                new MoneyItem(30564, 30588, 45011), // 10$ Bill
                new MoneyItem(30565, 30589, 45012), // 20$ Bill
                new MoneyItem(30565, 30590, 45012, 45010), // 25$ Bill
                new MoneyItem(30567, 30591, 45013), // 50$ Bill
                new MoneyItem(30568, 30592, 45014), // 100$ Bill
                new MoneyItem(30569, 30593, 45022), // 200$ Bill
            };

            ToolItems = new List<ToolItem>
            {
                new ToolItem(30602, ToolType.EmptyBasin),
                new ToolItem(30603, ToolType.FullBasin),
                new ToolItem(30604, ToolType.DryingRack)
            };
        }
    }
}
