using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Folha360.Infrastructure.Data;

public class TenantModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime)
    {
        if (context is Folha360DbContext folha360Context)
        {
            var tenantContext = folha360Context.GetTenantContext();
            return new ModelCacheKey(context, designTime);
        }

        return new ModelCacheKey(context, designTime);
    }

    public object Create(DbContext context)
    {
        return Create(context, false);
    }
}
