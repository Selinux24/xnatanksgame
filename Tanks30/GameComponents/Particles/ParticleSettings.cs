using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Particles
{
    /// <summary>
    /// Propiedades de una partícula
    /// </summary>
    public class ParticleSettings
    {
        /// <summary>
        /// Nombre de la textura a usar
        /// </summary>
        public string TextureName = null;

        /// <summary>
        /// Número máximo de partículas
        /// </summary>
        public int MaxParticles = 1000;
        
        /// <summary>
        /// Duración de la partículas
        /// </summary>
        public TimeSpan Duration = TimeSpan.FromSeconds(1);
        /// <summary>
        /// Si es mayor que 0, algunas partículas durarán más que otras
        /// </summary>
        public float DurationRandomness = 0;

        /// <summary>
        /// Indica cuanto se ven las partículas influenciadas por la velocidad del objeto que las creó
        /// </summary>
        public float EmitterVelocitySensitivity = 1;

        /// <summary>
        /// Mínimo valor que se aplica para variar la velocidad horizontal de cada partícula
        /// </summary>
        public float MinHorizontalVelocity = 0;
        /// <summary>
        /// Máximo valor que se aplica para variar la velocidad horizontal de cada partícula
        /// </summary>
        public float MaxHorizontalVelocity = 0;

        /// <summary>
        /// Mínimo valor que se aplica para variar la velocidad vertical de cada partícula
        /// </summary>
        public float MinVerticalVelocity = 0;
        /// <summary>
        /// Máximo valor que se aplica para variar la velocidad vertical de cada partícula
        /// </summary>
        public float MaxVerticalVelocity = 0;

        /// <summary>
        /// Dirección y magnitud de la fuerza de gravedad aplicada a cada partícula
        /// </summary>
        public Vector3 Gravity = Vector3.Zero;

        /// <summary>
        /// Efecto en la velocidad de cada partícula durante su existencia
        /// </summary>
        /// <remarks>
        ///  0 - Las partículas se congelan justo antes de desaparecer
        ///  1 - No hay variación de velocidad
        /// >1 - Las partículas se aceleran
        /// </remarks>
        public float EndVelocity = 1;

        /// <summary>
        /// Magnitud mínima de color y transparencia de cada partícula
        /// </summary>
        public Color MinColor = Color.White;
        /// <summary>
        /// Magnitud máxima de color y transparencia de cada partícula
        /// </summary>
        public Color MaxColor = Color.White;

        /// <summary>
        /// Velocidad mínima de rotación de cada partícula
        /// </summary>
        public float MinRotateSpeed = 0;
        /// <summary>
        /// Velocidad maxima de rotación de cada partícula
        /// </summary>
        public float MaxRotateSpeed = 0;

        /// <summary>
        /// Tamaño mínimo inicial de cada partícula
        /// </summary>
        public float MinStartSize = 100;
        /// <summary>
        /// Tamaño maximo inicial de cada partícula
        /// </summary>
        public float MaxStartSize = 100;

        /// <summary>
        /// Tamaño mínimo final de cada partícula
        /// </summary>
        public float MinEndSize = 100;
        /// <summary>
        /// Tamaño maximo final de cada partícula
        /// </summary>
        public float MaxEndSize = 100;


        /// <summary>
        /// Transparencia origen
        /// </summary>
        public Blend SourceBlend = Blend.SourceAlpha;
        /// <summary>
        /// Transparencia destino
        /// </summary>
        public Blend DestinationBlend = Blend.InverseSourceAlpha;
    }
}


