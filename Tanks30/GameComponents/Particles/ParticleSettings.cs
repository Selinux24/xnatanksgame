using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Particles
{
    /// <summary>
    /// Propiedades de una part�cula
    /// </summary>
    public class ParticleSettings
    {
        /// <summary>
        /// Nombre de la textura a usar
        /// </summary>
        public string TextureName = null;

        /// <summary>
        /// N�mero m�ximo de part�culas
        /// </summary>
        public int MaxParticles = 1000;
        
        /// <summary>
        /// Duraci�n de la part�culas
        /// </summary>
        public TimeSpan Duration = TimeSpan.FromSeconds(1);
        /// <summary>
        /// Si es mayor que 0, algunas part�culas durar�n m�s que otras
        /// </summary>
        public float DurationRandomness = 0;

        /// <summary>
        /// Indica cuanto se ven las part�culas influenciadas por la velocidad del objeto que las cre�
        /// </summary>
        public float EmitterVelocitySensitivity = 1;

        /// <summary>
        /// M�nimo valor que se aplica para variar la velocidad horizontal de cada part�cula
        /// </summary>
        public float MinHorizontalVelocity = 0;
        /// <summary>
        /// M�ximo valor que se aplica para variar la velocidad horizontal de cada part�cula
        /// </summary>
        public float MaxHorizontalVelocity = 0;

        /// <summary>
        /// M�nimo valor que se aplica para variar la velocidad vertical de cada part�cula
        /// </summary>
        public float MinVerticalVelocity = 0;
        /// <summary>
        /// M�ximo valor que se aplica para variar la velocidad vertical de cada part�cula
        /// </summary>
        public float MaxVerticalVelocity = 0;

        /// <summary>
        /// Direcci�n y magnitud de la fuerza de gravedad aplicada a cada part�cula
        /// </summary>
        public Vector3 Gravity = Vector3.Zero;

        /// <summary>
        /// Efecto en la velocidad de cada part�cula durante su existencia
        /// </summary>
        /// <remarks>
        ///  0 - Las part�culas se congelan justo antes de desaparecer
        ///  1 - No hay variaci�n de velocidad
        /// >1 - Las part�culas se aceleran
        /// </remarks>
        public float EndVelocity = 1;

        /// <summary>
        /// Magnitud m�nima de color y transparencia de cada part�cula
        /// </summary>
        public Color MinColor = Color.White;
        /// <summary>
        /// Magnitud m�xima de color y transparencia de cada part�cula
        /// </summary>
        public Color MaxColor = Color.White;

        /// <summary>
        /// Velocidad m�nima de rotaci�n de cada part�cula
        /// </summary>
        public float MinRotateSpeed = 0;
        /// <summary>
        /// Velocidad maxima de rotaci�n de cada part�cula
        /// </summary>
        public float MaxRotateSpeed = 0;

        /// <summary>
        /// Tama�o m�nimo inicial de cada part�cula
        /// </summary>
        public float MinStartSize = 100;
        /// <summary>
        /// Tama�o maximo inicial de cada part�cula
        /// </summary>
        public float MaxStartSize = 100;

        /// <summary>
        /// Tama�o m�nimo final de cada part�cula
        /// </summary>
        public float MinEndSize = 100;
        /// <summary>
        /// Tama�o maximo final de cada part�cula
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


