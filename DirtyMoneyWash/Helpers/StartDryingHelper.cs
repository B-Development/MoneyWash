using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SDG.Unturned;
using UnityEngine;

namespace DirtyMoneyWash.Helpers
{
    public class StartDryingHelper
    {
        public static void StartDrying(ushort wet_money, [NotNull] Transform wet_money_transform)
        {
            if (wet_money_transform == null) throw new ArgumentNullException(nameof(wet_money_transform));
            if (wet_money <= 0) throw new ArgumentOutOfRangeException(nameof(wet_money));


            Main.Instance.StartCoroutine(StartDryTimer(wet_money, wet_money_transform));
        }

        [Obsolete]
        private static IEnumerator StartDryTimer(ushort wet_money, Transform wet_money_transform)
        {
            var timer = Main.Instance.Configuration.Instance.DryTime;

            while (timer != 0)
            {
                timer--;
                yield return new WaitForSeconds(1);
            }


            var clean_money = Main.Instance.Configuration.Instance.MoneyItems
                .First(item => item.WetMoneyID == wet_money).CleanMoneyIDs;

            foreach (var money in clean_money)
            {
                ItemManager.dropItem(new Item(money, true), new Vector3(wet_money_transform.position.x, wet_money_transform.position.y + 5, wet_money_transform.position.z), true, true, true);
            }

            if (BarricadeManager.tryGetInfo(wet_money_transform, out var x, out var y, out var plant,
                    out var index,
                    out var region))
            {
                BarricadeManager.destroyBarricade(region, x, y, plant, index);
            }
        }
    }
}
