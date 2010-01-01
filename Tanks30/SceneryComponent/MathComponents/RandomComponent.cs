﻿using System;

namespace GameComponents.MathComponents
{
    /// <summary>
    /// Componente de generación de número aleatorios
    /// </summary>
    public static class RandomComponent
    {
        /// <summary>
        /// Cantidad de números aleatorios
        /// </summary>
        private const int m_RndListLength = 16;
        /// <summary>
        /// Lista de generadores
        /// </summary>
        private static Random[] m_RndList;
        /// <summary>
        /// Indice del generador actual
        /// </summary>
        private static int m_CurrentRnd = 0;

        /// <summary>
        /// Obtiene un nuevo número aleatorio
        /// </summary>
        /// <param name="maxValue">Valor máximo</param>
        /// <returns>Devuelve el número aleatorio generado</returns>
        public static int Next(int maxValue)
        {
            if (m_RndList == null)
            {
                m_RndList = new Random[m_RndListLength];

                for (int i = 0; i < m_RndListLength; i++)
                {
                    m_RndList[i] = new Random(DateTime.Now.TimeOfDay.Milliseconds + i);
                }
            }

            Random rnd = GetRandom();

            return rnd.Next(maxValue);
        }
        /// <summary>
        /// Obtiene el siguiente generador de números aleatorios
        /// </summary>
        /// <returns>Devuelve un generador de números aleatorios</returns>
        private static Random GetRandom()
        {
            Random res = m_RndList[m_CurrentRnd];

            if (m_CurrentRnd >= m_RndListLength - 1)
            {
                m_CurrentRnd = 0;
            }
            else if (m_CurrentRnd < 0)
            {
                m_CurrentRnd = m_RndListLength - 1;
            }
            else
            {
                m_CurrentRnd++;
            }

            return res;
        }
    }
}