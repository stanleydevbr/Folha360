using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Folha360.Relatorios.Application.DTOs;
using Folha360.Relatorios.Application.Services;

namespace Folha360.Relatorios.Infrastructure.Services;

public class RelatorioExportService : IRelatorioExportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = true,
    };

    public Task<Stream> ExportarCsvAsync<T>(IReadOnlyList<T> dados, CancellationToken ct)
    {
        if (dados.Count == 0)
        {
            return Task.FromResult<Stream>(new MemoryStream());
        }

        var sb = new StringBuilder();
        var properties = typeof(T).GetProperties()
            .Where(p => p.CanRead)
            .ToList();

        // BOM for UTF-8
        sb.Append('\uFEFF');

        // Header
        var headers = properties.Select(p => ToSnakeCase(p.Name)).ToList();
        sb.AppendLine(string.Join(";", headers));

        // Data
        foreach (var item in dados)
        {
            var values = properties.Select(p =>
            {
                var val = p.GetValue(item);
                return FormatCsvValue(val);
            });
            sb.AppendLine(string.Join(";", values));
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return Task.FromResult<Stream>(new MemoryStream(bytes));
    }

    public Task<Stream> ExportarCsvDirfAsync(IReadOnlyList<DirfDto> dados, CancellationToken ct)
    {
        var sb = new StringBuilder();
        sb.Append('\uFEFF');

        // Leiaute oficial DIRF (simplificado)
        sb.AppendLine("CPF;Nome;RendimentosTributaveis;RendimentosIsentos;IRRFRetido;13Salario;Ferias");

        foreach (var d in dados)
        {
            sb.AppendLine($"{d.Cpf};{EscapeCsv(d.Nome)};{d.RendimentosTributaveis:N2};{d.RendimentosIsentos:N2};{d.IrrfRetido:N2};{d.DecimoTerceiro:N2};{d.Ferias:N2}");
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return Task.FromResult<Stream>(new MemoryStream(bytes));
    }

    public Task<Stream> ExportarCsvRaisAsync(IReadOnlyList<RaisDto> dados, CancellationToken ct)
    {
        var sb = new StringBuilder();
        sb.Append('\uFEFF');

        // Leiaute oficial RAIS (simplificado)
        sb.AppendLine("CPF;Nome;PISPASEP;Admissao;Desligamento;MotivoDesligamento;RemunJan;RemunFev;RemunMar;RemunAbr;RemunMai;RemunJun;RemunJul;RemunAgo;RemunSet;RemunOut;RemunNov;RemunDez;RemunTotal;13Salario");

        foreach (var d in dados)
        {
            sb.AppendLine($"{d.Cpf};{EscapeCsv(d.Nome)};{d.PisPasep};{d.DataAdmissao:yyyy-MM-dd};{d.DataDesligamento:yyyy-MM-dd};{EscapeCsv(d.MotivoDesligamento)};" +
                $"{d.RemuneracaoJaneiro:N2};{d.RemuneracaoFevereiro:N2};{d.RemuneracaoMarco:N2};{d.RemuneracaoAbril:N2};{d.RemuneracaoMaio:N2};{d.RemuneracaoJunho:N2};" +
                $"{d.RemuneracaoJulho:N2};{d.RemuneracaoAgosto:N2};{d.RemuneracaoSetembro:N2};{d.RemuneracaoOutubro:N2};{d.RemuneracaoNovembro:N2};{d.RemuneracaoDezembro:N2};" +
                $"{d.RemuneracaoTotal:N2};{d.DecimoTerceiro:N2}");
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return Task.FromResult<Stream>(new MemoryStream(bytes));
    }

    public Task<Stream> ExportarXmlAsync<T>(IReadOnlyList<T> dados, string rootElementName, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(dados, JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        var xmlDoc = new XmlDocument();
        var root = xmlDoc.CreateElement(rootElementName);
        xmlDoc.AppendChild(root);

        ConvertJsonToXml(jsonDoc.RootElement, root, xmlDoc, "item");

        var stream = new MemoryStream();
        using var writer = XmlWriter.Create(stream, new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false,
        });
        xmlDoc.WriteTo(writer);
        writer.Flush();
        stream.Position = 0;
        return Task.FromResult<Stream>(stream);
    }

    private static void ConvertJsonToXml(JsonElement json, XmlElement parent, XmlDocument doc, string arrayItemName)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var prop in json.EnumerateObject())
                {
                    var child = doc.CreateElement(ToSnakeCase(prop.Name));
                    ConvertJsonToXml(prop.Value, child, doc, arrayItemName);
                    parent.AppendChild(child);
                }

                break;

            case JsonValueKind.Array:
                foreach (var item in json.EnumerateArray())
                {
                    var child = doc.CreateElement(arrayItemName);
                    ConvertJsonToXml(item, child, doc, arrayItemName);
                    parent.AppendChild(child);
                }

                break;

            default:
                parent.InnerText = json.ToString();
                break;
        }
    }

    private static string ToSnakeCase(string name)
    {
        return string.Concat(name.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "_" + char.ToLower(c, CultureInfo.InvariantCulture) : char.ToLower(c, CultureInfo.InvariantCulture).ToString()));
    }

    private static string FormatCsvValue(object? val)
    {
        if (val is null)
        {
            return string.Empty;
        }
        var str = val switch
        {
            DateTime dt => dt.ToString("yyyy-MM-dd"),
            decimal d => d.ToString("N2", CultureInfo.InvariantCulture),
            _ => val.ToString() ?? string.Empty,
        };
        return EscapeCsv(str);
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(';') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
