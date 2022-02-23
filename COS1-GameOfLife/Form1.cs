using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COS1_GameOfLife
{
    public partial class Form1 : Form
    {
        int seed = 135321;
        Color numColor = Color.Red;
        // The universe array
        bool[,] universe = new bool[30, 30];
        bool[,] scratchPad = new bool[30, 30];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            numColor = Properties.Settings.Default.NumColor;
            universe = new bool[Properties.Settings.Default.Width, Properties.Settings.Default.Height];
            scratchPad = new bool[Properties.Settings.Default.Width, Properties.Settings.Default.Height];
            timer.Interval = Properties.Settings.Default.Interval;



            // Setup the timer
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running

            // Update status strip time interval
            toolStripStatusLabelInterval.Text = "Interval = " + timer.Interval.ToString();
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            for (int y = 0; y < scratchPad.GetLength(1); y++)
            {
                for (int x = 0; x < scratchPad.GetLength(0); x++)
                {
                    scratchPad[x, y] = false;
                }
            }

            int livingNeighbors = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if(finiteToolStripMenuItem.CheckState == CheckState.Checked)
                    {
                        livingNeighbors = CountNeighborsFinite(x, y);
                    }
                    else if(toroidalToolStripMenuItem.CheckState == CheckState.Checked)
                    {
                        livingNeighbors = CountNeighborsToroidal(x, y);
                    }
                    if(universe[x, y] == true)
                    {
                        if (livingNeighbors < 2)
                        {
                            scratchPad[x, y] = false;
                        }
                        if (livingNeighbors > 3)
                        {
                            scratchPad[x, y] = false;
                        }
                        if (livingNeighbors == 2 || livingNeighbors == 3)
                        {
                            scratchPad[x, y] = true;
                        }
                    }
                    if (universe[x, y] == false)
                    {
                        if (livingNeighbors == 3)
                        {
                            scratchPad[x, y] = true;
                        }
                    }
                }
            }

            // swap scratchpad and universe
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            CountLivingCells();
                                         
            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            // Invalidate Graphics Panel
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void CountLivingCells()
        {
            // Update status strip livingCells
            int livingCells = 0;
            for (int y = 0; y < scratchPad.GetLength(1); y++)
            {
                for (int x = 0; x < scratchPad.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        livingCells++;
                    }
                }
            }

            toolStripStatusLabelLivingCells.Text = "Living Cells = " + livingCells.ToString();
        }

        private void Randomize()
        {
            Random rand = new Random();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // Call Next
                    int randomNum = rand.Next(0, 2);
                    if(randomNum == 0)
                    {
                        universe[x, y] = false;
                    }
                    else
                    {
                        universe[x, y] = true;
                    }
                }
            }

            CountLivingCells();

            graphicsPanel1.Invalidate();
        }        
        
        private void RandomizeBySeed(int seed)
        {
            Random rand = new Random(seed);
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // Call Next
                    int randomNum = rand.Next(0, 2);
                    if (randomNum == 0)
                    {
                        universe[x, y] = false;
                    }
                    else
                    {
                        universe[x, y] = true;
                    }
                }
            }

            CountLivingCells();

            graphicsPanel1.Invalidate();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);
            Pen gridPen2 = new Pen(gridColor, 5);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = Rectangle.Empty;
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
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    int neighbors = CountNeighborsFinite(x, y);
                    if (neighborCountToolStripMenuItem.CheckState == CheckState.Checked && neighbors != 0)
                    {
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;


                        Brush numBrush = new SolidBrush(numColor);

                        e.Graphics.DrawString(neighbors.ToString(), graphicsPanel1.Font, numBrush, cellRect, stringFormat);
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
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
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
                    // if xOffset and yOffset are both equal to 0 then continue
                    if(xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if(xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue0
                    if(xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
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
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then set to xLen - 1
                    if(xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if(xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }



        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            timer.Enabled = false;

            generations = 0;

            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            CountLivingCells();

            graphicsPanel1.Invalidate();
            
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = numColor;

            if(DialogResult.OK == dlg.ShowDialog())
            {
                numColor = dlg.Color;
            }

            graphicsPanel1.Invalidate();
        }

        private void modalToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ModalDialog dlg = new ModalDialog();


            dlg.Interval = timer.Interval;
            dlg.UniverseWidth = universe.GetLength(0);
            dlg.UniverseHeight = universe.GetLength(1);

            if (DialogResult.OK == dlg.ShowDialog())
            {
                timer.Interval = dlg.Interval;
                universe = new bool[dlg.UniverseWidth, dlg.UniverseHeight];
                scratchPad = new bool[dlg.UniverseWidth, dlg.UniverseHeight];

                CountLivingCells();

                graphicsPanel1.Invalidate();
            }
        }

        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = graphicsPanel1.BackColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
            }

            graphicsPanel1.Invalidate();
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(gridToolStripMenuItem.CheckState == CheckState.Checked)
            {
                ColorDialog dlg = new ColorDialog();

                dlg.Color = gridColor;

                if (DialogResult.OK == dlg.ShowDialog())
                {
                    gridColor = dlg.Color;
                }

                graphicsPanel1.Invalidate();
            }
        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = cellColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;
            }

            graphicsPanel1.Invalidate();
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(gridToolStripMenuItem.CheckState == CheckState.Checked)
            {
                gridToolStripMenuItem.CheckState = CheckState.Unchecked;
                gridColor = graphicsPanel1.BackColor;
            }
            else
            {
                gridToolStripMenuItem.CheckState = CheckState.Checked;
                gridColor = Color.Black;
            }

            graphicsPanel1.Invalidate();
        }

        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(neighborCountToolStripMenuItem.CheckState == CheckState.Checked)
            {
                neighborCountToolStripMenuItem.CheckState = CheckState.Unchecked;
            } else
            {
                neighborCountToolStripMenuItem.CheckState = CheckState.Checked;
            }

            NextGeneration();

            generations = 0;

            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            graphicsPanel1.Invalidate();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Update Property
            Properties.Settings.Default.PanelColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.NumColor = numColor;
            Properties.Settings.Default.Width = universe.GetLength(0);
            Properties.Settings.Default.Height = universe.GetLength(1);
            Properties.Settings.Default.Interval = timer.Interval;

            Properties.Settings.Default.Save();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

            // Reading the property
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            numColor = Properties.Settings.Default.NumColor;
            universe = new bool[Properties.Settings.Default.Width, Properties.Settings.Default.Height];
            scratchPad = new bool[Properties.Settings.Default.Width, Properties.Settings.Default.Height];
            Properties.Settings.Default.Interval = timer.Interval;
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();

            // Reading the property
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            numColor = Properties.Settings.Default.NumColor;
            universe = new bool[Properties.Settings.Default.Width, Properties.Settings.Default.Height];
            scratchPad = new bool[Properties.Settings.Default.Width, Properties.Settings.Default.Height];
            Properties.Settings.Default.Interval = timer.Interval;
        }

        private void timeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Randomize();
        }

        private void seedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SeedDialog dlg = new SeedDialog();


            dlg.Seed = seed;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                seed = dlg.Seed;

                RandomizeBySeed(seed);

                graphicsPanel1.Invalidate();
            }
        }

        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomizeBySeed(seed);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!This is my comment.");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if(universe[x, y] == true)
                        {
                            currentRow += "O";
                        } else if(universe[x, y] == false)
                        {
                            currentRow += ".";
                        }
                    }

                    writer.WriteLine(currentRow);
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if(row.Substring(0,1) == "!")
                    {
                        continue;
                    }

                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    maxHeight++;

                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    maxWidth = row.Length;
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                universe = new bool[maxWidth, maxHeight];
                scratchPad = new bool[maxWidth, maxHeight];

                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Iterate through the file again, this time reading in the cells.
                int yPos = 0;
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();
                    
                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.Substring(0, 1) == "!")
                    {
                        continue;
                    }
                    else
                    {
                        // If the row is not a comment then 
                        // it is a row of cells and needs to be iterated through.
                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            // If row[xPos] is a 'O' (capital O) then
                            // set the corresponding cell in the universe to alive.
                            if(row[xPos] == 'O')
                            {
                                universe[xPos, yPos] = true;
                            }
                            else if(row[xPos] == '.')
                            {
                                universe[xPos, yPos] = false;
                            }
                            // If row[xPos] is a '.' (period) then
                            // set the corresponding cell in the universe to dead.
                        }
                        yPos++;
                    }
                }

                // Close the file.
                reader.Close();

                CountLivingCells();

                graphicsPanel1.Invalidate();
            }
        }

        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(toroidalToolStripMenuItem.CheckState == CheckState.Checked)
            {
                toroidalToolStripMenuItem.CheckState = CheckState.Unchecked;
                finiteToolStripMenuItem.CheckState = CheckState.Checked;
            } else
            {
                toroidalToolStripMenuItem.CheckState = CheckState.Checked;
                finiteToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (finiteToolStripMenuItem.CheckState == CheckState.Checked)
            {
                finiteToolStripMenuItem.CheckState = CheckState.Unchecked;
                toroidalToolStripMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                finiteToolStripMenuItem.CheckState = CheckState.Checked;
                toroidalToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
        }
    }
}
