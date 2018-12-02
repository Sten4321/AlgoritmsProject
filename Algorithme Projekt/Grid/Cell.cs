using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Grid.CellType;

namespace Grid
{
    public enum CellType { START, GOAL, WALL, EMPTY, KEY, TOWER, CRYSTAL, PORTAL, ROAD, TREE, WATER, MONSTERCELL };

    public class Cell
    {
        /// <summary>
        /// The grid position of the cell
        /// </summary>
        public Point position { get; set; }

        /// <summary>
        /// The size of the cell
        /// </summary>
        private int cellSize;

        /// <summary>
        /// The cell's sprite
        /// </summary>
        public Image sprite;

        //For keys to remember their sprites
        public CellType initialType;

        /// <summary>
        /// the type of cell
        /// </summary>
        public CellType MyType { get; set; } = EMPTY;

        public Cell Parrent { get; set; }//stores the parrent cell
        public int HValue { get; set; }//stores the Hvalues of cell
        public int GValue { get; set; }//stores the gValue of the field
        public int FValue { get { return GValue + HValue; } }////stores the Fvalue of field

        /// <summary>
        /// The bounding rectangle of the cell
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle(position.X * cellSize, position.Y * cellSize, cellSize, cellSize);
            }
        }

        /// <summary>
        /// The cell's constructor
        /// </summary>
        /// <param name="position">The cell's grid position</param>
        /// <param name="size">The cell's size</param>
        public Cell(Point position, int size)
        {
            //Sets the position
            this.position = position;

            //Sets the cell size
            this.cellSize = size;

            AssignSprite();

        }



        public void AssignSprite()
        {
            switch (MyType)
            {
                case CellType.WATER:
                    FindCorrectWaterSprite();
                    break;
                case CellType.ROAD:
                    sprite = Image.FromFile(@"Images\Road.png");
                    break;
                case CellType.WALL:
                    sprite = Image.FromFile(@"Images\Road.png");

                    break;
                case CellType.TREE:
                    sprite = Image.FromFile(@"Images\Tree.png");
                    break;
                case CellType.EMPTY:
                    sprite = Image.FromFile(@"Images\Grass.png");
                    break;
                case CellType.KEY:
                    sprite = Image.FromFile(@"Images\Key.png");

                    break;
                case CellType.TOWER:
                    sprite = Image.FromFile(@"Images\Tower.png");

                    break;
                case CellType.CRYSTAL:
                    sprite = Image.FromFile(@"Images\Crystal.png");

                    break;
                case CellType.PORTAL:
                    sprite = Image.FromFile(@"Images\Portal.png");

                    break;
                
                case CellType.MONSTERCELL:
                    sprite = Image.FromFile(@"Images\Road.png");

                    break;


                default:
                    sprite = Image.FromFile(@"Images\Start.png");

                    break;
            }
        }

        /// <summary>
        /// Assigns the correct waterSprites to cells of the type Water
        /// </summary>
        private void FindCorrectWaterSprite()
        {
            // If my mother knew i coded this, she'd disown me

            if (position.X == 5 && (position.Y > 1 || position.Y < 7))
            {
                sprite = Image.FromFile(@"Images\Water.png");
            }
            if (position.X == 5 && (position.Y == 1))
            {
                sprite = Image.FromFile(@"Images\WaterEdge.png");
                sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);

            }
            if (position.X == 5 && (position.Y == 6))
            {
                sprite = Image.FromFile(@"Images\WaterEdge.png");

            }
            if (position.X == 4 && (position.Y > 1 && position.Y < 6))
            {
                sprite = Image.FromFile(@"Images\WaterEdge.png");
                sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);

            }
            if (position.X == 6 && (position.Y > 1 && position.Y < 6))
            {
                sprite = Image.FromFile(@"Images\WaterEdge.png");
                sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);

            }
            if (position.X == 4 && position.Y == 1)
            {
                sprite = Image.FromFile(@"Images\WaterCorner.png");
                sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);


            }
            if (position.X == 6 && position.Y == 1)
            {
                sprite = Image.FromFile(@"Images\WaterCorner.png");
                sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);


            }
            if (position.X == 6 && position.Y == 6)
            {
                sprite = Image.FromFile(@"Images\WaterCorner.png");
                sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            if (position.X == 4 && position.Y == 6)
            {
                sprite = Image.FromFile(@"Images\WaterCorner.png");
            }
        }

        /// <summary>
        /// Renders the cell
        /// </summary>
        /// <param name="dc">The graphic context</param>
        public void Render(Graphics dc)
        {
            //Draws the rectangles color
            if (Astar.path.Count > 0 && Astar.path.Contains(this))
            {
                dc.FillRectangle(new SolidBrush(Color.LightGreen), BoundingRectangle);
            }
            else if (Astar.ClosedList.Count > 0 && Astar.ClosedList.Contains(this))
            {
                dc.FillRectangle(new SolidBrush(Color.Blue), BoundingRectangle);
            }
            else if (Astar.openList.Count > 0 && Astar.openList.Contains(this))
            {
                dc.FillRectangle(new SolidBrush(Color.LightBlue), BoundingRectangle);
            }
            else
            {
                dc.FillRectangle(new SolidBrush(Color.White), BoundingRectangle);
            }

            //Draws the rectangles border
            dc.DrawRectangle(new Pen(Color.Black), BoundingRectangle);

            //If the cell has a sprite, then we need to draw it
            if (sprite != null)
            {
                dc.DrawImage(sprite, BoundingRectangle);
            }


            //Write's the cells grid position
            dc.DrawString(string.Format("{0}", position), new Font("Arial", 6, FontStyle.Regular), new SolidBrush(Color.Black), position.X * cellSize, (position.Y * cellSize) + 1);

#if DEBUG
            if (FValue != 0)
            {
                try
                {
                    dc.DrawString(string.Format("G: {0}", GValue), new Font("Arial", 6, FontStyle.Regular), new SolidBrush(Color.Black), position.X * cellSize, (position.Y * cellSize) + 9);
                    dc.DrawString(string.Format("H: {0}", HValue), new Font("Arial", 6, FontStyle.Regular), new SolidBrush(Color.Black), position.X * cellSize, (position.Y * cellSize) + 16);
                    dc.DrawString(string.Format("F: {0}", FValue), new Font("Arial", 6, FontStyle.Regular), new SolidBrush(Color.Black), position.X * cellSize, (position.Y * cellSize) + 23);
                }
                catch { }
            }
#endif
        }



       

        public override string ToString()
        {
            return MyType.ToString() + "  X: " + position.X + "  y: " + position.Y;
        }
    }
}
