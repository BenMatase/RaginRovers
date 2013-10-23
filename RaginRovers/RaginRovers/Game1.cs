using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RaginRoversLibrary;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics.Contacts;
using System.IO;
using RaginRovers;

namespace RaginRovers
{

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        public enum CannonState
        {
            ROTATE,
            POWER,
            SHOOT,
            COOLDOWN
        }


        CannonState cannonState = CannonState.ROTATE;

        List<CannonGroups> cannonGroups;


        // test
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera camera;

        CannonManager cannonManager;

        GameObjectFactory factory;
        TextureManager textureManager;
        
        bool EditMode = false;
        bool KeyDown = false, MouseDown = false;
        Keys Key = Keys.None;

        int groupofinterest = 2;

        int DragSprite = -1; // Which sprite are we dragging around
        Vector2 DragOffset = Vector2.Zero;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 727;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GameWorld.Initialize(0, 5700, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height, new Vector2(0, 9.8f));
            GameWorld.ViewPortXOffset = 0;

            SpriteCreators.Load("Content\\spritesheet.txt");

            // Create the texture manager
            textureManager = new TextureManager(Content);

            // Now load the sprite creator factory helper
            factory = GameObjectFactory.Instance;
            factory.Initialize(textureManager);

            cannonManager = new CannonManager();
            cannonGroups = new List<CannonGroups>();

            // Add a few sprite creators
            factory.AddCreator((int)GameObjectTypes.CAT, SpriteCreators.CreateCat);
            factory.AddCreator((int)GameObjectTypes.DOG, SpriteCreators.CreateDog);
            factory.AddCreator((int)GameObjectTypes.BOOM, SpriteCreators.CreateBoom);
            factory.AddCreator((int)GameObjectTypes.WOOD1, SpriteCreators.CreateWood1);
            factory.AddCreator((int)GameObjectTypes.WOOD2, SpriteCreators.CreateWood2);
            factory.AddCreator((int)GameObjectTypes.WOOD3, SpriteCreators.CreateWood3);
            factory.AddCreator((int)GameObjectTypes.WOOD4, SpriteCreators.CreateWood4);
            factory.AddCreator((int)GameObjectTypes.PLATFORM_LEFT, SpriteCreators.CreatePlatformLeft);
            factory.AddCreator((int)GameObjectTypes.PLATFORM_MIDDLE, SpriteCreators.CreatePlatformMiddle);
            factory.AddCreator((int)GameObjectTypes.PLATFORM_RIGHT, SpriteCreators.CreatePlatformRight);
            factory.AddCreator((int)GameObjectTypes.CANNON, SpriteCreators.CreateCannon);
            factory.AddCreator((int)GameObjectTypes.CANNONWHEEL, SpriteCreators.CreateCannonWheel);
            factory.AddCreator((int)GameObjectTypes.POWERMETERBAR, SpriteCreators.CreatePowerMeterBar);
            factory.AddCreator((int)GameObjectTypes.POWERMETERTAB, SpriteCreators.CreatePowerMeterTab);



            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new Camera(new Viewport(GameWorld.ViewPortXOffset, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height));
            camera.Origin = new Vector2(camera.ViewPort.Width / 2.0f, camera.ViewPort.Height);


            // Load all the textures we're going to need for this game
            textureManager.LoadTexture("background");
            textureManager.LoadTexture("spritesheet");
            textureManager.LoadTexture("boom");
            textureManager.LoadTexture("cursor");

            /*
            int cat = factory.Create((int)GameObjectTypes.CAT, Vector2.Zero, "spritesheet", Vector2.Zero, 0);
            int dog = factory.Create((int)GameObjectTypes.DOG, new Vector2(500, 50), "spritesheet", new Vector2(30, 0), 0);
            int boom2 = factory.Create((int)GameObjectTypes.BOOM, new Vector2(500, 50), "boom", new Vector2(30, 0), 0);
            

            factory.Objects[dog].sprite.PhysicsBody.Mass = 30;
            factory.Objects[dog].sprite.PhysicsBody.Restitution = 0.4f;

            factory.Objects[cat].sprite.PhysicsBody.Restitution = 0.8f;
            */

