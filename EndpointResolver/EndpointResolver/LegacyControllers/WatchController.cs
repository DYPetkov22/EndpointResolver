using EndpointResolver.Attributes;

namespace EndpointResolver.LegacyControllers;

[Route("api/[controller]/[action]")]
[Microsoft.AspNetCore.Mvc.NonController]
public class WatchController : ControllerBase
{
    [HttpGet]
    public void PlayVideo(int id)
    {
        Console.WriteLine($"Playing video with id: {id}");
    }

    public void PlayAudio()
    {
        Console.WriteLine("Playing audio...");
    }
}
