using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Readers
{
    using Common.Components;
    using Common.Primitives;
    using GameComponents.Scenery;

    class SceneryReader : ContentTypeReader<Scenery>
    {
        protected override Scenery Read(ContentReader input, Scenery existingInstance)
        {
            Scenery scenery = existingInstance;
            if (scenery == null)
            {
                scenery = new Scenery();
            }

            scenery.Terrain = input.ReadObject<Texture2D>();
            scenery.TerrainBufferDeclaration = input.ReadObject<VertexDeclaration>();
            scenery.TerrainBufferVertexStride = input.ReadInt32();
            scenery.TerrainBuffer = input.ReadObject<VertexBuffer>();
            scenery.TerrainBufferVertexCount = input.ReadInt32();

            List<SceneryTriangleNode> nodes = new List<SceneryTriangleNode>();

            int nodeCount = input.ReadInt32();
            for (int i = 0; i < nodeCount; i++)
            {
                SceneryTriangleNode node = input.ReadObject<SceneryTriangleNode>();

                nodes.Add(node);
            }

            SceneryNode root = BuildQuadtree(nodes.ToArray());

            scenery.Root = root;

            scenery.TerrainIndexBuffers.Add(LOD.High, input.ReadObject<IndexBuffer>());
            scenery.TerrainIndexBuffers.Add(LOD.Medium, input.ReadObject<IndexBuffer>());
            scenery.TerrainIndexBuffers.Add(LOD.Low, input.ReadObject<IndexBuffer>());

            scenery.Effect = input.ReadObject<Effect>();
            scenery.Texture1 = input.ReadObject<Texture2D>();
            scenery.Texture2 = input.ReadObject<Texture2D>();
            scenery.Texture3 = input.ReadObject<Texture2D>();
            scenery.Texture4 = input.ReadObject<Texture2D>();

            return scenery;
        }

        /// <summary>
        /// Construye el quadtree
        /// </summary>
        /// <param name="nodes">Nodos</param>
        /// <returns>Devuelve el nodo raíz del quadtree construido</returns>
        private SceneryNode BuildQuadtree(SceneryNode[] nodes)
        {
            SceneryNode[] resultNodes = nodes;

            while (resultNodes.Length > 1)
            {
                resultNodes = BuildQuadtreeNodes(resultNodes);
            }

            return resultNodes[0];
        }
        /// <summary>
        /// Construye los nodos de un quadtree según el nivel
        /// </summary>
        /// <param name="nodes">Nodos</param>
        /// <returns>Devuelve la lista de nodos superiores del quadtree</returns>
        private SceneryNode[] BuildQuadtreeNodes(SceneryNode[] nodes)
        {
            int upperNodeCount = nodes.Length / 4;
            int upperNodeSide = Convert.ToInt32(Math.Sqrt(nodes.Length));

            List<SceneryNode> resultNodes = new List<SceneryNode>();
            for (int y = 0; y < upperNodeSide; y += 2)
            {
                for (int x = 0; x < upperNodeSide; x += 2)
                {
                    int start = (y * upperNodeSide) + x;

                    int indexA = start;
                    int indexB = start + 1;
                    int indexC = start + upperNodeSide;
                    int indexD = start + upperNodeSide + 1;

                    //Asignar nodos al nuevo nodo
                    SceneryNode newNode = new SceneryNode();

                    newNode.Build(
                        nodes[indexA],
                        nodes[indexB],
                        nodes[indexC],
                        nodes[indexD]);

                    resultNodes.Add(newNode);
                }
            }

            return resultNodes.ToArray();
        }
    }
}
