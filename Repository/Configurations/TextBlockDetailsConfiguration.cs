﻿using DomainObjects.Blocks.Details;
using System.Data.Entity.ModelConfiguration;

namespace Repository.Configurations
{
    public class TextBlockDetailsConfiguration : EntityTypeConfiguration<TextBlockDetails>
    {
        public TextBlockDetailsConfiguration()
        {
            ToTable("TextBlockDetails");
            HasKey(t => t.Id);
        }
    }
}
