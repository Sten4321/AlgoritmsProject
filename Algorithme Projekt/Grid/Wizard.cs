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
        public Wizard(Point _coord, int spriteSize)
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
        }

        public void InteractWithCell(Cell cell)
        {
            switch (cell.MyType)
            {
                case CellType.KEY:
                    CollectKey(cell);
                    break;
                case CellType.TOWER:
                    if (keyCount == 2)
                    {
                        hasPotion = true;
                    }
                    break;
                case CellType.CRYSTAL:
                    if (hasPotion)
                    {
                        canEnterPortal = true;
                    }
                    break;
                case CellType.PORTAL:

                    if (canEnterPortal)
                    {
                        //GameOver
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Collects the key, and makes the key tile empty
        /// </summary>
        /// <param name="cell"></param>
        private void CollectKey(Cell cell)
        {
            keyCount++;

            foreach (Cell tmp in GridManager.grid)
            {
                if (tmp.position == cell.position)
                {
                    tmp.MyType = CellType.EMPTY;
                    tmp.AssignSprite();
                }
            }
        }
    }
}

