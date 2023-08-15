using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windowss.Classes
{
    public class Rezultati
    {


       private String NazivSlike;
       private String ImeIgraca;
       private String Tabela;
       private Double Rezultat;
       private int BrojPoteza;
       private Double Vreme;

        public Rezultati(string nazivSlike, string imeIgraca, string tabela, double Rezultat,int brojPoteza, double vreme)
        {
            NazivSlike = nazivSlike;
            ImeIgraca = imeIgraca;
            Tabela = tabela;
            BrojPoteza = brojPoteza;
            Vreme = vreme;
            this.Rezultat = Rezultat;
        }

        public string NazivSlike1 { get => NazivSlike; set => NazivSlike = value; }
        public string ImeIgraca1 { get => ImeIgraca; set => ImeIgraca = value; }
        public string Tabela1 { get => Tabela; set => Tabela = value; }
        public int BrojPoteza1 { get => BrojPoteza; set => BrojPoteza = value; }
        public double Vreme1 { get => Vreme; set => Vreme = value; }
        public double Rezultat1 { get => Rezultat; set => Rezultat = value; }

        static public double calculateRezultat(int brojPoteza, double vreme, string Tabela)
        {
            // 3*3*2
            //9 -  20  poteza  max  500p,21  - 30  400p,31 -  40  300p,41 -  50  200 p,51 - 60 100p, 61 - 70 80p,
            //71 - 80  60p,81 -  90 50p,91 - xxxx poteza 30p
            // time 0 - 30sec (-30), 31 - 50 (-50), 51-70(-80), 71-99(-100),100+(-200)...

            double rezultat;
            int pBroj;
            double ntime;

            if (Tabela.Equals("3x4x2"))
            {
                pBroj = brojPoteza - 6;
                ntime = vreme - 10;
            }
            else if (Tabela.Equals("4x4x2"))
            {
                pBroj = brojPoteza - 15;
                ntime = vreme - 20;
            }
            else
            {
                pBroj = brojPoteza;
                ntime = vreme;
            }

            if (pBroj <= 20)
                rezultat = 500;
            else if (pBroj <= 30)
                rezultat = 400;
            else if (pBroj <= 40)
                rezultat = 300;
            else if (pBroj <= 50)
                rezultat = 200;
            else if (pBroj <= 60)
                rezultat = 100;
            else if (pBroj <= 70)
                rezultat = 80;
            else if (pBroj <= 80)
                rezultat = 60;
            else if (pBroj <= 90)
                rezultat = 50;
            else
                rezultat = 30;

            // 0 - 20 0p
            // 21 - 30 20p
            // 31 - 40 40p
            // 41 - 50 60p
            // 51 - 60 80p
            // 61 - 70 90p
            // 71 - 80 120p
            // 81 - 90 150p
            // 90 - xxx 200p

            if (ntime <= 20)
                rezultat -= 0;
            else if (ntime <= 30)
                rezultat -= 20;
            else if (ntime <= 40)
                rezultat -= 40;
            else if (ntime <= 50)
                rezultat -= 60;
            else if (ntime <= 60)
                rezultat -= 80;
            else if (ntime <= 70)
                rezultat -= 90;
            else if (ntime <= 80)
                rezultat -= 120;
            else if (ntime <= 90)
                rezultat -= 150;
            else
                rezultat -= 200;


            if (rezultat <= 0)
                rezultat = 0;

            return rezultat;
        }
    }
}
