using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameComponents.Vehicles;

namespace GameComponents.Camera
{
    /// <summary>
    /// Componente de cámara en tercera persona
    /// </summary>
    public partial class ThirdPersonCameraGameComponent : BaseCameraGameComponent
    {
        // Posición relativa del espectador
        private readonly Vector3 viewerPosition = new Vector3(0f, 10f, 18f);
        // Modelo que sigue la cámara
        private TankGameComponent m_ModelToFollow;

        /// <summary>
        /// Obtiene o establece el modelo que sigue la cámara
        /// </summary>
        public TankGameComponent ModelToFollow
        {
            get
            {
                return m_ModelToFollow;
            }
            set
            {
                m_ModelToFollow = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public ThirdPersonCameraGameComponent(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Actualiza el estado del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            if (ModelToFollow != null)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    // Obtener la rotación del objetivo desde el quaternion
                    Matrix rotation = Matrix.CreateFromQuaternion(ModelToFollow.Rotation);

                    // Calcular la posición relativa de la cámara
                    Vector3 transformedCameraPosition = Vector3.Transform(viewerPosition, rotation);

                    // Añadir la posición relativa de la cámara a la posición del modelo para obtener la posición de la cámara
                    m_Position = ModelToFollow.Position + transformedCameraPosition;

                    //La cámara mira al modelo
                    m_Direction = ModelToFollow.Position - m_Position;

                    // Crear la matriz de vista
                    m_ViewMatrix = Matrix.CreateLookAt(
                        m_Position,
                        m_Position + m_Direction,
                        Vector3.Up);
                }
                else
                {
                    // Establecer la matriz
                    m_ViewMatrix = m_ModelToFollow.CurrentView;
                }
            }
        }
    }
}


