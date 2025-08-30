# Simple Makefile for NWinSnow

PROJECT = NWinSnow.csproj
FRAMEWORK = net6.0-windows
CONFIG = Release
OUT = dist

.PHONY: build package run-windowed run-screensaver run-wallpaper

build:
	dotnet build $(PROJECT) -c $(CONFIG) -f $(FRAMEWORK)

package:
	dotnet publish $(PROJECT) -c $(CONFIG) -f $(FRAMEWORK) -o $(OUT)

run-windowed: build
	dotnet run --project $(PROJECT) -- --mode windowed

run-screensaver: build
	dotnet run --project $(PROJECT) -- --mode screensaver

run-wallpaper: build
	dotnet run --project $(PROJECT) -- --mode wallpaper


