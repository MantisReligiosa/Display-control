using System.Collections.Generic;
using System.Xml.Serialization;
using Web.Models.Blocks;

namespace Web.Models
{
    [XmlInclude(typeof(PictureBlockDto))]
    [XmlInclude(typeof(TableBlockDto))]
    [XmlInclude(typeof(TextBlockDto))]
    [XmlInclude(typeof(DateTimeBlockDto))]
    [XmlInclude(typeof(MetaBlockDto))]
    public class ConfigDto
    {
        public string Background { get; set; }
        public List<BlockDto> Blocks { get; set; }
    }
}