using MemoryGame.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Drawing.Imaging;
using System.Media;
using System.Reflection;
using Windowss.Classes;
using Windowss;
namespace MemoryGame
{
   
    public partial class Game : Form
    {
        //Globalne promenljive
      
        Random random = new Random();
        PictureBox firstClicked, secondClicked;
        PictureBox copyFirst, copySecond;
        Bitmap[,] croppedImages;
        List<Card> cards;
        static Bitmap GameImageNotOpen = GameResource.Neotvoreno;
        Bitmap GameImage;
        string ImageName;
        int numOfHits = 0;
        int numOfMoves = 0;
        int numOfRows;
        int numOfCol;

        private PictureBox firstFadePictureBox;
        private PictureBox secondFadePictureBox;

        private PictureBox pb1, pb2;

        private int fadeSteps = 10; // Broj koraka za animaciju nestajanja
        private int currentStep = 0; // Trenutni korak animacije

        private Stack<Tuple<PictureBox, Image>> history = new Stack<Tuple<PictureBox, Image>>();
        private Stack<Tuple<PictureBox, Image>> historyOfUndo = new Stack<Tuple<PictureBox, Image>>();


        //muzika
        SoundPlayer playerCorrect = null;
        SoundPlayer playerWrong = null;
        Boolean soundOnOff = true;
        


        public Game(int numOfRows, int numOfCol,Bitmap GameImage, string nameOfImage)
        {
           
            InitializeComponent();
            playerWrong =  new SoundPlayer(Music.wrong_buzzer);
            playerCorrect = new SoundPlayer(Music.correct_choice);
         

            timer1.Start();
            timer2.Start();

            InitializeGameState(numOfRows, numOfCol, GameImage, nameOfImage);
            EnableDisablePictureClik("no");

           
     
        }
        //Ovo je za Serializaciju
        public Game(int numOfRows, int numOfCol, String nameOfImage, int numOfHits, int Moves, List<CardSerializable> cards, double time)
        {
            
            InitializeComponent();
            playerWrong = new SoundPlayer(Music.wrong_buzzer);
            playerCorrect = new SoundPlayer(Music.correct_choice);


            timer1.Start();
            timer2.Start();

            InitializeGameStateSerializable(numOfRows, numOfCol, nameOfImage, numOfHits, Moves, cards, time);
            EnableDisablePictureClik("no");


        }

     
        private void Form1_Load(object sender, EventArgs e)
        {

            int formTop = (int)(this.Height * 0.03); // 3% od vrha
            int formLeft = (int)(this.Width * 0.03); // 3% sa leve stran
     

          
            this.Top = formTop;
            this.Left = formLeft;

            panel3.Width = this.Width;
            panel2.Width = panel3.Width - tableLayoutPanel1.Width;
            


            // resize block
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // paneli ugasi
            panel4.Visible = false;
            panel5.Visible = false;
            panel7.Visible = false;


            // center game 
            label2.Anchor = AnchorStyles.None;
            label2.TextAlign = ContentAlignment.MiddleCenter;

            panel2.Controls.Add(label2);

           
            label2.Left = (panel2.ClientSize.Width - label2.Width) / 2;
            label2.Top = (panel2.ClientSize.Height - label2.Height) / 2;
            setVoiceIcon();
        }
        private void setVoiceIcon()
        {
            Bitmap image;
            if (soundOnOff)
                image = GameResource.soundOn;
            else
                image = GameResource.soundOff;

            image = ResizeImage(image, (int)(button4.Width - (button4.Width * 0.3)), (int)(button4.Height - (button4.Height * 0.2)));
            button4.Image = image;
        }
        //dodeli fotografiju
        private void TakeValue(Bitmap GameImage, String ImageName, int numRows, int numColumns)
        {

            this.GameImage = GameImage;
            this.ImageName = ImageName;
            this.numOfRows = numRows;
            this.numOfCol = numColumns;
        }

