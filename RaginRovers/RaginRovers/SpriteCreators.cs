﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Dynamics;
using System.IO;
using Microsoft.Xna.Framework.Storage;
using RaginRoversLibrary;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Factories;

namespace RaginRovers
{
    public enum GameObjectTypes
    {
        DOG,
        CAT,
        CLOUD1,
        CLOUD2,
        CLOUD3,
        CLOUD4,
        CLOUD5,
        CLOUD6,
        WOOD1,
        WOOD2,
        WOOD3,
        WOOD4,
        PLATFORM_LEFT,
        PLATFORM_MIDDLE,
        PLATFORM_RIGHT,
        CANNON,
        CANNONWHEEL,
        POWERMETERBAR,
        POWERMETERTAB,
        BOOM = 300,
        PUFF,
        DINO,
        SUN,
        EXPLOSION1,
        PLANE,
        CATSPLODE,
        DUSTSPLODE,
        EAHSCSLOGO,
        YOUWIN,
        YOULOSE,
        SCOREPLUS50,
        SCOREPLUS100,
        SCOREPLUS250,
        GROUND = 3000 // PLACEHOLDER - NOT A CREATEABLE TYPE
    }

    // Helper class to create sprites
    public static class SpriteCreators
    {
        public static Dictionary<string, Rectangle> spriteSourceRectangles;
        public static Random rand = new Random(System.Environment.TickCount);

        public static void Load(string path)
        {
            SpriteCreators.spriteSourceRectangles = new Dictionary<string, Rectangle>();

            // open a StreamReader to read the index

            using (StreamReader reader = new StreamReader(path))
            {
                // while we're not done reading...
                while (!reader.EndOfStream)
                {
                    // get a line
                    string line = reader.ReadLine();

                    // split at the equals sign
                    string[] sides = line.Split('=');

                    // trim the right side and split based on spaces
                    string[] rectParts = sides[1].Trim().Split(' ');

                    // create a rectangle from those parts
                    Rectangle r = new Rectangle(
                       int.Parse(rectParts[0]),
                       int.Parse(rectParts[1]),
                       int.Parse(rectParts[2]),
                       int.Parse(rectParts[3]));

                    // add the name and rectangle to the dictionary
                    SpriteCreators.spriteSourceRectangles.Add(sides[0].Trim(), r);
                }
            }

        }

        public static void AddFrames(Sprite sprite, string prefix, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                string key = prefix + i.ToString().PadLeft(2, '0');
                sprite.AddFrame(SpriteCreators.spriteSourceRectangles[key]);
            }
        }

        public static Sprite CreateWood(
                                            int type,
                                            Vector2 location,
                                            Texture2D texture,
                                            Vector2 velocity,
                                            float rotation)
        {
            Sprite sprite = new Sprite("woodshape" + type,
                                               location,
                                               texture,
                                               SpriteCreators.spriteSourceRectangles["woodShape" + type],
                                               velocity,
                                               BodyType.Dynamic,
                                               true);

            
            sprite.PhysicsBody.LinearDamping = 0.3f;
            sprite.PhysicsBody.AngularDamping = 0.3f;
            sprite.PhysicsBody.Restitution = 0.0f;
            sprite.PhysicsBody.Mass = 10f;
            sprite.PhysicsBody.Friction = 1f;
            sprite.PhysicsBody.Inertia = 0;
            //sprite.PhysicsBody.SleepingAllowed = true;
            //sprite.PhysicsBody.Awake = false;

            sprite.PhysicsBodyFixture.Restitution = 0f;
            
            
            sprite.Rotation = rotation;
            
            

            return sprite;
        }

        public static Sprite CreateWood1(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            return SpriteCreators.CreateWood(1, location, texture, velocity, rotation);
        }

        public static Sprite CreateWood2(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            return SpriteCreators.CreateWood(2, location, texture, velocity, rotation);
        }

        public static Sprite CreateWood3(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            return SpriteCreators.CreateWood(3, location, texture, velocity, rotation);
        }

        public static Sprite CreateWood4(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            return SpriteCreators.CreateWood(4, location, texture, velocity, rotation);
        }

