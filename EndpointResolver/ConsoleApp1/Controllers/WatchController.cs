using ConsoleApp1.Attributes;

namespace ConsoleApp1.Controllers;

[Route("api/[controller]/[action]")]
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
