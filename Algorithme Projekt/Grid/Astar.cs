using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grid
{
    class Astar : IFindPath
    {
        public static List<Cell> openList = new List<Cell>();// list of nodes to examine  
        public static List<Cell> ClosedList = new List<Cell>();//nodes that have been examined
        public static List<Cell> path = new List<Cell>();

        /// <summary>
        /// Returns the shortest route between point A and point B, using the A* algorithm 
        /// </summary>
        /// <param name="statingCell"></param>
        /// <param name="goalCell"></param>
        /// <param name="goalCellType"></param>
        /// <returns></returns>
        public List<Cell> FindPath(Cell statingCell, Cell goalCell)
        {

            Clear();

            CellType goalCellType = goalCell.MyType;

            openList.Add(statingCell); //starting point


            while (openList.Count > 0) //recursively adds neihbouring cells / nodes.
                                       //  Only runs out if there's no possible path
            {

                //Finds the best candidate node (the cheapest and most efficient one)
                Cell currentCell = FindLowestFCost(openList);


                openList.Remove(currentCell); // walkable squares

                ClosedList.Add(currentCell); // has been walked on



                if (currentCell.MyType == goalCellType)
                {
                    //found target
                    goalCell = currentCell;

                    ReturnPath(statingCell, goalCell); //extracts the found path
                    return path;
                }
                //all walkable neighbouring tiles, that are not out of bound

                List<Cell> neighbours = FindNeighbours(currentCell);
                foreach (Cell neighbour in neighbours)
                {
                    if ((neighbour.MyType != CellType.EMPTY && neighbour.MyType !=
                        goalCellType && neighbour.MyType != CellType.ROAD && neighbour.MyType != CellType.MONSTERCELL)
                        || ClosedList.Contains(neighbour))
                    {
                        //ignore if obsticle or tile has already been walked on
                        continue;
                    }

                    //calculates the new move cost for this neighbour
                    int newMoveCost = currentCell.GValue + GetDistance(currentCell, neighbour);

                    //if this neighbour is the best route to take
                    if (newMoveCost < neighbour.GValue || !openList.Contains(neighbour))
                    {
                        //
                        neighbour.GValue = newMoveCost;

                        //How far this cell is from the distance
                        neighbour.HValue = GetDistance(neighbour, goalCell);

                        //saves reference for returning path
                        neighbour.Parrent = currentCell;

                        if (!openList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                        }
                    }

                }

            }
            return path;
        }

        /// <summary>
        /// Returns true if there is a wall adjecent to the selected between it and the cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        private static bool AdjecentDioganalWall(Cell cell, Cell selected)
        {
            Point cellPosition = cell.position;
            Point selectedPosition = selected.position;

            if (!(cell.position.Y == selected.position.Y && cell.position.X <= selected.position.X)
                || !(cell.position.Y == selected.position.Y && cell.position.X >= selected.position.X)
                || !(cell.position.Y <= selected.position.Y && cell.position.X == selected.position.X)
                || !(cell.position.Y >= selected.position.Y && cell.position.X == selected.position.X))
            {
                if (cellPosition.Y < selectedPosition.Y && cellPosition.X < selectedPosition.X)
                {
                    if (IsWall(selectedPosition.X, selectedPosition.Y - 1) || IsWall(selectedPosition.X - 1, selectedPosition.Y))
                    {
                        return true;
                    }
                }
                else if (cellPosition.Y < selectedPosition.Y && cellPosition.X > selectedPosition.X)
                {
                    if (IsWall(selectedPosition.X, selectedPosition.Y - 1) || IsWall(selectedPosition.X + 1, selectedPosition.Y))
                    {
                        return true;
                    }
                }
                else if (cellPosition.Y > selectedPosition.Y && cellPosition.X < selectedPosition.X)
                {
                    if (IsWall(selectedPosition.X, selectedPosition.Y + 1) || IsWall(selectedPosition.X - 1, selectedPosition.Y))
                    {
                        return true;
                    }
                }
                else if (cellPosition.Y > selectedPosition.Y && cellPosition.X > selectedPosition.X)
                {
                    if (IsWall(selectedPosition.X, selectedPosition.Y + 1) || IsWall(selectedPosition.X + 1, selectedPosition.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// returns true if there is a wall at specified coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static bool IsWall(int x, int y)
        {
            foreach (Cell cell in GridManager.grid)
            {

                if ((cell.MyType == CellType.WALL || cell.MyType == CellType.WATER || cell.MyType == CellType.TREE)
                    && cell.position == new Point(x, y))

                if ((cell.MyType == CellType.WALL || cell.MyType == CellType.TREE || cell.MyType == CellType.WATER || cell.MyType == CellType.MONSTERCELL) && cell.position == new Point(x, y))

                {
                    return true;
                }
            }
            return false;
        }

        static void ReturnPath(Cell start, Cell end)
        {

            path = new List<Cell>();

            Cell currentNode = end;

            while (currentNode != start)
            {

                path.Add(currentNode);

                currentNode = currentNode.Parrent;

            }

            path.Reverse();

            GridManager.pathToEnd = path;


        }

        private static Cell FindLowestFCost(List<Cell> openList)
        {
            Cell currentCell = openList[0];

            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].FValue < currentCell.FValue
                    || openList[i].FValue == currentCell.FValue
                    && openList[i].GValue < currentCell.GValue)
                {
                    currentCell = openList[i];
                }
            }
            return currentCell;
        }

        public static List<Cell> FindNeighbours(Cell cell)
        {
            List<Cell> neighbours = new List<Cell>();

            //look at all neighboures in a 3x3 square

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    //Checks the coordinates, relative to the current Cell's position
                    //***
                    //*^*
                    //***
                    int XCheck = cell.position.X + x;
                    int YCheck = cell.position.Y + y;

                    if (XCheck >= 0 && XCheck < GridManager.cellRowCount) // x is not out of bounds
                    {
                        if (YCheck >= 0 && YCheck < GridManager.cellRowCount) // same for why
                        {

                            foreach (Cell _cell in GridManager.grid)
                            {
                                //find the cell in the grid list
                                if (_cell.position.X == XCheck && _cell.position.Y == YCheck)
                                {
                                    if (!AdjecentDioganalWall(_cell, cell)) //Does so it can't walk through corners
                                    {
                                        neighbours.Add(_cell);//cell is now a known neighbour of the currentCell
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return neighbours;
        }

        private static int GetDistance(Cell a, Cell b)
        {
            int xDistance = Math.Abs(a.position.X - b.position.X);
            int yDistance = Math.Abs(a.position.Y - b.position.Y);



            return xDistance > yDistance
                ? 14 * yDistance + 10 * (xDistance - yDistance)
                : 14 * xDistance + 10 * (yDistance - xDistance);

          

        }

        /// <summary>
        /// Clears Values
        /// </summary>
        private static void Clear()
        {
            path = new List<Cell>();//path to target
            openList = new List<Cell>();// list of nodes to examine  
            ClosedList = new List<Cell>(); //nodes that have been examined

            foreach (Cell cell in GridManager.grid)
            {
                cell.Parrent = null;
                cell.GValue = default(int);
                cell.HValue = default(int);
            }
        }
    }
}
