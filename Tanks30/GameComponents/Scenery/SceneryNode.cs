using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Physics;

namespace GameComponents.Scenery
{
    using GameComponents.Camera;

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
        /// Bbox
        /// </summary>
        private BoundingBox m_BoundingBox = new BoundingBox();

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
        /// Indica si el nodo ha sido dibujado
        /// </summary>
        protected bool Drawn = false;

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
        /// Obtiene le Bbox
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                return this.m_BoundingBox;
            }
            protected set
            {
                this.m_BoundingBox = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneryNode()
        {

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

            foreach (SceneryNode node in Childs.Values)
            {
                if (m_BoundingBox == new BoundingBox())
                {
                    m_BoundingBox = node.m_BoundingBox;
                }
                else
                {
                    m_BoundingBox = BoundingBox.CreateMerged(m_BoundingBox, node.m_BoundingBox);
                }
            }

            NodeCenter = Vector3.Divide(m_BoundingBox.Max + m_BoundingBox.Min, 2.0f);
        }
        /// <summary>
        /// Prepara el nodo y sus hijos para el dibujado
        /// </summary>
        public virtual void PrepareForDrawing()
        {
            Drawn = false;

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
                Drawn = true;

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
            BoundingFrustum frustum = BaseCameraGameComponent.gGlobalFrustum;
            if (lod == LOD.High)
            {
                frustum = BaseCameraGameComponent.gLODHighFrustum;
            }
            else if (lod == LOD.Medium)
            {
                frustum = BaseCameraGameComponent.gLODMediumFrustum;
            }
            else if (lod == LOD.Low)
            {
                frustum = BaseCameraGameComponent.gLODLowFrustum;
            }

            return frustum.Intersects(m_BoundingBox);
        }
        /// <summary>
        /// Obtiene la lista de nodos dibujados
        /// </summary>
        /// <returns>Lista de nodos dibujados</returns>
        public virtual SceneryInfoNodeDrawn[] GetNodesDrawn(LOD lod)
        {
            List<SceneryInfoNodeDrawn> nodesDrawn = new List<SceneryInfoNodeDrawn>();

            if (Drawn)
            {
                foreach (SceneryNode node in Childs.Values)
                {
                    nodesDrawn.AddRange(node.GetNodesDrawn(lod));
                }
            }

            return nodesDrawn.ToArray();
        }

        /// <summary>
        /// Obtiene si las coordenadas especificadas están sobre el nodo actual
        /// </summary>
        /// <param name="x">Coordenada x</param>
        /// <param name="z">Coordenada z</param>
        /// <returns>Devuelve si el punto está contenido en el nodo actual</returns>
        public ContainmentType Contains(float x, float z)
        {
            Vector3 point = new Vector3(x, NodeCenter.Y, z);

            return Contains(point);
        }
        /// <summary>
        /// Obtiene si las coordenadas especificadas están sobre el nodo actual
        /// </summary>
        /// <param name="point">Punto</param>
        /// <returns>Devuelve si el punto está contenido en el nodo actual</returns>
        public ContainmentType Contains(Vector3 point)
        {
            return m_BoundingBox.Contains(point);
        }

        /// <summary>
        /// Obtiene existe intersección entre el rayo y el nodo actual
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <returns>Devuelve verdadero si hay intersección o falso en el resto de los casos</returns>
        public virtual bool Intersects(Ray ray)
        {
            Triangle? triangle = null;
            Vector3? intersectionPoint = null;
            float? distanceToPoint = null;

            return Intersects(ray, out triangle, out intersectionPoint, out distanceToPoint);
        }
        /// <summary>
        /// Obtiene existe intersección entre el rayo y el nodo actual
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="triangle">Devuelve el triángulo de corte si existe</param>
        /// <returns>Devuelve verdadero si hay intersección o falso en el resto de los casos</returns>
        public virtual bool Intersects(Ray ray, out Triangle? triangle)
        {
            Vector3? intersectionPoint = null;
            float? distanceToPoint = null;

            return Intersects(ray, out triangle, out intersectionPoint, out distanceToPoint);
        }
        /// <summary>
        /// Obtiene existe intersección entre el rayo y el nodo actual
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="triangle">Devuelve el triángulo de corte si existe</param>
        /// <param name="intersectionPoint">Devuelve el punto de corte con el triángulo si existe</param>
        /// <returns>Devuelve verdadero si hay intersección o falso en el resto de los casos</returns>
        public virtual bool Intersects(Ray ray, out Triangle? triangle, out Vector3? intersectionPoint)
        {
            float? distanceToPoint = null;

            return Intersects(ray, out triangle, out intersectionPoint, out distanceToPoint);
        }
        /// <summary>
        /// Obtiene existe intersección entre el rayo y el nodo actual
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="triangle">Devuelve el triángulo de corte si existe</param>
        /// <param name="intersectionPoint">Devuelve el punto de corte con el triángulo si existe</param>
        /// <param name="distanceToPoint">Devuelve la distancia desde el origen del rayo al punto de corte si existe</param>
        /// <returns>Devuelve verdadero si hay intersección o falso en el resto de los casos</returns>
        public virtual bool Intersects(Ray ray, out Triangle? triangle, out Vector3? intersectionPoint, out float? distanceToPoint)
        {
            triangle = null;
            intersectionPoint = null;
            distanceToPoint = null;

            float? distance = m_BoundingBox.Intersects(ray);
            if (distance.HasValue)
            {
                foreach (SceneryNode node in Childs.Values)
                {
                    Triangle? nodeTriangle = null;
                    Vector3? nodeIntersectionPoint = null;
                    float? nodeDistanceToPoint = null;

                    if (node.Intersects(ray, out nodeTriangle, out nodeIntersectionPoint, out nodeDistanceToPoint))
                    {
                        triangle = nodeTriangle;
                        intersectionPoint = nodeIntersectionPoint;
                        distanceToPoint = nodeDistanceToPoint;

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
