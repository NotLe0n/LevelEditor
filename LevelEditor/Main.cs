using LevelEditor.UI;
using LevelEditor.UI.UIStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace LevelEditor
{
    public class Main : Game
    {
        // engine stuff
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static Texture2D solid;
        public static Texture2D panel;
        public static SpriteFont font;
        public static Game instance;
        public static Rectangle Screen
        {
            get
            {
                // Update Screen variable
                var sex = System.Windows.Forms.Control.FromHandle(instance.Window.Handle).Bounds;
                return new Rectangle(sex.X, sex.Y, sex.Width, sex.Height);
            }
        }
        public static string? MouseText;
        public static List<UIState> UIStates = new List<UIState>();

        // input stuff
        public static MouseState mouse = Mouse.GetState();
        public static MouseState lastmouse;
        public static KeyboardState keyboard;
        public static KeyboardState lastKeyboard;
        public static bool LeftHeld;
        public static bool RightHeld;
        public static bool LeftReleased;
        public static bool RightReleased;
        public static bool LeftClick;
        public static bool RightClick;
        public static bool mouseMoved;
        public static float scrollwheel;
        public static Vector2 mousedelta;
        public static bool MouseOverUI => UIStates.Exists(x => x.elements.Exists(x => x.IsMouseHovering));
        public static byte tool;

        // editor
        public static string selectedFile;
        public static TileMap tileMap;
        public static TextureMap textureMap;
        public static float zoom = 1;
        public static Vector2 camPos;
        public static int selectedMaterial = 1;
        public static Matrix UIScaleMatrix => Matrix.CreateScale(1f);
        public static float UIScale = 1f;
        public static Spawnpoint spawnPoint;
        public static List<Event> events = new List<Event>();
        public static Vector2 MousePos;

        // ui
        private UIState sidebar;
        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            instance = this;
            Window.AllowUserResizing = true;

            // Maximize Window
            System.Windows.Forms.Form form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            sidebar = new Sidebar();
            UIStates.Add(sidebar);
        }
        protected override void Initialize()
        {
            for (int i = 0; i < UIStates.Count; i++)
            {
                UIStates[i].Initialize();
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            solid = Content.Load<Texture2D>("solid");
            font = Content.Load<SpriteFont>("font");
            panel = Content.Load<Texture2D>("panel");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update Mouse and Keyboard Variables
            UpdateInput();

            if (tileMap != null)
            {
                // Update Zoom
                zoom -= scrollwheel;

                // Update Camera Position
                if (mouse.MiddleButton == ButtonState.Pressed)
                {
                    camPos -= mousedelta;
                }
                MousePos = mouse.Position.ToVector2() / zoom - camPos / zoom;
                for (int x = 0; x < tileMap.width; x++)
                {
                    for (int y = 0; y < tileMap.height; y++)
                    {
                        tileMap.tiles[x, y].Update();
                    }
                }

                // Update Game Objects
                for (int i = 0; i < GameObject.objects.Count; i++)
                {
                    GameObject.objects[i].Update();
                }
            }

            // Update UI
            for (int i = 0; i < UIStates.Count; i++)
            {
                UIStates[i].UpdateSelf(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            // Update Transform matrix
            Matrix transform = Matrix.CreateScale(new Vector3(zoom, zoom, 1));
            transform.Translation = new Vector3(camPos, 1);

            if (tileMap != null)
            {
                // Apply Transform matrix
                // Everything in here can be moved and zoomed by the mouse
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, null, null, null, null, transform);
                {
                    // Draw background
                    spriteBatch.Draw(solid, new Rectangle(tileMap.tiles[0, 0].Position.ToPoint(), new Point(tileMap.width * 50, tileMap.height * 50)), Color.Black * 0.5f);

                    // Draw tiles
                    for (int x = 0; x < tileMap.width; x++)
                    {
                        for (int y = 0; y < tileMap.height; y++)
                        {
                            tileMap.tiles[x, y].Draw();
                        }
                    }

                    // Draw GameObjects
                    for (int i = 0; i < GameObject.objects.Count; i++)
                    {
                        GameObject.objects[i].Draw();
                    }
                }
                spriteBatch.End();
            }

            // UI
            spriteBatch.Begin();
            {
                for (int i = 0; i < UIStates.Count; i++)
                {
                    UIStates[i].DrawSelf(spriteBatch);
                }

                if (MouseText != null)
                {
                    spriteBatch.DrawString(font, MouseText, mouse.Position.ToVector2() + new Vector2(10), Color.White);
                    MouseText = null;
                }
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Encodes data into .level file
        /// </summary>
        public static void SaveFile(MouseState evt, UIElement elm)
        {
            // encode spawnPoint
            string newFile = $"{spawnPoint}\n";

            // encode events
            newFile += $"events:\n";
            for (int i = 0; i < events.Count; i++)
            {
                newFile += $"{events[i]}\n";
            }

            // encode enemies
            newFile += $"enemies:\n";

            // encode texturemap
            newFile += $"map:\n{Path.GetFileName(textureMap.filePath)}\n";

            // encode tilemap
            for (int y = 0; y < tileMap.height; y++)
            {
                for (int x = 0; x < tileMap.width; x++)
                {
                    newFile += tileMap.tiles[x, y].TileID;
                }
                newFile += "\n";
            }

            Stream fileStream;

            // Configure save file dialog
            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "level files (*.level)|*.level",
                RestoreDirectory = true
            };

            // Open save file dialog
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if ((fileStream = saveFileDialog1.OpenFile()) != null)
                {
                    // save file
                    using var sr = new StreamWriter(fileStream);
                    sr.WriteLine(newFile.Trim());
                }
            }
        }

        /// <summary>
        /// Updates input variables
        /// </summary>
        private void UpdateInput()
        {
            lastmouse = mouse;
            mouse = Mouse.GetState();
            lastKeyboard = keyboard;
            keyboard = Keyboard.GetState();
            mouseMoved = mouse.Position != lastmouse.Position;
            mousedelta = (lastmouse.Position - mouse.Position).ToVector2();
            scrollwheel = (lastmouse.ScrollWheelValue - mouse.ScrollWheelValue) / 8000f;

            LeftHeld = mouse.LeftButton == ButtonState.Pressed;
            RightHeld = mouse.RightButton == ButtonState.Pressed;
            LeftReleased = mouse.LeftButton == ButtonState.Released;
            RightReleased = mouse.RightButton == ButtonState.Released;
            LeftClick = LeftReleased && lastmouse.LeftButton == ButtonState.Pressed;
            RightClick = RightReleased && lastmouse.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Loads texture from file
        /// </summary>
        /// <param name="path">absolute file path</param>
        /// <returns>the loaded Texture</returns>
        public static Texture2D LoadTexture(string path)
        {
            using FileStream fileStream = new FileStream(Path.GetDirectoryName(Path.GetDirectoryName(selectedFile)) + @"\Content\" + path + ".png", FileMode.Open);
            return Texture2D.FromStream(graphics.GraphicsDevice, fileStream);
        }
        /// <summary>
        /// Loads a part of a Texture from path defined by a sourceRectangle
        /// </summary>
        /// <param name="path">absolute file path</param>
        /// <param name="srcRect">source Rectangle which defines what Part of the Texture is loaded</param>
        /// <returns>A Texture2D containing the specified part</returns>
        public static Texture2D LoadTexturePart(string path, Rectangle srcRect)
        {
            using FileStream fileStream = new FileStream(Path.GetDirectoryName(Path.GetDirectoryName(selectedFile)) + @"\Content\" + path + ".png", FileMode.Open);

            Texture2D wholeTex = Texture2D.FromStream(graphics.GraphicsDevice, fileStream);
            Texture2D returnTex = new Texture2D(instance.GraphicsDevice, srcRect.Width, srcRect.Height);
            Color[] data = new Color[srcRect.Width * srcRect.Height];
            wholeTex.GetData(0, srcRect, data, 0, data.Length);
            returnTex.SetData(data);
            return returnTex;
        }
    }
}
