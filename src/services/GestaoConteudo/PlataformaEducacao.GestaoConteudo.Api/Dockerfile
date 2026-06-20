FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/building-blocks/PlataformaEducacao.Core/PlataformaEducacao.Core.csproj", "src/building-blocks/PlataformaEducacao.Core/"]
COPY ["src/building-blocks/PlataformaEducacao.MessageBus/PlataformaEducacao.MessageBus.csproj", "src/building-blocks/PlataformaEducacao.MessageBus/"]
COPY ["src/building-blocks/PlataformaEducacao.WebApi.Core/PlataformaEducacao.WebApi.Core.csproj", "src/building-blocks/PlataformaEducacao.WebApi.Core/"]
COPY ["src/services/GestaoConteudo/PlataformaEducacao.GestaoConteudo.Api/PlataformaEducacao.GestaoConteudo.Api.csproj", "src/services/GestaoConteudo/PlataformaEducacao.GestaoConteudo.Api/"]
COPY ["src/services/GestaoConteudo/PlataformaEducacao.GestaoConteudo.Application/PlataformaEducacao.GestaoConteudo.Application.csproj", "src/services/GestaoConteudo/PlataformaEducacao.GestaoConteudo.Application/"]
COPY ["src/services/GestaoConteudo/PlataformaEducacao.GestaoConteudo.Data/PlataformaEducacao.GestaoConteudo.Data.csproj", "src/services/GestaoConteudo/PlataformaEducacao.GestaoConteudo.Data/"]
COPY ["src/services/GestaoConteudo/PlataformaEducacao.GestaoConteudo.Domain/PlataformaEducacao.GestaoConteudo.Domain.csproj", "src/services/GestaoConteudo/PlataformaEducacao.GestaoConteudo.Domain/"]

RUN dotnet restore "src/services/GestaoConteudo/PlataformaEducacao.GestaoConteudo.Api/PlataformaEducacao.GestaoConteudo.Api.csproj"

COPY . .

WORKDIR "/src/src/services/GestaoConteudo/PlataformaEducacao.GestaoConteudo.Api"
RUN dotnet build "PlataformaEducacao.GestaoConteudo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlataformaEducacao.GestaoConteudo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

ARG BUILD_DATE="unknown"
ARG VCS_REF="unknown"
ARG VERSION="dev"
LABEL org.opencontainers.image.title="gestao-conteudo-api" \
      org.opencontainers.image.description="Gestão de Conteúdo API (.NET 8)" \
      org.opencontainers.image.created=$BUILD_DATE \
      org.opencontainers.image.revision=$VCS_REF \
      org.opencontainers.image.version=$VERSION

COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
    CMD curl --fail http://localhost:8080/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "PlataformaEducacao.GestaoConteudo.Api.dll"]