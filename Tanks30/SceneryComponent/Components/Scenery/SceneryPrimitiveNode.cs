using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using CustomProcessors;
using Physics;

namespace GameComponents.Scenery
{
    /// <summary>
    /// Nodo de escenario con primitivas
    /// </summary>
    internal class SceneryTriangleNode : SceneryNode
    {
        // Nivel de detalle con el que se dibujó la última vez
        private LOD m_Lod = LOD.None;

        // Diccionario de índices base para renderizar según el nivel de detalle
        private Dictionary<LOD, int> m_StartIndexes = new Dictionary<LOD, int>();
        // Diccionario de número de primitivas a renderizar según el nivel de detalle
        private Dictionary<LOD, int> m_TriangleCount = new Dictionary<LOD, int>();

        /// <summary>
        /// Lista de primitivas del nodo
        /// </summary>
        protected TriangleList m_Triangles = new TriangleList();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scenery">Escenario</param>
        public SceneryTriangleNode(SceneryGameComponent scenery)
            : base(scenery)
        {

        }

        /// <summary>
        /// Construye el nodo
        /// </summary>
        /// <param name="triangles">Lista de triángulos del nodo</param>
        public virtual void Build(TriangleList triangles)
        {
            m_Triangles = triangles;

            if ((m_Triangles != null) && (m_Triangles.Count > 0))
            {
                m_BoundingBox = m_Triangles.AABB;

                m_NodeCenter = Vector3.Divide(m_BoundingBox.Max + m_BoundingBox.Min, 2.0f);
            }
        }
        /// <summary>
        /// Prepara el nodo para ser dibujado
        /// </summary>
        public override void PrepareForDrawing()
        {
            m_Lod = LOD.None;

            m_Drawn = false;
        }
        /// <summary>
        /// Obtiene la lista de nodos hijo para dibujar
        /// </summary>
        /// <param name="lod">Nivel de detalle</param>
        /// <returns>Lista de nodos hijo a dibujar</returns>
        public override SceneryNode[] GetNodesToDraw(LOD lod)
        {
            // Sólo se procesa este nodo si no ha sido marcado como dibujado ya
            if (!m_Drawn)
            {
                // Comprobar si el nodo está en el cono de visión del nivel de detalle especificado
                if (NodeIsInLODFrustum(lod))
                {
                    // Establecer el nivel de detalle
                    m_Lod = lod;

                    // Establecer que se va a dibujar
                    m_Drawn = true;

                    // Devolverse a sí mismo como resultado
                    return new SceneryNode[] { this };
                }
            }

            // Devolver una colección vacía
            return new SceneryNode[] { };
        }
        /// <summary>
        /// Obtiene la lista de nodos hijo dibujados desde el nodo actual
        /// </summary>
        /// <returns>Devuelve la lista de nodos hijo dibujados desde el nodo actual</returns>
        public override SceneryInfoNodeDrawn[] GetNodesDrawn(LOD lod)
        {
            List<SceneryInfoNodeDrawn> nodesDrawn = new List<SceneryInfoNodeDrawn>();

            if ((m_Lod == lod) && (m_Lod != LOD.None))
            {
                if (m_Triangles.Count > 0)
                {
                    SceneryInfoNodeDrawn nodeDrawn = new SceneryInfoNodeDrawn(m_BoundingBox.Max.X, m_BoundingBox.Max.Z, m_BoundingBox.Min.X, m_BoundingBox.Min.Z);

                    nodesDrawn.Add(nodeDrawn);
                }
            }

            return nodesDrawn.ToArray();
        }

        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el nodo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="triangle">Devuelve el triángulo de corte más cercano si existe</param>
        /// <param name="intersectionPoint">Devuelve el punto de intersección con el triángulo si existe</param>
        /// <param name="distanceToPoint">Devuelve la distancia entre el origen del rayo y el punto de intersección si existe</param>
        /// <returns>Devuelve verdadero si hay intersección o falso en el resto de los casos</returns>
        public override bool Intersects(Ray ray, out Triangle? triangle, out Vector3? intersectionPoint, out float? distanceToPoint)
        {
            triangle = null;
            intersectionPoint = null;
            distanceToPoint = null;

            if (m_Triangles.Count > 0)
            {
                Triangle? pTriangle = null;
                Vector3? pIntersectionPoint = null;
                float? pDistanceToPoint = null;

                if (m_Triangles.Intersects(ray, out pTriangle, out pIntersectionPoint, out pDistanceToPoint))
                {
                    triangle = pTriangle;
                    intersectionPoint = pIntersectionPoint;
                    distanceToPoint = pDistanceToPoint;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Obtiene el índice base del indexbuffer para dibujar según el nivel de detalle especificado
        /// </summary>
        /// <param name="lod">Nivel de detalle</param>
        /// <returns>Devuelve el índice base del indexbuffer</returns>
        public int GetStartIndex(LOD lod)
        {
            return m_StartIndexes[lod];
        }
        /// <summary>
        /// Obtiene el número de primitivas a dibujar según el nivel de detalle especificado
        /// </summary>
        /// <param name="lod">Nivel de detalle</param>
        /// <returns>Devuelve el número de primitivas según el nivel de detalle</returns>
        public int GetPrimitiveCount(LOD lod)
        {
            return m_TriangleCount[lod];
        }
        /// <summary>
        /// Establece los parámetros para renderizar el nodo según el nivel de detalle
        /// </summary>
        /// <param name="lod">Nivel de detalle</param>
        /// <param name="startIndex">Indice base del index buffer</param>
        /// <param name="primitiveCount">Número de primitivas en el index buffer para este nodo</param>
        internal void SetRenderParams(LOD lod, int startIndex, int primitiveCount)
        {
            if (m_StartIndexes.ContainsKey(lod))
            {
                m_StartIndexes[lod] = startIndex;
            }
            else
            {
                m_StartIndexes.Add(lod, startIndex);
            }

            if (m_TriangleCount.ContainsKey(lod))
            {
                m_TriangleCount[lod] = primitiveCount;
            }
            else
            {
                m_TriangleCount.Add(lod, primitiveCount);
            }
        }
    }
}
