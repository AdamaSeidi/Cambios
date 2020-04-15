using Cambios.Modelos;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambios.Servicos
{
   public class DataService
   {
        private SQLiteConnection Connection;

        private SQLiteCommand command;

        private DialogService dialogService;
        public DataService()
        {
            dialogService = new DialogService();
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }
            var path = @"Data\Rates.sqlite";
            try
            {
                Connection = new SQLiteConnection("Data Source=" + path);
                Connection.Open();

                string sqlcommand =
                    "create table if not exists rates(RateId int, Code varchar(5), TaxRate Real, Name varchar(250))";
                command = new SQLiteCommand(sqlcommand, Connection);
                command.ExecuteNonQuery();

            }catch(Exception ex)
            {
                dialogService.ShowMessage("Erro", ex.Message);
            }   
        }


        public void SaveData(List<Rate> Rates)
        {
            try
            {
                foreach(var rate in Rates)
                {
                    string sql = string.Format("insert into Rates (RateId, Code, TaxRate, Name) values( {0}, '{1}', '{2}', '{3}')",
                        rate.RateId, rate.Code, rate.TaxRate, rate.Name);

                    command = new SQLiteCommand(sql, Connection);
                    command.ExecuteNonQuery();
                }
                Connection.Close();
                
            }catch(Exception ex)
            {
                dialogService.ShowMessage("Erro", ex.Message);
            }
        }

        public List<Rate> GetData()
        {
            List<Rate> rates = new List<Rate>();

            try
            {
                string sql = "select RateId, Code, TaxRate, Name from Rates";

                command = new SQLiteCommand(sql, Connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    rates.Add(new Rate
                    {
                        RateId = (int) reader["RateId"],
                        Code = (string) reader["Code"],
                        TaxRate = (double) reader["TaxRate"],
                        Name = (string) reader ["Name"]
                    });
                }
                Connection.Close();

                return rates;

            }catch(Exception ex)
            {
                dialogService.ShowMessage("Erro", ex.Message);
                return null;
            }
        }

        public void DeleteData()
        {
            try
            {
                string sql = "delete from Rates";
                command = new SQLiteCommand(sql, Connection);
                command.ExecuteNonQuery();
            }catch(Exception ex)
            {
                dialogService.ShowMessage("Erro", ex.Message);
            }
        }
   }
}
