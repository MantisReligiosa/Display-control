﻿using DomainObjects.Blocks;
using Nancy;
using Nancy.ModelBinding;
using NLog;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Models;
using Web.Models.Blocks;

namespace Web.Modules
{
    public class BlockModule : WrappedNancyModule
    {
        private readonly IBlockController _blockController;
        private readonly ISerializationController _serializationController;

        private readonly Dictionary<string, Func<dynamic>> _savers;
        private readonly Dictionary<string, Func<object>> _copiers;

        public BlockModule(
            IBlockController blockController,
            ISerializationController serializationController
            )
            : base()
        {
            _blockController = blockController;
            _serializationController = serializationController;

            Post["/api/setBackground"] = Wrap(SetBackground, "Ошибка установки фона");
            Get["/api/background"] = Wrap(GetBackground, "Ошибка загрузки фона");
            Post["/api/addTextBlock"] = Wrap(AddTextBlock, "Ошибка добавления текстового блока");
            Post["/api/addTableBlock"] = Wrap(AddTableBlock, "Ошибка добавления таблицы");
            Post["/api/addPictureBlock"] = Wrap(AddPictureBlock, "Ошибка добавления картинки");
            Post["/api/addDateTimeBlock"] = Wrap(AddDatetimeBlock, "Ошибка добавления блока даты/времени");
            Post["api/addMetaBlock"] = Wrap(AddMetaBlock, "Ошибка добавления метаблока");
            Get["/api/blocks"] = Wrap(GetBlocks, "Ошибка загрузки блоков");
            Post["/api/moveAndResize"] = Wrap(MoveAndResize, "Ошибка изменения размеров и положения блока");
            Post["/api/saveBlock"] = Wrap(SaveBlock, "Ошибка сохранения блоков");
            Post["/api/deleteBlock"] = Wrap(DeleteBlock, "Ошибка удаления блоков");
            Post["/api/copyBlock"] = Wrap(CopyBlock, "Ошибка копирования блоков");
            Post["/api/parseCSV"] = Wrap(ParseCSV, "Ошибка чтения csv");
            Get["/api/downloadConfig"] = DownloadConfig(blockController, serializationController, _logger);
            Post["/api/uploadConfig"] = Wrap(UploadConfig, "Ошибка загрузки конфигурации");

            _savers = new Dictionary<string, Func<dynamic>>()
            {
                { "text", () => SaveBlock<TextBlock, TextBlockDto>(b => _mapper.Map<TextBlockDto>(blockController.SaveTextBlock(b))) },
                { "table", () => SaveBlock<TableBlock, TableBlockDto>(b => _mapper.Map<TableBlockDto>(blockController.SaveTableBlock(b))) },
                { "picture", () => SaveBlock<PictureBlock, PictureBlockDto>(b => _mapper.Map<PictureBlockDto>(blockController.SavePictureBlock(b))) },
                { "datetime", () => SaveBlock<DateTimeBlock, DateTimeBlockDto>(b => _mapper.Map<DateTimeBlockDto>(blockController.SaveDateTimeBlock(b))) },
                { "meta", () => SaveBlock<MetaBlock, MetaBlockDto>(b => _mapper.Map<MetaBlockDto>(blockController.SaveMetabLock(b))) }
            };
            _copiers = new Dictionary<string, Func<object>>
            {
                { "text" , () => CopyBlock<TextBlock, TextBlockDto>(b => blockController.CopyTextBlock(b)) },
                { "table" , () => CopyBlock<TableBlock, TableBlockDto>(b => blockController.CopyTableBlock(b)) },
                { "picture" , () => CopyBlock<PictureBlock, PictureBlockDto>(b => blockController.CopyPictureBlock(b)) },
                { "datetime" , () => CopyBlock<DateTimeBlock, DateTimeBlockDto>(b => blockController.CopyDateTimeBlock(b)) },
                { "meta", () => CopyBlock<MetaBlock, MetaBlockDto>(b => blockController.CopyMetabLock(b)) }
            };
        }

        private void UploadConfig()
        {
            var data = this.Bind<ConfigDataDto>();
            var configDto = _serializationController.Deserialize<ConfigDto>(data.Text);
            _blockController.SetBackground(configDto.Background);
            _blockController.Cleanup();
            foreach (var b in configDto.Blocks)
            {
                if (b is TextBlockDto textBlock)
                {
                    var block = _mapper.Map<TextBlock>(textBlock);
                    _blockController.SaveTextBlock(block);
                }
                if (b is TableBlockDto tableBlock)
                {
                    var block = _mapper.Map<TableBlock>(tableBlock);
                    _blockController.SaveTableBlock(block);
                }
                if (b is PictureBlockDto pictureBlock)
                {
                    var block = _mapper.Map<PictureBlock>(pictureBlock);
                    _blockController.SavePictureBlock(block);
                }
            }
        }

        private Func<dynamic, dynamic> DownloadConfig(IBlockController blockController, ISerializationController serializationController, ILogger logger)
        {
            return parameters =>
            {
                try
                {
                    var response = new Response
                    {
                        ContentType = "text/xml",
                        Contents = (stream) => serializationController.SerializeXML(new ConfigDto
                        {
                            Background = blockController.GetBackground(),
                            Blocks = GetBlocks().ToList()
                        }).CopyTo(stream)
                    };
                    response.Headers.Add("Content-Disposition", "attachment; filename=config.xml");
                    return response;
                }
                catch (Exception ex)
                {
                    var exception = new Exception("Ошибка выгрузки конфигурации", ex);
                    logger.Error(exception);
                    throw exception;

                }
            };
        }

