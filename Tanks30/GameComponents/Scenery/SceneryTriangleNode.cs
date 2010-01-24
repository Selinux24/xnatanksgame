using System.Collections.Generic;
using Common.Components;
using Microsoft.Xna.Framework;
using Physics;

namespace GameComponents.Scenery
{
    /// <summary>
    /// Nodo de escenario con primitivas
    /// </summary>
    public class SceneryTriangleNode : SceneryNode
    {
        /// <summary>
        /// Nivel de detalle con el que se dibujó la última vez
        /// </summary>
        private LOD m_Lod = LOD.None;

        /// <summary>
        /// Lista de primitivas del nodo
        /// </summary>
        public readonly CollisionTriangleSoup TriangleSoup;
        /// <summary>
        /// Diccionario de índices base para renderizar según el nivel de detalle
        /// </summary>
        public readonly Dictionary<LOD, int> StartIndexes;
        /// <summary>
        /// Diccionario de número de primitivas a renderizar según el nivel de detalle
        /// </summary>
        public readonly Dictionary<LOD, int> PrimitiveCount;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scenery">Escenario</param>
        public SceneryTriangleNode(Triangle[] triangles, Dictionary<LOD, int> startIndexes, Dictionary<LOD, int> triangleCount)
            : base()
        {
            this.TriangleSoup = new CollisionTriangleSoup(triangles);

            this.StartIndexes = startIndexes;

            this.PrimitiveCount = triangleCount;

            if ((this.TriangleSoup != null) && (this.TriangleSoup.Triangles.Length > 0))
            {
                this.BoundingBox = this.TriangleSoup.AABB;

                this.NodeCenter = Vector3.Divide(this.BoundingBox.Max + this.BoundingBox.Min, 2.0f);
            }
        }

        /// <summary>
        /// Prepara el nodo para ser dibujado
        /// </summary>
        public override void PrepareForDrawing()
        {
            this.m_Lod = LOD.None;

            this.Drawn = false;
        }
        /// <summary>
        /// Obtiene la lista de nodos hijo para dibujar
        /// </summary>
        /// <param name="lod">Nivel de detalle</param>
        /// <returns>Lista de nodos hijo a dibujar</returns>
        public override SceneryNode[] GetNodesToDraw(LOD lod)
        {
            // Sólo se procesa este nodo si no ha sido marcado como dibujado ya
            if (!this.Drawn)
            {
                // Comprobar si el nodo está en el cono de visión del nivel de detalle especificado
                if (this.NodeIsInLODFrustum(lod))
                {
                    // Establecer el nivel de detalle
                    this.m_Lod = lod;

                    // Establecer que se va a dibujar
                    this.Drawn = true;

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

            if ((this.m_Lod == lod) && (this.m_Lod != LOD.None))
            {
                if (this.TriangleSoup.Triangles.Length > 0)
                {
                    SceneryInfoNodeDrawn nodeDrawn = new SceneryInfoNodeDrawn(
                        this.BoundingBox.Max.X,
                        this.BoundingBox.Max.Z,
                        this.BoundingBox.Min.X,
                        this.BoundingBox.Min.Z);

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

            if (this.TriangleSoup.Triangles.Length > 0)
            {
                Triangle? pTriangle = null;
                Vector3? pIntersectionPoint = null;
                float? pDistanceToPoint = null;

                if (IntersectionTests.TriangleSoupAndRay(this.TriangleSoup, ray, out pTriangle, out pIntersectionPoint, out pDistanceToPoint))
                {
                    triangle = pTriangle;
                    intersectionPoint = pIntersectionPoint;
                    distanceToPoint = pDistanceToPoint;

                    return true;
                }
            }

            return false;
        }
    }
}
