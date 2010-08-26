using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Scenery
{
    using Common.Components;
    using Common.Drawing;
    using Physics;

    /// <summary>
    /// Escenario
    /// </summary>
    public partial class Scenery : IScenery
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

        public Texture2D DetailTexture1;
        public Texture2D DetailTexture2;
        public Texture2D DetailTexture3;
        public Texture2D DetailTexture4;

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
                    Vector3 pointA = new Vector3(this.Root.AABB.Min.X, 0.0f, 0.0f);
                    Vector3 pointB = new Vector3(this.Root.AABB.Max.X, 0.0f, 0.0f);

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
                    Vector3 pointA = new Vector3(0.0f, 0.0f, this.Root.AABB.Min.Z);
                    Vector3 pointB = new Vector3(0.0f, 0.0f, this.Root.AABB.Max.Z);

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
                    Vector3 pointA = new Vector3(0.0f, this.Root.AABB.Min.Y, 0.0f);
                    Vector3 pointB = new Vector3(0.0f, this.Root.AABB.Max.Y, 0.0f);

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
                    return this.Root.AABB.Min.X;
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
                    return this.Root.AABB.Max.X;
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
                    return this.Root.AABB.Min.Z;
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
                    return this.Root.AABB.Max.Z;
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
                    return this.Root.AABB.Min.Y;
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
                    return this.Root.AABB.Max.Y;
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
        /// Obtiene la altura en el punto especificado
        /// </summary>
        /// <param name="x">Coordenada X</param>
        /// <param name="z">Coordenada Y</param>
        /// <returns>Devuelve la componente Y en las coordenadas X y Z dadas</returns>
        public float? GetHeigthAtPoint(float x, float z)
        {
            if (this.Root != null)
            {
                return this.Root.GetHeigthAtPoint(x, z);
            }

            return null;
        }

        /// <summary>
        /// Obtiene la lista de nodos que tienen intersección con el AABB
        /// </summary>
        /// <param name="aabb">AABB</param>
        /// <returns>Devuelve la lista de nodos que tienen intersección con el AABB</returns>
        private SceneryTriangleNode[] GetNodes(BoundingBox aabb)
        {
            List<SceneryTriangleNode> resultNodes = new List<SceneryTriangleNode>();

            this.GetNodes(aabb, this.Root, ref resultNodes);

            return resultNodes.ToArray();
        }
        /// <summary>
        /// Obtiene la lista de nodos que tienen intersección con el AABB a partir del nodo especificado
        /// </summary>
        /// <param name="aabb">AABB</param>
        /// <param name="node">Nodo inicial</param>
        /// <param name="resultNodes">Obtiene la lista de nodos finales con intersección</param>
        private void GetNodes(BoundingBox aabb, SceneryNode node, ref List<SceneryTriangleNode> resultNodes)
        {
            if (node != null)
            {
                if (node.HasChilds)
                {
                    if (aabb.Contains(node.NorthEast.AABB) != ContainmentType.Disjoint)
                    {
                        this.GetNodes(aabb, node.NorthEast, ref resultNodes);
                    }

                    if (aabb.Contains(node.NorthWest.AABB) != ContainmentType.Disjoint)
                    {
                        this.GetNodes(aabb, node.NorthWest, ref resultNodes);
                    }

                    if (aabb.Contains(node.SouthEast.AABB) != ContainmentType.Disjoint)
                    {
                        this.GetNodes(aabb, node.SouthEast, ref resultNodes);
                    }

                    if (aabb.Contains(node.SouthWest.AABB) != ContainmentType.Disjoint)
                    {
                        this.GetNodes(aabb, node.SouthWest, ref resultNodes);
                    }
                }
                else
                {
                    if (node is SceneryTriangleNode)
                    {
                        resultNodes.Add((SceneryTriangleNode)node);
                    }
                }
            }
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
            this.Effect.Parameters["xTexture1"].SetValue(this.Texture1);
            this.Effect.Parameters["xTexture2"].SetValue(this.Texture2);
            this.Effect.Parameters["xTexture3"].SetValue(this.Texture3);
            this.Effect.Parameters["xTexture4"].SetValue(this.Texture4);
            this.Effect.Parameters["xDetailTexture1"].SetValue(this.DetailTexture1);
            this.Effect.Parameters["xDetailTexture2"].SetValue(this.DetailTexture2);
            this.Effect.Parameters["xDetailTexture3"].SetValue(this.DetailTexture3);
            this.Effect.Parameters["xDetailTexture4"].SetValue(this.DetailTexture4);

            // Obtener los nodos visibles
            SceneryNode[] lowLODnodes = this.Root.GetNodesToDraw(LOD.Low);
            SceneryNode[] mediumLODnodes = this.Root.GetNodesToDraw(LOD.Medium);
            SceneryNode[] highLODnodes = this.Root.GetNodesToDraw(LOD.High);

            if (lowLODnodes != null && lowLODnodes.Length > 0)
            {
                // Dibujar el nivel de menos detalle
                this.LODDraw(device, gameTime, lowLODnodes, LOD.Low);
            }

            if (mediumLODnodes != null && mediumLODnodes.Length > 0)
            {
                // Dibujar el nivel de detalle medio
                this.LODDraw(device, gameTime, mediumLODnodes, LOD.Medium);
            }

            if (highLODnodes != null && highLODnodes.Length > 0)
            {
                // Dibujar el nivel de detall máximo
                this.LODDraw(device, gameTime, highLODnodes, LOD.High);
            }

#if DEBUG
            this.DrawDebug(device, gameTime);
#endif
        }
        /// <summary>
        /// Dibuja el contenido gráfico del componente atendiendo al nivel de detalle
        /// </summary>
        /// <param name="device">Dispositivo gráfico</param>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="nodesToDraw">Lista de nodos a dibujar</param>
        /// <param name="lod">Nivel de detalle</param>
        private void LODDraw(GraphicsDevice device, GameTime gameTime, SceneryNode[] nodesToDraw, LOD lod)
        {
            if (nodesToDraw != null && nodesToDraw.Length > 0)
            {
                // Comprobar si el nodo se debe dibujar
                if ((this.LevelOfDetail != LOD.None) && (this.LevelOfDetail != lod))
                {
                    return;
                }

                // Establecer los índices según el nivel de detalle
                device.Indices = this.TerrainIndexBuffers[lod];

                // Cantidad de vértices del buffer
                int vertexCount = this.TerrainBufferVertexCount;

                this.Effect.Begin();

                foreach (EffectPass pass in this.Effect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    foreach (SceneryNode node in nodesToDraw)
                    {
                        SceneryTriangleNode triNode = node as SceneryTriangleNode;
                        if (triNode != null)
                        {
                            int centerPrimitiveCount = triNode.IndexInfo[lod].CenterPrimitiveCount;
                            int borderConnectionPrimitiveCount = triNode.IndexInfo[lod].BorderConnectionPrimitiveCount;
                            int borderPrimitiveCount = triNode.IndexInfo[lod].BorderPrimitiveCount;

                            // Dibujar el centro
                            this.DrawNodePart(
                                device,
                                triNode.IndexInfo[lod].CenterOffset,
                                vertexCount,
                                centerPrimitiveCount);

                            // Dibujar los bordes si es necesario
                            if (borderPrimitiveCount > 0 || borderConnectionPrimitiveCount > 0)
                            {
                                if (triNode.IsNorthBorder)
                                {
                                    // Dibujar la conexión norte
                                    this.DrawNodePart(
                                        device,
                                        triNode.IndexInfo[lod].NorthConnectionOffset,
                                        vertexCount,
                                        borderConnectionPrimitiveCount);
                                }
                                else
                                {
                                    // Dibujar el norte
                                    this.DrawNodePart(
                                        device,
                                        triNode.IndexInfo[lod].NorthOffset,
                                        vertexCount,
                                        borderPrimitiveCount);
                                }

                                if (triNode.IsSouthBorder)
                                {
                                    // Dibujar la conexión norte
                                    this.DrawNodePart(
                                        device,
                                        triNode.IndexInfo[lod].SouthConnectionOffset,
                                        vertexCount,
                                        borderConnectionPrimitiveCount);
                                }
                                else
                                {
                                    // Dibujar el norte
                                    this.DrawNodePart(
                                        device,
                                        triNode.IndexInfo[lod].SouthOffset,
                                        vertexCount,
                                        borderPrimitiveCount);
                                }

                                if (triNode.IsWestBorder)
                                {
                                    // Dibujar la conexión norte
                                    this.DrawNodePart(
                                        device,
                                        triNode.IndexInfo[lod].WestConnectionOffset,
                                        vertexCount,
                                        borderConnectionPrimitiveCount);
                                }
                                else
                                {
                                    // Dibujar el norte
                                    this.DrawNodePart(
                                        device,
                                        triNode.IndexInfo[lod].WestOffset,
                                        vertexCount,
                                        borderPrimitiveCount);
                                }

                                if (triNode.IsEastBorder)
                                {
                                    // Dibujar la conexión norte
                                    this.DrawNodePart(
                                        device,
                                        triNode.IndexInfo[lod].EastConnectionOffset,
                                        vertexCount,
                                        borderConnectionPrimitiveCount);
                                }
                                else
                                {
                                    // Dibujar el norte
                                    this.DrawNodePart(
                                        device,
                                        triNode.IndexInfo[lod].EastOffset,
                                        vertexCount,
                                        borderPrimitiveCount);
                                }
                            }
                        }
                    }

                    pass.End();
                }

                this.Effect.End();
            }
        }
        /// <summary>
        /// Dibuja la parte del nodo especificada
        /// </summary>
        /// <param name="device">Dispositivo</param>
        /// <param name="offset">Indice de inicio de la parte</param>
        /// <param name="vertexCount">Número total de vértices</param>
        /// <param name="primitiveCount">Número de primitivas a dibujar</param>
        private void DrawNodePart(GraphicsDevice device, int offset, int vertexCount, int primitiveCount)
        {
            if (primitiveCount > 0)
            {
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    vertexCount,
                    offset,
                    primitiveCount);
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
