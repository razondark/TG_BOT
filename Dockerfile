FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY BotUI/BotUI.csproj ./BotUI/
COPY TG_Bot.sln ./

RUN dotnet restore "BotUI/BotUI.csproj"

COPY . ./

RUN dotnet publish "BotUI/BotUI.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "BotUI.dll"]