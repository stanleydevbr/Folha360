using Folha360.Processamento.Domain.Entities;

namespace Folha360.Processamento.Application.Services;

public interface IPdfGeradorService
{
    byte[] GerarHolerite(
        ProcessamentoFolha processamento,
        IEnumerable<ItemFolha> itens,
        string nomeFuncionario,
        string cpfFuncionario,
        string nomeEmpresa,
        string cnpjEmpresa,
        string? dadosBancarios = null);
}
