using InvoiceManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagement.Tests.Helpers
{
    public static class DbContextFactory
    {
        public static AppDbContext Create()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}
