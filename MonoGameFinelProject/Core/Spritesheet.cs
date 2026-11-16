namespace MonoGameFinelProject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class Spritesheet
{
    public int columns { get; set; }
    
    public int rows { get; set; }
    
    public Texture2D texture { get; set; }
    public bool isPadded { get; set; } = false;
    
    public Rectangle this[int x, int y]
    {
        get
        {
            if (isPadded)
            {
                int cellWidth = texture.Width / columns;
                int cellHeight = texture.Height / rows;
                return new Rectangle(
                    new Point(x * cellWidth + 1, y * cellHeight + 1),
                    new Point(cellWidth - 2, cellHeight - 2));
            }

            return new Rectangle(
                new Point(
                    (int)(texture.Width * ((float)x / columns)),
                    (int)(texture.Height * ((float)y / rows))),
                new Point(
                    (int)(texture.Width * (1.0f/ columns)),
                    (int)(texture.Height * (1.0f / rows)))
            );
        }
    }
}