using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameComponents.Scenery
{
    using Common.Components;
    using Common.Primitives;
    using Physics;

    public partial class Scenery : IPhysicObject
    {
        /// <summary>
        /// Obtiene la primitiva de colisión del terreno
        /// </summary>
        /// <returns>Siempre devuelve null</returns>
        public CollisionPrimitive GetPrimitive()
        {
            return null;
        }
        /// <summary>
        /// Obtiene la primitiva de colisión del objeto actual que potencialmente puede colisionar con el objeto especificado
        /// </summary>
        /// <param name="physicObject">Objeto físico</param>
        /// <returns>Devuelve una nueva primitiva de colisión si hay colisión potencial, o null en otro caso</returns>
        public CollisionPrimitive GetContactedPrimitive(IPhysicObject physicObject)
        {
            if (physicObject != null)
            {
                // Obtener la lista de triángulos potencialmente implicados en la colisión
                Triangle[] tris = GetIntersected(physicObject);
                if (tris != null && tris.Length > 0)
                {
                    // Crear una nueva lista de triángulos para la intersección
                    return new CollisionTriangleSoup(tris, float.PositiveInfinity);
                }
            }

            return null;
        }
        /// <summary>
        /// Obtiene la caja alineada con los ejes que rodea a todo el terreno
        /// </summary>
        /// <returns>Devuelve la caja alineada con los ejes que rodea a todo el terreno</returns>
        public BoundingBox GetAABB()
        {
            return this.Root.AABB;
        }
        /// <summary>
        /// Obtiene la esfera que rodea a todo el terreno
        /// </summary>
        /// <returns>Devuelve la esfera que rodea a todo el terreno</returns>
        public BoundingSphere GetSPH()
        {
            return this.Root.SPH;
        }
        /// <summary>
        /// Devuelve si el terreno está activo
        /// </summary>
        /// <returns>El terreno nunca está activo. Siempre devuelve falso</returns>
        public bool IsActive()
        {
            return false;
        }
        /// <summary>
        /// Obtiene la lista de triángulos con los que potencialmente puede haber colisión
        /// </summary>
        /// <param name="physicObject">Objeto físico</param>
        /// <returns>Devuelve la lista de triángulos que pueden colisionar con el objeto</returns>
        private Triangle[] GetIntersected(IPhysicObject physicObject)
        {
            // Obtener la primitiva de colisión del objeto
            if (physicObject != null)
            {
                // Obtener la caja circundante en coordenadas del objeto
                BoundingBox aabb = physicObject.GetAABB();

                // Obtener los nodos en los que se encentra el AABB del objeto
                SceneryTriangleNode[] nodes = this.GetNodes(aabb);
                if (nodes != null && nodes.Length > 0)
                {
                    // Obtener la esfera circundate en coordenadas del objeto
                    BoundingSphere shp = physicObject.GetSPH();

                    List<Triangle> triangleList = new List<Triangle>();

                    foreach (SceneryTriangleNode node in nodes)
                    {
                        // Obtener la lista de triángulos que potencial pueden intersectar con la SPH del objeto
                        Triangle[] triangles = node.GetIntersectedTriangles(shp);
                        if (triangles != null && triangles.Length > 0)
                        {
                            triangleList.AddRange(triangles);
                        }
                    }

                    if (triangleList.Count > 0)
                    {
                        return triangleList.ToArray();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Actualiza las variables físicas del escenario
        /// </summary>
        /// <param name="time">Tiempo de integración</param>
        public void Integrate(float time)
        {

        }

        /// <summary>
        /// Evento que se produce cuando el escenario ha sido contactado
        /// </summary>
        public event ObjectInContactDelegate OnObjectContacted;
        /// <summary>
        /// Disparador del evento de escenario contactado
        /// </summary>
        /// <param name="obj"></param>
        public void Contacted(IPhysicObject obj)
        {
            if (this.OnObjectContacted != null)
            {
                this.OnObjectContacted(obj);
            }
        }
    }
}
