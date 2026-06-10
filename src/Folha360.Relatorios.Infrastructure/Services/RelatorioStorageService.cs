using Folha360.Relatorios.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Polly;
using Polly.Retry;

namespace Folha360.Relatorios.Infrastructure.Services;

public class RelatorioStorageService : IRelatorioStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<RelatorioStorageService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public RelatorioStorageService(
        IConfiguration configuration,
        ILogger<RelatorioStorageService> logger)
    {
        _logger = logger;

        var endpoint = configuration["MinIO:Endpoint"] ?? "minio:9000";
        var accessKey = configuration["MinIO:AccessKey"] ?? "minioadmin";
        var secretKey = configuration["MinIO:SecretKey"] ?? "minioadmin";

        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(false)
            .Build();

        _retryPolicy = Policy
            .Handle<MinioException>()
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(exception,
                        "Tentativa {RetryCount} de operação MinIO falhou. Próxima tentativa em {Delay}s",
                        retryCount, timeSpan.TotalSeconds);
                });
    }

    public async Task<string> ArmazenarAsync(string bucket, string chave, Stream conteudo, string contentType, CancellationToken ct)
    {
        await GarantirBucketExisteAsync(bucket, ct);

        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var args = new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(chave)
                .WithStreamData(conteudo)
                .WithObjectSize(conteudo.Length)
                .WithContentType(contentType);

            var result = await _minioClient.PutObjectAsync(args, ct);
            _logger.LogInformation("Arquivo armazenado: {Bucket}/{Chave}", bucket, chave);
            return result.Etag;
        });
    }

    public async Task<Stream> RecuperarAsync(string bucket, string chave, CancellationToken ct)
    {
        var memoryStream = new MemoryStream();

        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var args = new GetObjectArgs()
                .WithBucket(bucket)
                .WithObject(chave)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream));

            await _minioClient.GetObjectAsync(args, ct);
            memoryStream.Position = 0;
            return memoryStream;
        });
    }

    public async Task<bool> ExisteAsync(string bucket, string chave, CancellationToken ct)
    {
        try
        {
            var args = new StatObjectArgs()
                .WithBucket(bucket)
                .WithObject(chave);

            await _minioClient.StatObjectAsync(args, ct);
            return true;
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }
        catch (BucketNotFoundException)
        {
            return false;
        }
    }

    public async Task<string> GerarUrlAssinadaAsync(string bucket, string chave, TimeSpan expiracao, CancellationToken ct)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(bucket)
                .WithObject(chave)
                .WithExpiry((int)expiracao.TotalSeconds);

            return await _minioClient.PresignedGetObjectAsync(args);
        });
    }

    private async Task GarantirBucketExisteAsync(string bucket, CancellationToken ct)
    {
        try
        {
            var existsArgs = new BucketExistsArgs().WithBucket(bucket);
            var exists = await _minioClient.BucketExistsAsync(existsArgs, ct);

            if (!exists)
            {
                var makeArgs = new MakeBucketArgs().WithBucket(bucket);
                await _minioClient.MakeBucketAsync(makeArgs, ct);
                _logger.LogInformation("Bucket criado: {Bucket}", bucket);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar/criar bucket {Bucket}", bucket);
            throw;
        }
    }
}
