using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics
{
    public class Force
    {
        Vector3 m_Direction;
        float m_Magnitude;
        float m_FrictionFactor = 0f;
        Vector3 m_CurrentMomentum;

        public Force(Vector3 direction, float magnitude)
        {
            m_Direction = direction;
            m_Magnitude = magnitude;
        }

        public void Update(GameTime gameTime)
        {
            double ms = gameTime.ElapsedGameTime.TotalMilliseconds;

            m_CurrentMomentum = Vector3.Multiply(m_Direction, m_Magnitude);

            m_Magnitude *= (1f - m_FrictionFactor);
        }
    }
}
