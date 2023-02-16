using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PasswordTools.src
{
    internal class HandleDatabase
    {
        public SQLiteConnection connection;
        public HandleDatabase()
        {
            connection = CreateConnection();
        }

        static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sQLiteConn;
            sQLiteConn = new SQLiteConnection("Data Source=database.db;version = 3;New = True; Compress = True;");
            try
            {
                sQLiteConn.Open();
            }
            catch
            {

            }
            return sQLiteConn;
        }

        public void DeleteTable(SQLiteConnection conn)
        {
            SQLiteCommand sQLiteCommand;
            sQLiteCommand = conn.CreateCommand();
            sQLiteCommand.CommandText = "DROP TABLE person";
            sQLiteCommand.ExecuteNonQuery();
        }

        public void CreateTable(SQLiteConnection conn)
        { 
            SQLiteCommand sQLiteCommand;
            sQLiteCommand = conn.CreateCommand();
            sQLiteCommand.CommandText = "CREATE TABLE IF NOT EXISTS people(id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, firstName TEXT, lastName TEXT  , birthday TEXT  , otherBirthdays TEXT  , nickname TEXT  , hobbys TEXT  , city TEXT  , cityAliases TEXT  , country TEXT  , petsName TEXT  , petsBirtday TEXT  , petType TEXT  , petBreed TEXT)\r\n";
            sQLiteCommand.ExecuteNonQuery();
        }

        public void AddPerson(SQLiteConnection conn, Person p)
        {
            CreateTable(conn);
            var cultureInfo = new CultureInfo("de-DE");
            try
            {
                SQLiteCommand sQLiteCommand;
                string insertSQL = "INSERT INTO people(firstName, lastName, birthday, otherBirthdays, nickname, hobbys, city, cityAliases, country, petsName, petsBirtday, petType, petBreed) VALUES (\"" + p.FirstName + "\",\"" + p.LastName + "\",\"" + p.Birthday + "\",\"" + String.Join(";", p.OtherBirthdays.ToArray()) + "\",\"" + p.Nickname + "\",\" \",\"" + p.City + "\",\"" +String.Join(";", p.CityAliases.ToArray()) + "\",\"" + p.Country + "\",\"" + p.PetsName + "\",\"" + p.PetsBirtday + "\",\"" + p.PetType + "\",\"" + p.PetBreed + "\")";
                sQLiteCommand = conn.CreateCommand();
                sQLiteCommand.CommandText = insertSQL;
                sQLiteCommand.ExecuteNonQuery();
            }catch(SQLiteException e)
            {
                Console.WriteLine(e.Message);
                AddPerson(conn,p);
            }
        }

        public Person GetPerson(SQLiteConnection conn, int id)
        {
            CreateTable(conn);
            SQLiteCommand sQLiteCommand;
            SQLiteDataReader sQLiteDataReader;
            sQLiteCommand = conn.CreateCommand();
            sQLiteCommand.CommandText = "SELECT * FROM people WHERE ID ="+ id;
            sQLiteDataReader = sQLiteCommand.ExecuteReader();

            Person person;
            while (sQLiteDataReader.Read())
            {
                var cultureInfo = new CultureInfo("de-DE");

                person = new Person(
                    sQLiteDataReader.GetInt32(0),//id
                    sQLiteDataReader.GetString(1),//firstName
                    sQLiteDataReader.GetString(2),//lastName
                    sQLiteDataReader.GetString(3),//birthday
                    sQLiteDataReader.GetString(4).Split(';').ToList(),//otherBirthdays
                    sQLiteDataReader.GetString(5),//nickname
                    sQLiteDataReader.GetString(7),//city
                    sQLiteDataReader.GetString(8).Split(';').ToList(),//cityAlliases
                    sQLiteDataReader.GetString(9),//country
                    sQLiteDataReader.GetString(10),//petsName
                    sQLiteDataReader.GetString(11),//petsBirthday
                    sQLiteDataReader.GetString(12),//petsType
                    sQLiteDataReader.GetString(13));//petsBreed

                sQLiteDataReader.Close();
                return person;
            }
            return null;
        }
        public List<Person> GetPerson(SQLiteConnection conn, string name)
        {
            SQLiteCommand sQLiteCommand;
            SQLiteDataReader sQLiteDataReader;
            sQLiteCommand = conn.CreateCommand();
            sQLiteCommand.CommandText = "SELECT * FROM people WHERE firstName = '" + name+"'";
            sQLiteDataReader = sQLiteCommand.ExecuteReader();

            List<Person> persons = new List<Person>();
            while (sQLiteDataReader.Read())
            {
                var cultureInfo = new CultureInfo("de-DE");

                Person person = new Person(
                    sQLiteDataReader.GetInt32(0),//id
                    sQLiteDataReader.GetString(1),//firstName
                    sQLiteDataReader.GetString(2),//lastName
                    sQLiteDataReader.GetString(3),//birthday
                    sQLiteDataReader.GetString(4).Split(';').ToList(),//otherBirthdays
                    sQLiteDataReader.GetString(5),//nickname
                    sQLiteDataReader.GetString(7),//city
                    sQLiteDataReader.GetString(8).Split(';').ToList(),//cityAlliases
                    sQLiteDataReader.GetString(9),//country
                    sQLiteDataReader.GetString(10),//petsName
                    sQLiteDataReader.GetString(11),//petsBirthday
                    sQLiteDataReader.GetString(12),//petsType
                    sQLiteDataReader.GetString(13));//petsBreed
                persons.Add(person);
            }
            sQLiteDataReader.Close();
            return persons;
        }

        public void DeletePerson(SQLiteConnection conn, string name)
        {
            SQLiteCommand sQLiteCommand;
            sQLiteCommand = conn.CreateCommand();
            sQLiteCommand.CommandText = "DELETE FROM people WHERE NAME = '" + name +"'";
            sQLiteCommand.ExecuteNonQuery();
        }
        public void DeletePerson(SQLiteConnection conn, int id)
        {
            CreateTable(conn);
            SQLiteCommand sQLiteCommand;
            sQLiteCommand = conn.CreateCommand();
            sQLiteCommand.CommandText = "DELETE FROM people WHERE ID = " + id;
            sQLiteCommand.ExecuteNonQuery();
        }

        public string GetPersonList(SQLiteConnection conn)
        {
            CreateTable(conn);
            SQLiteCommand sQLiteCommand;
            SQLiteDataReader sQLiteDataReader;
            sQLiteCommand = conn.CreateCommand();
            sQLiteCommand.CommandText = "SELECT * FROM people";
            sQLiteDataReader = sQLiteCommand.ExecuteReader();

            StringBuilder stringBuilder= new StringBuilder();
            while (sQLiteDataReader.Read())
            { 
                stringBuilder.AppendLine("[" + sQLiteDataReader.GetInt32(0) + "]\t"+ sQLiteDataReader.GetString(1) + " " + sQLiteDataReader.GetString(2));
            }
            sQLiteDataReader.Close();
            return stringBuilder.ToString();
        }
    }
}
