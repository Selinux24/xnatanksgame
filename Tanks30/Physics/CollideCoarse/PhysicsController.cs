using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Physics.CollideCoarse
{
    /// <summary>
    /// Controlador de f�sicas
    /// </summary>
    public class PhysicsController
    {
        /// <summary>
        /// N�mero m�ximo de contactos a generar en la simulaci�n
        /// </summary>
        private const int _MaxContacts = 1024;
        /// <summary>
        /// Estructura de datos de colisi�n
        /// </summary>
        private CollisionData m_ContactData = new CollisionData(_MaxContacts);
        /// <summary>
        /// Resolutor de contactos
        /// </summary>
        private ContactResolver m_ContactResolver = new ContactResolver(_MaxContacts * 8);

        /// <summary>
        /// Terreno
        /// </summary>
        private IPhysicObject m_EsceneryPrimitive = null;
        /// <summary>
        /// Colecci�n de veh�culos
        /// </summary>
        private List<IPhysicObject> m_VehicleData = new List<IPhysicObject>();
        /// <summary>
        /// Colecci�n de proyectiles
        /// </summary>
        private List<IPhysicObject> m_ProyectileData = new List<IPhysicObject>();
        /// <summary>
        /// Colecci�n de explosiones
        /// </summary>
        private List<Explosion> m_ExplosionData = new List<Explosion>();
        /// <summary>
        /// Lista de generadores de contactos
        /// </summary>
        private List<ContactGenerator> m_ContactGenerators = new List<ContactGenerator>();

        /// <summary>
        /// Registra la primitiva que act�a como suelo
        /// </summary>
        /// <param name="escenery">Primitiva</param>
        public void RegisterEscenery(IPhysicObject escenery)
        {
            this.m_EsceneryPrimitive = escenery;
        }
        /// <summary>
        /// Registra una primitiva de colisi�n que actuar� como veh�culo
        /// </summary>
        /// <param name="primitive">Primitiva de colisi�n</param>
        public void RegisterVehicle(IPhysicObject vehicle)
        {
            this.m_VehicleData.Add(vehicle);
        }
        /// <summary>
        /// Registra una primitiva de colisi�n que actuar� como disparo
        /// </summary>
        /// <param name="proyectile">Primitiva de colisi�n</param>
        public void RegisterProyectile(IPhysicObject proyectile)
        {
            this.m_ProyectileData.Add(proyectile);
        }
        /// <summary>
        /// Registra una explosi�n
        /// </summary>
        /// <param name="explosion">Explosi�n</param>
        public void RegisterExplosion(Explosion explosion)
        {
            this.m_ExplosionData.Add(explosion);
        }
        /// <summary>
        /// Registra un generador de contactos entre cuerpos
        /// </summary>
        /// <param name="contactGenerator">Generador de contactos</param>
        public void RegisterContactGenerator(ContactGenerator contactGenerator)
        {
            this.m_ContactGenerators.Add(contactGenerator);
        }

        /// <summary>
        /// Actualiza las f�sicas
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void Update(GameTime gameTime)
        {
            // Encontrar la duraci�n de este intervalo para las f�sicas
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f;
            if (time <= 0.0f)
            {
                return;
            }
            else if (time > 0.05f)
            {
                time = 0.05f;
            }

            // Actualizar los objetos
            this.UpdateObjects(time);

            // Generar los contactos
            this.GenerateContacts();

            // Resolver los contactos
            this.ResolveContacts(time);
        }
        /// <summary>
        /// Inicializar la posici�n de los cuerpos
        /// </summary>
        public void Reset()
        {
            // Finalizar todas las explosiones
            m_ExplosionData.Clear();
        }

        /// <summary>
        /// Actualizar el estado de los objetos
        /// </summary>
        /// <param name="time">Cantidad de tiempo</param>
        private void UpdateObjects(float time)
        {
            // Actualizar los proyectiles
            for (int i = 0; i < this.m_ProyectileData.Count; i++)
            {
                this.m_ProyectileData[i].Integrate(time);
            }

            // Actualizar los veh�culos
            for (int i = 0; i < this.m_VehicleData.Count; i++)
            {
                IPhysicObject vehicle = this.m_VehicleData[i];

                // Integrar y actualizar las variables
                vehicle.Integrate(time);

                // Actualizar las explosiones
                Explosion[] explosions = this.m_ExplosionData.ToArray();
                for (int e = 0; e < explosions.Length; e++)
                {
                    if (explosions[e].IsActive)
                    {
                        explosions[e].UpdateForce(ref vehicle, time);
                    }
                    else
                    {
                        this.m_ExplosionData.Remove(explosions[e]);
                    }
                }
            }
        }
        /// <summary>
        /// Obtiene los contactos para el momento actual
        /// </summary>
        private void GenerateContacts()
        {
            // Preparar datos de colisi�n
            this.m_ContactData.Reset();
            this.m_ContactData.Friction = 0.9f;
            this.m_ContactData.Restitution = 0.2f;
            this.m_ContactData.Tolerance = 0.1f;

            // Generadores de contactos
            foreach (ContactGenerator contactGenerator in this.m_ContactGenerators)
            {
                contactGenerator.AddContact(ref this.m_ContactData, 0);
            }

            // Chequear colisiones de los veh�culos
            foreach (IPhysicObject pObj in this.m_VehicleData)
            {
                // Comprobar si se pueden almacenar m�s contactos
                if (this.m_ContactData.HasFreeContacts())
                {
                    // Colisi�n contra el suelo de cada caja
                    if (this.m_EsceneryPrimitive != null)
                    {
                        if (CollisionDetector.BetweenObjects(pObj, this.m_EsceneryPrimitive, ref this.m_ContactData))
                        {
                            // Informar de la colisi�n entre la caja y el suelo
                            pObj.Contacted(null);
                        }
                    }

                    // Colisiones contra cada proyectil
                    foreach (IPhysicObject sObj in this.m_ProyectileData)
                    {
                        //if (shot.ShotType != ShotType.UnUsed)
                        {
                            // Colisi�n de bala y suelo
                            if (this.m_EsceneryPrimitive != null)
                            {
                                if (m_ContactData.HasFreeContacts())
                                {
                                    if (CollisionDetector.BetweenObjects(sObj, this.m_EsceneryPrimitive, ref m_ContactData))
                                    {
                                        //if (shot.ShotType == ShotType.Artillery)
                                        //{
                                        //    // Explosi�n
                                        //    m_ExplosionData.Add(Explosion.CreateArtilleryExplosion(shot.Position));
                                        //}

                                        // Informar de la colisi�n entre la bala y el suelo
                                        sObj.Contacted(null);

                                        // Bala anulada
                                        break;
                                    }
                                }
                            }

                            // Comprobar si se pueden almacenar m�s colisiones
                            if (m_ContactData.HasFreeContacts())
                            {
                                if (CollisionDetector.BetweenObjects(pObj, sObj, ref m_ContactData))
                                {
                                    //if (shot.ShotType == ShotType.Artillery)
                                    //{
                                    //    // Explosi�n
                                    //    m_ExplosionData.Add(Explosion.CreateArtilleryExplosion(shot.Position));
                                    //}

                                    // Informar de la colisi�n entre la caja y la bala
                                    pObj.Contacted(sObj);
                                    sObj.Contacted(pObj);
                                }
                            }
                        }
                    }
                }
            }

            // Chequear colisiones entre veh�culos
            for (int i = 0; i < m_VehicleData.Count; i++)
            {
                for (int x = i + 1; x < m_VehicleData.Count; x++)
                {
                    if (CollisionDetector.BetweenObjects(m_VehicleData[i], m_VehicleData[x], ref m_ContactData))
                    {
                        // Informar de la colisi�n entre cajas
                        m_VehicleData[i].Contacted(m_VehicleData[x]);
                        m_VehicleData[x].Contacted(m_VehicleData[i]);
                    }
                }
            }
        }
        /// <summary>
        /// Resoluci�n de contactos
        /// </summary>
        /// <param name="time">Cantidad de tiempo</param>
        private void ResolveContacts(float time)
        {
            if (this.m_ContactData.ContactCount > 0)
            {
                this.m_ContactResolver.ResolveContacts(ref this.m_ContactData, time);
            }
        }
    }
}
