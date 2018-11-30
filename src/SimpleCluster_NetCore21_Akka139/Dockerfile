FROM microsoft/dotnet:2.1-sdk AS builder

ARG sc=/src/SimpleCluster

# copy project files to image
COPY *.csproj ${sc}/
COPY NuGet/*.nupkg ${sc}/NuGet/
COPY NuGet.config ${sc}/

## Restore packages
RUN dotnet restore /${sc} 

### copy all sources
COPY . ${sc}/

##### Publish Project
RUN dotnet publish /${sc} --output /publish 

FROM microsoft/dotnet:2.1-runtime
WORKDIR /publish
COPY --from=builder /publish .
ENTRYPOINT [ "dotnet", "SimpleCluster_NetCore21_Akka139.dll" ]