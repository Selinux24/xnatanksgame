using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Physics;
using Physics.CollideCoarse;

namespace BigBallisticDemo
{
    /// <summary>
    /// Controlador de físicas
    /// </summary>
    class PhysicsController
    {
        /// <summary>
        /// Número máximo de contactos a generar en la simulación
        /// </summary>
        private const int _MaxContacts = 1024;

        private CollisionTriangleSoup m_TriangleSoup = null;
        /// <summary>
        /// Estructura de datos de colisión
        /// </summary>
        private CollisionData m_ContactData = new CollisionData(_MaxContacts);
        /// <summary>
        /// Resolutor de contactos
        /// </summary>
        private ContactResolver m_ContactResolver = new ContactResolver(_MaxContacts * 8);

        /// <summary>
        /// Árbol de detección de colisiones potenciales
        /// </summary>
        //private BVHNode m_Tree = null;

        //List<PotentialContact> contacts = new List<PotentialContact>();

        /// <summary>
        /// Colección de balas
        /// </summary>
        private List<AmmoRound> m_AmmoData = new List<AmmoRound>();
        /// <summary>
        /// Colección de cuerpos contenidos en cajas
        /// </summary>
        private List<CollisionBox> m_BoxData = new List<CollisionBox>();
        /// <summary>
        /// Lista de generadores de contactos
        /// </summary>
        private List<ContactGenerator> m_ContactGenerators = new List<ContactGenerator>();
        /// <summary>
        /// Colección de explosiones
        /// </summary>
        private List<Explosion> m_ExplosionData = new List<Explosion>();

