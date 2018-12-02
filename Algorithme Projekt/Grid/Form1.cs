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
        public bool levelIsPlaying;

        public float finalTime;

        int algorithmRotationIndex = 0;

        private float AStarHighScore;
        private int AStarAttemptsCount;
        public float bFShighScore;
        public int bFSattemptsCount;

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
                //+ cooldown amount in miliseconds
                timeStamp = stopWatch.ElapsedMilliseconds + 250;
            }

            if (Wizard.Instance.pathFinder is Astar)
            {
                this.Text = "A-STAR - Fastest Time: " + AStarHighScore / 1000 + "  Attempts: " + AStarAttemptsCount + "  Current Time: " + (timeThatHasPassedInThisLevel / 1000).ToString();

            }
            else
            {
                this.Text = "BFS - Fastest Time: " + bFShighScore / 1000 + "  Attempts: " + bFSattemptsCount + "  Current Time: " + (timeThatHasPassedInThisLevel / 1000).ToString();

            }

            timeThatHasPassedInThisLevel = +stopWatch.ElapsedMilliseconds;

            //remove: automatically loops 
            if (levelIsPlaying == false)
            {

                if (algorithmRotationIndex ==0)
                {
                    Wizard.Instance.pathFinder = new Astar();
                    algorithmRotationIndex++;
                }
                else
                {
                    Wizard.Instance.pathFinder = new BFS();
                    algorithmRotationIndex = 0;
                }
               
                StartGame();
                levelIsPlaying = true;

            }

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
                if (levelIsPlaying == false)
                {
                    //Sets wizards strategy to astar, and executes its algorithmes                    
                    Wizard.Instance.pathFinder = new Astar();
                    StartGame();
                    levelIsPlaying = true;
                }

            }
            if (e.KeyCode == Keys.Back)
            {
                if (levelIsPlaying == false)
                {
                    //Sets wizards strategy to BFS, and executes its algorithmes                    

                    Wizard.Instance.pathFinder = new BFS();
                    StartGame();
                    levelIsPlaying = true;
                }

            }
        }

        /// <summary>
        /// Starts the level, and tells wizard to go bananas
        /// </summary>
        private void StartGame()
        {
            //reset time tracking
            timeStamp = 0;
            timeThatHasPassedInThisLevel = 0;
            stopWatch.Restart();

            //Go get them, wizard!
            Wizard.Instance.FindClosestItemOfInterest();

            if (Wizard.Instance.pathFinder is Astar)
            {
            AStarAttemptsCount++;

            }
            else
            {
                bFSattemptsCount++;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Setup();
        }

        /// <summary>
        /// Sets up the game
        /// </summary>
        private void Setup()
        {
            HighScoreSetUp();




        }

        private void HighScoreSetUp()
        {
            //if there is not a highscore file
            if (!File.Exists("AStarHighScore.txt"))
            {
                //make one
                File.Create("AStarHighScore.txt").Close();

                //write high number, so any new score will always be lower and 
                File.WriteAllText("AStarHighScore.txt", int.MaxValue.ToString() + ";0");

            }
            //if there is not a highscore file
            if (!File.Exists("BFSHighScore.txt"))
            {
                //make one
                File.Create("BFSHighScore.txt").Close();

                //write high number, so any new score will always be lower and 
                File.WriteAllText("BFSHighScore.txt", int.MaxValue.ToString() + ";0");

            }
            //Remember the highscore
            string[] AStarArray = File.ReadAllText("AStarHighScore.txt").Split(';');
            string[] BFSArray = File.ReadAllText("BFSHighScore.txt").Split(';');

            try
            {
                float.TryParse(AStarArray[0], out AStarHighScore);
                int.TryParse(AStarArray[1], out AStarAttemptsCount);
            }
            catch (Exception) //if the file format has been updated, but old highscore file still exists
            {
                //make one
                File.Create("AStarHighScore.txt").Close();

                //write high number, so any new score will always be lower and 
                File.WriteAllText("AStarHighScore.txt", int.MaxValue.ToString() + ";0");

                //tries again
                HighScoreSetUp();
            }
            try
            {
                float.TryParse(BFSArray[0], out bFShighScore);
                int.TryParse(BFSArray[1], out bFSattemptsCount);
            }
            catch (Exception) //if the file format has been updated, but old highscore file still exists
            {
                //make one
                File.Create("BFSHighScore.txt").Close();

                //write high number, so any new score will always be lower and 
                File.WriteAllText("BFSHighScore.txt", int.MaxValue.ToString() + ";0");

                //tries again
                HighScoreSetUp();
            }
        }

        /// <summary>
        /// Checks if the highscore should be overwritten, and does so if needed
        /// </summary>
        public void ReWriteHighScore(IFindPath pathType)
        {
            if (pathType is Astar)
            {
                //Finds the current highscore
                string[] textArray = File.ReadAllText("AStarHighScore.txt").Split(';');

                //the highscore string
                float.TryParse(textArray[0], out float currentHighscore);


                //If this time was faster
                if (finalTime < currentHighscore)
                {
                    //it's the new highscore
                    AStarHighScore = finalTime;
                }
                File.WriteAllText("AStarHighScore.txt", AStarHighScore.ToString() + ";" + AStarAttemptsCount);

                return;
            }
            else
            {

                //Finds the current highscore
                string[] textArray = File.ReadAllText("BFSHighScore.txt").Split(';');

                //the highscore string
                float.TryParse(textArray[0], out float currentHighscore);


                //If this time was faster
                if (finalTime < currentHighscore)
                {
                    //it's the new highscore
                    bFShighScore = finalTime;
                }
                File.WriteAllText("BFSHighScore.txt", bFShighScore.ToString() + ";" + bFSattemptsCount);
            }

        }
    }
}
