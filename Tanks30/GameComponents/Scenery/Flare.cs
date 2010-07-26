using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Scenery
{
    class Flare
    {
        public static Flare[] Default
        {
            get
            {
                return new Flare[]
                {
                    new Flare(-0.5f, 0.7f, new Color( 50,  25,  50), "flare1"),
                    new Flare( 0.3f, 0.4f, new Color(100, 255, 200), "flare1"),
                    new Flare( 1.2f, 1.0f, new Color(100,  50,  50), "flare1"),
                    new Flare( 1.5f, 1.5f, new Color( 50, 100,  50), "flare1"),

                    new Flare(-0.3f, 0.7f, new Color(200,  50,  50), "flare2"),
                    new Flare( 0.6f, 0.9f, new Color( 50, 100,  50), "flare2"),
                    new Flare( 0.7f, 0.4f, new Color( 50, 200, 200), "flare2"),

                    new Flare(-0.7f, 0.7f, new Color( 50, 100,  25), "flare3"),
                    new Flare( 0.0f, 0.6f, new Color( 25,  25,  25), "flare3"),
                    new Flare( 2.0f, 1.4f, new Color( 25,  50, 100), "flare3"),
                };
            }
        }

        public Flare(float position, float scale, Color color, string textureName)
        {
            this.Position = position;
            this.Scale = scale;
            this.Color = color;
            this.TextureName = textureName;
        }

        public float Position;
        public float Scale;
        public Color Color;
        public string TextureName;
        public Texture2D Texture;
    }
}
