using Microsoft.EntityFrameworkCore;

namespace TradeSystem.FileContextCore.Infrastructure
{
	internal class FileContextDbContextOptionsBuilder
    {
        public FileContextDbContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            OptionsBuilder = optionsBuilder;
        }

        protected virtual DbContextOptionsBuilder OptionsBuilder { get; }
    }
}
