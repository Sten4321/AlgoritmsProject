using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grid
{
    public partial class Form1 : Form
    {
        private GridManager visualManager;

        float timeStamp = 0;

        Cell previousCell;

        public Form1()
        {
            InitializeComponent();

            //Sets the client size
            ClientSize = new Size(800, 800);

            //Instantiates the visual manager
            visualManager = new GridManager(CreateGraphics(), this.DisplayRectangle);
        }

        private void Loop_Tick(object sender, EventArgs e)
        {
            //Draws all our cells
            visualManager.Render();

        }


        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //Checks if we clicked a cell
            visualManager.ClickCell(this.PointToClient(Cursor.Position));

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (GridManager.startCell != null && GridManager.goalCell != null)
                {
                    Astar.FindPath(GridManager.startCell, GridManager.goalCell);
                }

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
