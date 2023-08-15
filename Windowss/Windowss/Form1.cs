using MemoryGame;
using MemoryGame.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Windowss
{
    public partial class Form1 : Form
    {
        private Bitmap GameImage = null;
        private int numRows = 3;
        private int numCols = 3;
        private String GameImagename = "no name";
        private SoundPlayer playMusic = null;
        private Settings settForm = new Settings();
        private BestScore best = new BestScore();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //stavljamo sliku na form-u
            Bitmap imageBack = Game.ResizeImage(MyResource.light_bulb,this.Width / 2, this.Height);
            this.BackgroundImage = imageBack;

            //stavljamo sliku na play
            Bitmap playButtonImage = Game.ResizeImage(MyResource.newgame, button1.Width, button1.Height);
            button1.BackgroundImageLayout = ImageLayout.Stretch;
            button1.BackgroundImage = playButtonImage;


            int formTop = (int)(this.Height * 0.03); // 3% od vrha
            int formLeft = (int)(this.Width * 0.03); // 3% sa leve stran

            this.Top = formTop;
            this.Left = formLeft;
            // resize block
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // set on center


            button1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button3.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button4.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button5.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(button1);
            this.Controls.Add(button2);
            this.Controls.Add(button3);
            this.Controls.Add(button4);
            this.Controls.Add(button5);

        }
       

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

      
        private void button1_Click_1(object sender, EventArgs e)
        {
            
            this.Hide();

            MemoryGame.Game newGame = new MemoryGame.Game(numRows, numCols, GameImage, GameImagename);
           
            if(newGame.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Show();
                best.SetBestScore();
            }
             
         }

        private void button3_Click(object sender, EventArgs e)
        {

           
            if (settForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.GameImage = settForm.gameImage;
                this.numRows = settForm.numRows;
                this.numCols = settForm.numColumns;
                this.GameImagename = settForm.nameOfImage;
                this.playMusic = settForm.playMusic;

            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            best.ShowDialog(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SerializableGame loadGame = Game.LoadGameState();
            if (loadGame != null)
            {
                this.Hide();
              
                Game gameSerialize = new Game(loadGame.Nrows, loadGame.Ncols, loadGame.ImageName1, loadGame.NumOfHits, loadGame.NumOfMoves, loadGame.Cards, loadGame.Time);
                if (gameSerialize.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.Show();
                    best.SetBestScore();
                }
            }
        }
    }
}