        private void EnableDisablePictureClik(String ans)
        {
            PictureBox pictureBox;


            for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
            {

                if (tableLayoutPanel1.Controls[i] is PictureBox)
                    pictureBox = (PictureBox)tableLayoutPanel1.Controls[i];
                else
                    continue;

                if (ans.Equals("yes"))
                    pictureBox.Enabled = true;
                else
                    pictureBox.Enabled = false;
            }

        }
        private void setDefaultImages()
        {
            
            PictureBox pictureBox;
            for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
            {

                if (tableLayoutPanel1.Controls[i] is PictureBox)
                    pictureBox = (PictureBox)tableLayoutPanel1.Controls[i];
                else
                    continue;


             
                pictureBox.BackgroundImage = GameImageNotOpen;
                
            }
            tableLayoutPanel1.BackgroundImage = ResizeImage(GameResource.light_bulb, tableLayoutPanel1.Width, tableLayoutPanel1.Height);
        }
       
        private void PictureClick(object sender, EventArgs e)
        {

            
            PictureBox clickedPic = sender as PictureBox;
            Tuple<bool, Image> answer;
            if (clickedPic != null) 
            {
                if (firstClicked == null)
                { 
                    firstClicked = clickedPic;
                    firstClicked.BackgroundImage = FindImage(firstClicked);

                    //zabraniti da se moze kliknuti na undo i redo
                    button2.Enabled = false;
                    button3.Enabled = false;
            
                }
                else
                {

                    if (firstClicked != clickedPic)
                    {
                        secondClicked = clickedPic;
                        answer = IsImageMatching(firstClicked, secondClicked);
                        button2.Enabled = true;
                        button3.Enabled = true;
                        if (answer.Item1 == true)
                        {

                            //Pusti muziku
                            if(playerCorrect != null)
                                playerCorrect.Play();
                            history.Clear();
                            historyOfUndo.Clear();
                           
                            copyFirst = firstClicked;
                            copySecond = secondClicked;
                            //timerForEquality.Start();
                           
                            // staviti da je card open true;
                            Card Card1 = cards.FirstOrDefault(x => x.PictureBox.Equals(firstClicked));
                            Card Card2 = cards.FirstOrDefault(x => x.PictureBox.Equals(secondClicked));
                            Card1.SetCardVisibility(true);
                            Card2.SetCardVisibility(true);

                            firstClicked.BackgroundImage = answer.Item2;
                            secondClicked.BackgroundImage = answer.Item2;

                            
                            firstClicked.Enabled = false;
                            secondClicked.Enabled = false;
                            
                            FadeOutPictureBox(firstClicked, secondClicked);
                            currentStep = 0;

                           

                            setNumOfMovesAndHits(++numOfHits, ++numOfMoves);

                            if (GameEnd())
                            {
                                //nema potrebe za serializacijom igra je gotova!
                                if (File.Exists("gamestate.dat"))
                                {
                                    File.Delete("gamestate.dat");
                                }

                                timer3.Stop();
                                DialogResult sacuvajRezultatRezultat = MessageBox.Show("Želite li da sačuvate rezultat?", "Sačuvaj rezultat", MessageBoxButtons.YesNo);
                                if (sacuvajRezultatRezultat == DialogResult.Yes)
                                {
                                   
                                    string imeIgraca = ShowInputDialog("Unesite ime igrača:", "Unos imena");

                                    if (!string.IsNullOrEmpty(imeIgraca))
                                    {
                                        
                                        double vreme = Convert.ToDouble(label13.Text);
                                        int potezi = Convert.ToInt32(label8.Text);
                                        string nazivSlike = Convert.ToString(label10.Text);
                                         string tabel = "";
                                        if (numOfRows == 3 && numOfCol == 3)
                                            tabel = "3x3x2";
                                        else if (numOfRows == 3 && numOfCol == 4)
                                            tabel = "3x4x2";
                                        else
                                            tabel = "4x4x2";

                                        double RezultatGame = 0;
                                        RezultatGame = Rezultati.calculateRezultat(potezi, vreme, tabel);
                                     

                                        Rezultati newrezultat = new Rezultati(nazivSlike,imeIgraca, tabel,RezultatGame,potezi,vreme);
                                        CommunicationWithDatabase comm = new CommunicationWithDatabase();
                                        comm.insertRezultat(newrezultat);
                                    }
                                }


                                DialogResult rezultat = MessageBox.Show("Želite li da igrate igru ponovo?  Vase vreme je " + label13, "Nova igra", MessageBoxButtons.YesNo);

                                        if (rezultat == DialogResult.Yes)
                                        {
                                            // Pokretanje nove igre
                                            ResetGame(); // Implementirajte funkciju za resetovanje igre prema potrebama
                                        }
                                        else
                                        {
                                            //sakrij applikaciju
                                            //this.Hide();
                                            this.DialogResult = System.Windows.Forms.DialogResult.OK;
                                            this.Close();

                                          }
                                    
                                
                            }
                            //da moze da se klikne na undo redo
                            

                        }
                        else
                        { 
                            secondClicked.BackgroundImage = FindImage(secondClicked);
                            timerForNotEqual.Start();
                            if(playerWrong != null)
                                playerWrong.Play();
                            pb1 = firstClicked;
                            pb2 = secondClicked;
                            setNumOfMovesAndHits(numOfHits, ++numOfMoves);
                        }
                        //novo
                        history.Push(new Tuple<PictureBox, Image>(firstClicked, firstClicked.BackgroundImage));
                        history.Push(new Tuple<PictureBox, Image>(secondClicked, secondClicked.BackgroundImage));
                        //
                       
                        firstClicked = null;
                        secondClicked = null;
                        

                    }

                }
            }

 
        }
        
     
        private void table_3_3()
        {
           
            RemoveRows(tableLayoutPanel1, tableLayoutPanel1.RowCount - 1, 1);
            HideLastRows(tableLayoutPanel1, 1);

            RemoveColumns(tableLayoutPanel1, 6, 2);
            HideLastColumns(tableLayoutPanel1, 2);
           
            label12.Text = "3x3x2";
        }
        private void table_3_4()
        {
            RemoveRows(tableLayoutPanel1, tableLayoutPanel1.RowCount - 1, 1);
            HideLastRows(tableLayoutPanel1, 1);

            label12.Text = "3x4x2";
        }
        private void table_4_4()
        {
            label12.Text = "4x4x2";
        }
        public static Bitmap ResizeImage(Bitmap originalImage, int newWidth, int newHeight)
        {
            
            Bitmap resizedImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return resizedImage;
        }
        private void InitializeGameState(int numRows, int numColumns, Bitmap GameImage, String nameOfImage)
        {

            TakeValue(GameImage, nameOfImage, numRows, numColumns);

            int cellWidth = 1;
            int cellHeight = 1;

            if (GameImage != null)
            {
                GameImage = ResizeImage(GameImage, tableLayoutPanel1.Width, tableLayoutPanel1.Height);
                cellWidth = (int)(GameImage.Width / numColumns); // Širina svakog dela
                cellHeight = (int)(GameImage.Height / numRows); // Visina svakog dela
            }
            
            if (numRows == 4 && numColumns == 4)
            {
                table_4_4();
            }

            else if (numRows == 3 && numColumns == 4)
            {
                table_3_4();
            }
            else if (numRows == 3 && numColumns == 3)
            {
                table_3_3();
            }
            else throw new Exception("Nema ta raspodela na board-u");


            //bitno 
            GameImageNotOpen = ResizeImage(GameImageNotOpen, (int)tableLayoutPanel1.Controls[1].Width, (int)tableLayoutPanel1.Controls[1].Height);

            // staviti nameOfIamge on center
            int size = 0;
            if (nameOfImage == null || nameOfImage.Equals("no name"))
            { 
                label10.Text = "Animals";
                size = 6;

            }
            else
                label10.Text = nameOfImage;

            label10.Dock = DockStyle.Fill;
            label10.TextAlign = ContentAlignment.MiddleCenter;
            int fontSize = CalculateFontSizeToFit(label10, panel8.ClientRectangle.Size);
            label10.Font = new Font(label10.Font.FontFamily, fontSize - size);
           
           
      
           

            cards = new List<Card>();
            croppedImages = new Bitmap[numRows, numColumns];

       

            for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numColumns; j++)
                    {
                        if (GameImage != null)
                        {
                            Rectangle cropRect = new Rectangle(j * cellWidth, i * cellHeight, cellWidth, cellHeight);
                            croppedImages[i, j] = GameImage.Clone(cropRect, GameImage.PixelFormat);
                            croppedImages[i, j] = ResizeImage(croppedImages[i, j], (int)tableLayoutPanel1.Controls[1].Width, (int)tableLayoutPanel1.Controls[1].Height);
                        }
                        else
                        {

                            int value = i * numColumns + j + 1;
                            String valueS = value.ToString();
                            croppedImages[i, j] = (Bitmap)AnimalsResources.ResourceManager.GetObject(valueS);
                            croppedImages[i, j] = ResizeImage(croppedImages[i, j], (int)tableLayoutPanel1.Controls[1].Width, (int)tableLayoutPanel1.Controls[1].Height);
                            

                        }
                    }

                }
            
