using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameComponents.Scenery;
using GameComponents.Camera;

namespace GameComponents.Debug
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public partial class LineDrawerGameComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        VertexDeclaration declaration;
        VertexBuffer buffer;
        BasicEffect effect;

        bool updateMatrix = true;
        Matrix m_ModelSpace = Matrix.Identity;

        private SceneryGameComponent m_Scenery = null;

        public float Length = 5000.0f;
        public Vector3 Position = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;
        public Matrix ModelSpace
        {
            get
            {
                return m_ModelSpace;
            }
            set
            {
                m_ModelSpace = value;

                updateMatrix = false;
            }
        }

        public LineDrawerGameComponent(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            m_Scenery = (SceneryGameComponent)this.Game.Services.GetService(typeof(SceneryGameComponent));

            declaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionColor.VertexElements);

            VertexPositionColor v1 = new VertexPositionColor();
            v1.Position = Vector3.Up;
            v1.Color = Color.Red;

            VertexPositionColor v2 = new VertexPositionColor();
            v2.Position = Vector3.Zero;
            v2.Color = Color.Red;

            VertexPositionColor v3 = new VertexPositionColor();
            v3.Position = Vector3.Right;
            v3.Color = Color.Blue;

            VertexPositionColor v4 = new VertexPositionColor();
            v4.Position = Vector3.Zero;
            v4.Color = Color.Blue;

            VertexPositionColor v5 = new VertexPositionColor();
            v5.Position = Vector3.Forward;
            v5.Color = Color.Green;

            VertexPositionColor v6 = new VertexPositionColor();
            v6.Position = Vector3.Zero;
            v6.Color = Color.Green;

            VertexPositionColor[] vertList = new VertexPositionColor[] { v1, v2, v3, v4, v5, v6 };

            //buffer = new VertexBuffer(
            //    this.GraphicsDevice,
            //    VertexPositionColor.SizeInBytes * vertList.Length,
            //    ResourceUsage.WriteOnly);
            buffer = new VertexBuffer(
                 this.GraphicsDevice,
                 VertexPositionColor.SizeInBytes * vertList.Length,
                 BufferUsage.WriteOnly);
            buffer.SetData<VertexPositionColor>(vertList);

            effect = new BasicEffect(this.GraphicsDevice, null);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (updateMatrix)
            {
                m_ModelSpace = Matrix.CreateScale(Length) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);
            }

            updateMatrix = true;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.GraphicsDevice.VertexDeclaration = declaration;
            this.GraphicsDevice.Vertices[0].SetSource(buffer, 0, VertexPositionColor.SizeInBytes);

            effect.View = BaseCameraGameComponent.gViewMatrix;
            effect.Projection = BaseCameraGameComponent.gGlobalProjectionMatrix;
            effect.World = m_ModelSpace;

            effect.TextureEnabled = false;
            effect.VertexColorEnabled = true;

            effect.Begin();

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                this.GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 3);

                pass.End();
            }

            effect.End();
        }
    }
}


