using Folha360.Application.DTOs;

namespace Folha360.Application;

/// <summary>
/// Métodos de extensão para converter <see cref="Result{T}"/> em respostas
/// padronizadas no formato Problem Details (RFC 7807).
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Cria um <see cref="ProblemDetailsResponse"/> a partir de um <see cref="Result{T}"/> com falha.
    /// </summary>
    public static ProblemDetailsResponse ToProblemDetails<T>(this Result<T> result, string path)
        => new(
            Type: "https://tools.ietf.org/html/rfc7807",
            Title: "Validation Error",
            Status: 422,
            Detail: "One or more validation errors occurred.",
            Instance: path,
            Errors: result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray()));

    /// <summary>
    /// Cria um <see cref="ProblemDetailsResponse"/> a partir de um <see cref="PaginatedResult{T}"/> com falha.
    /// </summary>
    public static ProblemDetailsResponse ToProblemDetails<T>(this PaginatedResult<T> result, string path)
        => new(
            Type: "https://tools.ietf.org/html/rfc7807",
            Title: "Validation Error",
            Status: 422,
            Detail: "One or more validation errors occurred.",
            Instance: path,
            Errors: result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray()));
}
