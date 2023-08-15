using MemoryGame.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MemoryGame;
using Windowss;
using System.Media;

namespace MemoryGame
{
    public partial class Settings : Form
    {

        Bitmap[,] croppedImages; // podeljena slika
        public Bitmap gameImage = null;
        public int numRows = 3;
        public int numColumns = 3;
        public string nameOfImage = "no name";
        public SoundPlayer playMusic =  null;
        public Settings()
        {

            InitializeComponent();
            Bitmap myImage = MyResource.upload;
            button1.Image = Game.ResizeImage(myImage, button1.Width / 2, (int)(button1.Height / 1.5));
            
            radioButton1.Checked = false;

            label1.Visible = false;
            radioButton1.Checked = true;

            makeTable( numRows,  numColumns);
            FillTabelWithImage(numRows, numColumns, gameImage);
        }

        private void ClearTableLayoutPanel(TableLayoutPanel tableLayoutPanel)
        {
            tableLayoutPanel.SuspendLayout();

            // Remove all controls from the TableLayoutPanel
            tableLayoutPanel.Controls.Clear();
            tableLayoutPanel.RowStyles.Clear();
            tableLayoutPanel.ColumnStyles.Clear();

            tableLayoutPanel.ResumeLayout();
        }
        private void makeTable(int numRows, int numColumns)
        {
            //zato sto imamo *2
            ClearTableLayoutPanel(tableLayoutPanel1);
            numColumns = numColumns * 2;

            tableLayoutPanel1.SuspendLayout();

          
            tableLayoutPanel1.RowCount = numRows;
            tableLayoutPanel1.ColumnCount = numColumns;

            for (int row = 0; row < numRows; row++)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / numRows));
            }

            for (int col = 0; col < numColumns; col++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / numColumns));
            }

  
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Dock = DockStyle.Fill;
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

                    tableLayoutPanel1.Controls.Add(pictureBox, col, row);
                }
            }

            tableLayoutPanel1.ResumeLayout();

        }
        private Bitmap ResizeImage(Bitmap originalImage, int newWidth, int newHeight)
        {

            Bitmap resizedImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {

                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return resizedImage;
        }
        private void FillTabelWithImage(int numRows, int numColumns, Bitmap GameImage)
        {


            if (GameImage != null)
            {
                GameImage = ResizeImage(GameImage, tableLayoutPanel1.Width, tableLayoutPanel1.Height);


                int cellWidth = (int)(GameImage.Width / numColumns); // Širina svakog dela
                int cellHeight = (int)(GameImage.Height / numRows); // Visina svakog dela



                croppedImages = new Bitmap[numRows, numColumns];

                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numColumns; j++)
                    {
                        Rectangle cropRect = new Rectangle(j * cellWidth, i * cellHeight, cellWidth, cellHeight);
                        croppedImages[i, j] = GameImage.Clone(cropRect, GameImage.PixelFormat);
                        croppedImages[i, j] = ResizeImage(croppedImages[i, j], (int)tableLayoutPanel1.Controls[1].Width, (int)tableLayoutPanel1.Controls[1].Height);

                    }

                }


                for (int row = 0; row < tableLayoutPanel1.RowCount; row++)
                {
                    for (int col = 0; col < tableLayoutPanel1.ColumnCount / 2; col++)
                    {
                        PictureBox pictureBox = tableLayoutPanel1.GetControlFromPosition(col, row) as PictureBox;
                        if (pictureBox != null)
                        {
                            pictureBox.BackgroundImage = croppedImages[row, col];
                        }
                    }
                }
                int numToMinus = tableLayoutPanel1.ColumnCount / 2;
                for (int row = 0; row < tableLayoutPanel1.RowCount; row++)
                {
                    for (int col = tableLayoutPanel1.ColumnCount / 2; col < tableLayoutPanel1.ColumnCount; col++)
                    {
                        PictureBox pictureBox = tableLayoutPanel1.GetControlFromPosition(col, row) as PictureBox;
                        if (pictureBox != null)
                        {
                            pictureBox.BackgroundImage = croppedImages[row, col - numToMinus];
                        }
                    }
                }
            }
            else
            {
                for(int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                {

                    PictureBox pictureBox = tableLayoutPanel1.Controls[i] as PictureBox;
                    pictureBox.BackColor = Color.White;
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                openFileDialog.Title = "Choose an Image";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedImagePath = openFileDialog.FileName;

                    if (!string.IsNullOrEmpty(selectedImagePath))
                    {
                        gameImage = new Bitmap(selectedImagePath);
                        string fileName = Path.GetFileName(selectedImagePath);
                        
                        label1.Text = fileName;
                        nameOfImage = fileName;
                        label1.Visible = true;
                        label1.Dock = DockStyle.Fill;
                        label1.TextAlign = ContentAlignment.MiddleCenter;
                        int fontSize = Game.CalculateFontSizeToFit(label1, panel4.ClientRectangle.Size);
                        label1.Font = new Font(label1.Font.FontFamily, fontSize);
                        panel4.Controls.Add(label1);

                      
                        FillTabelWithImage(numRows, numColumns, gameImage);
                    }
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            numRows = 3;
            numColumns = 3;
            makeTable(numRows, numColumns);
            FillTabelWithImage(numRows,numColumns, gameImage);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            numRows = 3;
            numColumns = 4;
            makeTable(numRows, numColumns);
            FillTabelWithImage(numRows, numColumns, gameImage);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            numRows = 4;
            numColumns = 4;
            makeTable(numRows, numColumns);
            FillTabelWithImage(numRows, numColumns, gameImage);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
           
                playMusic = new SoundPlayer(GameMusic.MemoryMusic);
                playMusic.PlayLooping();
            }
            else
            {
          
                if(playMusic != null)
                playMusic.Stop();
            }
        }
    }
}
