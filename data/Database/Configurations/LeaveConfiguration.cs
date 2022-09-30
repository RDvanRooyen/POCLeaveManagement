using data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data.Database.Configurations
{
    internal class LeaveConfiguration: IEntityTypeConfiguration<Leave>
    {
        public void Configure(EntityTypeBuilder<Leave> builder)
        {
            builder.Property(x => x.LeaveStart).IsRequired().HasConversion<DateOnlyConverter, DateOnlyComparer>();
            builder.Property(x => x.LeaveEnd).IsRequired().HasConversion<DateOnlyConverter, DateOnlyComparer>();
        }

        public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
        {
            public DateOnlyConverter() : base(
                    dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
                    dateTime => DateOnly.FromDateTime(dateTime))
            {
            }
        }

        public class DateOnlyComparer : ValueComparer<DateOnly>
        {
            public DateOnlyComparer() : base(
                (d1, d2) => d1.DayNumber == d2.DayNumber,
                d => d.GetHashCode())
            {
            }
        }
    }
}
