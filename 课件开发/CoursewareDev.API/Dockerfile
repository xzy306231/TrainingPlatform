FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["CoursewareDev.API/CoursewareDev.API.csproj", "CoursewareDev.API/"]
COPY ["CoursewareDev.Common/CoursewareDev.Common.csproj", "CoursewareDev.Common/"]
COPY ["CoursewareDev.BLL/CoursewareDev.BLL.csproj", "CoursewareDev.BLL/"]
RUN dotnet restore "CoursewareDev.API/CoursewareDev.API.csproj"
COPY . .
WORKDIR "/src/CoursewareDev.API"
RUN dotnet build "CoursewareDev.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CoursewareDev.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CoursewareDev.API.dll"]