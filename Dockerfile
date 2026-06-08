FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Copy solution and project files
COPY Folha360.slnx .
COPY Directory.Build.props .
COPY stylecop.ruleset .
COPY .stylecop.json .
COPY src/Folha360.Domain/*.csproj src/Folha360.Domain/
COPY src/Folha360.Application/*.csproj src/Folha360.Application/
COPY src/Folha360.Infrastructure/*.csproj src/Folha360.Infrastructure/
COPY src/Folha360.IoC/*.csproj src/Folha360.IoC/
COPY src/Folha360.WebApi/*.csproj src/Folha360.WebApi/
COPY src/Folha360.Cadastros.Domain/*.csproj src/Folha360.Cadastros.Domain/
COPY src/Folha360.Cadastros.Application/*.csproj src/Folha360.Cadastros.Application/
COPY src/Folha360.Cadastros.Infrastructure/*.csproj src/Folha360.Cadastros.Infrastructure/
COPY src/Folha360.Cadastros.Presentation/*.csproj src/Folha360.Cadastros.Presentation/
COPY src/Folha360.Eventos.Domain/*.csproj src/Folha360.Eventos.Domain/
COPY src/Folha360.Eventos.Application/*.csproj src/Folha360.Eventos.Application/
COPY src/Folha360.Eventos.Infrastructure/*.csproj src/Folha360.Eventos.Infrastructure/
COPY src/Folha360.Eventos.Presentation/*.csproj src/Folha360.Eventos.Presentation/
COPY src/Folha360.Processamento.Domain/*.csproj src/Folha360.Processamento.Domain/
COPY src/Folha360.Processamento.Application/*.csproj src/Folha360.Processamento.Application/
COPY src/Folha360.Processamento.Infrastructure/*.csproj src/Folha360.Processamento.Infrastructure/
COPY src/Folha360.Processamento.Presentation/*.csproj src/Folha360.Processamento.Presentation/
COPY tests/Folha360.Tests/*.csproj tests/Folha360.Tests/

# Restore
RUN dotnet restore src/Folha360.WebApi/Folha360.WebApi.csproj

# Copy all source code
COPY . .

# Build and publish
RUN dotnet publish src/Folha360.WebApi/Folha360.WebApi.csproj -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Folha360.WebApi.dll"]
