using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyMoneyWash.Models
{
    public class WashBasin
    {
        public float LocationX { get; set; }
        public float LocationY { get; set; }
        public float LocationZ { get; set; }
        public float Rot_X { get; set; }
        public float Rot_Y { get; set; }
        public float Rot_Z { get; set; }
        public float Rot_W { get; set; }

        public ulong Owner { get; set; }
        public ulong Group { get; set; }

        public int WaterLevel { get; set; }

        public WashBasin(float location_x, float location_y, float location_z, float rot_x, float rot_y, float rot_z, float rot_w, ulong owner, ulong group, int water_level = 0)
        {
            LocationX = location_x;
            LocationY = location_y;
            LocationZ = location_z;
            Rot_X = rot_x;
            Rot_Y = rot_y;
            Rot_Z = rot_z;
            Rot_W = rot_w;
            Owner = owner;
            Group = group;
            WaterLevel = water_level;
        }

        public WashBasin()
        {
        }
    }
}
