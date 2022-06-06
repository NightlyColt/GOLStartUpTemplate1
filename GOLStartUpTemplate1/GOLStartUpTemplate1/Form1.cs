using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace GOLStartUpTemplate1
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[30, 30];
        bool[,] scratchPad = new bool[30, 30];

        int seed = new Random().Next(int.MinValue, int.MaxValue);

        // Alive Cells
        int alive = 0;

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.LightGray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {

            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
            toolStripStatusLabelInterval.Text = "Interval: " + timer.Interval.ToString();
            seedLabel.Text = "Seed: " + seed.ToString();
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            bool[,] scratchPad = new bool[30, 30];
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(1); x++)
                {
                    DecideNeighborCount(out int count, x, y);
                    // Apply the Rules
                    if (count < 2) { scratchPad[x, y] = false; }
                    else if (count > 3) { scratchPad[x, y] = false; }
                    else if (count == 3) { scratchPad[x, y] = true; }
                    else if (count <= 3 && universe[x, y] == true) { scratchPad[x, y] = true; }

                    // Turn in/of the scratch Pad
                }
            }
            universe = scratchPad;

            graphicsPanel1.Invalidate();
            // Increment generation count
            generations++;
            alive = 0;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations: " + generations.ToString();
            toolStripStatusLabelInterval.Text = "Interval: " + timer.Interval.ToString();

            // Calculate how many alive cells there are
            CountAlive();
        }

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

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private int CountNeighborsFinite(int x, int y)
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

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // The Font for the count of neighbors
            Font font = new Font("Arial", 10f);
              
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
                            if (universe[x, y] == false && neighbors == 3)
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                            }
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
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

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

                CountAlive();
                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void RandomFill()
        {
            Random random = new Random(seed);
            scratchPad = new bool[30, 30];
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

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        private void PlayButton_Click(object sender, EventArgs e)
        {
            timer.Start(); // start timer running
            PlayButton.Enabled = false;
            NextButton.Enabled = false;
            PauseButton.Enabled = true;
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            timer.Stop(); // stop timer 
            PlayButton.Enabled = true;
            NextButton.Enabled = true;
            PauseButton.Enabled = false;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void MenuItemStart_Click(object sender, EventArgs e)
        {
            PlayButton_Click(sender, e);
            runToolStripMenuItem.Enabled = false;
            nextF7ToolStripMenuItem.Enabled = false;
            pauseF6ToolStripMenuItem.Enabled = true;
        }

        private void MenuItemPause_Click(object sender, EventArgs e)
        {
            PauseButton_Click(sender, e);
            runToolStripMenuItem.Enabled = true;
            nextF7ToolStripMenuItem.Enabled = true;
            pauseF6ToolStripMenuItem.Enabled = false;
        }

        private void MenuItemNext_Click(object sender, EventArgs e)
        {
            NextButton_Click(sender, e);
        }

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

        private void MenuItemFromSeed_Click(object sender, EventArgs e)
        {
            SeedDialog dlg = new SeedDialog();

            dlg.RandomSeed = seed;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                seed = dlg.RandomSeed;
                RandomFill();
            }
        }

        private void MenuItemCurrentSeed_Click(object sender, EventArgs e)
        {
            RandomFill();
        }

        private void MenuItemFromTime_Click(object sender, EventArgs e)
        {
        }

        private void MenuItemHUD_Click(object sender, EventArgs e)
        {
            if (MenuItemHUD.Checked == true)
            {
                MenuItemHUD.Checked = false;
            }
            else
            {
                MenuItemHUD.Checked = true;
            }
        }

        private void MenuItemNeighborCount_Click(object sender, EventArgs e)
        {
            if (neighborCountToolStripMenuItem.Checked == true)
            {
                neighborCountToolStripMenuItem.Checked = false;
            }
            else
            {
                neighborCountToolStripMenuItem.Checked = true;
            }
            graphicsPanel1.Invalidate();

        }

        private void MenuItemGrid_Click(object sender, EventArgs e)
        {
            if (MenuItemGrid.Checked == true)
            {
                MenuItemGrid.Checked = false;

            }
            else
            {
                MenuItemGrid.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        private void MenuItemToroidal_Click(object sender, EventArgs e)
        {
            finiteToolStripMenuItem.Checked = false;
            toroidalToolStripMenuItem.Checked = true;
        }

        private void MenuItemFinite_Click(object sender, EventArgs e)
        {
            toroidalToolStripMenuItem.Checked = false;
            finiteToolStripMenuItem.Checked = true;

        }

        private void MenuItemBackColor_Click(object sender, EventArgs e)
        {

        }

        private void MenuItemCellColor_Click(object sender, EventArgs e)
        {

        }

        private void MenuItemGridColor_Click(object sender, EventArgs e)
        {

        }

        private void MenuItemGridx10Color_Click(object sender, EventArgs e)
        {

        }

        private void MenuItemOptions_Click(object sender, EventArgs e)
        {

        }

        private void MenuItemReset_Click(object sender, EventArgs e)
        {

        }

        private void MenuItemReload_Click(object sender, EventArgs e)
        {

        }

        private void MenuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripButtonNew_Click(object sender, EventArgs e)
        {
            universe = new bool[30, 30];
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations: " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newToolStripButtonNew_Click(sender, e);
        }

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
                    universe = new bool[maxWidth, maxHeight];
                    scratchPad = new bool[maxWidth, maxHeight];

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
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);
        }
    }
}
