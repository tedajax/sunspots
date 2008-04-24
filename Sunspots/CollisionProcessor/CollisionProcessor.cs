using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace CollisionProcessor
{
    [ContentProcessor]
    public class CollisionProcessor : ModelProcessor
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> indicies = new List<Vector3>();
        float[,] heightData;
        int MaxX;
        int MaxZ;
        int MinX;
        int MinZ;
        public override ModelContent Process(NodeContent input,
                                             ContentProcessorContext context)
        {
            FindVertices(input);
            ModelContent model = base.Process(input, context);
            Dictionary<string, object> tagData = new Dictionary<string, object>();
            model.Tag = tagData;
            tagData.Add("Verticies", vertices);
            return model;
        }
     

        void FindVertices(NodeContent node)
        {
            MeshContent mesh = node as MeshContent;
            if (mesh != null)
            {
                Matrix absoluteTransform = mesh.AbsoluteTransform;
                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    foreach (int index in geometry.Indices)
                    {
                        Vector3 vertex = geometry.Vertices.Positions[index];
                        vertex = Vector3.Transform(vertex, absoluteTransform);
                        vertices.Add(vertex);
                    }

                }
            }
            foreach (NodeContent child in node.Children)
            {
                FindVertices(child);
            }
        }
    }
}