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
        public int keyCount = 0;

        public bool hasPotion = false;

        public bool canEnterPortal = false;

        //Coordinate on Grid
        public Point Coordinate { get; set; }

        /// <summary>
        /// Wizard Constructor
        /// </summary>
        /// <param name="_coord">Starting coordinate</param>
        public Wizard(Point _coord)
        {
            Coordinate = _coord;
        }

        public void EvaluateNewCell(Cell cell)
        {
            switch (cell.MyType)
            {

                case CellType.KEY:
                    keyCount++;
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
    }
}