        public static Sprite CreateCat(
                                            Vector2 location,
                                            Texture2D texture,
                                            Vector2 velocity,
                                            float rotation)
        {

            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["cat00"],
                                   velocity,
                                   BodyType.Dynamic,
                                   false);

            SpriteCreators.AddFrames(sprite, "cat", 10);

            sprite.Frame = rand.Next(0, 10);

            /*
            for (int i = 1; i <= 10; i++)
            {
                string key = "cat" + i.ToString().PadLeft(2, '0');
                sprite.AddFrame(SpriteCreators.spriteSourceRectangles[key]);
            }
            */

            sprite.Rotation = rotation;

            //rects
            //9, 19, 95, 111

            sprite.PhysicsBodyFixture = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(95), ConvertUnits.ToSimUnits(100), 1, ConvertUnits.ToSimUnits(new Vector2(9, 10)), sprite.PhysicsBody);
            sprite.PhysicsBodyFixture.OnCollision += new OnCollisionEventHandler(sprite.HandleCollision);

            return sprite;
        }

        public static Sprite CreateDog(
                                            Vector2 location,
                                            Texture2D texture,
                                            Vector2 velocity,
                                            float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["rover00"],
                                   velocity,
                                   BodyType.Dynamic,
                                   false);


            //FarseerPhysics.Common.Vertices verts = new FarseerPhysics.Common.Vertices();

            SpriteCreators.AddFrames(sprite, "rover", 12);

            sprite.Rotation = rotation;
            
            //option 1 with two fixtures
            //rects
            //45, 16, 153, 114
            //0, 60, 45, 70
            //sprite.PhysicsBodyFixture = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(153), ConvertUnits.ToSimUnits(114), 1, ConvertUnits.ToSimUnits(new Vector2(45, 16)), sprite.PhysicsBody);
            //sprite.PhysicsBodyFixture = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(45), ConvertUnits.ToSimUnits(70), 1, ConvertUnits.ToSimUnits(new Vector2(0, 60)), sprite.PhysicsBody);
            //option 2 with one fixture
            //rects
            //16, 12, 175, 125
            sprite.PhysicsBodyFixture = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(175), ConvertUnits.ToSimUnits(100), 10, ConvertUnits.ToSimUnits(new Vector2(16, 12)), sprite.PhysicsBody);
            sprite.PhysicsBodyFixture.Friction = 15f;

            sprite.PhysicsBodyFixture.OnCollision += new OnCollisionEventHandler(sprite.HandleCollision);

            sprite.PhysicsBody.Mass = 30f;

            return sprite;
        }

        public static Sprite CreateBoom(Vector2 location,
                                            Texture2D texture,
                                            Vector2 velocity,
                                            float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["boom00"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.PhysicsBody.CollidesWith = Category.None;
            sprite.FrameTime = 0.06f;

            SpriteCreators.AddFrames(sprite, "boom", 12);



            sprite.Rotation = rotation;

            return sprite;
        }

        public static Sprite CreateDustsplode(Vector2 location,
                                            Texture2D texture,
                                            Vector2 velocity,
                                            float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["dustsplode00"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.PhysicsBody.CollidesWith = Category.None;
            sprite.FrameTime = 0.06f;

            SpriteCreators.AddFrames(sprite, "dustsplode", 8);



            sprite.Rotation = rotation;

            return sprite;
        }

        public static Sprite CreateCatsplode(Vector2 location,
                                            Texture2D texture,
                                            Vector2 velocity,
                                            float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["catsplode00"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.PhysicsBody.CollidesWith = Category.None;
            sprite.FrameTime = 0.06f;

            SpriteCreators.AddFrames(sprite, "catsplode", 12);



            sprite.Rotation = rotation;

            return sprite;
        }

        public static Sprite CreatePuff(Vector2 location,
                                            Texture2D texture,
                                            Vector2 velocity,
                                            float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["puffs_01"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.PhysicsBody.CollidesWith = Category.None;
            sprite.FrameTime = 0.06f;

            SpriteCreators.AddFrames(sprite, "puffs_", 7);
            sprite.Rotation = rotation;

            return sprite;
        }

        public static Sprite CreateExplosion1(Vector2 location,
                                    Texture2D texture,
                                    Vector2 velocity,
                                    float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["explosion1_00"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.PhysicsBody.CollidesWith = Category.None;
            sprite.FrameTime = 0.06f;

            SpriteCreators.AddFrames(sprite, "explosion1_", 14);
            sprite.Rotation = rotation;

            return sprite;
        }


        public static Sprite CreateExplosion2(Vector2 location,
                                    Texture2D texture,
                                    Vector2 velocity,
                                    float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["explosion2_00"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.PhysicsBody.CollidesWith = Category.None;
            sprite.FrameTime = 0.06f;

            SpriteCreators.AddFrames(sprite, "explosion2_", 9);
            sprite.Rotation = rotation;

            return sprite;
        }


        public static Sprite CreatePlane(Vector2 location,
                                    Texture2D texture,
                                    Vector2 velocity,
                                    float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["plane00"],
                                   velocity,
                                   BodyType.Static,
                                   false);

            //sprite.FrameTime = 0.06f;

            SpriteCreators.AddFrames(sprite, "plane", 3);
            sprite.Rotation = rotation;

            return sprite;
        }

        public static Sprite CreateSun(Vector2 location,
                                            Texture2D texture,
                                            Vector2 velocity,
                                            float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["sun00"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.PhysicsBody.CollidesWith = Category.None;
            sprite.FrameTime = 0.06f;

            SpriteCreators.AddFrames(sprite, "sun", 4);
            sprite.Rotation = rotation;

            return sprite;
        }

        public static Sprite CreateDino(Vector2 location,
                                            Texture2D texture,
                                            Vector2 velocity,
                                            float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["dinosaur"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.PhysicsBody.CollidesWith = Category.None;
            sprite.PhysicsBody.IgnoreGravity = true;
            sprite.Rotation = rotation;

            return sprite;
        }
        /*public static Sprite CreateEAHSCSLogo(Vector2 location,
                                            Texture2D texture,
                                            Vector2 velocity,
                                            float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["eahscslogo"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.PhysicsBody.CollidesWith = Category.None;
            sprite.PhysicsBody.IgnoreGravity = true;
            sprite.Rotation = rotation;

            return sprite;
        }*/

        public static Sprite CreatePlatformLeft(Vector2 location,
                                                    Texture2D texture,
                                                    Vector2 velocity,
                                                    float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["platform_left"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.Rotation = rotation;

            return sprite;
        }

        public static Sprite CreatePlatformMiddle(Vector2 location,
                                                    Texture2D texture,
                                                    Vector2 velocity,
                                                    float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["platform_middle"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.Rotation = rotation;

            return sprite;
        }

        public static Sprite CreatePlatformRight(Vector2 location,
                                                    Texture2D texture,
                                                    Vector2 velocity,
                                                    float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["platform_right"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.Rotation = rotation;

            return sprite;
        }

        public static Sprite CreateCannon(Vector2 location,
                                                    Texture2D texture,
                                                    Vector2 velocity,
                                                    float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                    location,
                                    texture,
                                    SpriteCreators.spriteSourceRectangles["cannon_0001_Layer-3"],
                                    velocity,
                                    BodyType.Static,
                                    true);


            sprite.PhysicsBody.CollidesWith = Category.None;

            sprite.Rotation = rotation; //will change

            return sprite;
        }
        public static Sprite CreateCannonWheel(Vector2 location,
                                                    Texture2D texture,
                                                    Vector2 velocity,
                                                    float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   SpriteCreators.spriteSourceRectangles["cannon_0000_Layer-7"],
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.PhysicsBody.CollidesWith = Category.None;

            sprite.Rotation = rotation;

            return sprite;
        }
        public static Sprite CreatePowerMeterBar(Vector2 location,
                                                     Texture2D texture,
                                                     Vector2 velocity,
                                                     float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   new Rectangle(0, 914, 320, 50),
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.PhysicsBody.CollidesWith = Category.None;

            sprite.Rotation = rotation;

            return sprite;
        }
        public static Sprite CreatePowerMeterTab(Vector2 location,
                                                     Texture2D texture,
                                                     Vector2 velocity,
                                                     float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   new Rectangle(0, 966, 18, 45),
                                   velocity,
                                   BodyType.Static,
                                   true);

            sprite.PhysicsBody.CollidesWith = Category.None;

            sprite.Rotation = rotation;

            return sprite;
        }

        public static Sprite CreateCloudHelper(Rectangle source, Vector2 location,
                                                     Texture2D texture,
                                                     Vector2 velocity,
                                                     float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   source,
                                   velocity,
                                   BodyType.Dynamic,
                                   true);

            sprite.PhysicsBody.IgnoreGravity = true;
            sprite.PhysicsBody.CollidesWith = Category.None;
            sprite.Rotation = rotation;

            return sprite;
        }

        public static Sprite GenericCreator(Rectangle source, Vector2 location,
                                                     Texture2D texture,
                                                     Vector2 velocity,
                                                     float rotation)
        {
            Sprite sprite = new Sprite("sprite",
                                   location,
                                   texture,
                                   source,
                                   velocity,
                                   BodyType.Dynamic,
                                   true);

            sprite.PhysicsBody.IgnoreGravity = true;
            sprite.PhysicsBody.CollidesWith = Category.None;
            sprite.Rotation = rotation;

            return sprite;
        }

        public static Sprite CreateYouWin(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            return GenericCreator(SpriteCreators.spriteSourceRectangles["score_Winner"], location, texture, velocity, rotation);
        }

        public static Sprite CreateYouLose(Vector2 location,Texture2D texture,Vector2 velocity,float rotation)
        {
            return GenericCreator(SpriteCreators.spriteSourceRectangles["score_YouLose"], location, texture, velocity, rotation);
        }

        public static Sprite CreatePlus50(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            Sprite sprite = GenericCreator(SpriteCreators.spriteSourceRectangles["score_50points"], location, texture, velocity, rotation);
            sprite.Location -= new Vector2(sprite.BoundingBoxRect.Width / 2, sprite.BoundingBoxRect.Height);
            return sprite;
        }

        public static Sprite CreatePlus100(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            Sprite sprite = GenericCreator(SpriteCreators.spriteSourceRectangles["score_100points"], location, texture, velocity, rotation);
            sprite.Location -= new Vector2(sprite.BoundingBoxRect.Width / 2, sprite.BoundingBoxRect.Height);
            return sprite;
        }

        public static Sprite CreatePlus250(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            Sprite sprite = GenericCreator(SpriteCreators.spriteSourceRectangles["score_250points"], location, texture, velocity, rotation);
            sprite.Location -= new Vector2(sprite.BoundingBoxRect.Width / 2, sprite.BoundingBoxRect.Height);
            return sprite;
        }

        public static Sprite CreateCloud1(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            return CreateCloudHelper(SpriteCreators.spriteSourceRectangles["cloud1"], location, texture, velocity, rotation);
        }

        public static Sprite CreateCloud2(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            return CreateCloudHelper(SpriteCreators.spriteSourceRectangles["cloud2"], location, texture, velocity, rotation);
        }

        public static Sprite CreateCloud3(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            return CreateCloudHelper(SpriteCreators.spriteSourceRectangles["cloud3"], location, texture, velocity, rotation);
        }

        public static Sprite CreateCloud4(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            return CreateCloudHelper(SpriteCreators.spriteSourceRectangles["cloud4"], location, texture, velocity, rotation);
        }

        public static Sprite CreateCloud5(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            return CreateCloudHelper(SpriteCreators.spriteSourceRectangles["cloud5"], location, texture, velocity, rotation);
        }

        public static Sprite CreateCloud6(Vector2 location, Texture2D texture, Vector2 velocity, float rotation)
        {
            return CreateCloudHelper(SpriteCreators.spriteSourceRectangles["cloud6"], location, texture, velocity, rotation);
        }

    }
}
