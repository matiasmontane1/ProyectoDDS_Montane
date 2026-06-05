namespace Octopath_Traveler.Models.Passives;

public class EventPublisher
{
    private readonly List<IPassiveObserver> _subscribers = new();

    public void Subscribe(IPassiveObserver observer) => _subscribers.Add(observer);

    public IReadOnlyList<string> Publish(IPassiveEvent passiveEvent)
    {
        var messages = new List<string>();
        foreach (var subscriber in _subscribers)
            messages.AddRange(subscriber.Handle(passiveEvent));
        return messages;
    }
}
