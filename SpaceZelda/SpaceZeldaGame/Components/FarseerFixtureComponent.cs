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
        //Position is by default center of object
        public Body body { get; set; }

        // offset to transform body position to upper left corner
        public Vector2 ULoffset { get; set; }

        private static string[] DefaultBodyProperties = new string[]
        {
            "BodyType", "Density", "FixedRotation", "Friction", "Mass", "Restitution",  "Rotation"
        };

        public Fixture fixture { get; set; }
        public List<Fixture> fixtures { get; set; } //for polygons

        public World world { get; set; }

        // Creates Farseer Fixture based on tmxObj specifications
        // Can optionally be offset from position given by tmxObj
        public FarseerFixtureComponent(TmxObject tmxObj, World world, Vector2 offset = default(Vector2))
        {
            this.world = world;
            CreateBody(tmxObj, offset);
            CreateFixture(tmxObj);
        }

        // Returns converted position (to pixels) of object
        public Vector2 getULPosition()
        {
            return ConvertUnits.ToDisplayUnits(body.Position + ULoffset);
        }

        private void CreateBody(TmxObject tmxObj, Vector2 offset)
        {
            Vector2 position = ConvertUnits.ToSimUnits(new Vector2((float)tmxObj.X, (float)tmxObj.Y));
            position += ConvertUnits.ToSimUnits(offset);
            switch (tmxObj.ObjectType)
            {
                case TmxObjectType.Basic:
                case TmxObjectType.Tile:
                case TmxObjectType.Ellipse:
                    {
                        //convert position from top right to center
                        Vector2 size = ConvertUnits.ToSimUnits(new Vector2((float)tmxObj.Width, (float)tmxObj.Height));
                        position += new Vector2(size.X / 2, size.Y / 2);
                        ULoffset = new Vector2(-1 * size.X / 2, -1 * size.Y / 2);
                        break;
                    }
                default:
                    {
                        ULoffset = Vector2.Zero;
                        break;
                    }
            }

            body = new Body(world, position);

            foreach (var property in DefaultBodyProperties)
            {
                string value;
                tmxObj.Properties.TryGetValue(property, out value);
                // allow null elements to act as a flag to set default values
                SetProperty(property, value, body);
            }
        }

        private void CreateFixture(TmxObject tmxObj)
        {
            float density = 1f;
            string densityStr;
            tmxObj.Properties.TryGetValue("Density", out densityStr);
            if (densityStr != null)
            {
                density = float.Parse(densityStr);
            }

            // A Farseer Body's Position variable defines the CENTER of the Body, so we add half the width and height to get it to the desired location.
            switch (tmxObj.ObjectType)
            {
                case TmxObjectType.Basic:
                case TmxObjectType.Tile:
                    {
                        Vector2 size = ConvertUnits.ToSimUnits(new Vector2((float)tmxObj.Width, (float)tmxObj.Height));
                        fixture = FixtureFactory.AttachRectangle(size.X, size.Y, density, Vector2.Zero, body);
                        break;
                    }
                case TmxObjectType.Ellipse:
                    {
                        Vector2 size = ConvertUnits.ToSimUnits(new Vector2((float)tmxObj.Width, (float)tmxObj.Height));
                        if (size.X == size.Y)
                        {
                            fixture = FixtureFactory.AttachCircle(size.X / 2, density, body);
                        }
                        else
                        {
                            fixture = FixtureFactory.AttachEllipse(size.X / 2, size.Y / 2, Settings.MaxPolygonVertices, density, body);
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
                        fixtures = FixtureFactory.AttachCompoundPolygon(decomposedVertices, density, body);
                        break;
                    }
                case TmxObjectType.Polyline:
                    {
                        Vertices vertices = new Vertices();
                        foreach (var v in tmxObj.Points)
                        {
                            vertices.Add(ConvertUnits.ToSimUnits(new Vector2((float)v.X, (float)v.Y)));
                        }
                        fixture = FixtureFactory.AttachChainShape(vertices, body);
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
                    body.FixedRotation = true;
                }
                else if (value.Equals("False"))
                {
                    body.FixedRotation = false;;
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
