using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Informaci�n de un contacto
    /// </summary>
    public class Contact
    {
        const float velocityLimit = 0.25f;

        /// <summary>
        /// Lista de cuerpos envueltos en el contacto. Si el segundo cuerpo es null, se considera colisi�n contra el escenario
        /// </summary>
        public RigidBody[] Bodies = new RigidBody[2];
        /// <summary>
        /// Fricci�n lateral.
        /// </summary>
        public float Friction;
        /// <summary>
        /// Restituci�n normal.
        /// </summary>
        public float Restitution;
        /// <summary>
        /// Posici�n del contacto en coordenadas del mundo.
        /// </summary>
        public Vector3 ContactPoint;
        /// <summary>
        /// Normal del contacto en coordenadas del mundo.
        /// </summary>
        public Vector3 ContactNormal;
        /// <summary>
        /// Profundidad de penetraci�n del contacto. Si los dos cuerpos est�n informados, el punto de contacto ser� el punto medio de penetraci�n entre ambos.
        /// </summary>
        public float Penetration;
        /// <summary>
        /// La matriz de transformaci�n entre el espacio actual y el mundo. Es una matriz ortonormal.
        /// </summary>
        internal Matrix3 ContactToWorld;
        /// <summary>
        /// Velocidad en el momento de la colisi�n.
        /// </summary>
        internal Vector3 ContactVelocity;
        /// <summary>
        /// Cambio en la velocidad necesario para resolver el contacto.
        /// </summary>
        internal float DesiredDeltaVelocity;
        /// <summary>
        /// Posiciones relativas a cada uno de los cuerpos del punto de colisi�n, en coordenadas del mundo.
        /// </summary>
        internal Vector3[] RelativeContactPositions = new Vector3[2];

        /// <summary>
        /// Establece los datos que normalmente son independientes del contacto
        /// </summary>
        /// <param name="one">Cuerpo primero</param>
        /// <param name="two">Cuerpo segundo</param>
        /// <param name="friction">Nuevo coheficiente de fricci�n</param>
        /// <param name="restitution">Nuevo coheficiente de restituci�n</param>
        internal void SetBodyData(ref RigidBody one, ref RigidBody two, float friction, float restitution)
        {
            this.Bodies[0] = one;
            this.Bodies[1] = two;
            this.Friction = friction;
            this.Restitution = restitution;
        }
        /// <summary>
        /// Calcula los datos internos de estado del contacto. Esta funci�n es llamada antes de la resoluci�n del algoritmo de resoluci�n del contactos.
        /// </summary>
        /// <param name="duration">Intervalo de tiempo</param>
        internal void CalculateInternals(float duration)
        {
            // Si el primer cuerpo es null, se revierte el contacto
            if (this.Bodies[0] == null)
            {
                this.SwapBodies();
            }

            if (this.Bodies[0] != null)
            {
                // Calcular un eje de coordenadas a partir del punto de contacto
                this.CalculateContactBasis();

                // Almacenar la posici�n relativa del contacto, respecto de cada cuerpo
                this.RelativeContactPositions[0] = this.ContactPoint - this.Bodies[0].Position;
                if (this.Bodies[1] != null)
                {
                    this.RelativeContactPositions[1] = this.ContactPoint - this.Bodies[1].Position;
                }

                // Encontrar la velocidad relativa de cada cuerpo en el momento de la colisi�n
                this.ContactVelocity = this.CalculateLocalVelocity(0, duration);
                if (this.Bodies[1] != null)
                {
                    this.ContactVelocity -= this.CalculateLocalVelocity(1, duration);
                }

                // Calcular la velocidad necesaria para resolver el contacto
                CalculateDesiredDeltaVelocity(duration);
            }
        }
        /// <summary>
        /// Revierte el contacto.
        /// </summary>
        /// <remarks>
        /// Simplemente intercambia los cuerpos e invierte la normal de colisi�n. Los valores internos deber�n recalcularse con CalculateInternals m�s tarde.
        /// </remarks>
        internal void SwapBodies()
        {
            this.ContactNormal *= -1f;

            RigidBody temp = this.Bodies[0];
            this.Bodies[0] = this.Bodies[1];
            this.Bodies[1] = temp;
        }
        /// <summary>
        /// Actualiza el estado de activaci�n de los cuerpos del contacto.
        /// </summary>
        /// <remarks>
        /// Un cuerpo se activar� si se pone en contacto con un cuerpo activo.
        /// </remarks>
        internal void MatchAwakeState()
        {
            // Las colisiones con el mundo nunca despiertan cuerpos
            if (this.Bodies[1] != null)
            {
                bool body0awake = this.Bodies[0].IsAwake;
                bool body1awake = this.Bodies[1].IsAwake;

                // Despertar s�lo al que duerme
                if (body0awake ^ body1awake)
                {
                    if (body0awake)
                    {
                        this.Bodies[1].IsAwake = true;
                    }
                    else
                    {
                        this.Bodies[0].IsAwake = true;
                    }
                }
            }
        }
        /// <summary>
        /// Calcula y establece la velocidad necesaria para resolver el contacto.
        /// </summary>
        /// <param name="duration">Intervalo de tiempo</param>
        internal void CalculateDesiredDeltaVelocity(float duration)
        {
            // Calcular la velocidad acumulada por la aceleraci�n en este intervalo
            float velocityFromAcc = 0;

            if (this.Bodies[0].IsAwake)
            {
                //TODO: Parece c�digo inocuo
                //body[0].getLastFrameAcceleration() * duration * contactNormal;
            }

            if (this.Bodies[1] != null && this.Bodies[1].IsAwake)
            {
                //TODO: Ojo con el orden de operandos: velocityFromAcc -= body[1].getLastFrameAcceleration() * duration * contactNormal;
                velocityFromAcc -= Vector3.Dot(this.Bodies[1].LastFrameAcceleration * duration, this.ContactNormal);
            }

            // Si la velocidad es muy lenta, hay que limitar la restituci�n
            float thisRestitution = this.Restitution;
            if (Math.Abs(this.ContactVelocity.X) < velocityLimit)
            {
                thisRestitution = 0.0f;
            }

            // Combinar la velocidad de rebote con la velocidad robada a la aceleraci�n
            this.DesiredDeltaVelocity = -this.ContactVelocity.X - thisRestitution * (this.ContactVelocity.X - velocityFromAcc);
        }
        /// <summary>
        /// Obtiene la velocidad del punto de contacto del cuerpo especificado.
        /// </summary>
        /// <param name="bodyIndex">�ndice del cuerpo</param>
        /// <param name="duration">Intervalo de tiempo</param>
        internal Vector3 CalculateLocalVelocity(int bodyIndex, float duration)
        {
            // Obtener el cuerpo
            RigidBody body = this.Bodies[bodyIndex];
            if (body != null)
            {
                // Extraer la velocidad en el punto de contacto.
                Vector3 velocity = Vector3.Cross(body.Rotation, this.RelativeContactPositions[bodyIndex]);
                velocity += body.Velocity;

                // Convertir la velocidad a coordenadas de contacto.
                Vector3 contactVelocity = Matrix3.TransformTranspose(this.ContactToWorld, velocity);

                // Calcular la cantidad de velocidad disponible para las fuerzas sin tener en cuenta las reacciones.
                Vector3 accVelocity = body.LastFrameAcceleration * duration;

                // Calcular la cantidad de velocidad en coordendas de contacto.
                accVelocity = Matrix3.TransformTranspose(this.ContactToWorld, accVelocity);

                // Se ignoran las componentes de aceleraci�n en la direcci�n de la normal de contacto.
                // S�lo se tienen en cuenta las aceleraciones en el plano.
                accVelocity.X = 0;

                // A�adir las aceleraciones planares.
                // Si hay suficiente fricci�n, las fuerzas ser�n eliminadas durante la resoluci�n de la velocidad.
                contactVelocity += accVelocity;

                // Devolver la velocidad local
                return contactVelocity;
            }

            //HACKBYME
            return Vector3.Zero;
        }
        /// <summary>
        /// Calcula la base ortonormal para el punto de contaco, bas�ndose en la direcci�n primaria de la fricci�n o una orientaci�n aleatoria.
        /// </summary>
        /// <remarks>
        /// Direcci�n primaria de la fricci�n para fricci�n anisotr�pica o orientaci�n aleatoria para fricci�n isotr�pica
        /// </remarks>
        internal void CalculateContactBasis()
        {
            Vector3[] contactTangent = new Vector3[2];

            // Comprobar si el eje Z est� m�s cerca del eje X o del eje Y
            if (Math.Abs(this.ContactNormal.X) > Math.Abs(this.ContactNormal.Y))
            {
                // Obtener un factor de escalado para asegurarse de que los resultados est�n normalizados
                float s = 1.0f / (float)Math.Sqrt(ContactNormal.Z * ContactNormal.Z + ContactNormal.X * ContactNormal.X);

                // El nuevo eje X est� a 90 grados del eje Y del mundo
                contactTangent[0].X = this.ContactNormal.Z * s;
                contactTangent[0].Y = 0;
                contactTangent[0].Z = -this.ContactNormal.X * s;

                // El nuevo eje Y est� a 90 grados del os nuevos ejes X y Z
                contactTangent[1].X = this.ContactNormal.Y * contactTangent[0].X;
                contactTangent[1].Y = this.ContactNormal.Z * contactTangent[0].X - this.ContactNormal.X * contactTangent[0].Z;
                contactTangent[1].Z = -this.ContactNormal.Y * contactTangent[0].X;
            }
            else
            {
                // Obtener un factor de escalado para asegurarse de que los resultados est�n normalizados
                float s = 1.0f / (float)Math.Sqrt(ContactNormal.Z * ContactNormal.Z + ContactNormal.Y * ContactNormal.Y);

                // El nuevo eje X est� a 90 grados del eje X del mundo
                contactTangent[0].X = 0;
                contactTangent[0].Y = -this.ContactNormal.Z * s;
                contactTangent[0].Z = this.ContactNormal.Y * s;

                // El nuevo eje Y est� a 90 grados del os nuevos ejes X y Z
                contactTangent[1].X = this.ContactNormal.Y * contactTangent[0].Z - this.ContactNormal.Z * contactTangent[0].Y;
                contactTangent[1].Y = -this.ContactNormal.X * contactTangent[0].Z;
                contactTangent[1].Z = this.ContactNormal.X * contactTangent[0].Y;
            }

            // Hacer una matriz de los tres vectores
            this.ContactToWorld.SetComponents(this.ContactNormal, contactTangent[0], contactTangent[1]);
        }
        /// <summary>
        /// Aplica el impulso al cuerpo especificado, devolviendo el cambio en la velocidad.
        /// </summary>
        /// <param name="impulse">Impulso</param>
        /// <param name="body">Cuerpo</param>
        /// <param name="velocityChange">Cambio en la velocidad</param>
        /// <param name="rotationChange">Cambio en la rotaci�n</param>
        internal void ApplyImpulse(Vector3 impulse, ref RigidBody body, ref Vector3 velocityChange, ref Vector3 rotationChange)
        {
            //TODO: No encuentro esta funcionalidad
        }
        /// <summary>
        /// Realiza la resoluci�n del contacto bas�ndose en el impulso obtenido de la inercia.
        /// </summary>
        /// <param name="velocityChange">Cambios la velocidad</param>
        /// <param name="rotationChange">Cambios en la rotaci�n</param>
        internal void ApplyVelocityChange(ref Vector3[] velocityChange, ref Vector3[] rotationChange)
        {
            // Almacenar las masas inversas y los tensores de inercia inversos en coordenadas del mundo.
            Matrix3[] inverseInertiaTensor = new Matrix3[2];
            inverseInertiaTensor[0] = this.Bodies[0].InverseInertiaTensorWorld;
            if (this.Bodies[1] != null)
            {
                inverseInertiaTensor[1] = this.Bodies[1].InverseInertiaTensorWorld;
            }

            // Calcular el impulso en cada eje de contacto
            Vector3 impulseContact;
            if (this.Friction == 0f)
            {
                // Impulso sin fricci�n
                impulseContact = this.CalculateFrictionlessImpulse(inverseInertiaTensor);
            }
            else
            {
                // Impulso con fricci�n.
                impulseContact = this.CalculateFrictionImpulse(inverseInertiaTensor);
            }

            // Convertir el impulso a coordenadas del mundo
            Vector3 impulse = Matrix3.Transform(this.ContactToWorld, impulseContact);

            // Dividir el impulso en componentes lineares y rotaciones
            Vector3 impulsiveTorque = Vector3.Cross(this.RelativeContactPositions[0], impulse);
            rotationChange[0] = Matrix3.Transform(inverseInertiaTensor[0], impulsiveTorque);
            velocityChange[0] = Vector3.Zero;
            velocityChange[0] += Vector3.Multiply(impulse, this.Bodies[0].InverseMass);

            // Aplicar los cambios e el cuerpo
            this.Bodies[0].Velocity += velocityChange[0];
            this.Bodies[0].Rotation += rotationChange[0];

            if (this.Bodies[1] != null)
            {
                // Obtener impulos lineal y rotacional para el segundo cuerpo
                impulsiveTorque = Vector3.Cross(impulse, RelativeContactPositions[1]);
                rotationChange[1] = Matrix3.Transform(inverseInertiaTensor[1], impulsiveTorque);
                velocityChange[1] = Vector3.Zero;
                velocityChange[1] += Vector3.Multiply(impulse, -Bodies[1].InverseMass);

                // Aplicar los cambios.
                Bodies[1].Velocity += velocityChange[1];
                Bodies[1].Rotation += rotationChange[1];
            }
        }
        /// <summary>
        /// Realiza la resoluci�n de la penetraci�n del contacto bas�ndose en la inercia.
        /// </summary>
        /// <param name="linearChange">Cambio lineal</param>
        /// <param name="angularChange">Cambio angular</param>
        /// <param name="penetration">Penetraci�n</param>
        internal void ApplyPositionChange(ref Vector3[] linearChange, ref Vector3[] angularChange, float penetration)
        {
            float angularLimit = 0.2f;
            float[] angularMove = new float[2];
            float[] linearMove = new float[2];

            float totalInertia = 0f;
            float[] linearInertia = new float[2];
            float[] angularInertia = new float[2];

            // Tenemos que trabajar con la inercia de cada cuerpo en la direcci�n de la normal de contacto, y la inercia angular.
            for (uint i = 0; i < 2; i++)
            {
                if (this.Bodies[i] != null)
                {
                    Matrix3 inverseInertiaTensor = this.Bodies[i].InverseInertiaTensorWorld;

                    // Obtener la inercia angular.
                    Vector3 angularInertiaWorld = Vector3.Cross(RelativeContactPositions[i], ContactNormal);
                    angularInertiaWorld = Matrix3.Transform(inverseInertiaTensor, angularInertiaWorld);
                    angularInertiaWorld = Vector3.Cross(angularInertiaWorld, RelativeContactPositions[i]);
                    angularInertia[i] = Vector3.Dot(angularInertiaWorld, ContactNormal);

                    // El componente lineal es la inversa de la masa
                    linearInertia[i] = Bodies[i].InverseMass;

                    // Obtener la inercia total de todos los componentes
                    totalInertia += linearInertia[i] + angularInertia[i];
                }
            }

            // Volver a recorrer calculando los cambios y aplic�ndolos
            for (uint i = 0; i < 2; i++)
            {
                if (this.Bodies[i] != null)
                {
                    // Los movimientos angular y lineal son proporcionales a las dos inercias inversas.
                    float sign = (i == 0) ? 1 : -1;
                    angularMove[i] = sign * penetration * (angularInertia[i] / totalInertia);
                    linearMove[i] = sign * penetration * (linearInertia[i] / totalInertia);

                    // Para evitar proyecciones angualres demasiado grandes, se limita el movimiento angular.
                    Vector3 projection = this.RelativeContactPositions[i];
                    projection += Vector3.Multiply(this.ContactNormal, Vector3.Dot(-this.RelativeContactPositions[i], this.ContactNormal));

                    float maxMagnitude = angularLimit * projection.Length();

                    if (angularMove[i] < -maxMagnitude)
                    {
                        float totalMove = angularMove[i] + linearMove[i];
                        angularMove[i] = -maxMagnitude;
                        linearMove[i] = totalMove - angularMove[i];
                    }
                    else if (angularMove[i] > maxMagnitude)
                    {
                        float totalMove = angularMove[i] + linearMove[i];
                        angularMove[i] = maxMagnitude;
                        linearMove[i] = totalMove - angularMove[i];
                    }

                    // Tenemos la cantidad lineal de movimiento requerida para girar el cuerpo.
                    // Ahora hay que calcula la rotaci�n deseada para hacerlo rotar.
                    if (angularMove[i] == 0f)
                    {
                        // No hay movimiento angular. No hay rotaci�n.
                        angularChange[i] = Vector3.Zero;
                    }
                    else
                    {
                        // Obtener la direcci�n de la rotaci�n.
                        Vector3 targetAngularDirection = Vector3.Cross(this.RelativeContactPositions[i], this.ContactNormal);

                        Matrix3 inverseInertiaTensor = this.Bodies[i].InverseInertiaTensorWorld;

                        angularChange[i] = Matrix3.Transform(inverseInertiaTensor, targetAngularDirection) * (angularMove[i] / angularInertia[i]);
                    }

                    // Variaci�n de velocidad: movimiento lineal sobre la normal de contacto.
                    linearChange[i] = this.ContactNormal * linearMove[i];

                    // Aplicar el movimiento lineal
                    this.Bodies[i].Position += Vector3.Multiply(this.ContactNormal, linearMove[i]);

                    // Aplicar el cambio en la rotaci�n
                    Quaternion q = this.Bodies[i].Orientation;
                    Core.AddScaledVector(ref q, angularChange[i], 1.0f);
                    this.Bodies[i].Orientation = q;

                    // Hay que actualizar cada cuerpo que est� activo, para que los cambios se reflejen en el cuerpo.
                    // De otro modo, la resoluci�n no modificar� la posici�n para el objeto, y la siguiente ronda de detecci�n de colisi�n resultar� en la misma penetraci�n.
                    if (!this.Bodies[i].IsAwake)
                    {
                        this.Bodies[i].CalculateDerivedData();
                    }
                }
            }
        }
        /// <summary>
        /// Calcula el impulso necesario para resolver el contacto, sabiendo que no hay fricci�n.
        /// </summary>
        /// <param name="inverseInertiaTensor">Tensores de inercia</param>
        /// <remarks>
        /// Los dos tensores de inercia, uno por cada cuerpo del contacto, son necesarios para ahorrar c�lculos.
        /// </remarks>
        internal Vector3 CalculateFrictionlessImpulse(Matrix3[] inverseInertiaTensor)
        {
            Vector3 impulseContact;

            // Calcular un vector que muestra el cambio en la velocidad en coordenadas del mundo, para un impulso unitario en la direcci�n de la normal de contacto.
            Vector3 deltaVelWorld = Vector3.Cross(this.RelativeContactPositions[0], this.ContactNormal);
            deltaVelWorld = Matrix3.Transform(inverseInertiaTensor[0], deltaVelWorld);
            deltaVelWorld = Vector3.Cross(deltaVelWorld, this.RelativeContactPositions[0]);

            // Obtener la variaci�n de la velocidad en coordenadas de contacto.
            float deltaVelocity = Vector3.Dot(deltaVelWorld, this.ContactNormal);

            // A�adir la componente lineal de la variaci�n de la velocidad
            deltaVelocity += this.Bodies[0].InverseMass;

            if (this.Bodies[1] != null)
            {
                deltaVelWorld = Vector3.Cross(this.RelativeContactPositions[1], this.ContactNormal);
                deltaVelWorld = Matrix3.Transform(inverseInertiaTensor[1], deltaVelWorld);
                deltaVelWorld = Vector3.Cross(deltaVelWorld, this.RelativeContactPositions[1]);

                deltaVelocity += Vector3.Dot(deltaVelWorld, this.ContactNormal);

                deltaVelocity += this.Bodies[1].InverseMass;
            }

            // Calcular el tama�o requerido del impulso
            impulseContact.X = this.DesiredDeltaVelocity / deltaVelocity;
            impulseContact.Y = 0;
            impulseContact.Z = 0;

            return impulseContact;
        }
        /// <summary>
        /// Calcula el impulso requerido para resolver el contacto, tomando que hay fricci�n.
        /// </summary>
        /// <param name="inverseInertiaTensor">Tensores de inercia</param>
        /// <remarks>
        /// Los dos tensores de inercia, uno por cada cuerpo del contacto, son necesarios para ahorrar c�lculos.
        /// </remarks>
        internal Vector3 CalculateFrictionImpulse(Matrix3[] inverseInertiaTensor)
        {
            Vector3 impulseContact;
            float inverseMass = this.Bodies[0].InverseMass;

            // El equivalente del producto vectorial en matrices es la multiplicaci�n por la matriz skew symmetric.
            // La matriz se usar� para covertir cantidades lineales en angulares.
            Matrix3 impulseToTorque;
            impulseToTorque = Matrix3.SkewSymmetric(this.RelativeContactPositions[0]);

            // Obtener la matriz para convertir impulso de contacto a variaci�n de velocidad en coordenadas del mundo.
            Matrix3 deltaVelWorld = impulseToTorque;
            deltaVelWorld *= inverseInertiaTensor[0];
            deltaVelWorld *= impulseToTorque;
            deltaVelWorld *= -1;

            if (this.Bodies[1] != null)
            {
                impulseToTorque = Matrix3.SkewSymmetric(this.RelativeContactPositions[1]);

                // Calcular la matriz de modificaci�n de velocidad
                Matrix3 deltaVelWorld2 = impulseToTorque;
                deltaVelWorld2 *= inverseInertiaTensor[1];
                deltaVelWorld2 *= impulseToTorque;
                deltaVelWorld2 *= -1;

                // A�adir el total de la variaci�n de velocidad.
                deltaVelWorld += deltaVelWorld2;

                // A�adir la masa inversa.
                inverseMass += this.Bodies[1].InverseMass;
            }

            // Convertir a coordenadas del contacto cambiando la base.
            Matrix3 deltaVelocity = Matrix3.Transpose(this.ContactToWorld);
            deltaVelocity *= deltaVelWorld;
            deltaVelocity *= ContactToWorld;

            // A�adir la variaci�n de velocidad lineal.
            deltaVelocity.M11 += inverseMass;
            deltaVelocity.M22 += inverseMass;
            deltaVelocity.M33 += inverseMass;

            // Invertir para obtener el impulso necesario por unidad de velocidad.
            Matrix3 impulseMatrix = Matrix3.Invert(deltaVelocity);

            // Encontrar las velocidades que anular.
            Vector3 velKill = new Vector3(
                this.DesiredDeltaVelocity,
                -this.ContactVelocity.Y,
                -this.ContactVelocity.Z);

            // Encontrar el impulso para anular las velocidades
            impulseContact = Matrix3.Transform(impulseMatrix, velKill);

            // Chequear por fricci�n excesiva
            float planarImpulse = (float)Math.Sqrt(impulseContact.Y * impulseContact.Y + impulseContact.Z * impulseContact.Z);
            if (planarImpulse > impulseContact.X * Friction)
            {
                // Es necesario utilizar fricci�n din�mica
                impulseContact.Y /= planarImpulse;
                impulseContact.Z /= planarImpulse;
                impulseContact.X =
                    deltaVelocity.M11 +
                    deltaVelocity.M12 * Friction * impulseContact.Y +
                    deltaVelocity.M13 * Friction * impulseContact.Z;
                impulseContact.X = DesiredDeltaVelocity / impulseContact.X;
                impulseContact.Y *= Friction * impulseContact.X;
                impulseContact.Z *= Friction * impulseContact.X;
            }

            return impulseContact;
        }
    }
}