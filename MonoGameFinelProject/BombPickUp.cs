namespace MonoGameFinelProject;

public class BombPickUp : Item
{
    private int damage;
    public BombPickUp() : base("BombPickup" )
    {
        ItemName = "Bomb";
        ItemDescription = "A big bomb you can throw or sell ";
        ItemSprite = new Sprite("BombPickup");
        ItemType = e_ItemType.Weapon;
        IsSellable = true;
        ItemPrice = 5;
        ItemQuantity = 1;
        ItemID = "Bomb_Pickup";
        damage = 50;
    }
}