using AutoMapper;
using DomainObjects.Blocks;
using DomainObjects.Blocks.Details;
using System.Collections.Generic;
using System.Linq;
using Web.Models.Blocks;

namespace Web.Profiles
{
    public class TableBlockProfile : Profile
    {
        public TableBlockProfile()
        {
            CreateMap<TableBlock, TableBlockDto>()
                .ForMember(b => b.Type, opt => opt.MapFrom(b => BlockType.Table))
                .ForMember(b => b.Font, opt => opt.MapFrom(b => b.Details.FontName))
                .ForMember(b => b.FontSize, opt => opt.MapFrom(b => b.Details.FontSize))
                .ForMember(b => b.FontIndex, opt => opt.MapFrom(b => b.Details.FontIndex))
                .ForMember(b => b.HeaderStyle, opt => opt.MapFrom(b => b.Details.HeaderDetails))
                .ForMember(b => b.EvenStyle, opt => opt.MapFrom(b => b.Details.EvenRowDetails))
                .ForMember(b => b.OddStyle, opt => opt.MapFrom(b => b.Details.OddRowDetails))
                .ForMember(b => b.Header, opt => opt.MapFrom(b => b.Details.Cells.Where(c => c.Row == b.Details.Cells.Min(cell => cell.Row)).OrderBy(c => c.Column).Select(c => c.Value).ToArray()))
                .ForMember(b => b.Rows, opt => opt.MapFrom(b => b.Details.Cells.Where(c => c.Row != b.Details.Cells.Min(cell => cell.Row)).OrderBy(c => c.Row).GroupBy(c => c.Row).Select(g =>
                new RowDto
                {
                    Index = g.Key - b.Details.Cells.Min(cell => cell.Row) - 1,
                    Cells = g.OrderBy(c => c.Column).Select(c => c.Value).ToArray()
                }
                ).ToArray()))
                .ForMember(b => b.Caption, opt => opt.MapFrom(b => string.IsNullOrEmpty(b.Caption) ? "table" : b.Caption));

            CreateMap<TableBlockDto, TableBlock>()
                .ForMember(b => b.Details, opt => opt.MapFrom(b => b));

            CreateMap<TableBlockDto, TableBlockDetails>()
                .ForMember(b => b.FontName, opt => opt.MapFrom(b => b.Font))
                .ForMember(b => b.EvenRowDetails, opt => opt.MapFrom(b => b.EvenStyle))
                .ForMember(b => b.OddRowDetails, opt => opt.MapFrom(b => b.OddStyle))
                .ForMember(b => b.HeaderDetails, opt => opt.MapFrom(b => b.HeaderStyle))
                .AfterMap((dto, details) =>
                {
                    var rowIndex = 0;
                    var columnIndex = 0;
                    details.Cells = new List<TableBlockCellDetails>();
                    foreach (var headerCellValue in dto.Header)
                    {
                        details.Cells.Add(new TableBlockCellDetails
                        {
                            Column = columnIndex++,
                            Row = rowIndex,
                            Value = headerCellValue,
                            TableBlockDetailsId = details.Id,
                            TableBlockDetails = details
                        });
                    }
                    foreach (var row in dto.Rows)
                    {
                        rowIndex = row.Index + 1;
                        columnIndex = 0;
                        foreach (var cellValue in row.Cells)
                        {
                            details.Cells.Add(new TableBlockCellDetails
                            {
                                Column = columnIndex++,
                                Row = rowIndex,
                                Value = cellValue,
                                TableBlockDetailsId = details.Id,
                                TableBlockDetails = details
                            });
                        }
                    }
                });

            CreateMap<RowStyleDto, TableBlockRowDetails>();
        }
    }
}