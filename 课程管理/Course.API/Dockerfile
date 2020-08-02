FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Course.API/Course.API.csproj", "Course.API/"]
COPY ["Course.Model/Course.Model.csproj", "Course.Model/"]
COPY ["Course.BLL/Course.BLL.csproj", "Course.BLL/"]
COPY ["Course.DAL/Course.DAL.csproj", "Course.DAL/"]
RUN dotnet restore "Course.API/Course.API.csproj"
COPY . .
WORKDIR "/src/Course.API"
RUN dotnet build "Course.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Course.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Course.API.dll"]