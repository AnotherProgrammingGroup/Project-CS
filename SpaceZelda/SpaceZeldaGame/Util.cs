using System.Collections.Generic;
using TiledSharp;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;

namespace Util
{
    class MapWorldUtil
    {
        public struct TmxObjectBody
        {
            public Body Body { get; set; }

            public TmxObject TmxObject { get; set; }

            public TmxObjectBody(Body body, TmxObject obj)
            {
                Body = body;
                TmxObject = obj;
            }
        }

        private static string[] DefaultBodyProperties = new string[]
        {
                "BodyType", "Density", "FixedRotation", "Friction", "Mass", "Restitution",  "Rotation"
        };

        /// <summary>
        /// Inserts all objects from <paramref name="map"/> into <paramref name="world"/> 
        /// and returns a list of TmxObjectBodys (a struct which contains a reference to 
        /// the Body created and the corresponding TmxObject.)
        /// </summary>
        /// <param name="map"></param>
        /// <param name="world"></param>
        /// <returns> </returns> 
        public static List<TmxObjectBody> InsertObjects(TmxMap map, World world)
        {
            List<TmxObjectBody> tmxObjectBodys = new List<TmxObjectBody>();

            foreach (var objLayer in map.ObjectGroups)
            {
                foreach (var obj in objLayer.Objects)
                {
                    float density = 1f;
                    string densityStr;
                    obj.Properties.TryGetValue("Density", out densityStr);
                    if (densityStr != null)
                        density = float.Parse(densityStr);

                    Body body = CreateBody(obj, density, world);

                    foreach (var property in DefaultBodyProperties)
                    {
                        string value;
                        obj.Properties.TryGetValue(property, out value);
                        // allow null elements to set default value
                        SetProperty(property, value, body);
                    }

                    tmxObjectBodys.Add(new TmxObjectBody(body, obj));
                }
            }

            return tmxObjectBodys;
        }

        private static Body CreateBody(TmxObject obj, float density, World world)
        {
            // A Farseer Body's Position variable defines the CENTER of the Body, so we add half the width and height to get it to the desired location.
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
                        if (size.X == size.Y)
                        {
                            return BodyFactory.CreateCircle(world, size.X / 2, density, position);
                        }
                        else
                        {
                            return BodyFactory.CreateEllipse(world, size.X / 2, size.Y / 2, Settings.MaxPolygonVertices, density, position);
                        }
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

        private static void SetProperty(string property, string value, Body body)
        {
            if (property.Equals("BodyType"))
            {
                if (value == null)
                {
                    // defaults to static body
                }
                else if (value.Equals("Kinematic"))
                {
                    body.BodyType = BodyType.Kinematic;
                }
                else if (value.Equals("Dynamic"))
                {
                    body.BodyType = BodyType.Dynamic;
                }
            }
            else if (property.Equals("FixedRotation"))
            {
                if (value == null)
                {
                    body.FixedRotation = false;
                }
                else if (value.Equals("True"))
                {
                    body.FixedRotation = true;
                }
            }
            else if (property.Equals("Friction"))
            {
                if (value == null)
                {
                    body.Friction = 0.5f;
                }
                else
                {
                    body.Friction = float.Parse(value);
                }
            }
            else if (property.Equals("Mass"))
            {
                if (value == null)
                {
                }
                else
                {
                    body.Mass = float.Parse(value);
                }
            }
            else if (property.Equals("Restitution"))
            {
                if (value == null)
                {
                }
                else
                {
                    body.Restitution = float.Parse(value);
                }
            }
            else if (property.Equals("Rotation"))
            {
                if (value == null)
                {
                }
                else
                {
                    body.Rotation = float.Parse(value);
                }
            }
        }
    }
}
