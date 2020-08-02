FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY  . /app
ENV ASPNETCORE_ENVIRONMENT=Staging
RUN /bin/cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime && echo 'Asia/Shanghai' >/etc/timezone
EXPOSE 8107
ENTRYPOINT ["dotnet", "PagerQuestions.dll"]