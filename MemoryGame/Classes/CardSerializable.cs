using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryGame.Classes
{
    [Serializable]
    public  class CardSerializable
    {
     
            int myPostitionI;
            int myPostitionJ;
            Image myPicture;
            private Boolean open; // da li je otvorena ili ne (true jeste, false nije)
            public CardSerializable(int myPostitionI, int myPostitionJ, Image myPicture, Boolean open)
            {
                this.myPostitionI = myPostitionI;
                this.myPostitionJ = myPostitionJ;
                this.myPicture = myPicture;
           
                this.open = open;
            }

            public int MyPostitionI { get => myPostitionI; set => myPostitionI = value; }
            public int MyPostitionJ { get => myPostitionJ; set => myPostitionJ = value; }
            public Image MyPicture { get => myPicture; set => myPicture = value; }
   

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
