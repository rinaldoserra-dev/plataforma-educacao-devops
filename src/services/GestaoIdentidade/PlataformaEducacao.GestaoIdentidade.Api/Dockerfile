FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/services/GestaoIdentidade/PlataformaEducacao.GestaoIdentidade.Api/PlataformaEducacao.GestaoIdentidade.Api.csproj", "src/services/GestaoIdentidade/PlataformaEducacao.GestaoIdentidade.Api/"]
COPY ["src/building-blocks/PlataformaEducacao.Core/PlataformaEducacao.Core.csproj", "src/building-blocks/PlataformaEducacao.Core/"]
COPY ["src/building-blocks/PlataformaEducacao.MessageBus/PlataformaEducacao.MessageBus.csproj", "src/building-blocks/PlataformaEducacao.MessageBus/"]
COPY ["src/building-blocks/PlataformaEducacao.WebApi.Core/PlataformaEducacao.WebApi.Core.csproj", "src/building-blocks/PlataformaEducacao.WebApi.Core/"]

RUN dotnet restore "src/services/GestaoIdentidade/PlataformaEducacao.GestaoIdentidade.Api/PlataformaEducacao.GestaoIdentidade.Api.csproj"

COPY . .

WORKDIR "/src/src/services/GestaoIdentidade/PlataformaEducacao.GestaoIdentidade.Api"
RUN dotnet build "PlataformaEducacao.GestaoIdentidade.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlataformaEducacao.GestaoIdentidade.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

ARG BUILD_DATE="unknown"
ARG VCS_REF="unknown"
ARG VERSION="dev"
LABEL org.opencontainers.image.title="gestao-identidade-api" \
      org.opencontainers.image.description="Gestão de Identidade API (.NET 8)" \
      org.opencontainers.image.created=$BUILD_DATE \
      org.opencontainers.image.revision=$VCS_REF \
      org.opencontainers.image.version=$VERSION

RUN deluser app 2>/dev/null || true && delgroup app 2>/dev/null || true && \
    addgroup --system --gid 1001 app && \
    adduser --system --uid 1001 --ingroup app app && \
    chown -R app:app /app

COPY --from=publish --chown=app:app /app/publish .

USER app

HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "PlataformaEducacao.GestaoIdentidade.Api.dll"]
