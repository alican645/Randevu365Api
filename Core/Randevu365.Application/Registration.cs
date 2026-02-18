using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Randevu365.Application;

public static class Registration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODAxNTI2NDAwIiwiaWF0IjoiMTc3MDA2MDU1MSIsImFjY291bnRfaWQiOiIwMTljMWZkMmY4MGM3YTlmOTVhOGFkN2YyMzI2N2JiYSIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2dmeDk1YXJuejB5cHh6ang2eG43NzltIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.VZvJ_P6W7EwzRfgjXpkN0XMMAlpYbus_vIfML6h3n1vtaL-qppnT8SUUeVyQCoXjXjU-cddggCuq04Y6pKwjjX1JlzV93G5ByoOJenu2o5zVeCP-oFWEzvleNyPBuSSndpJF6vsF8Yx9yL1q18M2R0iQHJrm_kF0K6WVMFhfxDeyJ6KU3xkbmB_8GxwiH1xIXumlQhidQpesLKyt6rmUIkDKEnBRvcERM0kyYC0K-cCyRObsXFVkbBbP_U1HRS42qTjl_6nuWIP3NidApb07Ls1F5jUlw4dBAKaseQZjLzAuwBaNtGYvoJBjRj7wOFFIP7YKMbkAzDjINnTf-nLmGw";
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        
        return services;
    }
}
