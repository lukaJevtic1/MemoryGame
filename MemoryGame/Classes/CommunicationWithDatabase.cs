using MemoryGame.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windowss.Classes
{
    public class CommunicationWithDatabase
    {

        private DatabaseConnection connection = DatabaseConnection.Instance;
        public CommunicationWithDatabase()
        {
            //connection = DatabaseConnection.Instance;

            connection.Connection.Open();

            //proveravam postojanje tabele
            string checkTableQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Rezultati'";

            using (SqlCommand checkTableCommand = new SqlCommand(checkTableQuery, connection.Connection))
            {
                int tableCount = (int)checkTableCommand.ExecuteScalar();

                if (tableCount == 0) // Ako tabela ne postoji, pravim
                {
                    string createTableQuery = @"
                    CREATE TABLE Rezultati (
                        ID INT IDENTITY(1,1) PRIMARY KEY,
                        NazivSlike NVARCHAR(20),
                        ImeIgraca NVARCHAR(20),
                        Tabela NVARCHAR(10),
                        Rezultat REAL,
                        BrojPoteza INT,
                        Vreme REAL
                    )
                ";

                    using (SqlCommand createTableCommand = new SqlCommand(createTableQuery, connection.Connection))
                    {
                        createTableCommand.ExecuteNonQuery();
                    }
                }


            }
            connection.Connection.Close();


        }
    

    public List<Rezultati> getTopRezultati(string tabela)
    {
        List<Rezultati> rezultati = new List<Rezultati>();

         connection.Connection.Open();
        
        SqlCommand command = new SqlCommand(@"SELECT * FROM Rezultati Where Tabela = @tabela ORDER BY Rezultat DESC, Vreme ASC", connection.Connection);
        command.Parameters.AddWithValue("@tabela", tabela);

        using (SqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {

                Rezultati rezultat = new Rezultati(reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), Double.Parse(reader[4].ToString()), int.Parse(reader[5].ToString()), Double.Parse(reader[6].ToString()));

                rezultati.Add(rezultat);
            }
        }
            connection.Connection.Close();

            return rezultati;
    }
    public void insertRezultat(Rezultati rezultat)
    {

            connection.Connection.Open();

        SqlCommand command = new SqlCommand(@"insert into Rezultati 
                        (NazivSlike,
                        ImeIgraca,
                        Tabela,
                        Rezultat,
                        BrojPoteza,
                        Vreme) values (@naziv, @ime, @tabela,@rezultat,@brojP,@vreme)", connection.Connection);

        command.Parameters.AddWithValue("@naziv", rezultat.NazivSlike1);
        command.Parameters.AddWithValue("@ime", rezultat.ImeIgraca1);
        command.Parameters.AddWithValue("@tabela", rezultat.Tabela1);
        command.Parameters.AddWithValue("@rezultat", rezultat.Rezultat1);
        command.Parameters.AddWithValue("@brojP", rezultat.BrojPoteza1);
        command.Parameters.AddWithValue("@vreme", rezultat.Vreme1);

        if (command.ExecuteNonQuery() < 0)
        {
            throw new Exception("Greska pri dodavanju novog skora");
        }

            connection.Connection.Close();
        }



}
    
       
    
}