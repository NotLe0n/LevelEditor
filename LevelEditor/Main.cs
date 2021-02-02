using LevelEditor.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LevelEditor
{
    public class Main : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static Texture2D solid;
        public static SpriteFont font;
        public static Game instance;
        public static Rectangle screen {
            get
            {
                // Update Screen variable
                var sex = System.Windows.Forms.Control.FromHandle(instance.Window.Handle).Bounds;
                return new Rectangle(sex.X, sex.Y, sex.Width, sex.Height);
            }
        }
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
        public static bool mouseOverUI => UIElement.Elements.Exists(x => x.isMouseOver);

        // editor
        public static string selectedFile;
        public static TileMap tileMap;
        public static TextureMap textureMap;
        public static float zoom = 1;
        public static int selectedMaterial = 1;
        // ui
        private UIPanel panel;
        private UIButton inputBtn;
        private List<UIButton> materialList = new List<UIButton>();
        private UIButton saveBtn;
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
        }

        protected override void Initialize()
        {
            panel = new UIPanel(300, screen.Height, new Color(33, 33, 33));
            inputBtn = new UIButton(new UIText("Load tile", Color.White), 100, 30, Color.Blue);
            inputBtn.Click += (elm) =>
            {
                materialList = new List<UIButton>();
                selectedMaterial = 1;
                if (OpenFile())
                {
                    int temp = 0;
                    for (int i = 0; i < textureMap.textures.Count; i++)
                    {
                        var btn = new UIButton(new UIText(i, Color.White), 100, 50, Color.DarkGray);
                        temp += 70;
                        btn.absolutePos = new Vector2(0, temp);
                        btn.Click += (elm) =>
                        {
                            if (elm is UIButton button && int.TryParse(button.Text.Text, out int result))
                                selectedMaterial = result;
                            else
                                selectedMaterial = 0;
                        };
                        materialList.Add(btn);
                    }

                    saveBtn = new UIButton(new UIText("Save File", Color.White), 100, 30, Color.DarkGray);
                    saveBtn.absolutePos = new Vector2(50, screen.Height - 100);
                    saveBtn.Click += SaveFile;
                    panel.Append(saveBtn);
                    panel.Append(inputBtn);
                }
            };
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            solid = Content.Load<Texture2D>("solid");
            font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            UpdateInput();
            zoom -= scrollwheel;
            for (int i = 0; i < UIElement.Elements.Count; i++)
            {
                UIElement.Elements[i].Update(gameTime);
            }

            if (tileMap != null)
            {
                for (int x = 0; x < tileMap.width; x++)
                {
                    for (int y = 0; y < tileMap.height; y++)
                    {
                        tileMap.tiles[x, y].Update();
                    }
                }

                if (mouse.MiddleButton == ButtonState.Pressed)
                {
                    for (int x = 0; x < tileMap.width; x++)
                    {
                        for (int y = 0; y < tileMap.height; y++)
                        {
                            tileMap.tiles[x, y].Position -= mousedelta / zoom;
                        }
                    }
                }

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix transform = Matrix.CreateScale(new Vector3(zoom, zoom, 1));
            if (tileMap != null)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, null, null, null, null, transform);
                for (int x = 0; x < tileMap.width; x++)
                {
                    for (int y = 0; y < tileMap.height; y++)
                    {
                        tileMap.tiles[x, y].Draw();
                    }
                }
                spriteBatch.End();
            }

            spriteBatch.Begin();
            for (int i = 0; i < UIElement.Elements.Count; i++)
            {
                UIElement.Elements[i].Draw(spriteBatch);
            }
            for (int i = 0; i < materialList.Count; i++)
            {
                spriteBatch.Draw(textureMap.textures[i], new Rectangle(materialList[i].absolutePos.ToPoint() + new Point(10, materialList[i].Height / 4), new Point(32, 32)), Color.White);
            }
            spriteBatch.End();
            panel.Height = screen.Height;

            base.Draw(gameTime);
        }

        private bool OpenFile()
        {
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Content.RootDirectory;
                openFileDialog.Filter = "level files (*.level)|*.level";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedFile = openFileDialog.FileName;
                    tileMap = new TileMap(selectedFile);
                    return true;
                }
            }
            return false;
        }
        private void SaveFile(UIElement elm)
        {
            string newFile = Path.GetFileName(textureMap.filePath) + "\nmap:\n";
            for (int y = 0; y < tileMap.height; y++)
            {
                for (int x = 0; x < tileMap.width; x++)
                {
                    newFile += tileMap.tiles[x, y].TileID;
                }
                newFile += "\n";
            }
            Stream fileStream;

            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = "level files (*.level)|*.level";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if ((fileStream = saveFileDialog1.OpenFile()) != null)
                {
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
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                return Texture2D.FromStream(graphics.GraphicsDevice, fileStream);
            }
        }
        /// <summary>
        /// Loads a part of a Texture from path defined by a sourceRectangle
        /// </summary>
        /// <param name="path">absolute file path</param>
        /// <param name="srcRect">source Rectangle which defines what Part of the Texture is loaded</param>
        /// <returns>A Texture2D containing the specified part</returns>
        public static Texture2D LoadTexturePart(string path, Rectangle srcRect)
        {
            using (FileStream fileStream = new FileStream(Path.GetDirectoryName(Path.GetDirectoryName(selectedFile)) + @"\Content\" + path + ".png", FileMode.Open))
            {
                Texture2D wholeTex = Texture2D.FromStream(graphics.GraphicsDevice, fileStream);
                Texture2D returnTex = new Texture2D(instance.GraphicsDevice, srcRect.Width, srcRect.Height);
                Color[] data = new Color[srcRect.Width * srcRect.Height];
                wholeTex.GetData(0, srcRect, data, 0, data.Length);
                returnTex.SetData(data);
                return returnTex;
            }
        }
    }
}
