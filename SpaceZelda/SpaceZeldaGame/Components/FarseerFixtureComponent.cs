using Artemis.Interface;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TiledSharp;

namespace SpaceZelda.Components
{
    // Fixtures in Farseer can be though of as entities
    // They contain position as well as shape
    // Can be fixed or dynamic
    // Have other properties such as friction, elasticity, etc.
    // Has optional onCollision method which adds _____collisionComponents to be processed
    //     * optional because not all tmxObjects have collisions that trigger other events

    class FarseerFixtureComponent : IComponent
    {
        public Body body { get; set; }

        private static string[] DefaultBodyProperties = new string[]
        {
            "BodyType", "Density", "FixedRotation", "Friction", "Mass", "Restitution",  "Rotation"
        };

        public Fixture fixture { get; }

        public World world { get; set; }

        public FarseerFixtureComponent(TmxObject tmxObj, World world, Vector2 offset)
        {
            Vector2 position = ConvertUnits.ToSimUnits(new Vector2((float) tmxObj.X, (float) tmxObj.Y));
            position += ConvertUnits.ToSimUnits(offset);
            body = new Body(world, position);
            foreach (var property in DefaultBodyProperties)
            {
                string value;
                tmxObj.Properties.TryGetValue(property, out value);
                // allow null elements to act as a flag to set default values
                SetProperty(property, value, body);
            }

            float density = 1f;
            string densityStr;
            tmxObj.Properties.TryGetValue("Density", out densityStr);
            if (densityStr != null)
            {
                density = float.Parse(densityStr);
            }
            AttachShape(tmxObj, density);

            this.world = world;
        }

        private void AttachShape(TmxObject tmxObj, float density)
        {
            // A Farseer Body's Position variable defines the CENTER of the Body, so we add half the width and height to get it to the desired location.
            switch (tmxObj.ObjectType)
            {
                case TmxObjectType.Basic:
                case TmxObjectType.Tile:
                    {
                        Vector2 size = ConvertUnits.ToSimUnits(new Vector2((float)tmxObj.Width, (float)tmxObj.Height));
                        Vector2 offset = new Vector2(size.X / 2, size.Y / 2);
                        FixtureFactory.AttachRectangle(size.X, size.Y, density, offset, body);
                        break;
                    }
                case TmxObjectType.Ellipse:
                    {
                        Vector2 size = ConvertUnits.ToSimUnits(new Vector2((float)tmxObj.Width, (float)tmxObj.Height));
                        if (size.X == size.Y)
                        {
                            FixtureFactory.AttachCircle(size.X / 2, density, body);
                        }
                        else
                        {
                            FixtureFactory.AttachEllipse(size.X / 2, size.Y / 2, Settings.MaxPolygonVertices, density, body);
                        }
                        break;
                    }
                case TmxObjectType.Polygon:
                    {
                        Vertices vertices = new Vertices();
                        foreach (var v in tmxObj.Points)
                        {
                            vertices.Add(ConvertUnits.ToSimUnits(new Vector2((float)v.X, (float)v.Y)));
                        }
                        List<Vertices> decomposedVertices = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Bayazit);
                        FixtureFactory.AttachCompoundPolygon(decomposedVertices, density, body);
                        break;
                    }
                case TmxObjectType.Polyline:
                    {
                        Vertices vertices = new Vertices();
                        foreach (var v in tmxObj.Points)
                        {
                            vertices.Add(ConvertUnits.ToSimUnits(new Vector2((float)v.X, (float)v.Y)));
                        }
                        FixtureFactory.AttachChainShape(vertices, body);
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException("TmxObjectType not recognized: " + tmxObj.ObjectType.ToString());
                    }
            }
        }

        private void SetProperty(string property, string value, Body body)
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
