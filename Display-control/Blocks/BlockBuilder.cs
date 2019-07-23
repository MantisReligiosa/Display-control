using Display_control.Blocks.Builders;
using DomainObjects.Blocks;
using Helpers;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Display_control.Blocks
{
    public class BlockBuilder
    {
        private Dictionary<Type, AbstractBuilder> _builders = new Dictionary<Type, AbstractBuilder>
        {
            {typeof(TextBlock), new TextBlockBuilder() },
            {typeof(TableBlock), new TableBlockBuilder() },
            {typeof(PictureBlock), new PictureBlockBuilder() },
            {typeof(DateTimeBlock), new DateTimeBlockBuilder() },
            {typeof(MetaBlock), new MetaBlockBuilder(new MetablockScheduler()) }
        };

        public UIElement BuildElement(DisplayBlock displayBlock)
        {
            if (_builders.TryGetValue(displayBlock.GetType(), out AbstractBuilder builder))
            {
                try
                {
                    return builder.BuildElement(displayBlock);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }
    }
}
