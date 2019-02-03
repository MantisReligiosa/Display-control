﻿using AutoMapper;
using DomainObjects.Blocks;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using NLog;
using ServiceInterfaces;
using System;
using System.Linq;
using Web.Models;
using Web.Models.Blocks;
using Web.Profiles;

namespace Web.Modules
{
    public class Secured : NancyModule
    {
        private IMapper _mapper = AutoMapperConfig.Mapper;

        public Secured(
            ISystemController systemController,
            IBlockController blockController,
            IOperationController operationController
            )
        {
            this.RequiresAuthentication();
            var logger = LogManager.GetCurrentClassLogger();
            Get["/master"] = parameters => View["Views/Home/Master.cshtml"];
            Get["/"] = parameters => View["Views/Home/Master.cshtml"];
            Post["/api/fonts"] = parameters =>
            {
                try
                {
                    return Response.AsJson(new FontInfo
                    {
                        Fonts = systemController.GetFonts(),
                        Sizes = systemController.GetFontSizes(),
                        Indexes = systemController.GetFontHeightIndex()
                    });
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception("Ошибка загрузки шрифтов", ex);
                }
            };
            Post["/api/screenResolution"] = parameters =>
            {
                try
                {
                    var data = this.Bind<ScreenResolutionRequest>();
                    if (!data.RefreshData)
                    {
                        var screenInfo = systemController.GetDatabaseScreenInfo();
                        if (screenInfo == null)
                        {
                            screenInfo = systemController.GetSystemScreenInfo();
                            systemController.SetDatabaseScreenInfo(screenInfo);
                        }
                        return Response.AsJson(screenInfo);
                    }
                    else
                    {
                        var screenInfo = systemController.GetSystemScreenInfo();
                        systemController.SetDatabaseScreenInfo(screenInfo);
                        return Response.AsJson(screenInfo);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception("Ошибка загрузки информации о экранах", ex);
                }
            };
            Post["/api/setBackground"] = parameters =>
            {
                try
                {
                    var data = this.Bind<ScreenBackgroundRequest>();
                    blockController.SetBackground(data.Color);
                    return Response.AsJson(true);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception("Ошибка установки фона", ex);
                }
            };
            Get["/api/background"] = parameters =>
            {
                try
                {
                    return Response.AsJson(blockController.GetBackground());
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception("Ошибка загрузки фона", ex);
                }
            };
            Post["/api/addTextBlock"] = parameters =>
            {
                try
                {
                    var textBlock = blockController.AddTextBlock();
                    var block = _mapper.Map<TextBlockDto>(textBlock);
                    return Response.AsJson(block);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception("Ошибка добавления текстового блока", ex);
                }
            };
            Post["/api/addTableBlock"] = parameters =>
            {
                try
                {
                    var tableBlock = blockController.AddTableBlock();
                    var block = _mapper.Map<TableBlockDto>(tableBlock);
                    return Response.AsJson(block);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception("Ошибка добавления таблицы", ex);
                }
            };
            Post["/api/addPictureBlock"] = parameters =>
            {
                try
                {
                    var pictureBlock = blockController.AddPictureBlock();
                    var block = _mapper.Map<PictureBlockDto>(pictureBlock);
                    return Response.AsJson(block);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception("Ошибка добавления картинки", ex);
                }
            };
            Get["/api/blocks"] = parameters =>
            {
                try
                {
                    var blocks = blockController.GetBlocks().Select(b =>
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
                                return new BlockDto
                                {
                                    Height = b.Height,
                                    Id = b.Id,
                                    Left = b.Left,
                                    Top = b.Top,
                                    Width = b.Width
                                };
                            });
                    return Response.AsJson(blocks);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception("Ошибка загрузки блоков", ex);
                }
            };
            Post["/api/saveBlock"] = parameters =>
            {
                try
                {
                    var data = this.Bind<BlockDto>();
                    if (data.Type.Equals("text", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        var b = this.Bind<TextBlockDto>();
                        var block = _mapper.Map<TextBlock>(b);
                        blockController.SaveTextBlock(block);
                    }
                    else if (data.Type.Equals("table", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        var b = this.Bind<TableBlockDto>();
                        var block = _mapper.Map<TableBlock>(b);
                        blockController.SaveTableBlock(block);
                    }
                    else if (data.Type.Equals("picture", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        var b = this.Bind<PictureBlockDto>();
                        var block = _mapper.Map<PictureBlock>(b);
                        blockController.SavePictureBlock(block);
                    }

                    return Response.AsJson(true);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception("Ошибка сохранения блоков", ex);
                }
            };
            Post["/api/deleteBlock"] = parameters =>
            {
                try
                {
                    var data = this.Bind<BlockDto>();
                    blockController.DeleteBlock(data.Id);
                    return Response.AsJson(true);
                }
                catch (Exception ex)
                {

                    logger.Error(ex);
                    throw new Exception("Ошибка удаления блоков", ex);
                }
            };
            Post["/api/copyBlock"] = parameters =>
            {
                try
                {
                    var data = this.Bind<BlockDto>();
                    if (data.Type.Equals("text", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        var b = this.Bind<TextBlockDto>();
                        var block = _mapper.Map<TextBlock>(b);
                        return _mapper.Map<TextBlockDto>(blockController.CopyTextBlock(block));
                    }
                    else if (data.Type.Equals("table", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        var b = this.Bind<TableBlockDto>();
                        var block = _mapper.Map<TableBlock>(b);
                        return _mapper.Map<TableBlockDto>(blockController.CopyTableBlock(block));
                    }
                    else if (data.Type.Equals("picture", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        var b = this.Bind<PictureBlockDto>();
                        var block = _mapper.Map<PictureBlock>(b);
                        return _mapper.Map<PictureBlockDto>(blockController.CopyPictureBlock(block));
                    }

                    return Response.AsJson(true);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception("Ошибка копирования блоков", ex);
                }
            };
            Post["/api/startShow"] = parameters =>
            {
                try
                {
                    operationController.StartShow();
                    return Response.AsJson(true);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception("Ошибка запуска презентации", ex);
                }
            };
            Post["/api/stopShow"] = parameters =>
            {
                try
                {
                    operationController.StopShow();
                    return Response.AsJson(true);

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw new Exception("Ошибка остановки презентации", ex);
                }
            };
        }
    }
}