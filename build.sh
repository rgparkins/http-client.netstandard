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

echo
echo ===========================================================
echo Building containers
echo ===========================================================
echo

SRCDIR=`pwd`

docker run --rm \
           -v "$SRCDIR/:/build" \
           --workdir /build \
           --name app \
           mcr.microsoft.com/dotnet/sdk:5.0 dotnet build