        private CsvTableDto ParseCSV()
        {
            var linesSeparator = new char[] { '\r', '\n' };
            var itemSeparator = ',';

            var data = this.Bind<CsvDataDto>();
            var lines = data.Text.Split(linesSeparator, StringSplitOptions.RemoveEmptyEntries);
            var result = new CsvTableDto();
            result.Header.AddRange(lines.First().Split(itemSeparator));
            var rowIndex = 0;
            foreach (var line in lines.Skip(1))
            {
                var cells = line.Split(itemSeparator);
                var delta = cells.Length - result.Header.Count;
                if (delta > 0)
                    for (int i = 0; i < delta; i++)
                    {
                        result.Header.Add(string.Empty);
                    }
                result.Rows.Add(new RowDto
                {
                    Index = rowIndex,
                    Cells = cells
                });
                rowIndex++;
            }

            return result;
        }

        private object CopyBlock()
        {
            var data = this.Bind<BlockDto>();
            return _copiers.First(kvp => kvp.Key.Equals(data.Type, StringComparison.InvariantCultureIgnoreCase)).Value.Invoke();
        }

        private void DeleteBlock()
        {
            var data = this.Bind<BlockDto>();
            _blockController.DeleteBlock(data.Id);
        }

        private dynamic SaveBlock()
        {
            var data = this.Bind<BlockDto>();
            return _savers.First(kvp => kvp.Key.Equals(data.Type, StringComparison.InvariantCultureIgnoreCase)).Value.Invoke();
        }

        private void MoveAndResize()
        {
            var block = this.Bind<SizeAndPositionDto>();
            _blockController.MoveAndResizeBlock(block.Id, block.Height, block.Width, block.Left, block.Top);
        }

        private MetaBlockDto AddMetaBlock()
        {
            var metablock = _blockController.AddMetaBlock();
            var block = _mapper.Map<MetaBlockDto>(metablock);
            return block;
        }

        private DateTimeBlockDto AddDatetimeBlock()
        {
            var addBlockParams = this.Bind<AddBlockParamsDto>();
            var dateTimeBlock = _blockController.AddDateTimeBlock(addBlockParams.FrameId);
            var block = _mapper.Map<DateTimeBlockDto>(dateTimeBlock);
            return block;
        }

        private PictureBlockDto AddPictureBlock()
        {
            var addBlockParams = this.Bind<AddBlockParamsDto>();
            var pictureBlock = _blockController.AddPictureBlock(addBlockParams.FrameId);
            var block = _mapper.Map<PictureBlockDto>(pictureBlock);
            return block;
        }

        private TableBlockDto AddTableBlock()
        {
            var addBlockParams = this.Bind<AddBlockParamsDto>();
            var tableBlock = _blockController.AddTableBlock(addBlockParams.FrameId);
            var block = _mapper.Map<TableBlockDto>(tableBlock);
            return block;
        }

        private TextBlockDto AddTextBlock()
        {
            var addBlockParams = this.Bind<AddBlockParamsDto>();
            var textBlock = _blockController.AddTextBlock(addBlockParams.FrameId);
            var block = _mapper.Map<TextBlockDto>(textBlock);
            return block;
        }

        private string GetBackground()
        {
            return _blockController.GetBackground();
        }

        private void SetBackground()
        {
            var data = this.Bind<ScreenBackgroundRequest>();
            _blockController.SetBackground(data.Color);
        }

        private dynamic SaveBlock<TBlock, TBlockDto>(Func<TBlock, dynamic> saveAction)
            where TBlock : DisplayBlock
            where TBlockDto : BlockDto
        {
            var b = this.Bind<TBlockDto>();
            var block = _mapper.Map<TBlock>(b);
            return saveAction.Invoke(block);
        }

        private TBlockDto CopyBlock<TBlock, TBlockDto>(Func<TBlock, TBlock> copyFunction)
            where TBlock : DisplayBlock
            where TBlockDto : BlockDto
        {
            var b = this.Bind<TBlockDto>();
            var block = _mapper.Map<TBlock>(b);
            return _mapper.Map<TBlockDto>(copyFunction(block));
        }

        private IEnumerable<BlockDto> GetBlocks()
        {
            var blocks = _blockController.GetBlocks().Select(b =>
            {
                return MapBlock(b);
            });
            return blocks;
        }

        private BlockDto MapBlock(DisplayBlock b)
        {
            if (b is TextBlock textBlock)
            {
                var block = _mapper.Map<TextBlockDto>(textBlock);
                return block;
            }
            if (b is TableBlock tableBlock)
            {
                var block = _mapper.Map<TableBlockDto>(tableBlock);
                return block;
            }
            if (b is PictureBlock pictureBlock)
            {
                var block = _mapper.Map<PictureBlockDto>(pictureBlock);
                return block;
            }
            if (b is DateTimeBlock dateTimeBlock)
            {
                var block = _mapper.Map<DateTimeBlockDto>(dateTimeBlock);
                return block;
            }
            if (b is MetaBlock metaBlock)
            {
                var block = _mapper.Map<MetaBlockDto>(metaBlock);
                foreach (var frameDto in block.Frames)
                {
                    var frame = metaBlock.Details.Frames.FirstOrDefault(f => f.Id.Equals(frameDto.Id));
                    frameDto.Blocks = new List<BlockDto>(frame.Blocks.Select(f => MapBlock(f)));
                }
                return block;
            }
            return new BlockDto
            {
                Height = b.Height,
                Id = b.Id,
                Left = b.Left,
                Top = b.Top,
                Width = b.Width
            };
        }
    }
}