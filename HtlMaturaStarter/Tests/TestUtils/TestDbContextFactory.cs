using System;
using DataAccess;
using Microsoft.EntityFrameworkCore;

public static class TestDbContextFactory
{
    public static ApplicationDataContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDataContext(options);
    }
}
