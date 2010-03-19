using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Common.Primitives
{
    using Common;
    using Common.Components;

    /// <summary>
    /// Nodo del escenario
    /// </summary>
    public class SceneryNode
    {
        /// <summary>
        /// Bordes de un nodo
        /// </summary>
        public enum NodeHeads
        {
            /// <summary>
            /// Noroeste
            /// </summary>
            NorthWest,
            /// <summary>
            /// Noreste
            /// </summary>
            NorthEast,
            /// <summary>
            /// Suroeste
            /// </summary>
            SouthWest,
            /// <summary>
            /// Sureste
            /// </summary>
            SouthEast,
        }

        /// <summary>
        /// Caja alineada con los ejes que contiene el nodo completo
        /// </summary>
        private BoundingBox m_AABB;
        /// <summary>
        /// Esfera que contiene el nodo completo
        /// </summary>
        private BoundingSphere m_SPH;

        /// <summary>
        /// Nodo padre
        /// </summary>
        protected SceneryNode Parent;
        /// <summary>
        /// Nodos hijo
        /// </summary>
        protected Dictionary<NodeHeads, SceneryNode> Childs = new Dictionary<NodeHeads, SceneryNode>();
        /// <summary>
        /// Indica cuál es el punto geométrico central del nodo
        /// </summary>
        protected Vector3 NodeCenter = Vector3.Zero;
        /// <summary>
        /// Indica si el nodo debe ser dibujado
        /// </summary>
        protected bool ToDraw = false;

        /// <summary>
        /// Indica si el nodo tiene nodos hijo
        /// </summary>
        public bool HasChilds
        {
            get
            {
                if (this.Childs != null)
                {
                    return (this.Childs.Values.Count > 0);
                }

                return false;
            }
        }
        /// <summary>
        /// Obtiene el nodo hijo al noroeste
        /// </summary>
        public SceneryNode NorthWest
        {
            get
            {
                if (this.Childs != null)
                {
                    return this.Childs[NodeHeads.NorthWest];
                }

                return null;
            }
        }
        /// <summary>
        /// Obtiene el nodo hijo al noreste
        /// </summary>
        public SceneryNode NorthEast
        {
            get
            {
                if (this.Childs != null)
                {
                    return this.Childs[NodeHeads.NorthEast];
                }

                return null;
            }
        }
        /// <summary>
        /// Obtiene el nodo hijo al suroeste
        /// </summary>
        public SceneryNode SouthWest
        {
            get
            {
                if (this.Childs != null)
                {
                    return this.Childs[NodeHeads.SouthWest];
                }

                return null;
            }
        }
        /// <summary>
        /// Obtiene el nodo hijo al sureste
        /// </summary>
        public SceneryNode SouthEast
        {
            get
            {
                if (this.Childs != null)
                {
                    return this.Childs[NodeHeads.SouthEast];
                }

                return null;
            }
        }
        /// <summary>
        /// Obtiene la caja alineada con los ejes que rodea al nodo
        /// </summary>
        public BoundingBox AABB
        {
            get
            {
                return this.m_AABB;
            }
            protected set
            {
                this.m_AABB = value;
            }
        }
        /// <summary>
        /// Obtiene la esfera que rodea al nodo
        /// </summary>
        public BoundingSphere SPH
        {
            get
            {
                return this.m_SPH;
            }
            protected set
            {
                this.m_SPH = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneryNode()
        {

        }

        /// <summary>
        /// Obtiene la altura en el punto especificado
        /// </summary>
        /// <param name="x">Coordenada X</param>
        /// <param name="z">Coordenada Y</param>
        /// <returns>Devuelve la componente Y en las coordenadas X y Z dadas</returns>
        public virtual float? GetHeigthAtPoint(float x, float z)
        {
            float maxX = this.AABB.Max.X;
            float maxZ = this.AABB.Max.Z;

            float minX = this.AABB.Min.X;
            float minZ = this.AABB.Min.Z;

            if (x >= minX && x <= maxX && z >= minZ && z <= maxZ)
            {
                if (this.HasChilds)
                {
                    float? height = null;

                    height = this.Childs[NodeHeads.NorthEast].GetHeigthAtPoint(x, z);
                    if (height.HasValue)
                    {
                        return height;
                    }

                    height = this.Childs[NodeHeads.NorthWest].GetHeigthAtPoint(x, z);
                    if (height.HasValue)
                    {
                        return height;
                    }

                    height = this.Childs[NodeHeads.SouthEast].GetHeigthAtPoint(x, z);
                    if (height.HasValue)
                    {
                        return height;
                    }

                    height = this.Childs[NodeHeads.SouthWest].GetHeigthAtPoint(x, z);
                    if (height.HasValue)
                    {
                        return height;
                    }
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Construye la jerarquía del nodo actual
        /// </summary>
        /// <param name="northWest">Nodo al noroeste</param>
        /// <param name="northEast">Nodo al noreste</param>
        /// <param name="southWest">Nodo al suroeste</param>
        /// <param name="southEast">Nodo al sureste</param>
        public virtual void Build(SceneryNode northWest, SceneryNode northEast, SceneryNode southWest, SceneryNode southEast)
        {
            northWest.Parent = this;
            northEast.Parent = this;
            southWest.Parent = this;
            southEast.Parent = this;

            Childs.Clear();

            Childs.Add(NodeHeads.NorthWest, northWest);
            Childs.Add(NodeHeads.NorthEast, northEast);
            Childs.Add(NodeHeads.SouthWest, southWest);
            Childs.Add(NodeHeads.SouthEast, southEast);

            BoundingBox northAABB = BoundingBox.CreateMerged(northWest.m_AABB, northEast.m_AABB);
            BoundingBox southAABB = BoundingBox.CreateMerged(southWest.m_AABB, southEast.m_AABB);
            m_AABB = BoundingBox.CreateMerged(northAABB, southAABB);

            BoundingSphere northSPH = BoundingSphere.CreateMerged(northWest.m_SPH, northEast.m_SPH);
            BoundingSphere southSPH = BoundingSphere.CreateMerged(southWest.m_SPH, southEast.m_SPH);
            m_SPH = BoundingSphere.CreateMerged(northSPH, southSPH);

            NodeCenter = Vector3.Divide(m_AABB.Max + m_AABB.Min, 2.0f);
        }
        /// <summary>
        /// Prepara el nodo y sus hijos para el dibujado
        /// </summary>
        public virtual void PrepareForDrawing()
        {
            ToDraw = false;

            foreach (SceneryNode node in Childs.Values)
            {
                node.PrepareForDrawing();
            }
        }
        /// <summary>
        /// Obtiene los nodos a dibujar
        /// </summary>
        /// <returns>Lista de nodos a dibujar</returns>
        public virtual SceneryNode[] GetNodesToDraw(LOD lod)
        {
            // Comprobar si el nodo está en el cono de visión del nivel de detalle especificado
            if (NodeIsInLODFrustum(lod))
            {
                // Inicializar la colección de nodos
                List<SceneryNode> resultNodes = new List<SceneryNode>();

                foreach (SceneryNode childNode in Childs.Values)
                {
                    // Obtener los nodos a dibujar de los nodos hijo
                    resultNodes.AddRange(childNode.GetNodesToDraw(lod));
                }

                // Marcar este nodo para dibujar
                ToDraw = true;

                // Devolver los nodos obtenidos para dibujar
                return resultNodes.ToArray();
            }

            // Devolver una colección vacía
            return new SceneryNode[] { };
        }
        /// <summary>
        /// Obtiene si el nodo actual está en el cono de visión del nivel de detalle especificado
        /// </summary>
        /// <param name="lod">Nivel de detalle</param>
        /// <returns>Devuelve verdadero si está en el cono, falso si no lo está</returns>
        /// <remarks>Este test se realiza usando el BoundingBox del nodo</remarks>
        protected bool NodeIsInLODFrustum(LOD lod)
        {
            BoundingFrustum frustum = GlobalMatrices.gGlobalFrustum;
            if (lod == LOD.High)
            {
                frustum = GlobalMatrices.gLODHighFrustum;
            }
            else if (lod == LOD.Medium)
            {
                frustum = GlobalMatrices.gLODMediumFrustum;
            }
            else if (lod == LOD.Low)
            {
                frustum = GlobalMatrices.gLODLowFrustum;
            }

            return frustum.Intersects(m_AABB);
        }
        /// <summary>
        /// Obtiene la lista de nodos dibujados
        /// </summary>
        /// <returns>Lista de nodos dibujados</returns>
        public virtual SceneryInfoNodeDrawn[] GetNodesDrawn(LOD lod)
        {
            List<SceneryInfoNodeDrawn> nodesDrawn = new List<SceneryInfoNodeDrawn>();

            if (ToDraw)
            {
                foreach (SceneryNode node in Childs.Values)
                {
                    nodesDrawn.AddRange(node.GetNodesDrawn(lod));
                }
            }

            return nodesDrawn.ToArray();
        }

        /// <summary>
        /// Obtener el nodo del mismo nivel a norte
        /// </summary>
        /// <returns>Devuelve el nodo del mismo nivel al norte</returns>
        public virtual SceneryNode GetNorthNode()
        {
            if (this.Parent != null)
            {
                if (this == this.Parent.NorthWest)
                {
                    // Obtener el nodo al norte
                    SceneryNode node = this.Parent.GetNorthNode();
                    if (node != null)
                    {
                        // Obtener el nodo al suroeste del nodo al norte
                        return node.SouthWest;
                    }
                }
                else if (this == this.Parent.NorthEast)
                {
                    // Obtener el nodo al norte
                    SceneryNode node = this.Parent.GetNorthNode();
                    if (node != null)
                    {
                        // Obtener el nodo al sureste del nodo al norte
                        return node.SouthEast;
                    }
                }
                else if (this == this.Parent.SouthWest)
                {
                    // Obtener el nodo al noroeste del nodo actual
                    return this.Parent.NorthWest;
                }
                else if (this == this.Parent.SouthEast)
                {
                    // Obtener el nodo al noreste del nodo actual
                    return this.Parent.NorthEast;
                }
            }

            return null;
        }
        /// <summary>
        /// Obtener el nodo del mismo nivel a sur
        /// </summary>
        /// <returns>Devuelve el nodo del mismo nivel al sur</returns>
        public virtual SceneryNode GetSouthNode()
        {
            if (this.Parent != null)
            {
                if (this == this.Parent.SouthEast)
                {
                    SceneryNode node = this.Parent.GetSouthNode();
                    if (node != null)
                    {
                        return node.NorthEast;
                    }
                }
                else if (this == this.Parent.SouthWest)
                {
                    SceneryNode node = this.Parent.GetSouthNode();
                    if (node != null)
                    {
                        return node.NorthWest;
                    }
                }
                else if (this == this.Parent.NorthEast)
                {
                    return this.Parent.SouthEast;
                }
                else if (this == this.Parent.NorthWest)
                {
                    return this.Parent.SouthWest;
                }
            }

            return null;
        }
        /// <summary>
        /// Obtener el nodo del mismo nivel a oeste
        /// </summary>
        /// <returns>Devuelve el nodo del mismo nivel al oeste</returns>
        public virtual SceneryNode GetWestNode()
        {
            if (this.Parent != null)
            {
                if (this == this.Parent.SouthEast)
                {
                    return this.Parent.SouthWest;
                }
                else if (this == this.Parent.SouthWest)
                {
                    SceneryNode node = this.Parent.GetWestNode();
                    if (node != null)
                    {
                        return node.SouthEast;
                    }
                }
                else if (this == this.Parent.NorthEast)
                {
                    return this.Parent.NorthWest;
                }
                else if (this == this.Parent.NorthWest)
                {
                    SceneryNode node = this.Parent.GetWestNode();
                    if (node != null)
                    {
                        return node.NorthEast;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Obtener el nodo del mismo nivel a este
        /// </summary>
        /// <returns>Devuelve el nodo del mismo nivel al este</returns>
        public virtual SceneryNode GetEastNode()
        {
            if (this.Parent != null)
            {
                if (this == this.Parent.SouthEast)
                {
                    SceneryNode node = this.Parent.GetEastNode();
                    if (node != null)
                    {
                        return node.SouthWest;
                    }
                }
                else if (this == this.Parent.SouthWest)
                {
                    return this.Parent.SouthEast;
                }
                else if (this == this.Parent.NorthEast)
                {
                    SceneryNode node = this.Parent.GetEastNode();
                    if (node != null)
                    {
                        return node.NorthWest;
                    }
                }
                else if (this == this.Parent.NorthWest)
                {
                    return this.Parent.NorthEast;
                }
            }

            return null;
        }
    }
}