            PictureBox pictureBox;
            int randomNumberI;
            int randomNumberJ;
        
            int[,] usedPicureHelp = new int[numRows,numColumns];

            //initializing  usedPicutreHelp all 0

            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    usedPicureHelp[i, j] = 0;

            for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
            {

                if (tableLayoutPanel1.Controls[i] is PictureBox)
                    pictureBox = (PictureBox)tableLayoutPanel1.Controls[i];
                else
                    continue;

                 randomNumberI = random.Next(0, numRows); 
                 randomNumberJ = random.Next(0, numColumns);
                
                
                 
                while(usedPicureHelp[randomNumberI, randomNumberJ] > 1)
                {
                    randomNumberI = random.Next(0, numRows);
                    randomNumberJ = random.Next(0, numColumns);

                }
                usedPicureHelp[randomNumberI, randomNumberJ]++;


                //zamena sa onim***********
                TableLayoutPanelCellPosition cellPosition = tableLayoutPanel1.GetCellPosition(pictureBox);

                int row = cellPosition.Row;
                int column = cellPosition.Column;

                cards.Add(new Card(row, column, croppedImages[randomNumberI, randomNumberJ], pictureBox, false));
                pictureBox.BackgroundImage = croppedImages[randomNumberI, randomNumberJ];

                
            }
         
        }
      
        private Tuple<Boolean,Image> IsImageMatching(PictureBox clk1, PictureBox clk2)
        {

            Card card1 = null;
            Card card2 = null;
          
            foreach (var card in cards)
            {
                if (clk1.Equals(card.PictureBox))
                {
                    card1 = card;

                }
                if (clk2.Equals(card.PictureBox))
                {
                    card2 = card;
                }
            }
           
            if (card1 == null) return new Tuple<bool, Image>(false, null); 
            if(card2 == null) return new Tuple<bool, Image>(false, null);
            
            if (card1.MyPicture.Equals(card2.MyPicture))
                return new Tuple<bool, Image>(true, card1.MyPicture);
        
    
            return new Tuple<bool, Image>(false, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            setDefaultImages();
            timer1.Stop();
            EnableDisablePictureClik("yes");

            // pokazi panel4 i panel5
            panel4.Visible = true;
            panel5.Visible = true;
            panel7.Visible = true;
        }

        private Image FindImage(PictureBox clk)
        {
          
            foreach (var card in cards)
            {
                if (clk.Equals(card.PictureBox))
                    return card.MyPicture;
            }
            return null;
        }
        private void setNumOfMovesAndHits(int numHits, int numMoves)
        {
          
            label6.Text = Convert.ToString(numHits);
            label8.Text = Convert.ToString(numMoves);

        }
        private Boolean GameEnd()
        {

            int count = numOfCol * numOfRows;
            for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
            {
                if (tableLayoutPanel1.Controls[i] is PictureBox pictureBox)
                {
                    if (numOfHits == count)
                        return true;
                    if (pictureBox.BackgroundImage == null || pictureBox.BackgroundImage.Equals(GameImageNotOpen))
                    {
                        // Ako se pronadje barem jedan PictureBox koji nije zatvoren, igra još uvek traje
                        return false;
                    }
                  
                }
            }

            // Ako su svi PictureBox-i zatvoreni, igra je gotova
            return true;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int timer = Convert.ToInt32(label4.Text);
            timer = timer - 1;
            label4.Text = Convert.ToString(timer);
            if (timer == 0)
            {
                timer2.Stop();
                label4.Visible = false;
                label3.Visible = false;
                timer3.Start();
               
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveGameState();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
            //Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UndoLastMove();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RedoLastMove();
        }

        private void IncreaseTime(object sender, EventArgs e)
        {
           
            double timerClock = Convert.ToDouble(label13.Text);
            timerClock = timerClock + 0.1;
            label13.Text = Convert.ToString(timerClock);
        }

        private void ResetGame()
        {



            //prekriti numofmoves and hits
            panel4.Visible = false;
            panel5.Visible = false;
            panel7.Visible = false;

            //setovati skor i pokusaje na 0
            numOfHits = 0;
            numOfMoves = 0;
            label6.Text = "0";
            label8.Text = "0";
            label13.Text = "0.0";
            //tajmer prikazati na 5 i prikazati ga
            label4.Text = "5";
            label3.Visible = true;
            label4.Visible = true;

            ClearTableLayoutPanel(tableLayoutPanel1);
            CreateNewElements(tableLayoutPanel1, 4, 4 * 2);
            timer1.Start(); 
            timer2.Start();

            InitializeGameState(numOfRows, numOfCol, GameImage, ImageName);
            EnableDisablePictureClik("no");
            currentStep = 0;


        }

        private void ClearTableLayoutPanel(TableLayoutPanel tableLayoutPanel)
        {
            tableLayoutPanel.SuspendLayout();

            // ocisti TableLayoutPanel
            tableLayoutPanel.Controls.Clear();
            tableLayoutPanel.RowStyles.Clear();
            tableLayoutPanel.ColumnStyles.Clear();

            tableLayoutPanel.ResumeLayout();
        }
        private void FadeOutPictureBox(PictureBox pictureBox1, PictureBox pictureBox2)
        {
            firstFadePictureBox = pictureBox1;
            secondFadePictureBox = pictureBox2;
            currentStep = 0;
            timerForEquality.Start();
        }
        private void timerForEquality_Tick(object sender, EventArgs e)
        {
            currentStep++;
            float opacity = 1.0f - (float)currentStep / fadeSteps;

            if (opacity <= 0)
            {

                //Ugasi zvuk
                if(playerCorrect != null)
                   playerCorrect.Stop();
                //Zaustavi tajmer
                timerForEquality.Stop();

                // Postavljanje vidljivosti kliknutih PictureBox-ova na false
                if (firstFadePictureBox != null)
                    firstFadePictureBox.Visible = false;

                if (secondFadePictureBox != null)
                    secondFadePictureBox.Visible = false;

                firstFadePictureBox = null;
                secondFadePictureBox = null;
            }
            else
            {
                // Postavljanje promene opaciteta samo za kliknute PictureBox-ove
                if (firstFadePictureBox != null)
                    firstFadePictureBox.BackgroundImage = ChangeImageOpacity(firstFadePictureBox.BackgroundImage, opacity);

                if (secondFadePictureBox != null)
                    secondFadePictureBox.BackgroundImage = ChangeImageOpacity(secondFadePictureBox.BackgroundImage, opacity);
            }
        }
        private void timerForNotEqual_Tick(object sender, EventArgs e)
        {
            pb1.BackgroundImage = pb2.BackgroundImage = GameImageNotOpen;

            timerForNotEqual.Stop();
            if(playerWrong != null)
                playerWrong.Stop();
        }

        private void CreateNewElements(TableLayoutPanel tableLayoutPanel, int numRows, int numColumns)
        {
            tableLayoutPanel.SuspendLayout();

           
            tableLayoutPanel.RowCount = numRows;
            tableLayoutPanel.ColumnCount = numColumns;

            for (int row = 0; row < numRows; row++)
            {
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / numRows));
            }

            for (int col = 0; col < numColumns; col++)
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / numColumns));
            }

            // dodavanje PictureBox u TableLayoutPanel
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Dock = DockStyle.Fill;
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox.Click += new System.EventHandler(this.PictureClick);
                    tableLayoutPanel.Controls.Add(pictureBox, col, row);
                }
            }

            tableLayoutPanel.ResumeLayout();
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="opacity"></param>
        /// <returns></returns>
        private Bitmap ChangeImageOpacity(Image image, float opacity)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity;

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                graphics.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }

            return bmp;
        }
        public static int CalculateFontSizeToFit(System.Windows.Forms.Label label, Size containerSize)
        {
            int fontSize = 10; // Početna veličina fonta
            Font testFont = new Font(label.Font.FontFamily, fontSize);

            while (label.CreateGraphics().MeasureString(label.Text, testFont).Width < containerSize.Width)
            {
                fontSize++;
                testFont = new Font(label.Font.FontFamily, fontSize);
            }

            return fontSize - 1;
        }
        //*************************************************************

        /// <summary>
        ///  <param name="SaveGameDialog"></param>
        /// </summary>

        private string ShowInputDialog(string caption, string promptText)
        {
            Form inputForm = new Form();
            inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputForm.MaximizeBox = false;
            inputForm.MinimizeBox = false;
            inputForm.StartPosition = FormStartPosition.CenterParent;
            inputForm.Text = caption;

            Label label = new Label() { Left = 20, Top = 20, Width = 200, Text = promptText };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 200 };
            Button okButton = new Button() { Text = "OK", Left = 50, Width = 70, Top = 80, DialogResult = DialogResult.OK };
            Button cancelButton = new Button() { Text = "Cancel", Left = 150, Width = 70, Top = 80, DialogResult = DialogResult.Cancel };

            okButton.Click += (sender, e) => inputForm.Close();
            cancelButton.Click += (sender, e) => inputForm.Close();

            inputForm.Controls.Add(label);
            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(okButton);
            inputForm.Controls.Add(cancelButton);

            inputForm.AcceptButton = okButton;
            inputForm.CancelButton = cancelButton;

            return inputForm.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
        }
        //*************************************************************

       

        /// <summary>
        /// <param name="RedoAndUndo"></param>
        /// </summary>
        private void UndoLastMove()
        {
            EnableDisablePictureClik("yes");
            if (history.Count >= 2)
            {
                Tuple<PictureBox, Image> lastMove2 = history.Pop();
                Tuple<PictureBox, Image> lastMove1 = history.Pop();
                Tuple<bool, Image> answer = IsImageMatching(lastMove1.Item1, lastMove2.Item1);
                if (answer.Item1)
                {
                    //novo
                    historyOfUndo.Push(new Tuple<PictureBox, Image>(lastMove2.Item1, lastMove2.Item2));
                    historyOfUndo.Push(new Tuple<PictureBox, Image>(lastMove1.Item1, lastMove1.Item2));
                    //


                    PictureBox help1 = lastMove1.Item1;
                    PictureBox help2 = lastMove2.Item1;

                    help1.Visible = true;
                    help2.Visible = true;


                    lastMove2.Item1.BackgroundImage = GameImageNotOpen;
                    lastMove1.Item1.BackgroundImage = GameImageNotOpen;

                    numOfHits--;
                    numOfMoves--;
                    setNumOfMovesAndHits(numOfHits, numOfMoves);

                    // staviti da je card open false;
                    Card Card1 = cards.FirstOrDefault(x => x.PictureBox.Equals(help1));
                    Card Card2 = cards.FirstOrDefault(x => x.PictureBox.Equals(help2));
                    Card1.SetCardVisibility(false);
                    Card2.SetCardVisibility(false);
                }

                //numOfMoves--;
                // setNumOfMovesAndHits(numOfHits, numOfMoves);
            }

        }
        private void RedoLastMove()
        {
            if (historyOfUndo.Count >= 2)
            {
                Tuple<PictureBox, Image> redoMove1 = historyOfUndo.Pop();
                Tuple<PictureBox, Image> redoMove2 = historyOfUndo.Pop();

                redoMove1.Item1.Visible = true;
                redoMove2.Item1.Visible = true;

                redoMove1.Item1.BackgroundImage = FindImage(redoMove1.Item1);
                redoMove2.Item1.BackgroundImage = FindImage(redoMove2.Item1);


                redoMove1.Item1.Visible = false;
                redoMove2.Item1.Visible = false;

                // Povećati broj poteza i broj pogodaka na osnovu ove akcije

                numOfMoves++;
                numOfHits++;
                setNumOfMovesAndHits(numOfHits, numOfMoves);
                history.Push(new Tuple<PictureBox, Image>(redoMove2.Item1, redoMove2.Item2));
                history.Push(new Tuple<PictureBox, Image>(redoMove1.Item1, redoMove1.Item2));

                Card Card1 = cards.FirstOrDefault(x => x.PictureBox.Equals(redoMove1));
                Card Card2 = cards.FirstOrDefault(x => x.PictureBox.Equals(redoMove2));
                Card1.SetCardVisibility(true);
                Card2.SetCardVisibility(true);
            }
        }
        //*************************************************************


        /// <summary>
        /// 
        /// </summary>
        /// <param name="RemoveColumns"></param>
        /// <param name="RemoveRows"></param>
        /// <param name="HideColumnsAndRows"></param>
        private void RemoveColumns(TableLayoutPanel tableLayoutPanel, int columnIndex, int count)
        {
            if (columnIndex < 0 || columnIndex >= tableLayoutPanel.ColumnCount || count <= 0 || columnIndex + count > tableLayoutPanel.ColumnCount)
                return;

            tableLayoutPanel.SuspendLayout();


            for (int row = 0; row < tableLayoutPanel.RowCount; row++)
            {
                for (int i = columnIndex + count - 1; i >= columnIndex; i--)
                {
                    if (i >= 0 && i < tableLayoutPanel.ColumnCount)
                    {
                        Control control = tableLayoutPanel.GetControlFromPosition(i, row);
                        if (control != null)
                        {

                            control.Dispose();
                            tableLayoutPanel.Controls.Remove(control);
                        }
                    }
                }
            }


            for (int i = 0; i < count && columnIndex < tableLayoutPanel.ColumnStyles.Count; i++)
            {
                tableLayoutPanel.ColumnStyles.RemoveAt(columnIndex);
            }


            int totalColumns = tableLayoutPanel.ColumnCount;
            float percentage = 100f / totalColumns;

            tableLayoutPanel.ColumnStyles.Clear();
            for (int i = 0; i < totalColumns; i++)
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percentage));
            }

            tableLayoutPanel.ResumeLayout();
        }

        private void RemoveRows(TableLayoutPanel tableLayoutPanel, int rowIndex, int count)
        {
            if (rowIndex < 0 || rowIndex >= tableLayoutPanel.RowCount || count <= 0 || rowIndex + count > tableLayoutPanel.RowCount)
                return;

            tableLayoutPanel.SuspendLayout();

            // Step 1: Remove the controls in the target rows
            for (int col = 0; col < tableLayoutPanel.ColumnCount; col++)
            {
                for (int i = rowIndex + count - 1; i >= rowIndex; i--)
                {
                    if (i >= 0 && i < tableLayoutPanel.RowCount)
                    {
                        Control control = tableLayoutPanel.GetControlFromPosition(col, i);
                        if (control != null)
                        {
                            // Optionally, dispose of the control to release resources
                            control.Dispose();
                            tableLayoutPanel.Controls.Remove(control);
                        }
                    }
                }
            }

            // Step 2: Remove the RowStyles for the target rows
            for (int i = 0; i < count && rowIndex < tableLayoutPanel.RowStyles.Count; i++)
            {
                tableLayoutPanel.RowStyles.RemoveAt(rowIndex);
            }

            // Step 3: Adjust the remaining RowStyles to update the layout
            int totalRows = tableLayoutPanel.RowCount;
            float percentage = 100f / totalRows;

            tableLayoutPanel.RowStyles.Clear();
            for (int i = 0; i < totalRows; i++)
            {
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, percentage));
            }

            tableLayoutPanel.ResumeLayout();
        }
        private void HideLastColumns(TableLayoutPanel tableLayoutPanel, int count)
        {
            if (count <= 0 || count > tableLayoutPanel.ColumnCount)
                return;

            tableLayoutPanel.SuspendLayout();

            // Step 1: Set the width of the last 'count' columns to zero
            for (int i = tableLayoutPanel.ColumnCount - count; i < tableLayoutPanel.ColumnCount; i++)
            {
                tableLayoutPanel.ColumnStyles[i] = new ColumnStyle(SizeType.Absolute, 0);
            }

            tableLayoutPanel.ResumeLayout();
        }
        private void HideLastRows(TableLayoutPanel tableLayoutPanel, int count)
        {
            if (count <= 0 || count > tableLayoutPanel.RowCount)
                return;

            tableLayoutPanel.SuspendLayout();

            // Step 1: Set the height of the last 'count' rows to zero
            for (int i = tableLayoutPanel.RowCount - count; i < tableLayoutPanel.RowCount; i++)
            {
                tableLayoutPanel.RowStyles[i] = new RowStyle(SizeType.Absolute, 0);
            }

            tableLayoutPanel.ResumeLayout();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            if (soundOnOff) // upaljeno 
            {
                //gasimo
                playerCorrect = null;
                playerWrong = null;
                soundOnOff = false;
            }
            else
            {
                //palimo
                playerWrong = new SoundPlayer(Music.wrong_buzzer);
                playerCorrect = new SoundPlayer(Music.correct_choice);
                soundOnOff = true;
            }
            setVoiceIcon();
        }

        //*************************************************************

        /// <summary>
        /// /// <param name="ContinueGame"></param>
        /// </summary>
        /// <param name="Serializable"></param>
        private void SaveGameState()
        {

            double vreme = Convert.ToDouble(label13.Text);
            int potezi = Convert.ToInt32(label8.Text);
            string nazivSlike = Convert.ToString(label10.Text);
            int brojPogodjenih = Convert.ToInt32(label6.Text);

            List<CardSerializable> cardSerial = new List<CardSerializable>();
            foreach (var card in cards)
            {
                CardSerializable sc = new CardSerializable(card.MyPostitionI, card.MyPostitionJ, card.MyPicture,card.SeeCardVisibility());
                cardSerial.Add(sc);
            }
            SerializableGame serializable = new SerializableGame(numOfRows, numOfCol, nazivSlike, brojPogodjenih, potezi, cardSerial, vreme);
            using (FileStream fs = new FileStream("gamestate.dat", FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, serializable);
            }
        }
        public static SerializableGame LoadGameState()
        {
            if (File.Exists("gamestate.dat"))
            {
                using (FileStream fs = new FileStream("gamestate.dat", FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (SerializableGame)formatter.Deserialize(fs);
                }
            }
            return null;
        }
        private void InitializeGameStateSerializable(int numOfRows, int numOfCol, String nameOfImage, int numOfHits, int Moves, List<CardSerializable> cardsSerial, double time)
        {
            //Setovanje vrednosti 
            ImageName = nameOfImage;
            this.numOfHits = numOfHits;
            numOfMoves = Moves;
            this.numOfRows = numOfRows;
            this.numOfCol = numOfCol;

            setNumOfMovesAndHits(numOfHits, numOfMoves);
            label13.Text = Convert.ToString(time);

            //

            if (numOfRows == 4 && numOfCol == 4)
            {
                table_4_4();
            }

            else if (numOfRows == 3 && numOfCol == 4)
            {
                table_3_4();
            }
            else if (numOfRows == 3 && numOfCol == 3)
            {
                table_3_3();
            }
            else throw new Exception("Nema ta raspodela na board-u");


            //bitno 
            GameImageNotOpen = ResizeImage(GameImageNotOpen, (int)tableLayoutPanel1.Controls[1].Width, (int)tableLayoutPanel1.Controls[1].Height);

            // staviti nameOfIamge on center
            int size = 0;
            if (nameOfImage == null || nameOfImage.Equals("no name") || nameOfImage.Equals("Animals"))
            {
                label10.Text = "Animals";
                size = 6;

            }
            else
                label10.Text = nameOfImage;

            label10.Dock = DockStyle.Fill;
            label10.TextAlign = ContentAlignment.MiddleCenter;
            int fontSize = CalculateFontSizeToFit(label10, panel8.ClientRectangle.Size);
            label10.Font = new Font(label10.Font.FontFamily, fontSize - size);

          



            this.cards = new List<Card>();
           

            PictureBox pictureBox;
            for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
            {
                if (tableLayoutPanel1.Controls[i] is PictureBox)
                    pictureBox = (PictureBox)tableLayoutPanel1.Controls[i];
                else
                    continue;
                TableLayoutPanelCellPosition cellPosition = tableLayoutPanel1.GetCellPosition(pictureBox);

                int row = cellPosition.Row;    
                int column = cellPosition.Column;

                CardSerializable card = cardsSerial.FirstOrDefault(x => x.MyPostitionI == row  && x.MyPostitionJ == column );
                if (card != null)
                {
                    if (card.SeeCardVisibility())
                    {
                        
                        pictureBox.Visible = false;
                        cards.Add(new Card(row, column, card.MyPicture, pictureBox, true));
                        pictureBox.BackgroundImage = card.MyPicture;
                    }
                    else
                    {
                        cards.Add(new Card(row, column, card.MyPicture, pictureBox, false));
                        pictureBox.BackgroundImage = card.MyPicture;
                    }
                }
            }






         }

        

        //*************************************************************


    }
}
