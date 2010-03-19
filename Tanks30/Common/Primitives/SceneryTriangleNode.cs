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
        /// Indica si el nodo al norte se ha renderizado con menos resolución
        /// </summary>
        public bool IsNorthBorder
        {
            get
            {
                // Obtener el nodo al norte
                SceneryTriangleNode north = this.GetNorthNode();
                if (north != null && north.m_Lod != LOD.None)
                {
                    // El nodo al norte se ha dibujado
                    if ((int)north.m_Lod > (int)this.m_Lod)
                    {
                        // El nodo al norte tiene menos resolución que el nodo actual
                        return true;
                    }
                }

                return false;
            }
        }
        /// <summary>
        /// Indica si el nodo al sur se ha renderizado con menos resolución
        /// </summary>
        public bool IsSouthBorder
        {
            get
            {
                SceneryTriangleNode south = this.GetSouthNode();
                if (south != null && south.m_Lod != LOD.None)
                {
                    if ((int)south.m_Lod > (int)this.m_Lod)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        /// <summary>
        /// Indica si el nodo al oeste se ha renderizado con menos resolución
        /// </summary>
        public bool IsWestBorder
        {
            get
            {
                SceneryTriangleNode west = this.GetWestNode();
                if (west != null && west.m_Lod != LOD.None)
                {
                    if ((int)west.m_Lod > (int)this.m_Lod)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        /// <summary>
        /// Indica si el nodo al este se ha renderizado con menos resolución
        /// </summary>
        public bool IsEastBorder
        {
            get
            {
                SceneryTriangleNode east = this.GetEastNode();
                if (east != null && east.m_Lod != LOD.None)
                {
                    if ((int)east.m_Lod > (int)this.m_Lod)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Lista de primitivas del nodo
        /// </summary>
        public readonly Triangle[] TriangleList;
        /// <summary>
        /// Diccionario de índices base para renderizar según el nivel de detalle
        /// </summary>
        public readonly Dictionary<LOD, SceneryNodeIndexInfo> IndexInfo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scenery">Escenario</param>
        public SceneryTriangleNode(Triangle[] triangles, Dictionary<LOD, SceneryNodeIndexInfo> indexInfo)
            : base()
        {
            this.TriangleList = triangles;

            this.IndexInfo = indexInfo;

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

            this.ToDraw = false;
        }
        /// <summary>
        /// Obtiene la lista de nodos hijo para dibujar
        /// </summary>
        /// <param name="lod">Nivel de detalle</param>
        /// <returns>Lista de nodos hijo a dibujar</returns>
        public override SceneryNode[] GetNodesToDraw(LOD lod)
        {
            // Sólo se procesa este nodo si no ha sido marcado como dibujado ya
            if (!this.ToDraw)
            {
                // Comprobar si el nodo está en el cono de visión del nivel de detalle especificado
                if (this.NodeIsInLODFrustum(lod))
                {
                    // Establecer el nivel de detalle
                    this.m_Lod = lod;

                    // Establecer que se va a dibujar
                    this.ToDraw = true;

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
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public override float? GetHeigthAtPoint(float x, float z)
        {
            // Obtener un punto por encima del punto máximo del nodo
            Vector3 point = new Vector3(x, this.AABB.Max.Y + 1f, z);

            // Crear un rayo desde el punto hacia abajo
            Ray ray = new Ray(point, Vector3.Down);

            // Intersectar con cada triángulo
            foreach (Triangle triangle in this.TriangleList)
            {
                float? distance = ray.Intersects(triangle.Plane);
                if (distance.HasValue)
                {
                    Vector3 p = new Vector3(x, this.AABB.Max.Y + 1f - distance.Value, z);

                    if (Triangle.PointInTriangle(triangle, p))
                    {
                        return p.Y;
                    }
                }
            }

            return null;
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

        /// <summary>
        /// Obtener el nodo del mismo nivel a norte
        /// </summary>
        /// <returns>Devuelve el nodo del mismo nivel al norte</returns>
        public new SceneryTriangleNode GetNorthNode()
        {
            SceneryNode node = base.GetNorthNode();
            if (node != null)
            {
                return node as SceneryTriangleNode;
            }

            return null;
        }
        /// <summary>
        /// Obtener el nodo del mismo nivel a sur
        /// </summary>
        /// <returns>Devuelve el nodo del mismo nivel al sur</returns>
        public new SceneryTriangleNode GetSouthNode()
        {
            SceneryNode node = base.GetSouthNode();
            if (node != null)
            {
                return node as SceneryTriangleNode;
            }

            return null;
        }
        /// <summary>
        /// Obtener el nodo del mismo nivel a oeste
        /// </summary>
        /// <returns>Devuelve el nodo del mismo nivel al oeste</returns>
        public new SceneryTriangleNode GetWestNode()
        {
            SceneryNode node = base.GetWestNode();
            if (node != null)
            {
                return node as SceneryTriangleNode;
            }

            return null;
        }
        /// <summary>
        /// Obtener el nodo del mismo nivel a este
        /// </summary>
        /// <returns>Devuelve el nodo del mismo nivel al este</returns>
        public new SceneryTriangleNode GetEastNode()
        {
            SceneryNode node = base.GetEastNode();
            if (node != null)
            {
                return node as SceneryTriangleNode;
            }

            return null;
        }
    }
}
