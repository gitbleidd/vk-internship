FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["VkInternship.App/VkInternship.App.csproj", "VkInternship.App/"]
COPY ["VkInternship.Data/VkInternship.Data.csproj", "VkInternship.Data/"]
RUN dotnet restore "VkInternship.App/VkInternship.App.csproj"
COPY . .
WORKDIR "/src/VkInternship.App"
RUN dotnet build "VkInternship.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VkInternship.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VkInternship.App.dll"]