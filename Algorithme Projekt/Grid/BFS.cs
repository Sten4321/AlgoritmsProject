using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grid
{
    class BFS : IFindPath
    {
        public static List<Cell> path = new List<Cell>();
        static List<Cell> discovered;
        static Cell destinationCell;

        /// <summary>
        /// returns a path list
        /// </summary>
        /// <param name="wizard"></param>
        /// <param name="Destination"></param>
        /// <returns></returns>
        public  List<Cell> FindPath(Cell statingCell, Cell destination)
        {
            destinationCell = destination;
            discovered = new List<Cell>();
            Queue<Cell> s = new Queue<Cell>();//queue of cells

            s.Enqueue(statingCell);
            bool foundRoute = false;

            while (s.Count != 0)
            {
                Cell e = s.Dequeue();
                Console.WriteLine("Current Cell: " + e.position + ".");
                if (e == destination)
                {
                    foundRoute = true;
                    break;//stops if it found the end node
                }
                foreach (Cell w in GetAdjecent(e))
                {
                    if (!End(w))
                    {
                        s.Enqueue(w);//adds the new edges to the stack
                        discovered.Add(w);//sets endnode as discovered
                        w.Parrent = e;//sets endnodes parrent
                    }
                }
            }

            if (foundRoute)
            {
                AddToPath();
                path.Reverse();
            }
            return path;
        }

        /// <summary>
        /// returns a list of adjecent nodes
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static List<Cell> GetAdjecent(Cell e)
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


                    int XCheck = e.position.X + x;
                    int YCheck = e.position.Y + y;

                    if (XCheck >= 0 && XCheck < GridManager.cellRowCount) // x is not out of bounds
                    {
                        if (YCheck >= 0 && YCheck < GridManager.cellRowCount) // same for why
                        {
                            foreach (Cell _cell in GridManager.grid)
                            {
                                //find the cell in the grid list
                                if (_cell.position.X == XCheck && _cell.position.Y == YCheck)
                                {
                                    if (!AdjecentDioganalWall(_cell, e))
                                    {
                                        neighbours.Add(_cell);
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
                if ((cell.MyType == CellType.WALL || cell.MyType == CellType.TREE || cell.MyType == CellType.WATER || cell.MyType == CellType.MONSTERCELL) && cell.position == new Point(x, y))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds to paths by finding te route
        /// </summary>
        private static void AddToPath()
        {
            Cell current = destinationCell;
            path.Add(current);
            while (current.Parrent != null)
            {
                path.Add(current.Parrent);
                current = current.Parrent;
            }
        }

        /// <summary>
        /// checks if the node has been discovered
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool End(Cell e)
        {
            if (discovered.Contains(e))
            {
                return true;
            }
            return false;
        }
    }
}
