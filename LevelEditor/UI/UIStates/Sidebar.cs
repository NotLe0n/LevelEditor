using LevelEditor.UI.UIElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace LevelEditor.UI.UIStates
{
    class Sidebar : UIState
    {
        // ui
        private UIPanel panel;
        private UIPanel drawPanel;
        private UIPanel selectPanel;
        private UIButton inputBtn;
        private List<UIButton> materialList = new List<UIButton>();
        private UIButton saveBtn;
        private UIImage texmapImg;

        public override void Initialize()
        {
            panel = new UIPanel(new StyleDimension(300, 0), new StyleDimension(0, 0), new Color(33, 33, 33));
            panel.Height.Percent = 100;
            panel.Y.Pixels = -10;
            Append(panel);

            selectPanel = new UIPanel(new StyleDimension(300, 0), new StyleDimension(0, 1), new Color(44, 44, 44));
            selectPanel.Height.Percent = 100;
            panel.Append(selectPanel);

            drawPanel = new UIPanel(new StyleDimension(300, 0), new StyleDimension(0, 1), new Color(44, 44, 44));
            drawPanel.Height.Percent = 100;
            drawPanel.Display = Display.None;
            panel.Append(drawPanel);

            inputBtn = new UIButton(new UIText("Load tilemap", Color.White), 100, 30, Color.Blue);
            inputBtn.Padding = new Padding(5);
            inputBtn.X.Percent = 50;
            inputBtn.Y.Pixels = 10;
            inputBtn.OnClick += OpenFile;
            panel.Append(inputBtn);

            base.Initialize();
        }
        private void OpenFile(MouseState evt, UIElement elm)
        {
            using System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                InitialDirectory = Main.instance.Content.RootDirectory,
                Filter = "level files (*.level)|*.level",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Main.events.Clear();
                Main.selectedFile = openFileDialog.FileName;
                Main.tileMap = new TileMap(Main.selectedFile);

                var toolbtn0 = new UIButton(new UIText("Select tool", Color.Black), 100, 50, Color.LightGreen);
                toolbtn0.X.Percent = 25;
                toolbtn0.Y.Pixels = 70;
                toolbtn0.OnClick += (evt, elm) =>
                {
                    Main.tool = 0;
                    drawPanel.Display = Display.None;
                    selectPanel.Display = Display.Visible;
                };
                panel.Append(toolbtn0);

                // Spawnpoint
                var spawnPointInfo = new UIPanel(new StyleDimension(300, 0), new StyleDimension(800, 0), Color.Pink);
                spawnPointInfo.Y.Pixels = 150;
                spawnPointInfo.Display = Display.None;
                selectPanel.Append(spawnPointInfo);

                var spXPos = new UIInput<int>("X Position", 200, 50, Color.LightSkyBlue, Color.Black);
                spXPos.X.Percent = 35;
                spXPos.Y.Pixels = 50;
                spXPos.TextChanged += (evt, elm) =>
                {
                    if (int.TryParse(spXPos.Input.Text, out int result))
                        Main.spawnPoint.Position.X = result;
                };
                spawnPointInfo.Append(spXPos);

                var spYPos = new UIInput<int>("Y Position", 200, 50, Color.LightSkyBlue, Color.Black);
                spYPos.X.Percent = 35;
                spYPos.Y.Pixels = 100;
                spYPos.TextChanged += (evt, elm) =>
                {
                    if (int.TryParse(spYPos.Input.Text, out int result))
                        Main.spawnPoint.Position.Y = result;
                };
                spawnPointInfo.Append(spYPos);

                Main.spawnPoint.OnClick += (evt, elm) =>
                {
                    spawnPointInfo.Display = Display.Visible;
                };
                Main.spawnPoint.OnClickAway += (evt, elm) =>
                {
                    spawnPointInfo.Display = Display.None;
                };

                // Event
                Event selectedEvent = null;

                var EventInfo = new UIPanel(new StyleDimension(300, 0), new StyleDimension(800, 0), Color.LightBlue);
                EventInfo.Y.Pixels = 150;
                EventInfo.Display = Display.None;
                selectPanel.Append(EventInfo);

                var evtID = new UIInput<int>("ID", 200, 50, Color.LightSkyBlue, Color.Black);
                evtID.X.Percent = 35;
                evtID.Y.Pixels = 50;
                evtID.TextChanged += (evt, elm) =>
                {
                    if (int.TryParse(evtID.Input.Text, out int result))
                        selectedEvent.ID = result;
                };
                EventInfo.Append(evtID);

                var evtXPos = new UIInput<int>("X Position", 200, 50, Color.LightSkyBlue, Color.Black);
                evtXPos.X.Percent = 35;
                evtXPos.Y.Pixels = 100;
                evtXPos.TextChanged += (evt, elm) =>
                {
                    if (int.TryParse(evtXPos.Input.Text, out int result))
                        selectedEvent.Bounds.X = result;
                };
                EventInfo.Append(evtXPos);

                var evtYPos = new UIInput<int>("Y Position", 200, 50, Color.LightSkyBlue, Color.Black);
                evtYPos.X.Percent = 35;
                evtYPos.Y.Pixels = 150;
                evtYPos.TextChanged += (evt, elm) =>
                {
                    if (int.TryParse(evtYPos.Input.Text, out int result))
                        selectedEvent.Bounds.Y = result;
                };
                EventInfo.Append(evtYPos);

                var evtWidth = new UIInput<int>("Width", 200, 50, Color.LightSkyBlue, Color.Black);
                evtWidth.X.Percent = 35;
                evtWidth.Y.Pixels = 200;
                evtWidth.TextChanged += (evt, elm) =>
                {
                    if (int.TryParse(evtWidth.Input.Text, out int result))
                        selectedEvent.Bounds.Width = result;
                };
                EventInfo.Append(evtWidth);

                var evtHeight = new UIInput<int>("Height", 200, 50, Color.LightSkyBlue, Color.Black);
                evtHeight.X.Percent = 35;
                evtHeight.Y.Pixels = 250;
                evtHeight.TextChanged += (evt, elm) =>
                {
                    if (int.TryParse(evtHeight.Input.Text, out int result))
                        selectedEvent.Bounds.Height = result;
                };
                EventInfo.Append(evtHeight);

                var evtParams = new UIInput<string>("Parameters", 200, 50, Color.LightSkyBlue, Color.Black);
                evtParams.X.Percent = 35;
                evtParams.Y.Pixels = 300;
                evtParams.TextChanged += (evt, elm) => selectedEvent.Parameters = evtParams.Input.Text.Split(' ');
                EventInfo.Append(evtParams);

                for (int i = 0; i < Main.events.Count; i++)
                {
                    Main.events[i].OnClick += (evt, elm) =>
                    {
                        EventInfo.Display = Display.Visible;
                        selectedEvent = (Event)elm;
                    };
                    Main.events[i].OnClickAway += (evt, elm) =>
                    {
                        EventInfo.Display = Display.None;
                        selectedEvent = null;
                    };
                }

                var toolbtn1 = new UIButton(new UIText("Draw tool", Color.Black), 100, 50, Color.Pink);
                toolbtn1.X.Percent = 75;
                toolbtn1.Y.Pixels = 70;
                toolbtn1.OnClick += (evt, elm) =>
                {
                    Main.tool = 1;
                    drawPanel.Display = Display.Visible;
                    selectPanel.Display = Display.None;
                };
                panel.Append(toolbtn1);

                ReloadMaterialList();

                texmapImg = new UIImage(Main.textureMap.map, (int)(Main.textureMap.map.Width / 2.5f), (int)(Main.textureMap.map.Height / 2.5f));
                texmapImg.Y.Pixels = 70 * materialList.Count + 200;
                texmapImg.X.Pixels = 20;
                texmapImg.OnClick += OpenTexMap;
                drawPanel.Append(texmapImg);

                saveBtn = new UIButton(new UIText("Save File", Color.Black), 100, 30, Color.DarkGray);
                saveBtn.OnClick += Main.SaveFile;
                saveBtn.X.Percent = 50;
                saveBtn.Y.Percent = 95;
                panel.Append(saveBtn);
            }
        }
        private void OpenTexMap(MouseState evt, UIElement elm)
        {
            using System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                InitialDirectory = Main.instance.Content.RootDirectory,
                Filter = "Texmap files (*.texmap)|*.texmap",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = openFileDialog.FileName;
                Main.textureMap = new TextureMap(file);

                // change texture
                for (int x = 0; x < Main.tileMap.width; x++)
                {
                    for (int y = 0; y < Main.tileMap.height; y++)
                    {
                        try
                        {
                            Main.tileMap.tiles[x, y].texture = Main.textureMap.textures[Main.tileMap.tiles[x, y].TileID];
                        }
                        catch
                        {
                            // if the texture has an invalid ID
                            Main.tileMap.tiles[x, y].TileID = 0;
                        }
                    }
                }

                ReloadMaterialList();

                texmapImg.Remove();
                texmapImg = new UIImage(Main.textureMap.map, (int)(Main.textureMap.map.Width / 2.5f), (int)(Main.textureMap.map.Height / 2.5f));
                texmapImg.Y.Pixels = 70 * materialList.Count + 200;
                texmapImg.X.Pixels = 20;
                texmapImg.OnClick += OpenTexMap;
                panel.Append(texmapImg);
            }
        }
        private void ReloadMaterialList()
        {
            // Reset Material list
            for (int i = 0; i < materialList.Count; i++)
            {
                materialList[i].Remove();
            }
            materialList = new List<UIButton>();
            Main.selectedMaterial = 1;

            // Generate Material List
            int temp = 0;
            for (int i = 0; i < Main.textureMap.textures.Count; i++)
            {
                var btn = new UIButton(new UIText(i, Color.White), 150, 50, Color.DarkGray);
                temp += 70;
                btn.X.Percent = 50;
                btn.Y.Pixels = 100;
                btn.Y.Pixels += temp;
                btn.OnClick += (evt, elm) =>
                {
                    if (elm is UIButton button && int.TryParse(button.Text.Text, out int result))
                        Main.selectedMaterial = result;
                    else
                        Main.selectedMaterial = 0;
                };
                var matImg = new UIImage(Main.textureMap.textures[i], 32, 32);
                matImg.Y.Percent = 50;
                matImg.X.Percent = 25;
                btn.Append(matImg);
                materialList.Add(btn);
                drawPanel.Append(btn);
            }
        }
    }
}
