using System.Collections.Generic;
using Web.Models.Blocks;

namespace Web.Models
{
    public class ParsedTableDto
    {
        public List<string> Header { get; set; } = new List<string>();
        public List<RowDto> Rows { get; set; } = new List<RowDto>();
    }
}