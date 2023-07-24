using System.Diagnostics;

namespace Zafiro.FileSystem.SeaweedFS;

public class MyHttpClientHandler : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var sendAsync = await base.SendAsync(request, cancellationToken);
        var content = await sendAsync.Content.ReadAsStringAsync(cancellationToken);
        return sendAsync;
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpResponseMessage = base.Send(request, cancellationToken);
        return httpResponseMessage;
    }
}