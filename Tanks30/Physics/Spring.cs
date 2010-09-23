using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Un generador de fuerzas de tipo muelle
    /// </summary>
    public class Spring : ForceGenerator
    {
        /// <summary>
        /// Punto de conexión del amortiguador en coordenadas locales
        /// </summary>
        Vector3 m_ConnectionPoint;

        /// <summary>
        /// Punto de conexión del amortiguador con el otro cuerpo, en coordenadas del otro cuerpo
        /// </summary>
        Vector3 m_OtherConnectionPoint;
        /// <summary>
        /// El cuerpo al otro lado del amortiguador
        /// </summary>
        RigidBody m_Other;

        /// <summary>
        /// Constante de amortiguación
        /// </summary>
        float m_SpringConstant;
        /// <summary>
        /// Longitud en la que el amortiguador está en descanso
        /// </summary>
        float m_RestLength;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="localConnectionPt">Punto de conexión del amortiguador en coordenadas locales</param>
        /// <param name="other">El cuerpo al otro lado del amortiguador</param>
        /// <param name="otherConnectionPt">Punto de conexión del amortiguador con el otro cuerpo, en coordenadas del otro cuerpo</param>
        /// <param name="springConstant">Constante de amortiguación</param>
        /// <param name="restLength">Longitud en la que el amortiguador está en descanso</param>
        public Spring(
            Vector3 localConnectionPt,
            ref RigidBody other,
            Vector3 otherConnectionPt,
            float springConstant,
            float restLength)
        {
            m_ConnectionPoint = localConnectionPt;
            m_Other = other;
            m_OtherConnectionPoint = otherConnectionPt;
            m_SpringConstant = springConstant;
            m_RestLength = restLength;
        }

        /// <summary>
        /// Aplica la fuerza del amortiguador al objeto especificado
        /// </summary>
        /// <param name="obj">Objeto</param>
        /// <param name="duration">Duración</param>
        public override void UpdateForce(ref IPhysicObject obj, float duration)
        {
            // Obtener el cuerpo del objeto
            CollisionPrimitive primitive = obj.Primitive;
            if (primitive != null)
            {
                // Calculate the two ends in world space
                Vector3 lws = primitive.GetPointInWorldSpace(m_ConnectionPoint);
                Vector3 ows = m_Other.GetPointInWorldSpace(m_OtherConnectionPoint);

                // Calculate the vector of the spring
                Vector3 force = lws - ows;

                // Calculate the magnitude of the force
                float magnitude = force.Length();
                magnitude = Math.Abs(magnitude - m_RestLength);
                magnitude *= m_SpringConstant;

                // Calculate the final force and apply it
                force.Normalize();
                force *= -magnitude;
                primitive.AddForceAtPoint(force, lws);
            }
        }
    }
}
