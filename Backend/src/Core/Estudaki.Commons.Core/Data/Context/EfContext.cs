using Microsoft.EntityFrameworkCore;

namespace Estudaki.Commons.Core.Data.Context;

public abstract class EfContext : DbContext
{
    public EfContext(DbContextOptions options) : base(options)
    {
        
    }
}
