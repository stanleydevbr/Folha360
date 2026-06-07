using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Eventos.Application.Services;
using Folha360.Eventos.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Folha360.Eventos.Infrastructure.Services;

public class XmlGeradorService : IXmlGeradorService
{
    private readonly ILogger<XmlGeradorService> _logger;

    public XmlGeradorService(ILogger<XmlGeradorService> logger)
    {
        _logger = logger;
    }

    public string GerarXmlAdmissao(Admissao admissao, Empresa empresa, Funcionario funcionario)
    {
        var doc = new XDocument(
            new XElement("eSocial",
                new XElement("evtAdmissao",
                    new XAttribute("Id", $"ID1{admissao.Id:N}"),
                    new XElement("tpAmb", "2"),
                    new XElement("procEmi", "1"),
                    new XElement("verProc", "Folha360-1.0"),
                    new XElement("empregador",
                        new XElement("tpInsc", "1"),
                        new XElement("nrInsc", empresa.Cnpj)),
                    new XElement("trabalhador",
                        new XElement("cpfTrab", funcionario.Cpf ?? string.Empty),
                        new XElement("nmTrab", funcionario.Nome),
                        new XElement("dtNascto", funcionario.DataNascimento?.ToString("yyyy-MM-dd") ?? string.Empty)),
                    new XElement("vinculo",
                        new XElement("dtAdm", admissao.DataAdmissao.ToString("yyyy-MM-dd")),
                        new XElement("tpContrato", ((int)admissao.TipoContrato + 1).ToString()),
                        new XElement("salario", admissao.SalarioInicial.ToString("F2", System.Globalization.CultureInfo.InvariantCulture))))));

        return doc.ToString();
    }

    public string GerarXmlFerias(Ferias ferias, Empresa empresa, Funcionario funcionario)
    {
        var doc = new XDocument(
            new XElement("eSocial",
                new XElement("evtFerias",
                    new XAttribute("Id", $"ID1{ferias.Id:N}"),
                    new XElement("tpAmb", "2"),
                    new XElement("procEmi", "1"),
                    new XElement("verProc", "Folha360-1.0"),
                    new XElement("empregador",
                        new XElement("tpInsc", "1"),
                        new XElement("nrInsc", empresa.Cnpj)),
                    new XElement("trabalhador",
                        new XElement("cpfTrab", funcionario.Cpf ?? string.Empty)),
                    new XElement("infoFerias",
                        new XElement("dtInicio", ferias.DataInicio.ToString("yyyy-MM-dd")),
                        new XElement("diasGozo", ferias.DiasGozo.ToString()),
                        new XElement("dtAqInicio", ferias.PeriodoAquisitivoInicio.ToString("yyyy-MM-dd")),
                        new XElement("dtAqFim", ferias.PeriodoAquisitivoFim.ToString("yyyy-MM-dd"))))));

        return doc.ToString();
    }

    public string GerarXmlAfastamento(Afastamento afastamento, Empresa empresa, Funcionario funcionario)
    {
        var doc = new XDocument(
            new XElement("eSocial",
                new XElement("evtAfastTemp",
                    new XAttribute("Id", $"ID1{afastamento.Id:N}"),
                    new XElement("tpAmb", "2"),
                    new XElement("procEmi", "1"),
                    new XElement("verProc", "Folha360-1.0"),
                    new XElement("empregador",
                        new XElement("tpInsc", "1"),
                        new XElement("nrInsc", empresa.Cnpj)),
                    new XElement("trabalhador",
                        new XElement("cpfTrab", funcionario.Cpf ?? string.Empty)),
                    new XElement("infoAfastamento",
                        new XElement("dtInicio", afastamento.DataInicio.ToString("yyyy-MM-dd")),
                        new XElement("dtFimPrev", afastamento.DataFimPrevista.ToString("yyyy-MM-dd"))))));

        return doc.ToString();
    }

    public string GerarXmlDesligamento(Desligamento desligamento, Empresa empresa, Funcionario funcionario)
    {
        var doc = new XDocument(
            new XElement("eSocial",
                new XElement("evtDeslig",
                    new XAttribute("Id", $"ID1{desligamento.Id:N}"),
                    new XElement("tpAmb", "2"),
                    new XElement("procEmi", "1"),
                    new XElement("verProc", "Folha360-1.0"),
                    new XElement("empregador",
                        new XElement("tpInsc", "1"),
                        new XElement("nrInsc", empresa.Cnpj)),
                    new XElement("trabalhador",
                        new XElement("cpfTrab", funcionario.Cpf ?? string.Empty)),
                    new XElement("infoDeslig",
                        new XElement("dtDeslig", desligamento.DataDesligamento.ToString("yyyy-MM-dd")),
                        new XElement("mtvDeslig", ((int)desligamento.MotivoDesligamento + 1).ToString("D2"))))));

        return doc.ToString();
    }

    public string GerarXmlAlteracaoContratual(AlteracaoContratual alteracao, Empresa empresa, Funcionario funcionario)
    {
        var doc = new XDocument(
            new XElement("eSocial",
                new XElement("evtAltContratual",
                    new XAttribute("Id", $"ID1{alteracao.Id:N}"),
                    new XElement("tpAmb", "2"),
                    new XElement("procEmi", "1"),
                    new XElement("verProc", "Folha360-1.0"),
                    new XElement("empregador",
                        new XElement("tpInsc", "1"),
                        new XElement("nrInsc", empresa.Cnpj)),
                    new XElement("trabalhador",
                        new XElement("cpfTrab", funcionario.Cpf ?? string.Empty)),
                    new XElement("infoAlteracao",
                        new XElement("dtAlteracao", alteracao.DataAlteracao.ToString("yyyy-MM-dd"))))));

        return doc.ToString();
    }

    public XmlValidationResult ValidarContraXsd(string xml, string xsdResourceName)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var xsdStream = assembly.GetManifestResourceStream(xsdResourceName);

            if (xsdStream is null)
            {
                warnings.Add($"XSD resource '{xsdResourceName}' not found. Skipping validation.");
                return new XmlValidationResult(true, errors, warnings);
            }

            var schema = XmlSchema.Read(xsdStream, (sender, e) =>
            {
                if (e.Severity == XmlSeverityType.Error)
                    errors.Add($"XSD parse error: {e.Message}");
                else
                    warnings.Add($"XSD parse warning: {e.Message}");
            });

            if (schema is null)
            {
                errors.Add("Failed to load XSD schema.");
                return new XmlValidationResult(false, errors, warnings);
            }

            var schemas = new XmlSchemaSet();
            schemas.Add(schema);

            var doc = XDocument.Parse(xml);
            doc.Validate(schemas, (sender, e) =>
            {
                if (e.Severity == XmlSeverityType.Error)
                    errors.Add($"Validation error: {e.Message}");
                else
                    warnings.Add($"Validation warning: {e.Message}");
            });

            return new XmlValidationResult(errors.Count == 0, errors, warnings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating XML against XSD {XsdResource}", xsdResourceName);
            errors.Add($"Validation exception: {ex.Message}");
            return new XmlValidationResult(false, errors, warnings);
        }
    }
}
