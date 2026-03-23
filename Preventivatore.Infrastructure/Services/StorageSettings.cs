using Preventivatore.Core.DTOs;      // ✅
using Preventivatore.Core.Settings;       // per JwtSettings
using Preventivatore.Infrastructure.Services;
using Preventivatore.Core.Entities;
using Preventivatore.Core.Interfaces;


namespace Preventivatore.Core.Interfaces
{
    public class StorageSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string ContainerName { get; set; } = null!;
    }
}
