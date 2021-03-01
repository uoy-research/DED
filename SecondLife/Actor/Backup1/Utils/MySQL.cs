using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace DED.Utils
{
    class MySQL
    {
        MySqlConnection myConnection;

        public MySQL() { }


        public MySQL Connect(){
        //Connect to MySQL DB 
            Connect("detectiv_ma", "mjallhvit111", "www.detective101.net", "detectiv_ded");
            return this;
        }

        private void Connect(string username, string password, string server, string database)
        {
            string str = "Data Source=" + server + ";database=" + database + ";User ID=" +
             username + ";password=" + password + ";";

            this.myConnection = new MySqlConnection(str);
            try
            {
                Console.WriteLine("opening data connection: "+str);
                this.myConnection.Open();
                Console.WriteLine("DB open " );
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("DB NOT open ");
            }
        }

 

        public MySqlDataReader Query(string query)
        {
            
            MySqlCommand command = this.myConnection.CreateCommand();
            MySqlDataReader reader = null;
            command.CommandText = query;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception e)
            {
                Connect();
                reader = Query(query);
            }
            return reader;            
        }

        public void Delete(string query)
        {
            MySqlCommand command = this.myConnection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();
        }

        public void Insert(string query)
        {
            MySqlCommand command = this.myConnection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();
            //return Convert.ToInt32(command.ExecuteScalar);
        }


        public void Close()
        {
            try
            {
                this.myConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
