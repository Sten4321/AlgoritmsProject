﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Grid
{


    class GridManager
    {
        //Handeling of graphics
        private BufferedGraphics backBuffer;
        private Graphics dc;
        private Rectangle displayRectangle;

        /// <summary>
        /// Amount of rows in the grid
        /// </summary>
        public static int cellRowCount;

        /// <summary>
        /// This list contains all cells
        /// </summary>
        public static List<Cell> grid;

        /// <summary>
        /// The current click type
        /// </summary>
        private CellType clickType;

        public static Cell startCell, goalCell;

        public static List<Cell> pathToEnd = new List<Cell>();


        public GridManager(Graphics dc, Rectangle displayRectangle)
        {
            //Create's (Allocates) a buffer in memory with the size of the display
            this.backBuffer = BufferedGraphicsManager.Current.Allocate(dc, displayRectangle);

            //Sets the graphics context to the graphics in the buffer
            this.dc = backBuffer.Graphics;

            //Sets the displayRectangle
            this.displayRectangle = displayRectangle;

            //Sets the row count to then, this will create a 10 by 10 grid.
            cellRowCount = 10;

            CreateGrid();
        }

        /// <summary>
        /// Renders all the cells
        /// </summary>
        public void Render()
        {
            dc.Clear(Color.White);

            foreach (Cell cell in grid)
            {
                cell.Render(dc);
            }

            //Renders the content of the buffered graphics context to the real context(Swap buffers)
            backBuffer.Render();
        }

        /// <summary>
        /// Creates the grid
        /// </summary>
        public void CreateGrid()
        {
            //Instantiates the list of cells
            grid = new List<Cell>();

            //Sets the cell size
            int cellSize = displayRectangle.Width / cellRowCount;

            //Creates all the cells
            for (int x = 0; x < cellRowCount; x++)
            {
                for (int y = 0; y < cellRowCount; y++)
                {
                    grid.Add(new Cell(new Point(x, y), cellSize));
                }
            }

            CreateLevel();//creates the level
        }

        /// <summary>
        /// If the mouse clicks on a cell
        /// </summary>
        /// <param name="mousePos"></param>
        public void ClickCell(Point mousePos)
        {
            foreach (Cell cell in grid) //Finds the cell that we just clicked
            {
                if (cell.BoundingRectangle.IntersectsWith(new Rectangle(mousePos, new Size(1, 1))))
                {
                    cell.Click(ref clickType);
                }

            }
        }


        /// <summary>
        /// Creates the map, tile by tile
        /// (Hard-coded because dynamic / flexible map creation is not the point of this project)
        /// </summary>
        public void CreateLevel()
        {
            //Hard coded mess that not even the all-knowing Cthulhu would comprehend
            //You have been warned
            MakeRoads();
            MakeTrees();
            MakeWater();
        }

        private void MakeRoads()
        {
            #region ABOMINATION INSIDE! MAKE SURE YOU'RE WEARING A HAZMAT SUITE BEFORE OPENING!
            for (int x = 1; x <= 8; x += 7)
            {
                for (int y = 5; y < 9; y++)
                {
                    foreach (Cell cell in grid)
                    {
                        if (cell.position == new Point(x, y))
                        {
                            cell.MyType = CellType.ROAD;

                            cell.AssignSprite();
                        }
                    }
                }
            }
            for (int x = 3; x <= 7; x += 4)
            {
                for (int y = 0; y < 6; y++)
                {
                    foreach (Cell cell in grid)
                    {
                        if (cell.position == new Point(x, y))
                        {
                            cell.MyType = CellType.ROAD;

                            cell.AssignSprite();
                        }
                    }
                }
            }
            for (int x = 4; x <= 6; x++)
            {
                int y = 0;
                foreach (Cell cell in grid)
                {
                    if (cell.position == new Point(x, y))
                    {
                        cell.MyType = CellType.ROAD;

                        cell.AssignSprite();
                    }
                }
            }
            for (int x = 2; x <= 7; x++)
            {
                int y = 8;
                foreach (Cell cell in grid)
                {
                    if (cell.position == new Point(x, y))
                    {
                        cell.MyType = CellType.ROAD;

                        cell.AssignSprite();
                    }
                }
            }

            foreach (Cell cell in grid)
            {
                if (cell.position == new Point(2, 5))
                {
                    cell.MyType = CellType.ROAD;
                    cell.AssignSprite();

                }
            }
            #endregion;
        }

        private void MakeTrees()
        {
            for (int y = 7; y <= 9; y += 2)
            {
                for (int x = 2; x <= 6; x++)
                {
                    foreach (Cell cell in grid)
                    {
                        if (cell.position == new Point(x, y))
                        {
                            cell.MyType = CellType.TREE;

                            cell.AssignSprite();
                        }
                    }
                }
            }

        }
        private void MakeWater()
        {
            for (int x = 4; x < 7; x++)
            {

                for (int y = 1; y < 7; y++)
                {

                    foreach (Cell cell in grid)
                    {
                        if (cell.position == new Point(x, y))
                        {
                            cell.MyType = CellType.WATER;

                            cell.AssignSprite();
                        }
                    }
                }
            }

        }


    }


}