            Body body = BodyFactory.CreateBody(GameWorld.world);
            body.BodyType = BodyType.Static;
            body.Position = ConvertUnits.ToSimUnits(new Vector2(0, this.Window.ClientBounds.Height-105));

            //FixtureFactory.AttachRectangle((float)GameWorld.WorldWidth, 10, 1, new Vector2(0, ConvertUnits.ToDisplayUnits(this.Window.ClientBounds.Height-30)), body);
            FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(GameWorld.WorldWidth)*10, ConvertUnits.ToSimUnits(10), 1, Vector2.Zero, body);


            //create cannons
            cannonManager.CreateCannonStuff(factory, new Vector2(0, 500), camera, true, ref cannonGroups); //need how to figure out location
            cannonManager.CreateCannonStuff(factory, new Vector2(500, 500), camera, false, ref cannonGroups); //need how to figure out location

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected void DetectKeyPress(KeyboardState kb, Keys key)
        {
            if (kb.IsKeyDown(key))
            {
                Key = key;
                KeyDown = true;
            }    
        }

        

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (!EditMode)
                GameWorld.Update(gameTime);

            foreach (int key in factory.Objects.Keys)
            {
                factory.Objects[key].sprite.Update(gameTime);
            }

            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Right))
            {
                if (camera.Position.X < GameWorld.WorldWidth - this.Window.ClientBounds.Width)
                    camera.Position = new Vector2(camera.Position.X + 15, camera.Position.Y);
                    
            }
            if (kb.IsKeyDown(Keys.Left))
            {
                camera.Position = new Vector2(camera.Position.X - 15, camera.Position.Y);
                if (camera.Position.X < 0)
                    camera.Position = Vector2.Zero;

            }
            if (kb.IsKeyDown(Keys.Z))
            {
                if (camera.Zoom < 1)
                    camera.Zoom += 0.005f;
            }

            if (kb.IsKeyDown(Keys.X))
            {
                if (camera.Zoom > 0.3)
                    camera.Zoom -= 0.005f;
            }
            if (kb.IsKeyDown(Keys.P))
                camera.Zoom = 1;


            MouseState msState = Mouse.GetState();
            Vector2 ms = Vector2.Transform(new Vector2(msState.X, msState.Y), Matrix.Invert(camera.GetViewMatrix(Vector2.One)));
            this.Window.Title = "Local Mouse> X: " + msState.X + " Y: " + msState.Y + ", World Mouse> X: " + ms.X + " Y: " + ms.Y;

            DetectKeyPress(kb, Keys.OemTilde);
            DetectKeyPress(kb, Keys.D1);  // Record if this key is pressed
            DetectKeyPress(kb, Keys.D2);  // Record if this key is pressed
            DetectKeyPress(kb, Keys.D3);
            DetectKeyPress(kb, Keys.D4);
            DetectKeyPress(kb, Keys.D5);
            DetectKeyPress(kb, Keys.D6);
            DetectKeyPress(kb, Keys.D7);
            DetectKeyPress(kb, Keys.D8);
            DetectKeyPress(kb, Keys.D9);
            DetectKeyPress(kb, Keys.D0);
            DetectKeyPress(kb, Keys.P);
            DetectKeyPress(kb, Keys.R);
            DetectKeyPress(kb, Keys.B);
            DetectKeyPress(kb, Keys.Delete);
            DetectKeyPress(kb, Keys.M);
            DetectKeyPress(kb, Keys.L);
            DetectKeyPress(kb, Keys.Enter);
            DetectKeyPress(kb, Keys.Space);
            DetectKeyPress(kb, Keys.N);
            DetectKeyPress(kb, Keys.J);

            if (KeyDown)
            {
                if (kb.IsKeyUp(Key))
                {
                    switch (Key)
                    {
                        case Keys.Enter:

                            foreach (int i in factory.Objects.Keys)
                            {
                                if (factory.Objects[i].typeid == (int)GameObjectTypes.DOG)
                                    factory.Objects[i].sprite.PhysicsBody.ApplyLinearImpulse(new Vector2(60, 40));
                            }

                            break;
////////////////////////////////////////////////////
                        case Keys.Space:
                            if (!EditMode)
                            {
                                cannonManager.ChangeCannonState(cannonGroups[0]);
                                groupofinterest = 0;
                            }
                            break;

                        case Keys.N:
                            if (!EditMode)
                            {
                                cannonManager.ChangeCannonState(cannonGroups[1]);
                                groupofinterest = 1;
                            }
                            break;

                        case Keys.OemTilde:
                            EditMode = !EditMode;
                            //camera.Zoom = 1f;
                            this.Window.Title = "Ragin Rovers " + (EditMode ? " | EDITING MODE" : "");
                            break;

                        case Keys.D1:

                            if (EditMode)
                            {
                                int dog = factory.Create((int)GameObjectTypes.DOG, new Vector2((int)ms.X + camera.Position.X - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                                factory.Objects[dog].sprite.PhysicsBody.Mass = 30;
                                factory.Objects[dog].sprite.PhysicsBody.Restitution = 0.4f;
                            }

                            break;

                        case Keys.D2:

                            if (EditMode)
                            {
                                int cat = factory.Create((int)GameObjectTypes.CAT, new Vector2((int)ms.X + camera.Position.X - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                                factory.Objects[cat].sprite.PhysicsBody.Mass = 30;
                                factory.Objects[cat].sprite.PhysicsBody.Restitution = 0.8f;
                            }

                            break;

                        case Keys.D3:

                            if (EditMode)
                            {
                                int board = factory.Create((int)GameObjectTypes.WOOD1, new Vector2((int)ms.X + camera.Position.X - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;

                        case Keys.D4:

                            if (EditMode)
                            {
                                int board = factory.Create((int)GameObjectTypes.WOOD2, new Vector2((int)ms.X + camera.Position.X - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;

                        case Keys.D5:

                            if (EditMode)
                            {
                                int board = factory.Create((int)GameObjectTypes.WOOD3, new Vector2((int)ms.X + camera.Position.X - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;

                        case Keys.D6:

                            if (EditMode)
                            {
                                int board = factory.Create((int)GameObjectTypes.WOOD4, new Vector2((int)ms.X + camera.Position.X - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;

                        case Keys.D7:

                            if (EditMode)
                            {
                                factory.Create((int)GameObjectTypes.PLATFORM_LEFT, new Vector2((int)ms.X + camera.Position.X - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;

                        case Keys.D8:

                            if (EditMode)
                            {
                                factory.Create((int)GameObjectTypes.PLATFORM_MIDDLE, new Vector2((int)ms.X + camera.Position.X - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;

                        case Keys.D9:

                            if (EditMode)
                            {
                                factory.Create((int)GameObjectTypes.PLATFORM_RIGHT, new Vector2((int)ms.X + camera.Position.X - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;
                        case Keys.D0:

                            if (EditMode)
                            {
                                factory = cannonManager.CreateCannonStuff(factory, new Vector2(ms.X + camera.Position.X - 95, ms.Y - 80), camera, false, ref cannonGroups);
                                
                            }

                            break;

                        case Keys.P:

                            if (EditMode)
                            {
                                factory = cannonManager.CreateCannonStuff(factory, new Vector2(ms.X, ms.Y), camera, true, ref cannonGroups);

                            }

                            break;
                        case Keys.J:

                            cannonManager.CreateCannonStuff(factory, new Vector2(0, 500), camera, true, ref cannonGroups); //need how to figure out location
                            cannonManager.CreateCannonStuff(factory, new Vector2(500, 500), camera, false, ref cannonGroups); //need how to figure out location
                            break;

                        case Keys.B:

                            int boom = factory.Create((int)GameObjectTypes.BOOM, new Vector2((int)ms.X + camera.Position.X - 95, (int)ms.Y - 80), "boom", new Vector2(0, 0), 0, 0f, 0f);
                            
                            break;

                        case Keys.R:
                            
                            if (EditMode && MouseDown && DragSprite != -1)
                            {
                                if (factory.Objects[DragSprite].sprite.Rotation == 0)
                                    factory.Objects[DragSprite].sprite.Rotation = MathHelper.PiOver2;
                                else
                                    factory.Objects[DragSprite].sprite.Rotation = 0;
                            }
                            
                            break;

                        case Keys.Delete:

                            if (EditMode && MouseDown && DragSprite != -1)
                            {
                                factory.Remove(DragSprite);
                                DragSprite = -1;
                            }

                            break;

                        case Keys.L:

                            using (StreamReader infile = new StreamReader("map.txt"))
                            {
                                string objs = infile.ReadToEnd();
                                string[] lines = objs.Split('\n');

                                for (int i = 0; i < lines.Length; i++)
                                {
                                    if (lines[i].Length > 0)
                                    {
                                        string[] fields = lines[i].Split('\t');

                                        factory.Create(Convert.ToInt32(fields[1]),
                                                       new Vector2((float)Convert.ToDouble(fields[2]), (float)Convert.ToDouble(fields[3])),
                                                       fields[4],
                                                       Vector2.Zero,
                                                       (float)Convert.ToDouble(fields[5]),
                                                       0f,
                                                       0f);

                                    }
                                }
                            }

                            break;

                        case Keys.M:

                            string objlist = factory.Serialize();

                            using (StreamWriter outfile = new StreamWriter(@"map.txt"))
                            {
                                outfile.Write(objlist);
                            }

                            break;
                    }

                    KeyDown = false;
                    Key = Keys.None;
                }
            }

            if (!EditMode) 
            {
                for (int i = 0; i < cannonGroups.Count; i++)
                {
                    factory = cannonManager.ManipulateCannons(factory, cannonGroups[i]);
                }
            }

            cannonManager.Update(gameTime, factory, cannonGroups);

            if (EditMode)
            {
                if (msState.LeftButton == ButtonState.Pressed && !MouseDown)
                {
                    MouseDown = true;

                    foreach (int key in factory.Objects.Keys)
                    {
                        Sprite sprite = factory.Objects[key].sprite;
                        if (sprite.IsBoxColliding(new Rectangle((int)ms.X + (int)camera.Position.X, (int)ms.Y, 1, 1)))
                        {
                            DragSprite = key;
                            DragOffset = new Vector2(ms.X, ms.Y) - sprite.Location;
                        }
                    }
                }

                if (MouseDown && DragSprite != -1)
                {
                    factory.Objects[DragSprite].sprite.Location = new Vector2(ms.X, ms.Y) - DragOffset;
                }

                if (msState.LeftButton == ButtonState.Released)
                {
                    MouseDown = false;
                    DragSprite = -1;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(104, 179, 255, 255));

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.GetViewMatrix(Vector2.One));

            for (int x = -6; x < (GameWorld.WorldWidth / this.Window.ClientBounds.Width)+6; x++)
            {
                spriteBatch.Draw(textureManager.Texture("background"), new Rectangle(x * this.Window.ClientBounds.Width, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), new Rectangle(0, 0, textureManager.Texture("background").Width, textureManager.Texture("background").Height), Color.White);
            }

            foreach (int key in factory.SortedObjectsList)
            {
                factory.Objects[key].sprite.Draw(spriteBatch);
            }

            spriteBatch.End();

            
            if (EditMode)
            {
                spriteBatch.Begin();
                MouseState ms = Mouse.GetState();
                spriteBatch.Draw(textureManager.Texture("cursor"), new Vector2(ms.X, ms.Y), Color.White);
                spriteBatch.End();
            }
            

            base.Draw(gameTime);
        }
    }
}
