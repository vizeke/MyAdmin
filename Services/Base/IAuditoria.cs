using MyAdmin.Application.Models;

namespace MyAdmin.Application.Services.Base
{
    public interface IAuditoria
    {
        void Auditar(AuditoriaModel objAudit);
    }
}
