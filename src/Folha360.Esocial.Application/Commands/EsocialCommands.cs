using Folha360.Application;
using Folha360.Esocial.Application.DTOs;
using MediatR;

namespace Folha360.Esocial.Application.Commands;

public record EnviarLoteCommand(
    Guid EmpresaId,
    string TipoAmbiente) : IRequest<Result<LoteEnvioResultDto>>;

public record ReprocessarFalhaCommand(
    Guid FalhaId) : IRequest<Result<bool>>;

public record UploadCertificadoA1Command(
    Guid EmpresaId,
    byte[] ArquivoPfx,
    string Senha) : IRequest<Result<CertificadoDto>>;

public record TestarCertificadoA3Command(
    Guid EmpresaId,
    string Pin) : IRequest<Result<CertificadoDto>>;

public record EnviarEventosEsocialCommand(
    Guid EmpresaId,
    string Periodo) : IRequest<Result<bool>>;

public record ReverterEventosEsocialCommand(
    Guid EmpresaId,
    string Periodo) : IRequest<Result<bool>>;
