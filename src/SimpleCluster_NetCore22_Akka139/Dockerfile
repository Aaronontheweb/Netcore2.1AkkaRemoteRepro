FROM microsoft/dotnet:2.2-sdk AS builder

ARG sc=/src/SimpleCluster

# copy project files to image
COPY *.csproj ${sc}/

## Restore packages
RUN dotnet restore /${sc} 

### copy all sources
COPY . ${sc}/

##### Publish Project
RUN dotnet publish /${sc} --output /publish 

FROM microsoft/dotnet:2.2-runtime
WORKDIR /publish
COPY --from=builder /publish .
ENTRYPOINT [ "dotnet", "SimpleCluster_NetCore22_Akka139.dll" ]