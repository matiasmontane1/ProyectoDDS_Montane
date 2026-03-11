namespace Octopath_Traveler.Models;

public abstract class Unit
{
    public string Name { get; set; }
    public Stats Stats { get; set; }

    // Estado durante el combate
    public int CurrentHP { get; protected set; }
    public bool IsDead => CurrentHP <= 0;

    // Inicializa la vida al empezar el combate
    public virtual void InitializeState()
    {
        CurrentHP = Stats.HP;
    }

    // Lógica limpia para recibir daño
    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0)
        {
            CurrentHP = 0;
        }
    }
}