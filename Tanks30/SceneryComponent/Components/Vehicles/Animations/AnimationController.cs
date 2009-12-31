using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Vehicles.Animations
{
    /// <summary>
    /// Controlador de animaci�n
    /// </summary>
    public class AnimationController
    {
        // Diccionario de animaciones por �ndice
        private readonly Dictionary<int, Animation> m_AnimationList = new Dictionary<int, Animation>();

        /// <summary>
        /// Obtiene la lista de animaciones
        /// </summary>
        public Animation[] AnimationList
        {
            get
            {
                List<Animation> list = new List<Animation>(m_AnimationList.Values);

                return list.ToArray();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AnimationController()
        {

        }

        /// <summary>
        /// A�ade una animaci�n al controlador
        /// </summary>
        /// <param name="animation">Animaci�n</param>
        public void Add(Animation animation)
        {
            if (animation != null)
            {
                if (!m_AnimationList.ContainsKey(animation.Index))
                {
                    m_AnimationList.Add(animation.Index, animation);
                }
            }
        }
        /// <summary>
        /// A�ade una lista de animaciones al controlador
        /// </summary>
        /// <param name="animationList">Animaciones</param>
        public void AddRange(Animation[] animationList)
        {
            if (animationList != null && animationList.Length > 0)
            {
                foreach (Animation animation in animationList)
                {
                    this.Add(animation);
                }
            }
        }
        /// <summary>
        /// Elimina una animaci�n del controlador
        /// </summary>
        /// <param name="animation">Animaci�n</param>
        public void Remove(Animation animation)
        {
            if (animation != null)
            {
                if (m_AnimationList.ContainsKey(animation.Index))
                {
                    m_AnimationList.Remove(animation.Index);
                }
            }
        }
        /// <summary>
        /// Actualiza �a informaci�n del controlador
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void Update(GameTime gameTime)
        {
            foreach (Animation animation in m_AnimationList.Values)
            {
                // Actualizar todos los elementos de la colecci�n de animaciones
                animation.Update(gameTime);
            }
        }
        /// <summary>
        /// Obtiene la transformaci�n parcial del bone con el �ndice especificado
        /// </summary>
        /// <param name="index">Indice de bone</param>
        /// <returns>Devuelve la transformaci�n parcial del bone con el �ndice especificado</returns>
        public Matrix GetTransform(int index)
        {
            if (m_AnimationList.ContainsKey(index))
            {
                // Si hay animaci�n espec�fica del �ndice se devuelve
                return m_AnimationList[index].Transform;
            }

            // Por defecto se devuelve transformaci�n identidad
            return Matrix.Identity;
        }
        /// <summary>
        /// Obtiene la transformaci�n parcial del bone especificado
        /// </summary>
        /// <param name="bone">Bone</param>
        /// <returns>Devuelve la transformaci�n parcial del bone especificado</returns>
        public Matrix GetTransform(ModelBone bone)
        {
            return GetTransform(bone.Index);
        }
        /// <summary>
        /// Obtiene la transformaci�n final del bone especificado
        /// </summary>
        /// <param name="bone">Bone</param>
        /// <param name="parentTransform">Matriz de transformaci�n del padre</param>
        /// <returns>Devuelve la transformaci�n final del bone especificado</returns>
        /// <remarks>S�lo es necesario especificar la matriz del padre cuando se est� calculando la matriz de forma recursiva</remarks>
        private Matrix GetAbsoluteTransform(ModelBone bone, Matrix parentTransform)
        {
            // Acumular la transformaci�n del padre + la del bone en animaci�n + la del bone inicial
            Matrix result = parentTransform * GetTransform(bone) * bone.Transform;

            if (bone.Parent != null)
            {
                // Si hay padre se sigue acumulando
                return GetAbsoluteTransform(bone.Parent, result);
            }
            else
            {
                // Si no hay padre se devuelve el resultado
                return result;
            }
        }
        /// <summary>
        /// Obtiene la transformaci�n final del bone especificado
        /// </summary>
        /// <param name="bone">Bone</param>
        /// <returns>Devuelve la transformaci�n final del bone especificado</returns>
        public Matrix GetAbsoluteTransform(ModelBone bone)
        {
            return GetAbsoluteTransform(bone, Matrix.Identity);
        }
        /// <summary>
        /// Copia la lista de transformaciones a la lista de matrices especificada
        /// </summary>
        /// <param name="bone">Bone</param>
        /// <param name="transforms">Lista de transformaciones</param>
        private void CopyAbsoluteBoneTransformsTo(ModelBone bone, Matrix[] transforms)
        {
            // Establecemos en la colecci�n la transformaci�n absoluta del bone especificado
            transforms[bone.Index] = GetAbsoluteTransform(bone, Matrix.Identity);

            foreach (ModelBone childBone in bone.Children)
            {
                // Continuamos haciendo lo mismo con los hijos
                CopyAbsoluteBoneTransformsTo(childBone, transforms);
            }
        }
        /// <summary>
        /// Copia la lista de transformaciones a la lista de matrices especificada
        /// </summary>
        /// <param name="model">Modelo</param>
        /// <param name="transforms">Lista de transformaciones</param>
        public void CopyAbsoluteBoneTransformsTo(Model model, Matrix[] transforms)
        {
            // Actualizar la matriz de transformaciones usando el nodo ra�z del modelo
            CopyAbsoluteBoneTransformsTo(model.Root, transforms);
        }
    }
}
