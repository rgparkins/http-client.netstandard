#!/usr/bin/env bash

error() {

  echo ">>>>>> Failed to build <<<<<<<<<"
  echo docker logs app

  exit 1
}

cleanup() {
  echo "....Cleaning up"

  if [ "$(docker images -f "dangling=true" -q | awk '{print $3}' | sort -u)x" != "x" ]
  then
    docker rmi $(docker images --filter "dangling=true" -q --no-trunc)
  fi

  echo "....Cleaning up done"
}

trap error ERR
trap cleanup EXIT

if [ -z "$GO_PIPELINE_COUNTER" ]; then
    export GO_PIPELINE_COUNTER=0
fi

if [ -z "$GO_STAGE_COUNTER" ]; then
    export GO_STAGE_COUNTER=0
fi

if [ -z "$NUGET_SUFFIX" ]; then
    export NUGET_SUFFIX=
fi

if [ -z "$NUGET_MAJOR_VERSION" ]; then
    export NUGET_MAJOR_VERSION=2
fi

if [ -z "$NUGET_SERVER" ]; then
    export NUGET_SERVER=https://nuget.org
fi

VERSION=${NUGET_MAJOR_VERSION}.${GO_PIPELINE_COUNTER}.${GO_STAGE_COUNTER}${NUGET_SUFFIX}

ARTIFACTDIR=`pwd`/deployment

echo
echo =============================================================================
echo Pushing NuGet package : $ARTIFACTDIR - VERSION $VERSION
echo =============================================================================

echo "using key $NUGET_API_KEY"

docker run --rm \
           -v "$ARTIFACTDIR/:/build" \
           --workdir /build \
           --name app \
           mcr.microsoft.com/dotnet/sdk:5.0 dotnet nuget push Http-client.Netstandard.$VERSION.nupkg -k $NUGET_API_KEY --source $NUGET_SERVER

docker run --rm \
           -v "$ARTIFACTDIR/:/build" \
           --workdir /build \
           --name app \
           mcr.microsoft.com/dotnet/sdk:5.0 dotnet nuget push Http-client.Autofac.Netstandard.$VERSION.nupkg -k $NUGET_API_KEY --source $NUGET_SERVER

echo Done!