        /// <summary>
        /// Registra una bala
        /// </summary>
        /// <param name="ammo">Bala</param>
        public void RegisterAmmoData(AmmoRound ammo)
        {
            this.m_AmmoData.Add(ammo);
        }
        /// <summary>
        /// Registra una caja
        /// </summary>
        /// <param name="box">Caja</param>
        public void RegisterBox(CollisionBox box)
        {
            this.m_BoxData.Add(box);
        }
        /// <summary>
        /// Registra una explosión
        /// </summary>
        /// <param name="explosion">Explosión</param>
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
        /// Actualiza las físicas
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void Update(GameTime gameTime)
        {
            //contacts.Clear();
            //this.m_Tree.GetPotentialContacts(ref contacts, _MaxContacts);

            // Encontrar la duración de este intervalo para las físicas
            float duration = (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f;
            if (duration <= 0.0f)
            {
                return;
            }
            else if (duration > 0.05f)
            {
                duration = 0.05f;
            }

            // Actualizar los objetos
            this.UpdateObjects(duration);

            // Generar los contactos
            this.GenerateContacts();

            // Resolver los contactos
            this.ResolveContacts(duration);
        }
        /// <summary>
        /// Inicializar la posición de los cuerpos
        /// </summary>
        public void Reset()
        {
            // Marcar todas las balas como no usadas
            foreach (AmmoRound shot in this.m_AmmoData)
            {
                shot.Deactivate();
            }

            // Finalizar todas las explosiones
            m_ExplosionData.Clear();
        }
        /// <summary>
        /// Disparar una bala
        /// </summary>
        /// <param name="position">Posición inicial de la bala</param>
        /// <param name="direction">Dirección del disparo</param>
        /// <param name="shotType">Tipo de disparo</param>
        public void Fire(Vector3 position, Vector3 direction, ShotType shotType)
        {
            // Buscar la primera bala disponible
            for (int i = 0; i < m_AmmoData.Count; i++)
            {
                if (m_AmmoData[i].ShotType == ShotType.UnUsed)
                {
                    // Establecer el estado inicial de la bala con el tipo de munición actual y la posición por defecto
                    m_AmmoData[i].Fire(shotType, position, direction);

                    break;
                }
            }
        }

        /// <summary>
        /// Actualizar el estado de los objetos
        /// </summary>
        /// <param name="duration">Cantidad de tiempo</param>
        private void UpdateObjects(float duration)
        {
            // Actualizar las físicas de cada bala
            for (int i = 0; i < m_AmmoData.Count; i++)
            {
                if (m_AmmoData[i].IsAlive())
                {
                    // Integrar y actualizar las variables
                    m_AmmoData[i].Body.Integrate(duration);
                }
            }

            // Actualizar las cajas
            for (int i = 0; i < m_BoxData.Count; i++)
            {
                // Integrar y actualizar las variables
                m_BoxData[i].Body.Integrate(duration);

                // Actualizar las explosiones
                Explosion[] explosions = m_ExplosionData.ToArray();
                for (int e = 0; e < explosions.Length; e++)
                {
                    if (explosions[e].IsActive)
                    {
                        explosions[e].UpdateForce(ref m_BoxData[i].Body, duration);
                    }
                    else
                    {
                        m_ExplosionData.Remove(explosions[e]);
                    }
                }
            }
        }
        /// <summary>
        /// Obtiene los contactos para el momento actual
        /// </summary>
        private void GenerateContacts()
        {
            // Preparar datos de colisión
            m_ContactData.Reset();
            m_ContactData.Friction = 0.9f;
            m_ContactData.Restitution = 0.2f;
            m_ContactData.Tolerance = 0.1f;

            // Generadores de contactos
            foreach (ContactGenerator contactGenerator in m_ContactGenerators)
            {
                contactGenerator.AddContact(ref m_ContactData, 0);
            }

            // Chequear colisiones de las cajas
            foreach (CollisionBox box in m_BoxData)
            {
                // Comprobar si se pueden almacenar más contactos
                if (m_ContactData.HasMoreContacts())
                {
                    // Colisión contra el suelo de cada caja
                    //if (CollisionDetector.BoxAndHalfSpace(box, m_Plane, ref m_ContactData))
                    if (CollisionDetector.BoxAndTriangleSoup(box, m_TriangleSoup, ref m_ContactData))
                    {
                        // Informar de la colisión entre la caja y el suelo
                        box.PrimitiveContacted(null);
                    }

                    // Colisiones contra cada bala
                    foreach (AmmoRound shot in m_AmmoData)
                    {
                        if (shot.ShotType != ShotType.UnUsed)
                        {
                            if (m_ContactData.HasMoreContacts())
                            {
                                // Colisión de bala y suelo
                                if (CollisionDetector.SphereAndTriangleSoup(shot, m_TriangleSoup, ref m_ContactData))
                                {
                                    if (shot.ShotType == ShotType.Artillery)
                                    {
                                        // Explosión
                                        m_ExplosionData.Add(Explosion.CreateArtilleryExplosion(shot.Position));
                                    }

                                    // Informar de la colisión entre la bala y el suelo
                                    shot.PrimitiveContacted(null);
                                }
                                else
                                {
                                    // Comprobar si se pueden almacenar más colisiones
                                    if (m_ContactData.HasMoreContacts())
                                    {
                                        if (CollisionDetector.BoxAndSphere(box, shot, ref m_ContactData))
                                        {
                                            if (shot.ShotType == ShotType.Artillery)
                                            {
                                                // Explosión
                                                m_ExplosionData.Add(Explosion.CreateArtilleryExplosion(shot.Position));
                                            }

                                            // Informar de la colisión entre la caja y la bala
                                            box.PrimitiveContacted(shot);
                                            shot.PrimitiveContacted(box);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Chequear colisiones entre cajas
            for (int i = 0; i < m_BoxData.Count; i++)
            {
                for (int x = i + 1; x < m_BoxData.Count; x++)
                {
                    if (CollisionDetector.BoxAndBox(m_BoxData[i], m_BoxData[x], ref m_ContactData))
                    {
                        // Informar de la colisión entre cajas
                        m_BoxData[i].PrimitiveContacted(m_BoxData[x]);
                        m_BoxData[x].PrimitiveContacted(m_BoxData[i]);
                    }
                }
            }
        }
        /// <summary>
        /// Resolución de contactos
        /// </summary>
        /// <param name="duration">Cantidad de tiempo</param>
        private void ResolveContacts(float duration)
        {
            this.m_ContactResolver.ResolveContacts(ref m_ContactData, duration);
        }

        public void RegisterTerrain(CollisionTriangleSoup collisionTriangleSoup)
        {
            this.m_TriangleSoup = collisionTriangleSoup;
        }
    }
}
