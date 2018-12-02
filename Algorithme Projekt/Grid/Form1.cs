using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grid
{
    public partial class Form1 : Form
    {
        public GridManager visualManager;


        //for taking time
        public Stopwatch stopWatch = new Stopwatch();

        //for cooldown, slowing the wizards move speed
        public float timeStamp = 0;

        Cell previousCell;

        //for showing passed time and determining coolddowns
        public float timeThatHasPassedInThisLevel = 0;

        //determines when the wizard starts moving
        public bool shouldStart;

        public float finalTime;
        private float highScore;

        public Form1()
        {
            InitializeComponent();

            //Sets the client size
            ClientSize = new Size(600, 600);

            //Instantiates the visual manager
            visualManager = new GridManager(CreateGraphics(), this.DisplayRectangle, this);
        }

        private void Loop_Tick(object sender, EventArgs e)
        {
            //Draws all our cells
            visualManager.Render();


            //Updates the wizard's movement once every 0.5 sec
            if (timeThatHasPassedInThisLevel > timeStamp)
            {
                Wizard.Instance.Update();
                timeStamp = stopWatch.ElapsedMilliseconds + 0;
            }

            this.Text = "Fastest Time: "+ highScore/1000+"  Current Time: "+(timeThatHasPassedInThisLevel / 1000).ToString();

            timeThatHasPassedInThisLevel = +stopWatch.ElapsedMilliseconds;

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
                if (shouldStart == false)
                {
                    StartGame();
                    shouldStart = true;
                }

            }
        }

        private void StartGame()
        {
            timeStamp = 0;
            timeThatHasPassedInThisLevel = 0;
            stopWatch.Restart();
            Wizard.Instance.FindClosestItemOfInterest();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Setup();
        }


        private void Setup()
        {
            if (!File.Exists("HighScore.txt"))
            {

                File.Create("HighScore.txt").Close();

                File.WriteAllText("HighScore.txt", int.MaxValue.ToString());

            }
            float.TryParse(File.ReadAllText("HighScore.txt"), out highScore);
        }

        internal void ReWriteHighScore()
        {
            float currentHighscore;

            float.TryParse(File.ReadAllText("HighScore.txt"), out currentHighscore);

            if (finalTime < currentHighscore)
            {
                File.WriteAllText("HighScore.txt", finalTime.ToString());

                highScore = finalTime;
            }
        }
    }
}
