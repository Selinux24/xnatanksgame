
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace GameComponents.Camera
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public partial class FirstPersonCameraGameComponent : BaseCameraGameComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public FirstPersonCameraGameComponent(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Obtener la dirección fw de la matriz de rotación actual
            m_Direction = gRotationMatrix.Forward;

            // Calcular la matriz de vista
            m_ViewMatrix = Matrix.CreateLookAt(
                m_Position,
                m_Position + m_Direction,
                Vector3.Up);
        }
    }
}