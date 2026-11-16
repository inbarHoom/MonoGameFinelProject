using Microsoft.Xna.Framework;

namespace MonoGameFinelProject;

public class HealthPotion : Item
{
    public const float healFraction = 0.2f;
    public HealthPotion() : base("Potions")
    {
        _sourceRectangle = new Rectangle(0, 0, 32, 32);
        ItemName = "Health Potion";
        ItemDescription = "Restores 20% of max health";
        ItemType = e_ItemType.Potion;
        IsSellable = true;
        ItemPrice = 10;
        ItemQuantity = 1;
        ItemID = "Health_Potion";
    }
}