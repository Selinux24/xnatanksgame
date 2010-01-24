using System.Collections.Generic;
using Common.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Physics;

namespace GameComponents.Scenery
{
    public class Scenery
    {
        /// <summary>
        /// Textura del mapa de alturas del terreno
        /// </summary>
        public Texture2D Terrain;
        /// <summary>
        /// Declaración de vértices del buffer del terreno
        /// </summary>
        public VertexDeclaration TerrainBufferDeclaration;
        /// <summary>
        /// Tamaño en bytes de un vértice del buffer
        /// </summary>
        public int TerrainBufferVertexStride;
        /// <summary>
        /// Buffer de vértices del terreno
        /// </summary>
        public VertexBuffer TerrainBuffer;
        /// <summary>
        /// Número de vértices en el buffer
        /// </summary>
        public int TerrainBufferVertexCount;
        /// <summary>
        /// Buffers de índices según el nivel del detalle
        /// </summary>
        public Dictionary<LOD, IndexBuffer> TerrainIndexBuffers = new Dictionary<LOD, IndexBuffer>();

        /// <summary>
        /// Efecto para dibujar
        /// </summary>
        public Effect Effect;
        /// <summary>
        /// Textura 1 para el terreno
        /// </summary>
        public Texture2D Texture1;
        /// <summary>
        /// Textura 2 para el terreno
        /// </summary>
        public Texture2D Texture2;
        /// <summary>
        /// Textura 3 para el terreno
        /// </summary>
        public Texture2D Texture3;
        /// <summary>
        /// Textura 4 para el terreno
        /// </summary>
        public Texture2D Texture4;

        /// <summary>
        /// Nodo principal
        /// </summary>
        public SceneryNode Root;

        /// <summary>
        /// Indica el nivel de detalle a dibujar
        /// </summary>
        public LOD LevelOfDetail = LOD.None;

