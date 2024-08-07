FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /App

COPY *.sln .
COPY MenuApp.Core/*.csproj ./MenuApp.Core/
COPY MenuApp/*.csproj ./MenuApp/
COPY MenuApp.Data/*.csproj ./MenuApp.Data/

RUN dotnet restore

COPY MenuApp.Core/. ./MenuApp.Core/
COPY MenuApp/. ./MenuApp/
COPY MenuApp.Data/. ./MenuApp.Data/

WORKDIR /App/MenuApp
RUN dotnet publish -c Release -o out

FROM build AS runtime
RUN apt-get update
RUN apt-get install -y apt-utils
RUN apt-get install -y libgdiplus
RUN apt-get install -y libc6-dev 
RUN ln -s /usr/lib/libgdiplus.so/usr/lib/gdiplus.dll
RUN apt-get update && apt-get install -y \
curl
WORKDIR /App
COPY --from=build /App/MenuApp/out ./
ENTRYPOINT ["dotnet", "MenuApp.dll"]

