using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Physics.CollideCoarse
{
    public delegate void ExplosionHandler(Explosion explosion);

    public delegate void VehicleHandler(IPhysicObject vehicle);

    public delegate void AmmoRoundHandler(AmmoRound proyectile);

    /// <summary>
    /// Controlador de físicas
    /// </summary>
    public class PhysicsController
    {
        /// <summary>
        /// Número máximo de contactos a generar en la simulación
        /// </summary>
        private const int _MaxContacts = 1024;

        /// <summary>
        /// Estructura de datos de colisión
        /// </summary>
        private CollisionData m_ContactData = new CollisionData(_MaxContacts);
        /// <summary>
        /// Resolutor de contactos
        /// </summary>
        private ContactResolver m_ContactResolver = new ContactResolver(_MaxContacts * 8);

        /// <summary>
        /// Terreno
        /// </summary>
        private IScenery m_SceneryPrimitive = null;
        /// <summary>
        /// Colección de vehículos
        /// </summary>
        private List<IPhysicObject> m_ObjectData = new List<IPhysicObject>();
        /// <summary>
        /// Colección de proyectiles
        /// </summary>
        private List<AmmoRound> m_ProjectileData = new List<AmmoRound>();
        /// <summary>
        /// Colección de explosiones
        /// </summary>
        private List<Explosion> m_ExplosionData = new List<Explosion>();
        /// <summary>
        /// Lista de generadores de contactos
        /// </summary>
        private List<ContactGenerator> m_ContactGenerators = new List<ContactGenerator>();

        /// <summary>
        /// Obtiene el escenario registrado
        /// </summary>
        public IScenery Scenery
        {
            get
            {
                return this.m_SceneryPrimitive;
            }
        }
        /// <summary>
        /// Obtiene la lista de vehículos registrados
        /// </summary>
        public IPhysicObject[] Vehicles
        {
            get
            {
                return this.m_ObjectData.ToArray();
            }
        }
        /// <summary>
        /// Obtiene la lista de proyectiles registrados
        /// </summary>
        public AmmoRound[] Projectiles
        {
            get
            {
                return this.m_ProjectileData.ToArray();
            }
        }
        /// <summary>
        /// Obtiene la lista de explosiones registradas
        /// </summary>
        public Explosion[] Explosions
        {
            get
            {
                return this.m_ExplosionData.ToArray();
            }
        }

        /// <summary>
        /// Obtiene la cantidad de contactos en uso
        /// </summary>
        public int UsedContacts
        {
            get
            {
                if (this.m_ContactData != null)
                {
                    return this.m_ContactData.ContactCount;
                }

                return 0;
            }
        }

        /// <summary>
        /// Evento que se produce cuando empieza una explosión
        /// </summary>
        public event ExplosionHandler ExplosionStarts;
        /// <summary>
        /// Evento que se produce cuando se actualiza una explosión
        /// </summary>
        public event ExplosionHandler ExplosionUpdated;
        /// <summary>
        /// Evento que se produce cuando termina una explosión
        /// </summary>
        public event ExplosionHandler ExplosionEnds;
        /// <summary>
        /// Evento que se produce cuando se mueve un proyectil
        /// </summary>
        public event AmmoRoundHandler ProjectileMoved;
        /// <summary>
        /// Evento que se produce cuando se mueve un vehículo
        /// </summary>
        public event VehicleHandler VehicleMoved;

        /// <summary>
        /// Registra la primitiva que actúa como suelo
        /// </summary>
        /// <param name="scenery">Primitiva</param>
        public void RegisterScenery(IScenery scenery)
        {
            this.m_SceneryPrimitive = scenery;
        }
        /// <summary>
        /// Registra una primitiva de colisión que actuará como objecto
        /// </summary>
        /// <param name="primitive">Primitiva de colisión</param>
        public void RegisterObject(IPhysicObject obj)
        {
            this.m_ObjectData.Add(obj);

            if (obj is IVehicle)
            {
                ((IVehicle)obj).Destroyed += new VehicleStateHandler(PhysicsController_OnVehicleDestroyed);
            }
        }
        /// <summary>
        /// Registra una primitiva de colisión que actuará como disparo
        /// </summary>
        /// <param name="proyectile">Primitiva de colisión</param>
        public void RegisterProjectile(AmmoRound proyectile)
        {
            this.m_ProjectileData.Add(proyectile);
        }
        /// <summary>
        /// Registra una explosión
        /// </summary>
        /// <param name="explosion">Explosión</param>
        public void RegisterExplosion(Explosion explosion)
        {
            this.m_ExplosionData.Add(explosion);

            this.FireExplosionStartsEvent(explosion);
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
            // Encontrar la duración de este intervalo para las físicas
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
        /// Inicializar la posición de los cuerpos
        /// </summary>
        public void Reset()
        {
            // Desactivar todas las balas
            foreach (AmmoRound shot in this.m_ProjectileData)
            {
                shot.Deactivate();
            }

            // Finalizar todas las explosiones
            foreach (Explosion explosion in m_ExplosionData)
            {
                if (explosion.IsActive)
                {
                    this.FireExplosionEndsEvent(explosion);
                }
            }

            m_ExplosionData.Clear();
        }

        /// <summary>
        /// Actualizar el estado de los objetos
        /// </summary>
        /// <param name="time">Cantidad de tiempo</param>
        private void UpdateObjects(float time)
        {
            // Actualizar los proyectiles
            for (int i = 0; i < this.m_ProjectileData.Count; i++)
            {
                // Si está activo
                if (this.m_ProjectileData[i].IsActive())
                {
                    this.m_ProjectileData[i].Integrate(time);

                    this.FireProjectileMovedEvent(this.m_ProjectileData[i]);
                }
            }

            // Actualizar los vehículos
            for (int i = 0; i < this.m_ObjectData.Count; i++)
            {
                // Si está activo
                if (this.m_ObjectData[i].IsActive())
                {
                    // Integrar y actualizar las variables
                    this.m_ObjectData[i].Integrate(time);

                    this.FireVehicleMovedEvent(this.m_ObjectData[i]);
                }

                // Actualizar las explosiones contra el vehículo actual
                Explosion[] explosions = this.m_ExplosionData.ToArray();
                for (int e = 0; e < explosions.Length; e++)
                {
                    if (explosions[e].IsActive)
                    {
                        IPhysicObject pObj = this.m_ObjectData[i];
                        explosions[e].UpdateForce(ref pObj, time);

                        this.FireExplosionUpdatedEvent(explosions[e]);
                    }
                    else
                    {
                        this.m_ExplosionData.Remove(explosions[e]);

                        this.FireExplosionEndsEvent(explosions[e]);
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
            this.m_ContactData.Reset();
            this.m_ContactData.Friction = 0.75f;
            this.m_ContactData.Restitution = 0.1f;
            this.m_ContactData.Tolerance = 0.1f;

            // Generadores de contactos
            foreach (ContactGenerator contactGenerator in this.m_ContactGenerators)
            {
                if (this.m_ContactData.HasFreeContacts())
                {
                    contactGenerator.AddContact(ref this.m_ContactData, 0);
                }
                else
                {
                    return;
                }
            }

            // Colisiones de cada proyectil y cada vehículo contra el suelo
            if (this.m_SceneryPrimitive != null)
            {
                // Recorrer los proyectiles
                foreach (AmmoRound projectileObj in this.m_ProjectileData)
                {
                    // Si hay contactos libres
                    if (this.m_ContactData.HasFreeContacts())
                    {
                        // Comprobar si el proyectil está activo
                        if (projectileObj.IsActive())
                        {
                            // Obtener las primitivas del proyectil y del terreno
                            CollisionPrimitive projectileObjPrimitive = projectileObj.GetPrimitive();
                            CollisionPrimitive sceneryPrimitive = this.m_SceneryPrimitive.GetContactedPrimitive(projectileObj);
                            if (CollisionDetector.BetweenObjects(ref projectileObjPrimitive, ref sceneryPrimitive, ref m_ContactData))
                            {
                                // Generar la explosión
                                if (projectileObj.GenerateExplosion)
                                {
                                    // Explosión
                                    Explosion explosion = Explosion.CreateArtilleryExplosion(projectileObjPrimitive.Position);

                                    this.m_ExplosionData.Add(explosion);

                                    this.FireExplosionStartsEvent(explosion);
                                }

                                // Informar de la colisión entre la bala y el suelo
                                projectileObj.SetContactedWith(this.m_SceneryPrimitive);
                            }
                        }
                    }
                    else
                    {
                        // Fin de la detección
                        return;
                    }
                }

                // Recorrer los vehículos
                foreach (IPhysicObject vehicleObj in this.m_ObjectData)
                {
                    // Si hay contactos libres
                    if (this.m_ContactData.HasFreeContacts())
                    {
                        // Comprobar si el vehículo está activo
                        if (vehicleObj.IsActive())
                        {
                            // Obtener las primitivas del vehículo y del terreno
                            CollisionPrimitive vehicleObjPrimitive = vehicleObj.GetPrimitive();
                            CollisionPrimitive sceneryPrimitive = this.m_SceneryPrimitive.GetContactedPrimitive(vehicleObj);
                            if (CollisionDetector.BetweenObjects(ref vehicleObjPrimitive, ref sceneryPrimitive, ref this.m_ContactData))
                            {
                                // Informar de la colisión entre el vehículo y el terreno
                                vehicleObj.SetContactedWith(this.m_SceneryPrimitive);
                            }
                        }
                    }
                    else
                    {
                        // Fin de la detección
                        return;
                    }
                }
            }

            // Chequear colisiones de los vehículos y los proyectiles
            foreach (AmmoRound projectileObj in this.m_ProjectileData)
            {
                // Si hay contactos libres
                if (this.m_ContactData.HasFreeContacts())
                {
                    if (projectileObj.IsActive())
                    {
                        CollisionPrimitive projectileObjPrimitive = projectileObj.GetPrimitive();

                        // Recorrer los vehículos
                        foreach (IPhysicObject vehicleObj in this.m_ObjectData)
                        {
                            // Si hay contactos libres
                            if (this.m_ContactData.HasFreeContacts())
                            {
                                CollisionPrimitive vehicleObjPrimitive = vehicleObj.GetContactedPrimitive(projectileObj);
                                if (CollisionDetector.BetweenObjects(ref vehicleObjPrimitive, ref projectileObjPrimitive, ref m_ContactData))
                                {
                                    if (projectileObj.GenerateExplosion)
                                    {
                                        // Explosión
                                        Explosion explosion = Explosion.CreateArtilleryExplosion(projectileObjPrimitive.Position);

                                        this.m_ExplosionData.Add(explosion);

                                        this.FireExplosionStartsEvent(explosion);
                                    }

                                    // Informar de la colisión entre la caja y la bala
                                    vehicleObj.SetContactedWith(projectileObj);
                                    projectileObj.SetContactedWith(vehicleObj);
                                }
                            }
                            else
                            {
                                // Fin de la detección
                                return;
                            }
                        }
                    }
                }
                else
                {
                    // Fin de la detección
                    return;
                }
            }

            // Chequear colisiones entre vehículos
            for (int i = 0; i < this.m_ObjectData.Count; i++)
            {
                if (this.m_ContactData.HasFreeContacts())
                {
                    // Obtener la primitiva de colisión
                    CollisionPrimitive primitive1 = this.m_ObjectData[i].GetPrimitive();

                    for (int x = i + 1; x < this.m_ObjectData.Count; x++)
                    {
                        if (this.m_ContactData.HasFreeContacts())
                        {
                            if (this.m_ObjectData[i].IsActive() || this.m_ObjectData[x].IsActive())
                            {
                                // Obtener la segunda primitiva de colisión
                                CollisionPrimitive primitive2 = this.m_ObjectData[x].GetContactedPrimitive(this.m_ObjectData[i]);
                                if (CollisionDetector.BetweenObjects(ref primitive1, ref primitive2, ref this.m_ContactData))
                                {
                                    // Informar de la colisión entre cajas
                                    this.m_ObjectData[i].SetContactedWith(this.m_ObjectData[x]);
                                    this.m_ObjectData[x].SetContactedWith(this.m_ObjectData[i]);
                                }
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }
        /// <summary>
        /// Resolución de contactos
        /// </summary>
        /// <param name="time">Cantidad de tiempo</param>
        private void ResolveContacts(float time)
        {
            if (this.m_ContactData.ContactCount > 0)
            {
                this.m_ContactResolver.ResolveContacts(ref this.m_ContactData, time);
            }
        }

        /// <summary>
        /// Inicializar los proyectiles
        /// </summary>
        /// <param name="quantity">Cantidad de proyectiles</param>
        public void InitializeProyectiles(int quantity)
        {
            List<AmmoRound> roundList = new List<AmmoRound>();
            for (int i = 0; i < quantity; i++)
            {
                AmmoRound round = new AmmoRound();
                this.RegisterProjectile(round);
                roundList.Add(round);
            }
        }
        /// <summary>
        /// Dispara un proyectil
        /// </summary>
        /// <param name="mass">Masa del proyectil</param>
        /// <param name="range">Rango de disparo</param>
        /// <param name="damage">Daño</param>
        /// <param name="penetration">Penetración del blindaje</param>
        /// <param name="position">Posición de origen del disparo</param>
        /// <param name="direction">Dirección del disparo</param>
        /// <param name="appliedGravity">Gravedad aplicada</param>
        /// <param name="radius">Radio</param>
        /// <param name="generateExplosion">Indica si la colisión generará una explosión</param>
        public void Fire(float mass, float range, float damage, float penetration, Vector3 position, Vector3 direction, Vector3 appliedGravity, float radius, bool generateExplosion)
        {
            // Buscar la primera bala disponible
            for (int i = 0; i < m_ProjectileData.Count; i++)
            {
                AmmoRound round = m_ProjectileData[i] as AmmoRound;
                if (round != null && !round.IsActive())
                {
                    // Establecer el estado inicial de la bala con el tipo de munición actual y la posición por defecto
                    round.Fire(
                        mass, 
                        range, 
                        damage,
                        penetration,
                        position, 
                        direction, 
                        appliedGravity, 
                        radius, 
                        generateExplosion);

                    break;
                }
            }
        }

        /// <summary>
        /// Disparador del evento de explosión añadida
        /// </summary>
        /// <param name="explosion">Explosión</param>
        private void FireExplosionStartsEvent(Explosion explosion)
        {
            if (this.ExplosionStarts != null)
            {
                this.ExplosionStarts(explosion);
            }
        }
        /// <summary>
        /// Disparador del evento de explosión actualizada
        /// </summary>
        /// <param name="explosion">Explosión</param>
        private void FireExplosionUpdatedEvent(Explosion explosion)
        {
            if (this.ExplosionUpdated != null)
            {
                this.ExplosionUpdated(explosion);
            }
        }
        /// <summary>
        /// Disparador del evento de explosión eliminada
        /// </summary>
        /// <param name="explosion">Explosión</param>
        private void FireExplosionEndsEvent(Explosion explosion)
        {
            if (this.ExplosionEnds != null)
            {
                this.ExplosionEnds(explosion);
            }
        }
        /// <summary>
        /// Disparador del evento de proyectil en movimiento
        /// </summary>
        /// <param name="ammoRound">Proyectil</param>
        private void FireProjectileMovedEvent(AmmoRound ammoRound)
        {
            if (this.ProjectileMoved != null)
            {
                this.ProjectileMoved(ammoRound);
            }
        }
        /// <summary>
        /// Disparador del evento de vehículo en movimiento
        /// </summary>
        /// <param name="vehicle">Vehículo</param>
        private void FireVehicleMovedEvent(IPhysicObject vehicle)
        {
            if (this.VehicleMoved != null)
            {
                this.VehicleMoved(vehicle);
            }
        }

        /// <summary>
        /// Evento que se produce cuando se destruye un vehículo
        /// </summary>
        /// <param name="vehicle">Vehículo</param>
        private void PhysicsController_OnVehicleDestroyed(IVehicle vehicle)
        {
            this.RegisterExplosion(Explosion.CreateArtilleryExplosion(vehicle.GetPosition()));
        }
    }
}
