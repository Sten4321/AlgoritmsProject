using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grid
{
    class Wizard
    {
        //Strategy pattern - the algorithm it uses to find path
        public IFindPath pathFinder = new Astar();

        public Point position { get; set; }

        private Image sprite;

        private int spriteSize;

        private static Wizard instance;

        //The path, cell by cell, that wizard should walk. He will do so automatically, is it not empty.
        private List<Cell> pathToNextItem = new List<Cell>();

        //For writing on screen
        public string currentTaskText = string.Empty;

        public static Wizard Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Wizard(new Point(1, 8), GridManager.cellSize);
                }
                return instance;
            }
        }

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle(position.X * spriteSize, position.Y * spriteSize, spriteSize, spriteSize);
            }
        }


        //for tracking objectives
        public int keyCount = 0;
        public bool hasPotion = false;
        public bool canEnterPortal = false;
        //


        /// <summary>
        /// Wizard Constructor
        /// </summary>
        /// <param name="_coord">Starting coordinate</param>
        private Wizard(Point _coord, int spriteSize)
        {
            sprite = Image.FromFile(@"Images\Wizard.png");
            position = _coord;
            this.spriteSize = spriteSize;
        }
        /// <summary>
        /// Draws the wizard and his current objective
        /// </summary>
        /// <param name="dc"></param>
        public void Draw(Graphics dc)
        {
            if (sprite != null)
            {
                dc.DrawImage(sprite, BoundingRectangle);
            }
            //If wizard has a task, it writes the task on the screen
            if (currentTaskText != string.Empty)
            {
                dc.DrawString(currentTaskText, new Font("Arial", 30, FontStyle.Regular),
                    new SolidBrush(Color.Red), new Point(0, 0));

            }
        }

        /// <summary>
        /// Handles what happens when the wizard steps on a cell
        /// </summary>
        /// <param name="cell"></param>
        public void InteractWithCell(Cell cell)
        {
            switch (cell.MyType)
            {
                case CellType.KEY:
                    //Picks up key and resets its sprite
                    Console.Beep(500, 50);

                    CollectKey(cell);
                    break;
                case CellType.TOWER:
                    //Wizard have the potion it needs for crystal
                    if (keyCount == 2)
                    {
                        Console.Beep(300, 50);

                        hasPotion = true;
                    }
                    break;
                case CellType.CRYSTAL:
                    //if wizard have the potion, the wizard will  delivere the potion
                    if (hasPotion)
                    {
                        Console.Beep(300, 50);

                        canEnterPortal = true;
                    }
                    break;
                case CellType.PORTAL:
                    // if it has delivered the potion, it may enter the portal, and reset the level
                    if (canEnterPortal)
                    {
                        Console.Beep(300, 300);

                        //Let's window know it's the next algorithms turn
                        GridManager.formRef.AlgorithmRotationIndex++;

                        instance = null;
                        //resets lvl
                        GridManager.formRef.visualManager.ResetLevel();
                    }
                    break;

                case CellType.MONSTERCELL:
                    Console.Beep(100, 200);

                    //Makes the monstercell Unwalkable and changes its sprite
                    ActivateMonsterCell(cell);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Activates the monstercell (makes it unwalkable)
        /// </summary>
        /// <param name="cell"></param>
        private void ActivateMonsterCell(Cell cell)
        {
            //Changes sprite
            cell.sprite = Image.FromFile(@"Images\Monster.png");
            cell.sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
            //Makes it unwalkable
            cell.MyType = CellType.WALL;
        }

        /// <summary>
        /// Collects the key, and makes the key tile empty
        /// </summary>
        /// <param name="cell"></param>
        private void CollectKey(Cell cell)
        {
            //picks up a key
            keyCount++;


            foreach (Cell tmp in GridManager.grid)
            {
                //if this cell is the specefic key cell 
                if (tmp.position == cell.position)
                {
                    //remove the keycell sprite, and make it what it used to be
                    tmp.MyType = tmp.initialType;
                    tmp.AssignSprite();

                }
            }
        }

        /// <summary>
        /// finds, then walks to the closest point of interest
        /// </summary>
        public void FindClosestItemOfInterest()
        {

            /* 
             Objectives in order:
            - get 2 keys
            - unlock tower to get potion
            - use potion on crystal
            - enter portal
             */

            //Wizard's cell
            Cell startCell = new Cell(new Point(0, 0), 0);

            foreach (Cell cell in GridManager.grid)
            {
                //find the cell you're currently standing on
                if (cell.position.X == this.position.X && cell.position.Y == this.position.Y)
                {
                    startCell = cell;
                    break;
                }
            }

            //the cell to find
            Cell targetCell = new Cell(new Point(0, 0), 0);


            //Tries to find the next item in its sequence
            foreach (Cell cell in GridManager.grid)
            {
                if (cell.MyType == CellType.KEY && keyCount < 2)
                {
                    if (keyCount == 0)
                    {
                        //Finds and walks to the closest key, if there are more than one
                        FindClosestKey(startCell, cell);
                        return;//method handles the pathfinding
                    }
                    else
                    {
                        //FIND KEY
                        targetCell = cell;
                        currentTaskText = "FIND KEYS: " + (2 - keyCount);
                        break;
                    }
                }
                if (cell.MyType == CellType.TOWER && keyCount == 2 && hasPotion == false)
                {
                    //FIND TOWER
                    targetCell = cell;
                    currentTaskText = "UNLOCK TOWER";
                    break;
                }
                if (cell.MyType == CellType.CRYSTAL && hasPotion == true && canEnterPortal == false)
                {
                    //FIND CRYSTAL
                    targetCell = cell;
                    currentTaskText = "BRING POTION TO CRYSTAL";

                    break;
                }
                if (cell.MyType == CellType.PORTAL && canEnterPortal == true)
                {
                    //FIND CRYSTAL
                    targetCell = cell;
                    currentTaskText = "ENTER PORTAL";

                    break;
                }
            }

            //Finds the path based on the wizard's objective
            pathToNextItem = pathFinder.FindPath(startCell, targetCell);

        }

        /// <summary>
        /// Finds, and walks to the closest key
        /// </summary>
        /// <param name="startCell"></param>
        /// <param name="firstKey"></param>
        private void FindClosestKey(Cell startCell, Cell firstKey)
        {
            //Announcing the wizards task
            currentTaskText = "FIND KEYS: " + (2 - keyCount);

            //a list containing the two key paths
            List<List<Cell>> keyPaths = new List<List<Cell>>();

            //Adds the first path
            keyPaths.Add(pathFinder.FindPath(startCell, firstKey));

            //Then finds and adds the second
            foreach (Cell cell in GridManager.grid)
            {
                if (cell.MyType == CellType.KEY && cell != firstKey)
                {
                    keyPaths.Add(pathFinder.FindPath(startCell, cell));
                    break;
                }
            }

            //Find the path with the least amount of move counts
            pathToNextItem = GetShortestRouteBetweenTwoPaths(keyPaths[0], keyPaths[1]);



        }

        /// <summary>
        /// Returns the path with the least amount of moves
        /// </summary>
        /// <param name="firstPath"></param>
        /// <param name="secondPath"></param>
        /// <returns></returns>
        public static List<Cell> GetShortestRouteBetweenTwoPaths(List<Cell> firstPath, List<Cell> secondPath)
        {
            //if the first is shorter, return that one, else return the second one
            return firstPath.Count < secondPath.Count ? firstPath : secondPath;
        }

        /// <summary>
        /// Handles what the wizard does
        /// </summary>
        public void Update()
        {
            //If the wizard has somewhere to go
            if (pathToNextItem != null && pathToNextItem.Count > 0)
            {
                Cell nextCell = pathToNextItem[0];

                //Move to the next cell
                position = nextCell.position;

                //Interact with the next cell
                InteractWithCell(nextCell);

                //Remove it from the list
                pathToNextItem.Remove(nextCell);
            }
            else
            {
                //If the game is still going (there are tasks to do)
                if (GridManager.formRef.levelIsPlaying)
                {
                    //Find the next item
                    FindClosestItemOfInterest();
                }

            }
        }
    }
}




