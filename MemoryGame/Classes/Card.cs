using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryGame.Classes
{
   
    public class Card
    {

        int myPostitionI;
        int myPostitionJ;
        Image myPicture;
        PictureBox pictureBox;
        private Boolean open = false; // da li je otvorena ili ne (true jeste, false nije)
        public Card(int myPostitionI, int myPostitionJ,Image myPicture, PictureBox pictureBox, Boolean open)
        {
            this.myPostitionI = myPostitionI;
            this.myPostitionJ = myPostitionJ;
            this.myPicture = myPicture;
            this.PictureBox = pictureBox;
            this.open = open;
        }

        public int MyPostitionI { get => myPostitionI; set => myPostitionI = value; }
        public int MyPostitionJ { get => myPostitionJ; set => myPostitionJ = value; }
        public Image MyPicture { get => myPicture; set => myPicture = value; }
        public PictureBox PictureBox { get => pictureBox; set => pictureBox = value; }
        public void SetCardVisibility(bool open)
        {
            this.open = open;

        }
        public Boolean SeeCardVisibility()
        {

            return open;
        }


    }
}
