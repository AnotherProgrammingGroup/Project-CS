using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledSharp;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;

namespace TiledSharpUtil
{
    class TmxMapUtil
    {
        public static string[] BodyProperties = new string[]
        {
                "BodyType", "Density", "FixedRotation", "Friction", "Mass", "Restitution",  "Rotation"
        };

        /// <summary>
        /// Inserts all objects from <paramref name="map"/> into <paramref name="world"/>.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="world"></param>
        public static void InsertObjects(TmxMap map, World world)
        {
            List<BodyData> datas = new List<BodyData>();

            // Tiled defines the position of an object as its center
            foreach (var objLayer in map.ObjectGroups)
            {
                foreach (var obj in objLayer.Objects)
                {
                    float density = 1f;
                    string densityStr;
                    bool foundDensity = obj.Properties.TryGetValue("Density", out densityStr);
                    if (foundDensity)
                        density = float.Parse(densityStr);

                    Body body = Insert(obj, density, world);

                    foreach (var property in BodyProperties)
                    {
                        string value;
                        obj.Properties.TryGetValue(property, out value);
                        SetProperty(property, value, body);
                    }
                }
            }
        }

        private static void SetProperty(string property, string value, Body body)
        {
            bool realValue = value != null;
            if (property.Equals("BodyType"))
            {
                if (value.Equals("Kinematic"))
                    body.BodyType = BodyType.Kinematic;
                else if (value.Equals("Dynamic"))
                    body.BodyType = BodyType.Dynamic;
            } else if (property.Equals("FixedRotation"))
            {
                    body.FixedRotation = value.Equals("True");
            } else if (property.Equals("Friction"))
            {
                if (realValue)
                    body.Friction = float.Parse(value);
                else
                    body.Friction = 0.5f;
            } else if (property.Equals("Mass"))
            {
                if (realValue)
                    body.Mass = float.Parse(value);
            } else if (property.Equals("Restitution"))
            {
                if (realValue)
                    body.Restitution = float.Parse(value);
            } else if (property.Equals("Rotation"))
            {
                if (realValue)
                    body.Rotation = float.Parse(value);
            }
        }

        private static Body Insert(TmxObject obj, float density, World world)
        {
            switch (obj.ObjectType)
            {
                case TmxObjectType.Basic:
                case TmxObjectType.Tile:
                    {
                        Vector2 size = ConvertUnits.ToSimUnits(new Vector2((float)obj.Width, (float)obj.Height));
                        Vector2 position = ConvertUnits.ToSimUnits(new Vector2((float)obj.X, (float)obj.Y));
                        position.X += size.X / 2;
                        position.Y += size.Y / 2;
                        return BodyFactory.CreateRectangle(world, size.X, size.Y, density, position);
                    }
                case TmxObjectType.Ellipse:
                    {
                        Vector2 size = ConvertUnits.ToSimUnits(new Vector2((float)obj.Width, (float)obj.Height));
                        Vector2 position = ConvertUnits.ToSimUnits(new Vector2((float)obj.X, (float)obj.Y));
                        position.X += size.X / 2;
                        position.Y += size.Y / 2;
                        return BodyFactory.CreateEllipse(world, size.X / 2, size.Y / 2, Settings.MaxPolygonVertices, density, position);
                    }
                case TmxObjectType.Polygon:
                    {
                        Vertices vertices = new Vertices();
                        foreach (var v in obj.Points)
                        {
                            vertices.Add(ConvertUnits.ToSimUnits(new Vector2((float)v.X, (float)v.Y)));
                        }
                        List<Vertices> decomposedVertices = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Bayazit);
                        Vector2 position = ConvertUnits.ToSimUnits(new Vector2((float)obj.X, (float)obj.Y));
                        return BodyFactory.CreateCompoundPolygon(world, decomposedVertices, density, position);
                    }
                case TmxObjectType.Polyline:
                    {
                        Vertices vertices = new Vertices();
                        foreach (var v in obj.Points)
                        {
                            vertices.Add(ConvertUnits.ToSimUnits(new Vector2((float)v.X, (float)v.Y)));
                        }
                        Vector2 position = ConvertUnits.ToSimUnits(new Vector2((float)obj.X, (float)obj.Y));
                        return BodyFactory.CreateChainShape(world, vertices, position);
                    }
                default:
                    {
                        // can't get here
                        return null;
                    }
            }
        }
    }

    public class BodyData
    {
        public Body Body { get; set; }

        public TmxObject TmxObject { get; set; }
    }
}
