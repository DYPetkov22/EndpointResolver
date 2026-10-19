namespace EndpointResolver.LegacyControllers;

[Microsoft.AspNetCore.Mvc.NonController]
public class GameController : ControllerBase
{
    public void PlayGame()
    {
        Console.WriteLine("Playing game...");
    }

    public void StopGame()
    {
        Console.WriteLine("Stopping game...");
    }
}
