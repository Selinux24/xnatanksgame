using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics.CollideCoarse
{
    /// <summary>
    /// �rbol binario de vol�menes
    /// </summary>
    public class BVHNode
    {
        /// <summary>
        /// Nodo inmediatamente superior al actual
        /// </summary>
        public BVHNode Parent = null;
        /// <summary>
        /// El primer hijo de este nodo
        /// </summary>
        public BVHNode FirstChildren = null;
        /// <summary>
        /// El segundo hijo de este nodo
        /// </summary>
        public BVHNode LastChildren = null;
        /// <summary>
        /// Volumen que contiene todos los hijos de este nodo
        /// </summary>
        public BoundingSphere Volume;
        /// <summary>
        /// Holds the rigid body at this node of the hierarchy.
        /// Only leaf nodes can have a rigid body defined (see isLeaf).
        /// Note that it is possible to rewrite the algorithms in this
        /// class to handle objects at all levels of the hierarchy,
        /// but the code provided ignores this vector unless firstChild
        /// is NULL.
        /// </summary>
        public RigidBody Body = null;

        /// <summary>
        /// Indica si el nodo es el �ltimo de la rama
        /// </summary>
        /// <returns>Devuelve verdadero si el nodo es el �ltimo de la rama</returns>
        public bool IsLeaf
        {
            get
            {
                return (Body != null);
            }
        }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public BVHNode()
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Nodo superior</param>
        /// <param name="volume">Volumen</param>
        /// <param name="body">Cuerpo</param>
        public BVHNode(BVHNode parent, BoundingSphere volume, RigidBody body)
        {
            this.Parent = parent;
            this.Volume = volume;
            this.Body = body;
        }

        /// <summary>
        /// Obtiene el n�mero de contactos potenciales a lo largo de la estructura, hasta el l�mite establecido
        /// </summary>
        /// <param name="contacts">Contactos potenciales</param>
        /// <param name="limit">L�mite de contactos</param>
        /// <returns>Devuelve el n�mero de contactos potenciales obtenidos, hasta el l�mite de contactos</returns>
        public int GetPotentialContacts(ref List<PotentialContact> contacts, int limit)
        {
            // Si hemos llegado al final o hemos alcanzado el l�mite se termina el proceso
            if (this.IsLeaf || limit == 0)
            {
                return 0;
            }

            // Obtener los contactos potenciales entre los hijos
            return this.FirstChildren.GetPotentialContactsWith(ref this.LastChildren, ref contacts, limit);
        }
        /// <summary>
        /// Inserta el cuerpo con el vol�men especificado en la jerarqu�a
        /// </summary>
        /// <param name="newBody">Cuerpo</param>
        /// <param name="newVolume">Vol�men</param>
        public void Insert(RigidBody newBody, BoundingSphere newVolume)
        {
            if (this.IsLeaf)
            {
                // Si estamos en una rama final, la �nica opci�n es crear dos nuevos hijos y poner el nuevo cuerpo en uno de ellos

                // El primer hijo es una copia de este nodo
                this.FirstChildren = new BVHNode(this, Volume, Body);

                // El segundo hijo tiene la informaci�n del nuevo cuerpo
                this.LastChildren = new BVHNode(this, newVolume, newBody);

                // Limpiamos el cuerpo de esta rama, pues ahora est� en el primer hijo
                this.Body = null;

                // Recalcular el volumen de este nodo
                this.RecalculateBoundingVolume();
            }
            else
            {
                // Si no somos rama final, hay que decidir qu� hijo se quedar� con el cuerpo
                if (this.GetGrowth(this.FirstChildren.Volume, newVolume) <
                    this.GetGrowth(this.LastChildren.Volume, newVolume))
                {
                    this.FirstChildren.Insert(newBody, newVolume);
                }
                else
                {
                    this.LastChildren.Insert(newBody, newVolume);
                }
            }
        }

        /// <summary>
        /// Busca contactos entre los nodos de la jerarqu�a y el nodo especificado
        /// </summary>
        /// <param name="other">Nodo con cuyo vol�men se compara</param>
        /// <returns>Devuelve verdadero si existe contacto</returns>
        protected bool Overlaps(BVHNode other)
        {
            return (this.Volume.Contains(other.Volume) != ContainmentType.Disjoint);
        }
        /// <summary>
        /// Busca los contactos potenciales entre este nodo y el nodo especificado, rellenando la lista de contactos potenciales facilitada, hasta el l�mite especificado
        /// </summary>
        /// <param name="other">Nodo con el que comparar</param>
        /// <param name="contacts">Lista de contactos poteciales</param>
        /// <param name="limit">L�mite</param>
        /// <returns>Devuelve el n�mero de contactos potenciales</returns>
        protected int GetPotentialContactsWith(ref BVHNode other, ref List<PotentialContact> contacts, int limit)
        {
            // Si no hay contacto entre los vol�menes superiores o el l�mite es 0, se termina el proceso
            if (!this.Overlaps(other) || limit == 0)
            {
                return 0;
            }

            // Si ambos son ramas finales, hay un contacto potencial
            if (this.IsLeaf && other.IsLeaf)
            {
                contacts.Add(new PotentialContact(this.Body, other.Body));

                return 1;
            }

            // Determinar por cual nodo descender
            // Si uno es rama final, se desciende por el otro
            // Si ambos son ramas intermedias, entonces se debe usar el m�s grande
            if (other.IsLeaf || (!this.IsLeaf && this.GetSphereVolume(this.Volume) >= this.GetSphereVolume(other.Volume)))
            {

                // Bajar por nuestro primer hijo
                int count = this.FirstChildren.GetPotentialContactsWith(ref other, ref contacts, limit);

                // Comprobar si tenemos suficiente espacio para continuar a�adiendo contactos parciales
                if (limit > count)
                {
                    return count + this.LastChildren.GetPotentialContactsWith(ref other, ref contacts, limit - count);
                }
                else
                {
                    return count;
                }
            }
            else
            {
                // Bajar por el primer hijo del otro
                int count = this.GetPotentialContactsWith(ref other.FirstChildren, ref contacts, limit);

                // Comprobar si queda espacio
                if (limit > count)
                {
                    return count + this.GetPotentialContactsWith(ref other.LastChildren, ref contacts, limit - count);
                }
                else
                {
                    return count;
                }
            }
        }
        /// <summary>
        /// Para ramas intermedias, este m�todo recalcula los vol�menes que engloban a todos los hijos
        /// </summary>
        protected void RecalculateBoundingVolume()
        {
            this.RecalculateBoundingVolume(true);
        }
        /// <summary>
        /// Para ramas intermedias, este m�todo recalcula los vol�menes que engloban a todos los hijos, si as� se indica
        /// </summary>
        /// <param name="recurse">Indica si se debe hacer el c�lculo recursivamente por todos los hijos hacia arriba</param>
        protected void RecalculateBoundingVolume(bool recurse)
        {
            if (!this.IsLeaf)
            {
                // Crear el nuevo vol�men con los vol�menes de este nodo
                this.Volume = BoundingSphere.CreateMerged(this.FirstChildren.Volume, this.LastChildren.Volume);

                // Subir por el padre
                if (this.Parent != null)
                {
                    this.Parent.RecalculateBoundingVolume(true);
                }
            }
        }

        /// <summary>
        /// Obtiene el �ndice de crecimiento al a�adir los vol�menes especificados
        /// </summary>
        /// <param name="sphere1">Esfera primera</param>
        /// <param name="sphere2">Esfera segunda</param>
        /// <returns>Devuelve el �ndice de crecimiento al a�adir los vol�menes especificados</returns>
        private float GetGrowth(BoundingSphere sphere1, BoundingSphere sphere2)
        {
            BoundingSphere newSphere = BoundingSphere.CreateMerged(sphere1, sphere2);

            // Se devuelve un valor proporcional al cambio en el �rea de superficie de la esfera
            return (newSphere.Radius * newSphere.Radius) - (sphere1.Radius * sphere1.Radius);
        }
        /// <summary>
        /// Obtiene el vol�men de la esfera especificada
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <returns>Devuelve el vol�men de la esfera especificada</returns>
        private float GetSphereVolume(BoundingSphere sphere)
        {
            // 4/3 de PI por el radio al cubo
            return 1.333333f * MathHelper.Pi * sphere.Radius * sphere.Radius * sphere.Radius;
        }
    }

    /// <summary>
    /// Contacto potencial entre dos cuerpos r�gidos
    /// </summary>
    public struct PotentialContact
    {
        /// <summary>
        /// Cuerpo r�gido primero
        /// </summary>
        public readonly RigidBody BodyOne;
        /// <summary>
        /// Cuerpo r�gido segundo
        /// </summary>
        public readonly RigidBody BodyTwo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bodyOne">Cuerpo r�gido primero</param>
        /// <param name="bodyTwo">Cuerpo r�gido segundo</param>
        public PotentialContact(RigidBody bodyOne, RigidBody bodyTwo)
        {
            this.BodyOne = bodyOne;
            this.BodyTwo = bodyTwo;
        }
    }
}
