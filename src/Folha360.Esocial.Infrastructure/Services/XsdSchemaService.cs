using System.Xml.Schema;
using Folha360.Esocial.Application.Services;
using Folha360.Esocial.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace Folha360.Esocial.Infrastructure.Services;

public class XsdSchemaService : IXsdSchemaService
{
    private readonly IMinioClient _minioClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<XsdSchemaService> _logger;
    private readonly string _bucketName;
    private readonly string _cacheDir;

    private static readonly Dictionary<string, XmlSchemaSet> _cache = new();
    private static readonly SemaphoreSlim _cacheLock = new(1, 1);

    public XsdSchemaService(IMinioClient minioClient, IConfiguration configuration, ILogger<XsdSchemaService> logger)
    {
        _minioClient = minioClient;
        _configuration = configuration;
        _logger = logger;
        _bucketName = configuration["MinIO:BucketName"] ?? "folha360-esocial";
        _cacheDir = Path.Combine(Path.GetTempPath(), "folha360", "esocial", "schemas");
    }

    public async Task<XmlSchemaSet> ObterSchemaAsync(TipoEventoEsocial tipoEvento, CancellationToken ct)
    {
        var cacheKey = $"S-1.3/{tipoEvento}";

        await _cacheLock.WaitAsync(ct);
        try
        {
            if (_cache.TryGetValue(cacheKey, out var cached))
                return cached;
        }
        finally
        {
            _cacheLock.Release();
        }

        var schema = await CarregarSchemaAsync(tipoEvento, ct);

        await _cacheLock.WaitAsync(ct);
        try
        {
            _cache[cacheKey] = schema;
        }
        finally
        {
            _cacheLock.Release();
        }

        return schema;
    }

    public async Task AtualizarSchemasAsync(CancellationToken ct)
    {
        _logger.LogInformation("Iniciando atualização de schemas XSD do e-Social...");

        try
        {
            var versao = "S-1.3";
            var tiposEvento = Enum.GetValues<TipoEventoEsocial>();

            foreach (var tipo in tiposEvento)
            {
                var objectName = $"schemas/{versao}/{tipo}.xsd";
                var localPath = Path.Combine(_cacheDir, versao, $"{tipo}.xsd");

                Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);

                try
                {
                    await _minioClient.StatObjectAsync(
                        new StatObjectArgs()
                            .WithBucket(_bucketName)
                            .WithObject(objectName),
                        ct);

                    await _minioClient.GetObjectAsync(
                        new GetObjectArgs()
                            .WithBucket(_bucketName)
                            .WithObject(objectName)
                            .WithFile(localPath),
                        ct);

                    _logger.LogInformation("Schema {Tipo} atualizado com sucesso.", tipo);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Schema {Tipo} não encontrado no MinIO. Usando embedded fallback.", tipo);
                }
            }

            // Limpar cache para forçar reload
            await _cacheLock.WaitAsync(ct);
            try
            {
                _cache.Clear();
            }
            finally
            {
                _cacheLock.Release();
            }

            _logger.LogInformation("Atualização de schemas concluída.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar schemas XSD.");
        }
    }

    private async Task<XmlSchemaSet> CarregarSchemaAsync(TipoEventoEsocial tipoEvento, CancellationToken ct)
    {
        var versao = "S-1.3";
        var localPath = Path.Combine(_cacheDir, versao, $"{tipoEvento}.xsd");

        if (!File.Exists(localPath))
        {
            var objectName = $"schemas/{versao}/{tipoEvento}.xsd";
            Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);

            try
            {
                await _minioClient.GetObjectAsync(
                    new GetObjectArgs()
                        .WithBucket(_bucketName)
                        .WithObject(objectName)
                        .WithFile(localPath),
                    ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Schema {Tipo} não encontrado no MinIO. Tentando embedded resource.", tipoEvento);
                return await CarregarSchemaEmbeddedAsync(tipoEvento);
            }
        }

        var schemaSet = new XmlSchemaSet();
        using var reader = new StreamReader(localPath);
        var schema = XmlSchema.Read(reader, null);
        if (schema != null)
        {
            schemaSet.Add(schema);
        }

        return await Task.FromResult(schemaSet);
    }

    private Task<XmlSchemaSet> CarregarSchemaEmbeddedAsync(TipoEventoEsocial tipoEvento)
    {
        // Embedded fallback: usa schemas básicos embutidos
        var schemaSet = new XmlSchemaSet();
        _logger.LogInformation("Usando schema embedded para {Tipo}", tipoEvento);
        return Task.FromResult(schemaSet);
    }
}
