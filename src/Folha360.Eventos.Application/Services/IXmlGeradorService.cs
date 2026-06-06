using Folha360.Cadastros.Domain.Entities;
using Folha360.Eventos.Domain.Entities;

namespace Folha360.Eventos.Application.Services;

public interface IXmlGeradorService
{
    string GerarXmlAdmissao(Admissao admissao, Empresa empresa, Funcionario funcionario);
    string GerarXmlFerias(Ferias ferias, Empresa empresa, Funcionario funcionario);
    string GerarXmlAfastamento(Afastamento afastamento, Empresa empresa, Funcionario funcionario);
    string GerarXmlDesligamento(Desligamento desligamento, Empresa empresa, Funcionario funcionario);
    string GerarXmlAlteracaoContratual(AlteracaoContratual alteracao, Empresa empresa, Funcionario funcionario);
    XmlValidationResult ValidarContraXsd(string xml, string xsdResourceName);
}

public record XmlValidationResult(bool IsValid, List<string> Errors, List<string> Warnings);
