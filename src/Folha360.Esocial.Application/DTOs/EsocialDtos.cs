namespace Folha360.Esocial.Application.DTOs;

public record LoteEnvioResultDto(
    Guid LoteId,
    string Protocolo,
    int QuantidadeEventos,
    string Status);

public record LoteDto(
    Guid Id,
    Guid EmpresaId,
    string TipoAmbiente,
    string Status,
    string? ProtocoloEnvio,
    int QuantidadeEventos,
    DateTime? DataEnvio,
    DateTime? DataProcessamento,
    DateTime CriadoEm);

public record EventoEsocialDto(
    Guid Id,
    Guid EmpresaId,
    Guid? FuncionarioId,
    string TipoEvento,
    string Status,
    string IdEvento,
    Guid? LoteId,
    DateTime CriadoEm,
    DateTime? ProcessadoEm);

public record FalhaEsocialDto(
    Guid Id,
    Guid EventoId,
    string TipoErro,
    string? CodigoErro,
    string MensagemErro,
    int Tentativas,
    DateTime DataUltimaTentativa,
    DateTime? ResolvidoEm);

public record CertificadoDto(
    Guid Id,
    string Tipo,
    string Emitente,
    string Cnpj,
    DateTime DataExpiracao,
    int DiasRestantes,
    bool Ativo,
    bool Expirado);

public record CertificadoUploadRequest(
    byte[] ArquivoPfx,
    string Senha);

public record CertificadoA3TestRequest(
    string Pin);
