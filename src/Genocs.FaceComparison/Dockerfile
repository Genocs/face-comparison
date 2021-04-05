#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Genocs.FaceComparison/Genocs.FaceComparison.csproj", "src/Genocs.FaceComparison/"]
RUN dotnet restore "src/Genocs.FaceComparison/Genocs.FaceComparison.csproj"
COPY . .
WORKDIR "/src/src/Genocs.FaceComparison"
RUN dotnet build "Genocs.FaceComparison.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Genocs.FaceComparison.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Genocs.FaceComparison.dll"]