using System;
using Microsoft.Xna.Framework;
using Physics;
using Microsoft.Xna.Framework.Graphics;
using DrawingComponents;

namespace BigBallisticDemo
{
    /// <summary>
    /// Una bala
    /// </summary>
    class AmmoRound : CollisionSphere
    {
        /// <summary>
        /// Tipo de munición
        /// </summary>
        private ShotType m_ShotType;
        /// <summary>
        /// Posición original del disparo
        /// </summary>
        private Vector3 OriginalPosition;

        /// <summary>
        /// Obtiene el tipo de disparo
        /// </summary>
        public ShotType ShotType
        {
            get
            {
                return m_ShotType;
            }
        }

        /// <summary>
        /// Establece el tipo de munición de la bala y la activa
        /// </summary>
        /// <param name="shotType">Tipo de munición</param>
        /// <param name="position">Posición de partida de la bala</param>
        /// <param name="direction">Dirección del disparo</param>
        public void Fire(ShotType shotType, Vector3 position, Vector3 direction)
        {
            this.m_ShotType = shotType;
            this.OriginalPosition = position;

            // Establece las propiedades de la bala según el tipo especificado
            if (this.m_ShotType == ShotType.HeavyBolter)
            {
                this.Body.Mass = 1f;
                this.Body.Velocity = Vector3.Normalize(direction) * 50.0f;
                this.Body.Acceleration = Physics.Constants.FastProyectileGravityForce;
                this.Body.SetDamping(0.99f, 0.8f);
                this.Radius = 0.2f;
            }
            else if (this.m_ShotType == ShotType.Artillery)
            {
                this.Body.Mass = 500f;
                this.Body.Velocity = Vector3.Normalize(direction + Vector3.Up) * 50.0f;
                this.Body.Acceleration = Physics.Constants.GravityForce;
                this.Body.SetDamping(0.99f, 0.8f);
                this.Radius = 0.4f;
            }
            else if (this.m_ShotType == ShotType.FlameThrower)
            {
                this.Body.Mass = 0.1f;
                this.Body.Velocity = Vector3.Normalize(direction + (Vector3.Up * 0.5f)) * 30.0f;
                this.Body.Acceleration = Physics.Constants.GravityForce;
                this.Body.SetDamping(0.99f, 0.8f);
                this.Radius = 0.6f;
            }
            else if (this.m_ShotType == ShotType.Laser)
            {
                this.Body.Mass = 0.1f;
                this.Body.Velocity = Vector3.Normalize(direction) * 100.0f;
                this.Body.Acceleration = Physics.Constants.ZeroMassGravityForce;
                this.Body.SetDamping(0.99f, 0.8f);
                this.Radius = 0.2f;
            }

            // Activar el cuerpo
            this.Body.CanSleep = false;
            this.Body.IsAwake = true;

            // Establecer la inercia
            float coeff = 0.4f * Body.Mass * Radius * Radius;
            this.Body.InertiaTensor = Core.SetInertiaTensorCoeffs(coeff, coeff, coeff);

            // Establecer la posición inicial de la bala
            this.Body.Position = position;

            // Limpiar los acumuladores de fuerzas
            this.Body.CalculateDerivedData();

            // Calcular las variables internas
            this.CalculateInternals();
        }
        /// <summary>
        /// Valida si la bala sigue viva. Si no sigue viva modifica su tipo a Sin Uso
        /// </summary>
        /// <returns>Devuelve verdadero si la bala sigue viva</returns>
        public bool IsAlive()
        {
            float distance = Math.Abs(Vector3.Distance(this.OriginalPosition, this.Position));
            if (this.m_ShotType == ShotType.HeavyBolter)
            {
                if (distance > 100.0f)
                {
                    this.m_ShotType = ShotType.UnUsed;
                }
            }
            else if (this.m_ShotType == ShotType.Artillery)
            {
                if (distance > 1000.0f)
                {
                    this.m_ShotType = ShotType.UnUsed;
                }
            }
            else if (this.m_ShotType == ShotType.FlameThrower)
            {
                if (distance > 60.0f)
                {
                    this.m_ShotType = ShotType.UnUsed;
                }
            }
            else if (this.m_ShotType == ShotType.Laser)
            {
                if (distance > 300.0f)
                {
                    this.m_ShotType = ShotType.UnUsed;
                }
            }

            return (this.m_ShotType != ShotType.UnUsed);
        }
        /// <summary>
        /// Desactiva la bala
        /// </summary>
        public void Deactivate()
        {
            this.m_ShotType = ShotType.UnUsed;
        }

        /// <summary>
        /// Indica que la bala ha impactado con algo
        /// </summary>
        /// <param name="primitive">Primitiva de colisión con la que ha impactado la bala</param>
        public override void PrimitiveContacted(CollisionPrimitive primitive)
        {
            base.PrimitiveContacted(primitive);

            // Desactivar la bala
            this.m_ShotType = ShotType.UnUsed;
        }
    }
}