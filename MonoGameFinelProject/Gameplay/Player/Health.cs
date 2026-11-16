namespace MonoGameFinelProject;
using System;
public class Health
{
    public int Current { get; private set; }
    public int Maximum { get; }

    public Health(int maximum)
    {
        Maximum = maximum;
        Current = maximum;
    }

    public void TakeDamage(int amount)
    {
        Current = Math.Max(0, Current - amount);
    }

    public void Heal(int amount)
    {
        Current = Math.Min(Maximum, Current + amount);
    }
}