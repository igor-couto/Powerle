FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine3.23-arm64v8 AS build
RUN apk add --no-cache icu-libs
WORKDIR /src

COPY EnergySourceGame.csproj ./
RUN dotnet restore EnergySourceGame.csproj --disable-parallel --runtime linux-musl-arm64 /p:PublishReadyToRun=true

COPY . ./

RUN dotnet publish EnergySourceGame.csproj -c Release -o /app/publish --no-restore --runtime linux-musl-arm64 --self-contained true /p:PublishReadyToRun=true
RUN set -eux; \
    mkdir -p /app/publish/wwwroot/_framework; \
    for asset in blazor.web.js blazor.server.js; do \
      if [ ! -f "/app/publish/wwwroot/_framework/$asset" ]; then \
        source_path="$(find /root/.nuget/packages /usr/share/dotnet -type f -path "*/_framework/$asset" 2>/dev/null | head -n 1)"; \
        test -n "$source_path"; \
        cp "$source_path" "/app/publish/wwwroot/_framework/$asset"; \
      fi; \
    done

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine3.23-arm64v8 AS final
RUN apk add --no-cache icu-libs
WORKDIR /app

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_URLS=http://+:50060 \
    ASPNETCORE_HTTP_PORTS=50060 \
    ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish ./

EXPOSE 50060

ENTRYPOINT ["./EnergySourceGame"]
