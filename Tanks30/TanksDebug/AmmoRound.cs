using System;
using Microsoft.Xna.Framework;

namespace TanksDebug
{
    using Physics;

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
        /// Constructor
        /// </summary>
        /// <param name="shotType">Tipo de disparo</param>
        /// <param name="radius">Radio</param>
        /// <param name="mass">Masa</param>
        public AmmoRound(ShotType shotType, float radius, float mass)
            : base(radius, mass)
        {
            this.m_ShotType = shotType;
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
                this.SetMass(1f);
                this.SetVelocity(Vector3.Normalize(direction) * 50.0f);
                this.SetAcceleration(Physics.Constants.FastProyectileGravityForce);
                this.SetDamping(0.99f, 0.8f);
                this.Radius = 0.2f;
            }
            else if (this.m_ShotType == ShotType.Artillery)
            {
                this.SetMass(500f);
                this.SetVelocity(Vector3.Normalize(direction + Vector3.Up) * 50.0f);
                this.SetAcceleration(Physics.Constants.GravityForce);
                this.SetDamping(0.99f, 0.8f);
                this.Radius = 0.4f;
            }
            else if (this.m_ShotType == ShotType.FlameThrower)
            {
                this.SetMass(0.1f);
                this.SetVelocity(Vector3.Normalize(direction + (Vector3.Up * 0.5f)) * 30.0f);
                this.SetAcceleration(Physics.Constants.GravityForce);
                this.SetDamping(0.99f, 0.8f);
                this.Radius = 0.6f;
            }
            else if (this.m_ShotType == ShotType.Laser)
            {
                this.SetMass(0.1f);
                this.SetVelocity(Vector3.Normalize(direction) * 100.0f);
                this.SetAcceleration(Physics.Constants.ZeroMassGravityForce);
                this.SetDamping(0.99f, 0.8f);
                this.Radius = 0.2f;
            }

            // Establecer la inercia
            this.SetIntertia(0.4f * this.Mass * this.Radius * this.Radius);

            // Establecer la posición inicial de la bala
            this.SetPosition(position);

            // Este objeto no puede dormir
            this.CanSleep = false;
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
    }
}