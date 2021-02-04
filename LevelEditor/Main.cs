using LevelEditor.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LevelEditor
{
    public class Main : Game
    {
        // engine stuff
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static Texture2D solid;
        public static SpriteFont font;
        public static Game instance;
        public static Rectangle screen
        {
            get
            {
                // Update Screen variable
                var sex = System.Windows.Forms.Control.FromHandle(instance.Window.Handle).Bounds;
                return new Rectangle(sex.X, sex.Y, sex.Width, sex.Height);
            }
        }
        public static string? MouseText;
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
        public static bool mouseOverUI => UIElement.Parents.Exists(x => x.IsMouseHovering);

        // editor
        public static string selectedFile;
        public static TileMap tileMap;
        public static TextureMap textureMap;
        public static float zoom = 1;
        public static int selectedMaterial = 1;
        public static Matrix UIScaleMatrix => Matrix.CreateScale(1f);
        public static float UIScale = 1f;
        // ui
        private UIPanel panel;
        private UIButton inputBtn;
        private List<UIButton> materialList = new List<UIButton>();
        private UIButton saveBtn;
        private UIImage texmapImg;
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

            panel = new UIPanel(new StyleDimension(300, 0), new StyleDimension(0, 1), new Color(33, 33, 33));
            inputBtn = new UIButton(new UIText("Load tile", Color.White), 100, 30, Color.Blue);
            panel.Append(inputBtn);
            inputBtn.Padding = new Padding(5);
            inputBtn.X.Percent = 50;
            inputBtn.Y.Pixels = 10;
            inputBtn.OnClick += OpenFile;
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
            for (int i = 0; i < UIElement.Parents.Count; i++)
            {
                UIElement.Parents[i].InternalUpdate(gameTime);
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
            GraphicsDevice.Clear(Color.Gray);

            Matrix transform = Matrix.CreateScale(new Vector3(zoom, zoom, 1));
            if (tileMap != null)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, null, null, null, null, transform);
                spriteBatch.Draw(solid, new Rectangle(tileMap.tiles[0, 0].Position.ToPoint(), new Point(tileMap.width * 50, tileMap.height * 50)), Color.Black * 0.5f);
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
            for (int i = 0; i < UIElement.Parents.Count; i++)
            {
                UIElement.Parents[i].InternalDraw(spriteBatch);
            }

            if (MouseText != null)
            {
                spriteBatch.DrawString(font, MouseText, (mouse.Position + new Point(10)).ToVector2(), Color.White);
                MouseText = null;
            }
            spriteBatch.End();
            panel.Height.Pixels = screen.Height;

            base.Draw(gameTime);
        }

        private void OpenFile(MouseState evt, UIElement elm)
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

                    ReloadMaterialList();

                    texmapImg = new UIImage(textureMap.map, (int)(textureMap.map.Width / 2.5f), (int)(textureMap.map.Height / 2.5f));
                    texmapImg.Y.Pixels = 70 * materialList.Count + 100;
                    texmapImg.X.Pixels = 20;
                    texmapImg.OnClick += OpenTexMap;
                    panel.Append(texmapImg);

                    saveBtn = new UIButton(new UIText("Save File", Color.White), 100, 30, Color.DarkGray);
                    saveBtn.OnClick += SaveFile;
                    saveBtn.X.Percent = 50;
                    saveBtn.Y.Percent = 95;
                    panel.Append(saveBtn);
                }
            }
        }

        private void OpenTexMap(MouseState evt, UIElement elm)
        {
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Content.RootDirectory;
                openFileDialog.Filter = "Texmap files (*.texmap)|*.texmap";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string file = openFileDialog.FileName;
                    textureMap = new TextureMap(file);
                    for (int x = 0; x < tileMap.width; x++)
                    {
                        for (int y = 0; y < tileMap.height; y++)
                        {
                            try
                            {
                                tileMap.tiles[x, y].texture = textureMap.textures[tileMap.tiles[x, y].TileID];
                            }
                            catch
                            {
                                tileMap.tiles[x, y].TileID = 0;
                            }
                        }
                    }

                    ReloadMaterialList();

                    texmapImg.Remove();
                    texmapImg = new UIImage(textureMap.map, (int)(textureMap.map.Width / 2.5f), (int)(textureMap.map.Height / 2.5f));
                    texmapImg.Y.Pixels = 70 * materialList.Count + 100;
                    texmapImg.X.Pixels = 20;
                    texmapImg.OnClick += OpenTexMap;
                    panel.Append(texmapImg);
                }
            }
        }

        private void SaveFile(MouseState evt, UIElement elm)
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
        private void ReloadMaterialList()
        {
            for(int i = 0; i < materialList.Count; i++)
            {
                materialList[i].Remove();
            }
            materialList = new List<UIButton>();
            selectedMaterial = 1;
            int temp = 0;
            for (int i = 0; i < textureMap.textures.Count; i++)
            {
                var btn = new UIButton(new UIText(i, Color.White), 150, 50, Color.DarkGray);
                temp += 70;
                btn.X.Percent = 50;
                btn.Y.Pixels = temp;
                btn.OnClick += (evt, elm) =>
                {
                    if (elm is UIButton button && int.TryParse(button.Text.Text, out int result))
                        selectedMaterial = result;
                    else
                        selectedMaterial = 0;
                };
                var matImg = new UIImage(textureMap.textures[i], 32, 32);
                matImg.Y.Percent = 50;
                matImg.X.Percent = 25;
                btn.Append(matImg);
                materialList.Add(btn);
                panel.Append(btn);
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
            using (FileStream fileStream = new FileStream(Path.GetDirectoryName(Path.GetDirectoryName(selectedFile)) + @"\Content\" + path + ".png", FileMode.Open))
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
