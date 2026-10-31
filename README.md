# VolirizeMP Launcher

A Windows desktop launcher starter built with **.NET 8 WPF**. Open `VolirizeMP-launcher.sln` in Visual Studio 2022.

## Included
- Spacious dark launcher layout
- Separate native WPF XAML pages for Home, News, Downloads, Account and Settings
- Customisable backgrounds: PNG/JPG/BMP, animated GIF, and MP4/WMV/MOV video
- Global backgrounds or individual backgrounds for each page
- Adjustable background blur saved between launches
- Background media copied into the current Windows user's AppData folder
- Backend health, account and news API service
- Windows-user encrypted session token storage helper

## Requirements
- Windows 10/11
- Visual Studio 2022 with the **.NET desktop development** workload
- .NET 8 SDK

## Run
1. Open `VolirizeMP-launcher.sln` in Visual Studio.
2. Restore NuGet packages.
3. Set `ApiBaseUrl` in `VolirizeMP.Launcher/Config.cs`.
4. Build and run.

## Background settings
Open **Settings** in the launcher. Choose whether media applies globally or to one page, select a PNG/JPG/BMP, animated GIF or supported video, adjust blur, then save. Settings and imported media are stored locally under `%AppData%/VolirizeMP`. A page-specific background takes priority over the global background.

## Backend contract
- `GET /` — health check
- `GET /news` — JSON array containing `title`, `description`, `date`, `category`, and optional `image`
- `GET /auth/me` — authenticated user profile

Discord OAuth is not implemented in this starter yet. The Account page is a separate XAML page ready to connect to your OAuth flow. Never put bot tokens or admin secrets in the launcher.
