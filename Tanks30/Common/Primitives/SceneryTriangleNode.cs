using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Common.Primitives
{
    using Common.Components;

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
        public readonly Triangle[] TriangleList;
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
            this.TriangleList = triangles;

            this.StartIndexes = startIndexes;

            this.PrimitiveCount = triangleCount;

            if ((this.TriangleList != null) && (this.TriangleList.Length > 0))
            {
                this.AABB = BoundingBoxEx.CreateFromTriangles(this.TriangleList);
                this.SPH = BoundingSphereEx.CreateBoundingSphereFromTriangles(this.TriangleList);

                this.NodeCenter = Vector3.Divide(this.AABB.Max + this.AABB.Min, 2.0f);
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
                if (this.TriangleList.Length > 0)
                {
                    SceneryInfoNodeDrawn nodeDrawn = new SceneryInfoNodeDrawn(
                        this.AABB.Max.X,
                        this.AABB.Max.Z,
                        this.AABB.Min.X,
                        this.AABB.Min.Z);

                    nodesDrawn.Add(nodeDrawn);
                }
            }

            return nodesDrawn.ToArray();
        }

        /// <summary>
        /// Obtiene la lista de triángulos en intersección la esfera seleccionada
        /// </summary>
        /// <param name="sph">Esfera</param>
        /// <returns>Devuelve una lista de triángulos en intersección o null si no hay intersección</returns>
        public Triangle[] GetIntersectedTriangles(BoundingSphere sph)
        {
            List<Triangle> resultList = new List<Triangle>();

            foreach (Triangle triangle in this.TriangleList)
            {
                // TODO: Demasiados resultados
                if (sph.Intersects(triangle.Plane) == PlaneIntersectionType.Intersecting)
                {
                    resultList.Add(triangle);
                }
            }

            return resultList.ToArray();
        }
    }
}