        /// <summary>
        /// Obtiene la anchura del escenario
        /// </summary>
        public float Width
        {
            get
            {
                if (this.Root != null)
                {
                    Vector3 pointA = new Vector3(this.Root.BoundingBox.Min.X, 0.0f, 0.0f);
                    Vector3 pointB = new Vector3(this.Root.BoundingBox.Max.X, 0.0f, 0.0f);

                    return Vector3.Distance(pointA, pointB);
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene la largura del escenario
        /// </summary>
        public float Long
        {
            get
            {
                if (this.Root != null)
                {
                    Vector3 pointA = new Vector3(0.0f, 0.0f, this.Root.BoundingBox.Min.Z);
                    Vector3 pointB = new Vector3(0.0f, 0.0f, this.Root.BoundingBox.Max.Z);

                    return Vector3.Distance(pointA, pointB);
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene la altura del escenario
        /// </summary>
        public float Height
        {
            get
            {
                if (this.Root != null)
                {
                    Vector3 pointA = new Vector3(0.0f, this.Root.BoundingBox.Min.Y, 0.0f);
                    Vector3 pointB = new Vector3(0.0f, this.Root.BoundingBox.Max.Y, 0.0f);

                    return Vector3.Distance(pointA, pointB);
                }

                return 0.0f;
            }
        }

        /// <summary>
        /// Obtiene el límite mínimo de anchura
        /// </summary>
        public float MinWidth
        {
            get
            {
                if (this.Root != null)
                {
                    return this.Root.BoundingBox.Min.X;
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene el límite máximo de anchura
        /// </summary>
        public float MaxWidth
        {
            get
            {
                if (this.Root != null)
                {
                    return this.Root.BoundingBox.Max.X;
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene el límite mínimo de profundidad
        /// </summary>
        public float MinLong
        {
            get
            {
                if (this.Root != null)
                {
                    return this.Root.BoundingBox.Min.Z;
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene el límite máximo de profundidad
        /// </summary>
        public float MaxLong
        {
            get
            {
                if (this.Root != null)
                {
                    return this.Root.BoundingBox.Max.Z;
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene el límite mínimo de altura
        /// </summary>
        public float MinHeight
        {
            get
            {
                if (this.Root != null)
                {
                    return this.Root.BoundingBox.Min.Y;
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene el límite máximo de altura
        /// </summary>
        public float MaxHeight
        {
            get
            {
                if (this.Root != null)
                {
                    return this.Root.BoundingBox.Max.Y;
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene el punto central del terreno
        /// </summary>
        public Vector3 Center
        {
            get
            {
                return new Vector3((MaxWidth - MinWidth) / 2.0f, (MaxHeight - MinHeight) / 2.0f, (MaxLong - MinLong) / 2.0f);
            }
        }

        /// <summary>
        /// Obtiene el nodo bajo las coordenadas especificadas
        /// </summary>
        /// <param name="x">Componente x</param>
        /// <param name="z">Componente z</param>
        /// <returns>Devuelve el nodo bajo las coordenadas especificadas o null si no existe</returns>
        private SceneryTriangleNode GetNode(float x, float z)
        {
            return GetNode(x, z, this.Root);
        }
        /// <summary>
        /// Obtiene el subnodo bajo las coordenadas especificadas en el nodo especificado
        /// </summary>
        /// <param name="x">Coordenada x</param>
        /// <param name="z">Coordenada z</param>
        /// <param name="node">Nodo en el que buscar el subnodo</param>
        /// <returns>Devuelve el subnodo del nodo especificado bajo las coordenadas x, z o null si no existe</returns>
        private SceneryTriangleNode GetNode(float x, float z, SceneryNode node)
        {
            if (node != null)
            {
                if (node.HasChilds)
                {
                    if (node.NorthEast.Contains(x, z) != ContainmentType.Disjoint)
                    {
                        return GetNode(x, z, node.NorthEast);
                    }
                    else if (node.NorthWest.Contains(x, z) != ContainmentType.Disjoint)
                    {
                        return GetNode(x, z, node.NorthWest);
                    }
                    else if (node.SouthEast.Contains(x, z) != ContainmentType.Disjoint)
                    {
                        return GetNode(x, z, node.SouthEast);
                    }
                    else if (node.SouthWest.Contains(x, z) != ContainmentType.Disjoint)
                    {
                        return GetNode(x, z, node.SouthWest);
                    }
                }
                else
                {
                    return node as SceneryTriangleNode;
                }
            }

            return null;
        }
        /// <summary>
        /// Obtiene si existe intersección entre el ray y el escenario
        /// </summary>
        /// <param name="x">Componente x</param>
        /// <param name="z">Componente z</param>
        /// <param name="triangle">Devuelve el triángulo de intersección si existe</param>
        /// <param name="intersectionPoint">Devuelve el punto de intersección en el triángulo</param>
        /// <param name="distanceToPoint">Devuelve la distancia entre el punto de intersección y el origen del rayo</param>
        /// <returns>Devuelve verdadero si existe intersección o falso en el resto de los casos</returns>
        public bool Intersects(float x, float z, out Triangle? triangle, out Vector3? intersectionPoint, out float? distanceToPoint)
        {
            Vector3 point = new Vector3(x, this.MinHeight, z);

            Ray ray = new Ray(point, Vector3.Up);

            return Intersects(ray, out triangle, out intersectionPoint, out distanceToPoint);
        }
        /// <summary>
        /// Obtiene si existe intersección entre el ray y el escenario
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="triangle">Devuelve el triángulo de intersección si existe</param>
        /// <param name="intersectionPoint">Devuelve el punto de intersección en el triángulo</param>
        /// <param name="distanceToPoint">Devuelve la distancia entre el punto de intersección y el origen del rayo</param>
        /// <returns>Devuelve verdadero si existe intersección o falso en el resto de los casos</returns>
        public bool Intersects(Ray ray, out Triangle? triangle, out Vector3? intersectionPoint, out float? distanceToPoint)
        {
            triangle = null;
            intersectionPoint = null;
            distanceToPoint = null;

            SceneryTriangleNode node = this.GetNode(ray.Position.X, ray.Position.Z);
            if (node != null)
            {
                return node.Intersects(ray, out triangle, out intersectionPoint, out distanceToPoint);
            }

            return false;
        }

        /// <summary>
        /// Establecer las matrices
        /// </summary>
        /// <param name="world">Martriz mundo</param>
        /// <param name="view">Matriz vista</param>
        /// <param name="projection">Matriz proyección</param>
        public void SetWorldViewProjection(Matrix world, Matrix view, Matrix projection)
        {
            this.Effect.Parameters["xProjection"].SetValue(projection);
            this.Effect.Parameters["xView"].SetValue(view);
            this.Effect.Parameters["xWorld"].SetValue(world);
        }
        /// <summary>
        /// Establecer las luces
        /// </summary>
        /// <param name="enabled">Indica si la luz está habilitada</param>
        /// <param name="direction">Dirección de la luz</param>
        /// <param name="ambientValue">Ambiente</param>
        public void SetLights(bool enabled, Vector3 direction, float ambientValue)
        {
            this.Effect.Parameters["xEnableLighting"].SetValue(enabled);
            this.Effect.Parameters["xLightDirection"].SetValue(direction);
            this.Effect.Parameters["xAmbient"].SetValue(ambientValue);
        }
        /// <summary>
        /// Dibujar
        /// </summary>
        /// <param name="device">Dispositivo gráfico</param>
        /// <param name="gameTime">Tiempo de juego</param>
        public void Draw(GraphicsDevice device, GameTime gameTime)
        {
            // Inicializar los nodos para el dibujado
            this.Root.PrepareForDrawing();

            // Establecer la declaración de las primitivas
            device.VertexDeclaration = this.TerrainBufferDeclaration;
            // Establecer el buffer de vértices
            device.Vertices[0].SetSource(
                this.TerrainBuffer, 
                0, 
                this.TerrainBufferVertexStride);

            // Establecer los parámetros en el efecto
            this.Effect.CurrentTechnique = this.Effect.Techniques["MultiTextured"];
            this.Effect.Parameters["xSnowTexture"].SetValue(this.Texture1);
            this.Effect.Parameters["xRockTexture"].SetValue(this.Texture2);
            this.Effect.Parameters["xGrassTexture"].SetValue(this.Texture3);
            this.Effect.Parameters["xSandTexture"].SetValue(this.Texture4);

            // Dibujar el nivel de detall máximo
            this.LODDraw(device, gameTime, LOD.High);
            // Dibujar el nivel de detalle medio
            this.LODDraw(device, gameTime, LOD.Medium);
            // Dibujar el nivel de menos detalle
            this.LODDraw(device, gameTime, LOD.Low);
        }
        /// <summary>
        /// Dibuja el contenido gráfico del componente atendiendo al nivel de detalle
        /// </summary>
        /// <param name="device">Dispositivo gráfico</param>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="lod">Nivel de detalle</param>
        private void LODDraw(GraphicsDevice device, GameTime gameTime, LOD lod)
        {
            // Obtener los nodos visibles
            SceneryNode[] nodesToDraw = this.Root.GetNodesToDraw(lod);
            if (nodesToDraw.Length > 0)
            {
                // Comprobar si el nodo se debe dibujar
                if ((this.LevelOfDetail != LOD.None) && (this.LevelOfDetail != lod))
                {
                    return;
                }

                // Establecer los índices
                device.Indices = this.TerrainIndexBuffers[lod];

                this.Effect.Begin();

                foreach (EffectPass pass in this.Effect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    foreach (SceneryNode node in nodesToDraw)
                    {
                        SceneryTriangleNode triNode = node as SceneryTriangleNode;
                        if (triNode != null)
                        {
                            int vertexCount = this.TerrainBufferVertexCount;
                            int startIndex = triNode.StartIndexes[lod];
                            int primitivesToDraw = triNode.PrimitiveCount[lod];

                            // Dibujar el nodo
                            device.DrawIndexedPrimitives(
                                PrimitiveType.TriangleList,
                                0,
                                0,
                                vertexCount,
                                startIndex,
                                primitivesToDraw);
                        }
                    }

                    pass.End();
                }

                this.Effect.End();
            }
        }

        /// <summary>
        /// Obtiene una lista de nodos dibujados en el último frame
        /// </summary>
        /// <returns>Devuelve la lista de nodos dibujados en el último frame, o null si no se ha inicializado</returns>
        internal SceneryInfoNodeDrawn[] GetNodesDrawn(LOD lod)
        {
            if (this.Root != null)
            {
                return this.Root.GetNodesDrawn(lod);
            }

            return new SceneryInfoNodeDrawn[] { };
        }
    }
}
