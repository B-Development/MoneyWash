using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DirtyMoneyWash.Helpers
{
    public class BarricadeHelper
    {
        private static readonly List<RegionCoordinate> RegionBuffer = new List<RegionCoordinate>(48);

        public static List<BarricadeDrop> GetBarricadesNearby(float range, Vector3 origin)
        {
            ThreadUtil.assertIsGameThread();
            RegionBuffer.Clear();
            Regions.getRegionsInRadius(origin, range, RegionBuffer);
            range *= range;
            List<BarricadeDrop> rtn = new List<BarricadeDrop>();
            for (int r = 0; r < RegionBuffer.Count; r++)
            {
                RegionCoordinate rc = RegionBuffer[r];
                BarricadeRegion region = BarricadeManager.regions[rc.x, rc.y];
                foreach (BarricadeDrop barricade in region.drops)
                {
                    if ((barricade.model.position - origin).sqrMagnitude <= range)
                    {
                        rtn.Add(barricade);
                    }
                }
            }

            return rtn;
        }
    }
}
