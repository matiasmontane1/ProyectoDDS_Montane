using Octopath_Traveler.Models.Passives.Observers;

namespace Octopath_Traveler.Models.Passives;

public static class PassiveObserverFactory
{
    public static IPassiveObserver? Create(string passiveName, Traveler traveler)
    {
        return passiveName switch
        {
            "Boost Start"       => new BoostStartObserver(traveler),
            "Stat Swap"         => new StatSwapObserver(traveler),
            "Vim and Vigor"     => new VimAndVigorObserver(traveler),
            "Second Wind"       => new SecondWindObserver(traveler),
            "Patience"          => new PatienceObserver(traveler),
            "Hang Tough"        => new HangToughObserver(traveler),
            "Encore"            => new EncoreObserver(traveler),
            "Inspiration"       => new InspirationObserver(traveler),
            "Heightened Healing"=> new HeightenedHealingObserver(traveler),
            "Persistence"       => new PersistenceObserver(traveler),
            "SP Saver"          => new SpSaverObserver(traveler),
            "Divine Aura"       => new DivineAuraObserver(traveler),
            "The Show Goes On"  => new TheShowGoesOnObserver(traveler),
            _                   => null
        };
    }
}
