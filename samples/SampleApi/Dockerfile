FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["samples/Entr.Products/Entr.Products.csproj", "Entr.Products/"]
COPY ["src/Entr.Domain/Entr.Domain.csproj", "Entr.Domain/"]
COPY ["src/Entr.CommandQuery/Entr.CommandQuery.csproj", "Entr.CommandQuery/"]
COPY ["src/Entr.Data/Entr.Data.csproj", "Entr.Data/"]
COPY ["src/Entr.CommandQuery.Autofac/Entr.CommandQuery.Autofac.csproj", "Entr.CommandQuery.Autofac/"]
COPY ["src/Entr.Data.EntityFramework/Entr.Data.EntityFramework.csproj", "Entr.Data.EntityFramework/"]
COPY ["src/Entr.Domain.Generators/Entr.Domain.Generators.csproj", "Entr.Domain.Generators/"]
COPY ["src/Entr.Data.EntityFramework.Generators/Entr.Data.EntityFramework.Generators.csproj", "Entr.Data.EntityFramework.Generators/"]
RUN dotnet restore "Entr.Products/Entr.Products.csproj"
WORKDIR "/src/Entr.Products"
RUN dotnet build "Entr.Products.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Entr.Products.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Entr.Products.dll"]
