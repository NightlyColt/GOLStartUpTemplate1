using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;


namespace GOLStartUpTemplate1
{
    public partial class Form1 : Form
    {
        // size of the universe
        static int WIDTH = Properties.Settings.Default.UniverseWidth;
        static int HEIGHT = Properties.Settings.Default.UniverseHeight;

        // The universe array
        bool[,] universe = new bool[WIDTH, HEIGHT];
        bool[,] scratchPad = new bool[WIDTH, HEIGHT];

        // seed of universe
        int seed = new Random().Next(int.MinValue, int.MaxValue);

        // Alive Cells
        int alive = 0;

        // Drawing colors
        Color gridColor = Properties.Settings.Default.GridColor;
        Color gridx10Color = Properties.Settings.Default.Gridx10Color;
        Color cellColor = Properties.Settings.Default.CellColor;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            graphicsPanel1.BackColor = Properties.Settings.Default.BackGroundColor;
            timer.Interval = Properties.Settings.Default.TimeInterval; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // stop timer running
            // Setup the labels
            toolStripStatusLabelInterval.Text = "Interval: " + timer.Interval.ToString();
            seedLabel.Text = "Seed: " + seed.ToString();
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            // iterate through the y position in the universe
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // iterate through the x position in the universe
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    DecideNeighborCount(out int count, x, y);
                    // Apply the Rules
                    if (universe[x, y] == true)
                    {
                        if (count < 2) { scratchPad[x, y] = false; }
                        else if (count > 3) { scratchPad[x, y] = false; }
                        else if (count == 2 || count == 3) { scratchPad[x, y] = true; }
                    }
                    else
                    {
                        if (count == 3) { scratchPad[x, y] = true; }
                        else if (count != 3) { scratchPad[x, y] = false; }
                    }
                    

