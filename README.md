FlareSolverrSharp
==================
[![NuGet](https://img.shields.io/nuget/v/FlareSolverrSharp.svg?maxAge=60)](https://www.nuget.org/packages/FlareSolverrSharp)

FlareSolverr .Net DelegatingHandler / interceptor.

[FlareSolverr](https://github.com/FlareSolverr/FlareSolverr) is a proxy server to bypass Cloudflare protection.

:warning: This project is in beta state. Some things may not work and the API can change at any time.

# Installation
Full-Featured library:

`PM> Install-Package FlareSolverr`

# Dependencies
- [.NET Standard 1.3](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard1.3.md)

You need a running [FlareSolverr](https://github.com/FlareSolverr/FlareSolverr) service.

# Issues
Cloudflare regularly modifies their protection challenge and improves their bot detection capabilities.

If you notice that the anti-bot page has changed, or if library suddenly stops working, please create a GitHub issue so that I can
update the code accordingly.

Before submitting an issue, just be sure that you have the latest version of the library.

# Usage

A [DelegatingHandler](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler?view=netstandard-1.3) that
handles the challenge solution automatically.

> A type for HTTP handlers that delegate the processing of HTTP response messages to another handler, called the inner handler.

It checks on every request if the clearance is required or not, if required, it solves the challenge in background then returns the response.

Websites not using Cloudflare will be treated normally. You don't need to configure or call anything further, and you can effectively treat
all websites as if they're not protected with anything.

```csharp
var handler = new ClearanceHandler("http://localhost:8191/")
{
    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36",
    MaxTimeout = 60000
};

var client = new HttpClient(handler);
var content = await client.GetStringAsync("https://uam.hitmehard.fun/HIT");
Console.WriteLine(content);
```

**Full example [here](https://github.com/FlareSolverr/FlareSolverrSharp/tree/master/sample/FlareSolverrSharp.Sample)**

# Options
### FlareSolverr Service API
You have to set the FlareSolverr service API in the ClearanceHandler constructor. If you set an empty or null endpoint,
FlareSolverrSharp will be able to detect challenges, but it will not be able to solve them.

Example: http://localhost:8191/

### UserAgent
The User-Agent which will be used across this session. If you didn't set it, the default FlareSolverr User-Agent will be used.

**User-Agent must be the same as the one used to solve the challenge, otherwise Cloudflare will flag you as a bot.**

Example: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36

### MaxTimeout
Max timeout to solve the challenge.

**MaxTimeout should be greater than 15000 (15 seconds) because starting the web browser and solving the challenge takes time.**

Example: 60000
