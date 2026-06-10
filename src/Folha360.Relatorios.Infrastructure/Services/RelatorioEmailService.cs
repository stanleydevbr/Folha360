using Folha360.Relatorios.Application.DTOs;
using Folha360.Relatorios.Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Polly;
using Polly.CircuitBreaker;

namespace Folha360.Relatorios.Infrastructure.Services;

public class RelatorioEmailService : IRelatorioEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RelatorioEmailService> _logger;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly AsyncPolicy _retryPolicy;

    public RelatorioEmailService(
        IConfiguration configuration,
        ILogger<RelatorioEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _circuitBreaker = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(5, TimeSpan.FromMinutes(5),
                onBreak: (ex, ts) =>
                {
                    _logger.LogWarning(ex, "Circuit breaker SMTP aberto por {Duration}min", ts.TotalMinutes);
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit breaker SMTP resetado");
                });

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(exception, "Tentativa {RetryCount} de envio de email falhou", retryCount);
                });

        _retryPolicy = _retryPolicy.WrapAsync(_circuitBreaker);
    }

    public async Task EnviarAsync(EmailDestinoDto destino, Stream? anexo, string? nomeArquivo, CancellationToken ct)
    {
        var configSmtp = ObterConfigSmtp(destino.EmpresaId);

        await _retryPolicy.ExecuteAsync(async () =>
        {
            using var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Folha360", configSmtp.Usuario));
            foreach (var email in destino.Destinatarios)
                message.To.Add(MailboxAddress.Parse(email));

            message.Subject = destino.Assunto ?? "Relatório Folha360";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = ConstruirCorpoHtml(destino),
            };

            if (anexo is not null && nomeArquivo is not null && anexo.Length <= 10 * 1024 * 1024)
            {
                anexo.Position = 0;
                bodyBuilder.Attachments.Add(nomeArquivo, anexo);
            }
            else if (!string.IsNullOrEmpty(destino.Mensagem))
            {
                bodyBuilder.HtmlBody += $"<p><a href=\"{destino.Mensagem}\">Clique aqui para baixar o relatório</a></p>";
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(configSmtp.Host, configSmtp.Porta,
                configSmtp.Ssl ? SecureSocketOptions.StartTls : SecureSocketOptions.None, ct);
            await client.AuthenticateAsync(configSmtp.Usuario, configSmtp.Senha, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            _logger.LogInformation("Email enviado para {Count} destinatário(s)", destino.Destinatarios.Count);
        });
    }

    private (string Host, int Porta, bool Ssl, string Usuario, string Senha) ObterConfigSmtp(Guid empresaId)
    {
        // In production, this would read from empresa.config_smtp (JSONB)
        // For now, use environment-level config
        var host = _configuration["SMTP:Host"] ?? "localhost";
        var porta = int.Parse(_configuration["SMTP:Porta"] ?? "587");
        var ssl = bool.Parse(_configuration["SMTP:SSL"] ?? "true");
        var usuario = _configuration["SMTP:Usuario"] ?? string.Empty;
        var senha = _configuration["SMTP:Senha"] ?? string.Empty;

        return (host, porta, ssl, usuario, senha);
    }

    private static string ConstruirCorpoHtml(EmailDestinoDto destino)
    {
        return $"""
            <html>
            <body style="font-family: Arial, sans-serif;">
                <h2 style="color: #1a5276;">Folha360 — Relatório</h2>
                <p>Segue em anexo o relatório solicitado.</p>
                <p>{destino.Mensagem}</p>
                <hr />
                <p style="font-size: 12px; color: #666;">Este email foi gerado automaticamente pelo sistema Folha360.</p>
            </body>
            </html>
            """;
    }
}
