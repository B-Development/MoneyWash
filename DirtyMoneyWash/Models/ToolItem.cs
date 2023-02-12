using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DirtyMoneyWash.Models
{
    public class ToolItem
    {
        [XmlAttribute("ToolID")]
        public ushort ToolID { get; set; }
        [XmlAttribute("ToolType")]
        public ToolType ToolType { get; set; }

        public ToolItem(ushort tool_id, ToolType tool_type)
        {
            if (tool_id <= 0) throw new ArgumentOutOfRangeException(nameof(tool_id));
            if (!Enum.IsDefined(typeof(ToolType), tool_type))
                throw new InvalidEnumArgumentException(nameof(tool_type), (int)tool_type, typeof(ToolType));

            ToolID = tool_id;
            ToolType = tool_type;
        }

        public ToolItem()
        {
        }
    }
}
