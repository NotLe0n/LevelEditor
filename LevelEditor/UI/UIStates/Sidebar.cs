using LevelEditor.GameObjects;
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
                Main.selectedFile = openFileDialog.FileName;
                Main.level = new Level(Main.selectedFile);

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
                CreateSpawnPointMenu();

                // Event
                CreateEventMenu();

                // Enemy
                CreateEnemyMenu();

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
        private void CreateSpawnPointMenu()
        {
            var spawnPointInfo = new UIPanel(new StyleDimension(300, 0), new StyleDimension(800, 0), Color.Pink);
            spawnPointInfo.Y.Pixels = 150;
            spawnPointInfo.Display = Display.None;
            selectPanel.Append(spawnPointInfo);

            // Toggle Display
            Main.level.spawnPoint.OnClick += (evt, elm) => spawnPointInfo.Display = Display.Visible;
            Main.level.spawnPoint.OnClickAway += (evt, elm) => spawnPointInfo.Display = Display.None;

            // Settings
            var spXPos = new UIInput<int>("X Position", 200, 50, Color.LightSkyBlue, Color.Black);
            spXPos.X.Percent = 35;
            spXPos.Y.Pixels = 50;
            spXPos.TextChanged += (evt, elm) =>
            {
                if (int.TryParse(spXPos.Input.Text, out int result))
                    Main.level.spawnPoint.Position.X = result;
            };
            spawnPointInfo.Append(spXPos);

            var spYPos = new UIInput<int>("Y Position", 200, 50, Color.LightSkyBlue, Color.Black);
            spYPos.X.Percent = 35;
            spYPos.Y.Pixels = 100;
            spYPos.TextChanged += (evt, elm) =>
            {
                if (int.TryParse(spYPos.Input.Text, out int result))
                    Main.level.spawnPoint.Position.Y = result;
            };
            spawnPointInfo.Append(spYPos);
        }
        private void CreateEventMenu()
        {
            EventTrigger selectedEvent = null;

            var eventInfo = new UIPanel(new StyleDimension(300, 0), new StyleDimension(800, 0), Color.LightBlue);
            eventInfo.Y.Pixels = 150;
            eventInfo.Display = Display.None;
            selectPanel.Append(eventInfo);

            // Toggle Display
            for (int i = 0; i < Main.level.EventTriggers.Count; i++)
            {
                Main.level.EventTriggers[i].OnClick += (evt, elm) =>
                {
                    eventInfo.Display = Display.Visible;
                    selectedEvent = (EventTrigger)elm;

                    selectedEvent.OnClickAway += (evt, elm) =>
                    {
                        eventInfo.Display = Display.None;
                        selectedEvent = null;
                    };
                };
            }

            // Settings
            var evtID = new UIInput<int>("ID", 200, 50, Color.LightSkyBlue, Color.Black);
            evtID.X.Percent = 35;
            evtID.Y.Pixels = 50;
            evtID.TextChanged += (evt, elm) =>
            {
                if (int.TryParse(evtID.Input.Text, out int result))
                    selectedEvent.ID = result;
            };
            eventInfo.Append(evtID);

            var evtXPos = new UIInput<int>("X Position", 200, 50, Color.LightSkyBlue, Color.Black);
            evtXPos.X.Percent = 35;
            evtXPos.Y.Pixels = 100;
            evtXPos.TextChanged += (evt, elm) =>
            {
                if (int.TryParse(evtXPos.Input.Text, out int result))
                    selectedEvent.Bounds.X = result;
            };
            eventInfo.Append(evtXPos);

            var evtYPos = new UIInput<int>("Y Position", 200, 50, Color.LightSkyBlue, Color.Black);
            evtYPos.X.Percent = 35;
            evtYPos.Y.Pixels = 150;
            evtYPos.TextChanged += (evt, elm) =>
            {
                if (int.TryParse(evtYPos.Input.Text, out int result))
                    selectedEvent.Bounds.Y = result;
            };
            eventInfo.Append(evtYPos);

            var evtWidth = new UIInput<int>("Width", 200, 50, Color.LightSkyBlue, Color.Black);
            evtWidth.X.Percent = 35;
            evtWidth.Y.Pixels = 200;
            evtWidth.TextChanged += (evt, elm) =>
            {
                if (int.TryParse(evtWidth.Input.Text, out int result))
                    selectedEvent.Bounds.Width = result;
            };
            eventInfo.Append(evtWidth);

            var evtHeight = new UIInput<int>("Height", 200, 50, Color.LightSkyBlue, Color.Black);
            evtHeight.X.Percent = 35;
            evtHeight.Y.Pixels = 250;
            evtHeight.TextChanged += (evt, elm) =>
            {
                if (int.TryParse(evtHeight.Input.Text, out int result))
                    selectedEvent.Bounds.Height = result;
            };
            eventInfo.Append(evtHeight);

            var evtParams = new UIInput<string>("Parameters", 200, 50, Color.LightSkyBlue, Color.Black);
            evtParams.X.Percent = 35;
            evtParams.Y.Pixels = 300;
            evtParams.TextChanged += (evt, elm) => selectedEvent.Parameters = evtParams.Input.Text.Split(' ');
            eventInfo.Append(evtParams);
        }
        private void CreateEnemyMenu()
        {
            Enemy selectedEnemy = null;

            var enemyInfo = new UIPanel(new StyleDimension(300, 0), new StyleDimension(800, 0), Color.LightYellow);
            enemyInfo.Y.Pixels = 150;
            enemyInfo.Display = Display.None;
            selectPanel.Append(enemyInfo);

            // Toggle Display
            for (int i = 0; i < Main.level.Enemies.Count; i++)
            {
                Main.level.Enemies[i].OnClick += (evt, elm) =>
                {
                    enemyInfo.Display = Display.Visible;
                    selectedEnemy = (Enemy)elm;

                    selectedEnemy.OnClickAway += (evt, elm) =>
                    {
                        enemyInfo.Display = Display.None;
                        selectedEnemy = null;
                    };
                };
            }

            // Settings
            var enemyID = new UIInput<int>("ID", 200, 50, Color.LightSkyBlue, Color.Black);
            enemyID.X.Percent = 35;
            enemyID.Y.Pixels = 50;
            enemyID.TextChanged += (evt, elm) =>
            {
                if (int.TryParse(enemyID.Input.Text, out int result))
                    selectedEnemy.ID = result;
            };
            enemyInfo.Append(enemyID);

            var enemyXPos = new UIInput<int>("X Position", 200, 50, Color.LightSkyBlue, Color.Black);
            enemyXPos.X.Percent = 35;
            enemyXPos.Y.Pixels = 100;
            enemyXPos.TextChanged += (evt, elm) =>
            {
                if (int.TryParse(enemyXPos.Input.Text, out int result))
                    selectedEnemy.Bounds.X = result;
            };
            enemyInfo.Append(enemyXPos);

            var enemyYPos = new UIInput<int>("Y Position", 200, 50, Color.LightSkyBlue, Color.Black);
            enemyYPos.X.Percent = 35;
            enemyYPos.Y.Pixels = 150;
            enemyYPos.TextChanged += (evt, elm) =>
            {
                if (int.TryParse(enemyYPos.Input.Text, out int result))
                    selectedEnemy.Bounds.Y = result;
            };
            enemyInfo.Append(enemyYPos);

            var enemyParams = new UIInput<string>("Parameters", 200, 50, Color.LightSkyBlue, Color.Black);
            enemyParams.X.Percent = 35;
            enemyParams.Y.Pixels = 200;
            enemyParams.TextChanged += (evt, elm) => selectedEnemy.Parameters = enemyParams.Input.Text.Split(' ');
            enemyInfo.Append(enemyParams);
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
                for (int x = 0; x < Main.level.width; x++)
                {
                    for (int y = 0; y < Main.level.height; y++)
                    {
                        try
                        {
                            Main.level.tiles[x, y].texture = Main.textureMap.textures[Main.level.tiles[x, y].TileID];
                        }
                        catch
                        {
                            // if the texture has an invalid ID
                            Main.level.tiles[x, y].TileID = 0;
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
