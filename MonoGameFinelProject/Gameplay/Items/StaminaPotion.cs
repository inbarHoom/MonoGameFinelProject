using Microsoft.Xna.Framework;

namespace MonoGameFinelProject;

public class StaminaPotion : Item
{
    public StaminaPotion() : base("Potions")
    {
        _sourceRectangle = new Rectangle(32, 0, 32, 32);
        ItemName = "Stamina Potion";
        ItemDescription = "Restores stamina";
        ItemType = e_ItemType.Potion;
        IsSellable = true;
        ItemPrice = 12;
        ItemQuantity = 1;
        ItemID = "Stamina_Potion";
    }
}