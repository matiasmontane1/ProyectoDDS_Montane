namespace Octopath_Traveler.Models.Passives;

public interface IPassiveObserver
{
    IReadOnlyList<string> Handle(IPassiveEvent passiveEvent);
}
