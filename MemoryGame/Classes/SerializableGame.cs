using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Classes
{
    [Serializable]
    public class SerializableGame
    {

        private int nrows;
        private int ncols;
        private string ImageName;
        private int numOfHits;
        private int numOfMoves;
        private double time;
        private List<CardSerializable> cards;

        public SerializableGame(int nrows, int ncols, string imageName, int numOfHits, int numOfMoves, List<CardSerializable> cards, double time)
        {
            this.Nrows = nrows;
            this.Ncols = ncols;
            ImageName1 = imageName;
            this.NumOfHits = numOfHits;
            this.NumOfMoves = numOfMoves;
            this.Cards = cards;
            this.Time = time;
        }

        public int Nrows { get => nrows; set => nrows = value; }
        public int Ncols { get => ncols; set => ncols = value; }
        public string ImageName1 { get => ImageName; set => ImageName = value; }
        public int NumOfHits { get => numOfHits; set => numOfHits = value; }
        public int NumOfMoves { get => numOfMoves; set => numOfMoves = value; }
        public List<CardSerializable> Cards { get => cards; set => cards = value; }
        public double Time { get => time; set => time = value; }
    }
}