                    // Turn in/of the scratch Pad
                }
            }
            // copy the scratchPad into the universe
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;
            
            // Increment generation count
            generations++;

            // Update status strip generations and interval
            toolStripStatusLabelGenerations.Text = "Generations: " + generations.ToString();
            toolStripStatusLabelInterval.Text = "Interval: " + timer.Interval.ToString();

            // Calculate how many alive cells there are and update the alive count
            CountAlive();

            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        /// <summary>
        /// Counts the a live cells in a finite space (limited)
        /// </summary>
        /// <param name="x"></param> The x coordinate
        /// <param name="y"></param> the y coordinate
        /// <returns> the amount of neighbors surrounded by the cell that are alive</returns>
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;

            int xLen = universe.GetLength(0);

            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    // Calculate the position for x and y around a cell
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if (xOffset == 0 && yOffset == 0) { continue; }
                    else if (xCheck < 0) { continue; }
                    else if (yCheck < 0) { continue; }
                    else if (xCheck >= xLen) { continue; }
                    else if (yCheck >= yLen) { continue; }

                    if (universe[xCheck, yCheck] == true)
                    {
                        count++;
                    }

                }
            }

            return count;
        }

        /// <summary>
        /// Counts the a live cells in a toroidal space (warped)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns> the amount of neighbors surrounded by the cell that are alive</returns>
        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;

            int xLen = universe.GetLength(0);

            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if (xOffset == 0 && yOffset == 0) { continue; }
                    if (xCheck < 0) { xCheck = xLen - 1; }
                    if (yCheck < 0) { yCheck = yLen - 1; }
                    if (xCheck >= xLen) { xCheck = 0; }
                    if (yCheck >= yLen) { yCheck = 0; }

                    if (universe[xCheck, yCheck] == true)
                    {
                        count++;
                    }

                }
            }


            return count;
        }

        // Paints the panel
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);
            Pen gridx10Pen = new Pen(gridx10Color, 3);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Brush hudBrush = new SolidBrush(Color.Black);

            // The Font for the count of neighbors
            Font font = new Font("Arial", 10f);
            Font hudFont = new Font("Palatino Linotype", 13f);

            // Setup the format for the neighbors
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    if (MenuItemGrid.Checked)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }
                    DecideNeighborCount(out int neighbors, x, y);

                    // Draw the neighbor count on the cell
                    if (neighborCountToolStripMenuItem.Checked)
                    {
                        if (neighbors > 0)
                        {
                            // writes the number green if the cell has three neighbors and its dead while also filling the square
                            if (universe[x, y] == false && neighbors == 3)
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                            }
                            // writes the number green if the cell is alive and has 2 or 3 neighbors
                            else if (universe[x, y] == true && (neighbors == 2 || neighbors == 3))
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                            }

                            else
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                            }
                        }
                    }

                    // Draw the gridx10 line
                    if (MenuItemGrid.Checked)
                    {
                        if (x % 10 == 0 && x != 0)
                        {
                            Point startingPoint = new Point((int)(x * cellWidth), 0);
                            Point endPoint = new Point((int)(x * cellWidth), (int)((universe.GetLength(1) + 1) * cellHeight));

                            e.Graphics.DrawLine(gridx10Pen, startingPoint, endPoint);
                        }
                        if (y % 10 == 0 && y != 0)
                        {
                            Point startingPoint = new Point(0, (int)(y * cellHeight));
                            Point endPoint = new Point((int)((universe.GetLength(0) + 1) * cellWidth), (int)(y * cellHeight));

                            e.Graphics.DrawLine(gridx10Pen, startingPoint, endPoint);
                        }
                    }
                }
            }

            // Draw the the HUD
            if (MenuItemHUD.Checked)
            {
                Point point = new Point(0, (int)((universe.GetLength(1) - 5) * cellHeight));
                e.Graphics.DrawString($"Generations: {generations}", hudFont, hudBrush, point);

                point.Y = (int)((universe.GetLength(1) - 4) * cellHeight);
                e.Graphics.DrawString($"Cell Count: {alive}", hudFont, hudBrush, point);

                point.Y = (int)((universe.GetLength(1) - 3) * cellHeight);
                string boundryType = (finiteToolStripMenuItem.Checked) ? "Finite" : "Toroidal";
                e.Graphics.DrawString($"Boundry Type: {boundryType}", hudFont, hudBrush, point);

                point.Y = (int)((universe.GetLength(1) - 2) * cellHeight);
                e.Graphics.DrawString($"Universe Size: {"{Width=" + WIDTH + ", Height=" + HEIGHT + "}"} ", hudFont, hudBrush, point);

            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            gridx10Pen.Dispose();
            cellBrush.Dispose();
            hudBrush.Dispose();
        }

        // interacts with cells that are either alive or dead
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = (int)(e.X / cellWidth);
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = (int)(e.Y / cellHeight);

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Update the alive count
                CountAlive();
                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }
        #region MenuStrips
        // runs the game
        private void PlayButton_Click(object sender, EventArgs e)
        {
            timer.Start(); // start timer running
            PlayButton.Enabled = false;
            NextButton.Enabled = false;
            PauseButton.Enabled = true;
        }

        // pauses the game
        private void PauseButton_Click(object sender, EventArgs e)
        {
            timer.Stop(); // stop timer 
            PlayButton.Enabled = true;
            NextButton.Enabled = true;
            PauseButton.Enabled = false;
        }

        // goes through the next iteration from the current generation
        private void NextButton_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        // menu item to start the game
        private void MenuItemStart_Click(object sender, EventArgs e)
        {
            PlayButton_Click(sender, e);
            runToolStripMenuItem.Enabled = false;
            nextF7ToolStripMenuItem.Enabled = false;
            pauseF6ToolStripMenuItem.Enabled = true;
        }

        // menu item to pause the game
        private void MenuItemPause_Click(object sender, EventArgs e)
        {
            PauseButton_Click(sender, e);
            runToolStripMenuItem.Enabled = true;
            nextF7ToolStripMenuItem.Enabled = true;
            pauseF6ToolStripMenuItem.Enabled = false;
        }

        // menu item to the next iteration of the universe from the current generation
        private void MenuItemNext_Click(object sender, EventArgs e)
        {
            NextButton_Click(sender, e);
        }

        // iterates the generations by a given set number of times
        private void MenuItemTo_Click(object sender, EventArgs e)
        {
            Form2 dlg = new Form2();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                int generation = dlg.IntValue;

                while (generation != 0)
                {
                    NextGeneration();
                    generation--;
                }
            }

        }

        // A modal dialog to randomly make a seed and then randomly generate a universe
        private void MenuItemFromSeed_Click(object sender, EventArgs e)
        {
            SeedDialog dlg = new SeedDialog();

            dlg.RandomSeed = seed;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                seed = dlg.RandomSeed;
                RandomFill();
                CountAlive();
            }
        }

        // randomly generate a universe from the current seed
        private void MenuItemCurrentSeed_Click(object sender, EventArgs e)
        {
            RandomFill();
            CountAlive();
        }

        // randomly generate a universe based on time
        private void MenuItemFromTime_Click(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now; 

            int currentTime = time.Hour + time.Minute + time.Second + time.Millisecond;
            seed = currentTime;
            RandomFill(); // randomly fill the universe with the new seed
            CountAlive();
        }

        // enables or disables the HUD
        private void MenuItemHUD_Click(object sender, EventArgs e)
        {
            Toggle(MenuItemHUD);
            graphicsPanel1.Invalidate();
        }

        // enables or disables the neighbor count print
        private void MenuItemNeighborCount_Click(object sender, EventArgs e)
        {
            Toggle(neighborCountToolStripMenuItem);
            graphicsPanel1.Invalidate();
        }

        // enables or disables the grid from the panel
        private void MenuItemGrid_Click(object sender, EventArgs e)
        {
            Toggle(MenuItemGrid);
            graphicsPanel1.Invalidate();
        }

        // makes the universe toroidal (warped)
        private void MenuItemToroidal_Click(object sender, EventArgs e)
        {
            // toggle between finite and toroidal
            finiteToolStripMenuItem.Checked = false; 
            toroidalToolStripMenuItem.Checked = true;
            graphicsPanel1.Invalidate();

        }

        // makes the universe finite (limited)
        private void MenuItemFinite_Click(object sender, EventArgs e)
        {
            toroidalToolStripMenuItem.Checked = false;
            finiteToolStripMenuItem.Checked = true;
            graphicsPanel1.Invalidate();
        }

        // opens a color modal dialog to change the back color of the panel
        private void MenuItemBackColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = graphicsPanel1.BackColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
            }
        }

        // opens a color modal dialog to change the cell color
        private void MenuItemCellColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = cellColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        // opens a color modal dialog to change the grid color
        private void MenuItemGridColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        // opens a color modal dialog to change grid x 10 color
        private void MenuItemGridx10Color_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridx10Color;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridx10Color = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        // opens an option modal dialog to change the size and timer interval of the universe
        private void MenuItemOptions_Click(object sender, EventArgs e)
        {
            OptionDialog dlg = new OptionDialog();

            dlg.Interval = timer.Interval;
            dlg.UWidth = WIDTH;
            dlg.UHeight = HEIGHT;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                timer.Interval = dlg.Interval; 
                WIDTH = dlg.UWidth;
                HEIGHT = dlg.UHeight;
                universe = new bool[WIDTH, HEIGHT];
                scratchPad = new bool[WIDTH, HEIGHT];
                toolStripStatusLabelInterval.Text = "Interval: " + timer.Interval.ToString();

                graphicsPanel1.Invalidate();
            }
        }

        // resets the settings to the default value
        private void MenuItemReset_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            WIDTH = Properties.Settings.Default.UniverseWidth;
            HEIGHT = Properties.Settings.Default.UniverseHeight;
            timer.Interval = Properties.Settings.Default.TimeInterval;

            gridColor = Properties.Settings.Default.GridColor;
            gridx10Color = Properties.Settings.Default.Gridx10Color;
            cellColor = Properties.Settings.Default.CellColor;
            graphicsPanel1.BackColor = Properties.Settings.Default.BackGroundColor;

            universe = new bool[WIDTH, HEIGHT];
            scratchPad = new bool[WIDTH, HEIGHT];
            CountAlive(); // update the alive member variable
            graphicsPanel1.Invalidate();
        }

        // reloads the previous settings values
        private void MenuItemReload_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            WIDTH = Properties.Settings.Default.UniverseWidth;
            HEIGHT = Properties.Settings.Default.UniverseHeight;
            timer.Interval = Properties.Settings.Default.TimeInterval;

            gridColor = Properties.Settings.Default.GridColor;
            gridx10Color = Properties.Settings.Default.Gridx10Color;
            cellColor = Properties.Settings.Default.CellColor;
            graphicsPanel1.BackColor = Properties.Settings.Default.BackGroundColor;

            universe = new bool[WIDTH, HEIGHT];
            scratchPad = new bool[WIDTH, HEIGHT];
            CountAlive(); // update the alive member variable
            graphicsPanel1.Invalidate();
        }

        // creates a way to close the program
        private void MenuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // empties or clears the universe
        private void newToolStripButtonNew_Click(object sender, EventArgs e)
        {
            universe = new bool[WIDTH, HEIGHT];
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations: " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        // menu item way to empty the universe
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newToolStripButtonNew_Click(sender, e);
        }

        // opens a modal dialog box to open a file of cells
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                using (StreamReader reader = new StreamReader(dlg.FileName))
                {
                    int maxWidth = 0;
                    int maxHeight = 0;
                    while (!reader.EndOfStream)
                    {
                        string row = reader.ReadLine();
                        // skip if the row is a comment
                        if (row[0] == '!')
                        {
                            continue;
                        }
                        else
                        {
                            maxHeight++;
                            if (row.Length > maxWidth)
                            {
                                maxWidth = row.Length;
                            }
                        }
                    }
                    WIDTH = maxWidth;
                    HEIGHT = maxHeight;

                    universe = new bool[WIDTH, HEIGHT];
                    scratchPad = new bool[WIDTH, HEIGHT];

                    // Reset the file pointer back to the beginning of the file.
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    int y = 0;
                    while (!reader.EndOfStream)
                    {

                        string row = reader.ReadLine();
                        if (row[0] == '!')
                        {
                            continue;
                        }
                        else
                        {
                            for (int xPos = 0; xPos < row.Length; xPos++)
                            {
                                if (row[xPos] == 'O')
                                {
                                    universe[xPos, y] = true;
                                }
                                else
                                {
                                    universe[xPos, y] = false;
                                }
                            }
                            y++;
                        }

                    }
                }
            }
            graphicsPanel1.Invalidate();
        }

        // opens a modal dialog box to save the current file into a new file
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                using (StreamWriter writer = new StreamWriter(dlg.FileName))
                {
                    for (int y = 0; y < universe.GetLength(1); y++)
                    {
                        String currentRow = string.Empty;
                        for (int x = 0; x < universe.GetLength(0); x++)
                        {
                            if (universe[x, y] == true)
                            {
                                currentRow += "O";
                            }
                            else
                            {
                                currentRow += ".";
                            }
                        }
                        writer.WriteLine(currentRow);
                    }
                }
            }
            string tile = dlg.Title;
        }

        // menu item to open a file
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }

        // menu item to save a file
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);
        }

        // saves the current settings when the application is closed
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.UniverseWidth = WIDTH;
            Properties.Settings.Default.UniverseHeight = HEIGHT;
            Properties.Settings.Default.TimeInterval = timer.Interval;

            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.Gridx10Color = gridx10Color;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.BackGroundColor = graphicsPanel1.BackColor;

            Properties.Settings.Default.Save();
        }

        // Context menu
        // changing colors
        private void backColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MenuItemBackColor_Click(sender, e);
        }

        // context menu way of changing the cell color
        private void cellColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MenuItemCellColor_Click(sender, e);
        }

        // context menu way of changing the grid color
        private void gridColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MenuItemGridColor_Click(sender, e);
        }

        // context menu way of changing the grid x 10 color
        private void gridx10ColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MenuItemGridx10Color_Click(sender, e);
        }

        // changing view accessability
        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Toggle(hUDToolStripMenuItem);

            MenuItemHUD_Click(sender, e);
        }

        // context menu way of enabling/disabling the neighbor count on the cells
        private void neighborCountToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Toggle(neighborCountToolStripMenuItem1);

            MenuItemNeighborCount_Click(sender, e);
        }

        // context menu way of enabling/disabling the grid outlines
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Toggle(gridToolStripMenuItem);
            MenuItemGrid_Click(sender, e);
        }

        #endregion

        #region HerperMethods

        //|--------------------------------------- Helper Methods -------------------------------------------------|

        /// <summary>
        /// Count how many cells are alive in the panel
        /// </summary>
        private void CountAlive()
        {
            int count = 0;
            for (int yNum = 0; yNum < universe.GetLength(1); yNum++)
            {
                for (int xNum = 0; xNum < universe.GetLength(0); xNum++)
                {
                    if (universe[xNum, yNum] == true)
                    {
                        count++;

                    }
                }
            }
            alive = count;
            toolStripStatusLabelAlive.Text = "Alive: " + alive.ToString();
        }

        /// <summary>
        /// Randomly fills the universe based on a seed
        /// </summary>
        private void RandomFill()
        {
            Random random = new Random(seed);
            scratchPad = new bool[WIDTH, HEIGHT];
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int rand = random.Next(0, 2);
                    if (rand == 0)
                    {
                        scratchPad[x, y] = true;
                    }
                    else
                    {
                        scratchPad[x, y] = false;
                    }
                }
            }
            universe = scratchPad;
            graphicsPanel1.Invalidate();
        }

        // toggles a menu item object
        private void Toggle(ToolStripMenuItem menuItem)
        {
            if (menuItem.Checked == true)
            {
                menuItem.Checked = false;

            }
            else
            {
                menuItem.Checked = true;
            }
        }

        // Decides the color for the dead cell based on the back color of the panel
        private void DecideColor(out Brush brush)
        {
            Color deadColor;

            if (graphicsPanel1.BackColor != Color.Red)
            {
                deadColor = Color.Red;
            }
            else
            {
                deadColor = Color.White;
            }

            brush = new SolidBrush(deadColor);
        }

        /// <summary>
        /// Decides if the universe is finite or toroidal
        /// </summary>
        /// <param name="neighbors"></param> the count
        /// <param name="x"></param> the x coordinate
        /// <param name="y"></param> the y coordinate
        private void DecideNeighborCount(out int neighbors, int x, int y)
        {
            if (finiteToolStripMenuItem.Checked)
            {
                neighbors = CountNeighborsFinite(x, y);
            }
            else
            {
                neighbors = CountNeighborsToroidal(x, y);
            }
        }

    }

    #endregion
}
