using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windowss.Classes;

namespace Windowss
{
    public partial class BestScore : Form
    {
        public BestScore()
        {
            InitializeComponent();
            SetBestScore();
        }

        public void SetBestScore()
        {


            CommunicationWithDatabase conn = new CommunicationWithDatabase();


                    List<Rezultati> rezultati1 = conn.getTopRezultati("3x3x2");
                    ClearDataGridView(dataGridView1);
                    AddRezultatiToDataGridView(rezultati1,dataGridView1);

                    List<Rezultati> rezultati2 = conn.getTopRezultati("3x4x2");
                    ClearDataGridView(dataGridView2);
                    AddRezultatiToDataGridView(rezultati2,dataGridView2);

                    List<Rezultati> rezultati3 = conn.getTopRezultati("4x4x2");
                    ClearDataGridView(dataGridView3);
                    AddRezultatiToDataGridView(rezultati3,dataGridView3);
                
            
        }
        private static void AddRezultatiToDataGridView(List<Rezultati> rezultati, DataGridView dgv)
        {
          
            foreach (var rezultat in rezultati)
            {
                dgv.Rows.Add(rezultat.ImeIgraca1, rezultat.NazivSlike1, rezultat.Vreme1, rezultat.Rezultat1);
            }
        }
        private void ClearDataGridView(DataGridView dgv)
        {
            dgv.Rows.Clear();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
