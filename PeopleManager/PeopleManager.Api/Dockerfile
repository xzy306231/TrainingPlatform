FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY  . /app
EXPOSE 5060
ENV ASPNETCORE_ENVIRONMENT=Staging
RUN /bin/cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime && echo 'Asia/Shanghai' >/etc/timezone
ENTRYPOINT ["dotnet", "PeopleManager.Api.dll"]