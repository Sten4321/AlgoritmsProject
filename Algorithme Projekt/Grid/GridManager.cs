﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Grid
{


    public class GridManager
    {
        //Handeling of graphics
        private BufferedGraphics backBuffer;
        private Graphics dc;
        private Rectangle displayRectangle;

        public static int cellSize;
        /// <summary>
        /// Amount of rows in the grid
        /// </summary>
        public static int cellRowCount;

        /// <summary>
        /// This list contains all cells
        /// </summary>
        public static List<Cell> grid;

        public Point key1Pos, key2Pos = new Point();

        /// <summary>
        /// The current click type
        /// </summary>
        private CellType clickType;

        //  public static Cell startCell, goalCell;

        public static Form1 formRef;

        public GridManager(Graphics dc, Rectangle displayRectangle, Form1 _formRef)
        {
            formRef = _formRef;
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



            Wizard.Instance.Draw(dc);


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
            cellSize = displayRectangle.Width / cellRowCount;

            //Creates all the cells
            for (int x = 0; x < cellRowCount; x++)
            {
                for (int y = 0; y < cellRowCount; y++)
                {
                    grid.Add(new Cell(new Point(x, y), cellSize));
                }
            }

            CreateLevel(cellSize);//creates the level

        }

        public void ResetLevel()
        {
            //Notes the time it took to beat the level
            formRef.finalTime = formRef.stopWatch.ElapsedMilliseconds;

            //For showing final time
            formRef.stopWatch.Stop();

            //allows player to press start button

            //Rewrites the highscore
            formRef.ReWriteHighScore(Wizard.Instance.pathFinder);
            //resets all cell positions and spawns random keys
            CreateGrid();


            formRef.levelIsPlaying = false;
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
                    cell.Click();
                    break;

                }

            }
        }


        /// <summary>
        /// Creates the map, tile by tile
        /// (Hard-coded because dynamic / flexible map creation is not the point of this project)
        /// </summary>
        public void CreateLevel(int cellSize)
        {
            //Hard coded mess that not even the all-knowing Cthulhu would comprehend
            //You have been warned
            Wizard.Instance.ToString();
            MakeRoads();
            MakeTrees();
            MakeWater();
            MakeTower();
            MakeCrystal();
            MakePortal();
            MakeMonsterCell();

            //If the loop has restarted, or is starting
            if (formRef.AlgorithmRotationIndex == 0)
            {
                //Reset old keypositions
                key1Pos = Point.Empty;
                key2Pos = Point.Empty;

                //Make two keys on random spots that are "walkable" and not a monster tile
                for (int i = 0; i < 2; i++)
                {
                    MakeKeys();
                }

            }
            else
            {
                //let BFS try the same key layout
                RecycleKeys();
            }

        }

        /// <summary>
        /// copies the keys from the previous level to the game
        /// </summary>
        private void RecycleKeys()
        {
            foreach (Cell cell in grid)
            {
                //if the cell is walkable
                if (cell.position == key1Pos && (cell.MyType == CellType.EMPTY || cell.MyType == CellType.ROAD))
                {
                    //if it's not the cell wizard is standing on
                    if (cell.position != Wizard.Instance.position)
                    {
                        //For remembering its original appearence
                        cell.initialType = cell.MyType;

                        //changes type and appearence
                        cell.MyType = CellType.KEY;
                        cell.AssignSprite();

                        break;
                    }
                }
            }
            foreach (Cell cell in grid)
            {
                //if the cell is walkable
                if (cell.position == key2Pos && (cell.MyType == CellType.EMPTY || cell.MyType == CellType.ROAD))
                {
                    //if it's not the cell wizard is standing on
                    if (cell.position != Wizard.Instance.position)
                    {
                        //For remembering its original appearence
                        cell.initialType = cell.MyType;

                        //changes type and appearence
                        cell.MyType = CellType.KEY;
                        cell.AssignSprite();

                        break;
                    }
                }
            }
        }

        private void MakeMonsterCell()
        {
            foreach (Cell cell in grid)
            {
                if (cell.position == new Point(6, 8))
                {
                    cell.MyType = CellType.MONSTERCELL;
                    cell.AssignSprite();
                    break;
                }
            }
        }

        /// <summary>
        /// generates a random key
        /// </summary>
        private void MakeKeys()
        {
            Random rnd = new Random();

            //untill the key is being placed on a valid position
            while (true)
            {
                Point keyPos = new Point(rnd.Next(cellRowCount), rnd.Next(cellRowCount)); //position of the key

                foreach (Cell cell in grid)
                {
                    //if the cell is walkable
                    if (cell.position == keyPos && (cell.MyType == CellType.EMPTY || cell.MyType == CellType.ROAD))
                    {
                        //if it's not the cell wizard is standing on
                        if (cell.position != Wizard.Instance.position)
                        {
                            //For remembering its original appearence
                            cell.initialType = cell.MyType;

                            //changes type and appearence
                            cell.MyType = CellType.KEY;
                            cell.AssignSprite();

                            //If key1 has not been assigned
                            if (key1Pos.IsEmpty)
                            {
                                //assign key1 position
                                key1Pos = keyPos;
                            }
                            else
                            {
                                key2Pos = keyPos;
                            }
                            return;
                        }
                    }
                }
            }
        }

        private void MakePortal()
        {
            foreach (Cell cell in grid)
            {
                if (cell.position == new Point(0, 8))
                {
                    cell.MyType = CellType.PORTAL;
                    cell.AssignSprite();
                    break;

                }
            }
        }

        private void MakeTower()
        {
            foreach (Cell cell in grid)
            {
                if (cell.position == new Point(2, 4))
                {
                    cell.MyType = CellType.TOWER;
                    cell.AssignSprite();
                    break;

                }
            }
        }

        private void MakeCrystal()
        {
            foreach (Cell cell in grid)
            {
                if (cell.position == new Point(8, 6))
                {
                    cell.MyType = CellType.CRYSTAL;
                    cell.AssignSprite();
                    break;

                }
            }
        }

        private void MakeRoads()
        {
            #region ABOMINATION INSIDE! MAKE SURE YOU'RE WEARING A HAZMAT SUITE BEFORE ENTERING!
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
                    if (x != 6)
                    {

                        if (cell.position == new Point(x, y))
                        {
                            cell.MyType = CellType.ROAD;

                            cell.AssignSprite();
                        }
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
                            break;

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
                            break;

                        }
                    }
                }
            }

        }


    }


}

