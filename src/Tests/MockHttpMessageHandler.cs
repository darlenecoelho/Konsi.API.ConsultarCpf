namespace Tests;

public class MockHttpMessageHandler : DelegatingHandler
{
    private readonly HttpResponseMessage _fakeResponse;

    public MockHttpMessageHandler(HttpResponseMessage responseMessage)
    {
        _fakeResponse = responseMessage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_fakeResponse);
    }
}

