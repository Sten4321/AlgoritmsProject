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


        public Point position { get; set; }

        private Image sprite;

        private int spriteSize;

        private static Wizard instance;

        private List<Cell> pathToNextItem = new List<Cell>();

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

        public int keyCount = 0;

        public bool hasPotion = false;

        public bool canEnterPortal = false;



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

        public void Draw(Graphics dc)
        {
            if (sprite != null)
            {
                dc.DrawImage(sprite, BoundingRectangle);
            }
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
                    CollectKey(cell);
                    break;
                case CellType.TOWER:
                    //Wizard have the potion it needs for crystal
                    if (keyCount == 2)
                    {
                        hasPotion = true;
                    }
                    break;
                case CellType.CRYSTAL:
                    //if wizard have the potion, the wizard will  delivere the potion
                    if (hasPotion)
                    {
                        canEnterPortal = true;
                    }
                    break;
                case CellType.PORTAL:
                    // if it has delivered the potion, it may enter the portal, and reset the level
                    if (canEnterPortal)
                    {
                        GridManager.formRef.visualManager.ResetLevel();
                        instance = null;
                    }
                    break;

                case CellType.MONSTERCELL:
                    //Makes the monstercell Unwalkable and changes its sprite
                    ActivateMonsterCell(cell);
                    break;
                default:
                    break;
            }
        }

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
            Cell startCell = new Cell(new Point(0, 0), 0);

            foreach (Cell cell in GridManager.grid)
            {
                //find the cell you're currently standing on
                if (cell.position.X == position.X && cell.position.Y == position.Y)
                {
                    startCell = cell;
                    break;
                }
            }

            Cell targetCell = new Cell(new Point(0, 0), 0);
            CellType type = CellType.EMPTY;

            //Tries to find the next item in its sequence
            foreach (Cell cell in GridManager.grid)
            {
                if (cell.MyType == CellType.KEY && keyCount < 2)
                {
                    if (keyCount == 0)
                    {
                        FindClosestKey(startCell, cell);

                        return;
                    }
                    else
                    {
                        //FIND KEY
                        targetCell = cell;
                        type = CellType.KEY;
                        currentTaskText = "FIND KEY";
                        break;
                    }
                }
                if (cell.MyType == CellType.TOWER && keyCount == 2 && hasPotion == false)
                {
                    //FIND TOWER
                    targetCell = cell;
                    type = CellType.TOWER;
                    currentTaskText = "UNLOCK TOWER";
                    break;
                }
                if (cell.MyType == CellType.CRYSTAL && hasPotion == true && canEnterPortal == false)
                {
                    //FIND CRYSTAL
                    targetCell = cell;
                    type = CellType.CRYSTAL;
                    currentTaskText = "BRING POTION TO CRYSTAL";

                    break;
                }
                if (cell.MyType == CellType.PORTAL && canEnterPortal == true)
                {
                    //FIND CRYSTAL
                    targetCell = cell;
                    type = CellType.PORTAL;
                    currentTaskText = "ENTER PORTAL";

                    break;
                }
            }

            pathToNextItem = Astar.FindPath(startCell, targetCell, type);

        }

        /// <summary>
        /// Finds, and walks to the closest key
        /// </summary>
        /// <param name="startCell"></param>
        /// <param name="firstKey"></param>
        private void FindClosestKey(Cell startCell, Cell firstKey)
        {
            //Announcing the wizards task
            currentTaskText = "FIND KEY";

            //a list containing the two key paths
            List<List<Cell>> keyPaths = new List<List<Cell>>();

            //Adds the first path
            keyPaths.Add(Astar.FindPath(startCell, firstKey, CellType.KEY));

            //Then finds and adds the second
            foreach (Cell cell in GridManager.grid)
            {
                if (cell.MyType == CellType.KEY && cell != firstKey)
                {
                    keyPaths.Add(Astar.FindPath(startCell, cell, CellType.KEY));
                }
            }

            //Find the path with the least amount of move counts
            pathToNextItem = GetLeastMoves(keyPaths[0], keyPaths[1]);

          

        }

        /// <summary>
        /// Returns the path with the least amount of moves
        /// </summary>
        /// <param name="firstPath"></param>
        /// <param name="secondPath"></param>
        /// <returns></returns>
        public static List<Cell> GetLeastMoves(List<Cell> firstPath, List<Cell> secondPath)
        {
            return firstPath.Count < secondPath.Count ? firstPath : secondPath;
        }

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
                if (GridManager.formRef.shouldStart)
                {
                    FindClosestItemOfInterest();
                }

            }
        }
    }
}

