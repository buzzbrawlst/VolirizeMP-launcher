# VolirizeMP Launcher

A starter Windows launcher built with **.NET 8 WPF** and **WebView2**. Open `VolirizeMP-launcher.sln` in Visual Studio 2022.

## Included
- WPF desktop shell with Home, News, Downloads and Account navigation
- HTML pages in `VolirizeMP.Launcher/WebPages/` rendered inside the launcher
- Configurable backend URL/IP and port
- Central `ApiService` for health, account and news requests
- Windows-user encrypted session token storage helper
- Discord invite shortcut and responsive dark UI

## Requirements
- Windows 10/11
- Visual Studio 2022 with the **.NET desktop development** workload
- .NET 8 SDK
- Microsoft Edge WebView2 Runtime

## Run it
1. Clone this repository.
2. Open `VolirizeMP-launcher.sln` in Visual Studio.
3. Restore NuGet packages.
4. Open `VolirizeMP.Launcher/Config.cs` and set `ApiBaseUrl` and your Discord invite.
5. Set the same API URL in `WebPages/news.html` as `API_BASE_URL`.
6. Build and run.

## Backend contract
The starter expects:
- `GET /` — health check
- `GET /news` — JSON array containing `title`, `description`, `date`, `category`, and optional `image`
- `GET /auth/me` — authenticated user profile if a session token exists

Change `Services/ApiService.cs` to match your backend's actual routes and response shapes. Discord OAuth is not implemented in this starter yet; the Account page is ready to connect to your chosen OAuth flow. `SessionStore` is secure token storage, not an authentication system by itself.

## IP-based backend
Set `Config.ApiBaseUrl` to your API origin including port, e.g. `http://192.0.2.10:3000` for local testing. For public production use HTTPS, validate tokens server-side, and never put bot tokens, admin secrets, or private keys in the launcher or HTML.

## HTML pages
Edit `WebPages/news.html` and `WebPages/downloads.html` directly. The project copies these files to the build output, and WebView2 opens them inside the launcher. News fetches the public `/news` endpoint from the configured API URL.