using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DirtyMoneyWash.Models
{
    public class MoneyItem
    {
        [XmlAttribute("DirtMoneyID")]
        public ushort DirtMoneyID { get; set; }
        [XmlAttribute("WetMoneyID")]
        public ushort WetMoneyID { get; set; }
        [XmlAttribute("CleanMoneyID")]
        public List<ushort> CleanMoneyIDs { get; set; }

        public MoneyItem(ushort dirt_money_id, ushort wet_money_id, params ushort[] clean_money_id)
        {
            if (dirt_money_id <= 0) throw new ArgumentOutOfRangeException(nameof(dirt_money_id));
            if (wet_money_id <= 0) throw new ArgumentOutOfRangeException(nameof(wet_money_id));

            DirtMoneyID = dirt_money_id;
            WetMoneyID = wet_money_id;
            CleanMoneyIDs = clean_money_id.ToList();
        }

        public MoneyItem()
        {
        }
    }
}
