using System;
using Microsoft.Data.SqlClient;

namespace _01.InitialSetup
{
    public class StartUp
    {
        //Use to connect to master DB
        //const string SqlConnectionString =
        //    "Server=localhost;User Id = SA;Password = Qawsed12;Database=master";

        const string SqlConnectionString =
            "Server=localhost;User Id = SA;Password = Qawsed12;Database=MinionsDB";

        public static void Main(string[] args)
        {

            using var connection = new SqlConnection(SqlConnectionString);
            connection.Open();

            //Use to create the MinionsDB
            string createDataBase = "CREATE DATABASE MinionsDB";
            using (var command = new SqlCommand(createDataBase, connection))
            {
                command.ExecuteNonQuery();
            }


            //Use to create all the table statements
            var createTableStatements = GetCreateTableStatements();

            foreach (var query in createTableStatements)
            {
                ExecuteNonQuery(connection, query);
            }

            //Use to insert all the data statements
            var insertStatements = GetInsertDataStatements();

            foreach (var query in insertStatements)
            {
                ExecuteNonQuery(connection, query);
            }
        }

        private static void ExecuteNonQuery(SqlConnection connection, string query)
        {
            using var  command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
        }

        private static string[] GetInsertDataStatements()
        {
            var result = new string[]
            {
                "INSERT INTO Countries ([Name]) VALUES('Bulgaria'),('England'),('Cyprus'),('Germany'),('Norway')",

                "INSERT INTO Towns([Name], CountryCode) VALUES('Plovdiv', 1),('Varna', 1),('Burgas', 1),('Sofia', 1),('London', 2),('Southampton', 2),('Bath', 2),('Liverpool', 2),('Berlin', 3),('Frankfurt', 3),('Oslo', 4)",

                "INSERT INTO Minions(Name, Age, TownId) VALUES('Bob', 42, 3),('Kevin', 1, 1),('Bob ', 32, 6),('Simon', 45, 3),('Cathleen', 11, 2),('Carry ', 50, 10),('Becky', 125, 5),('Mars', 21, 1),('Misho', 5, 10),('Zoe', 125, 5),('Json', 21, 1)",

                "INSERT INTO EvilnessFactors(Name) VALUES('Super good'),('Good'),('Bad'), ('Evil'),('Super evil')",

                "INSERT INTO Villains(Name, EvilnessFactorId) VALUES('Gru', 2),('Victor', 1),('Jilly', 3),('Miro', 4),('Rosen', 5),('Dimityr', 1),('Dobromir', 2)",

                "INSERT INTO MinionsVillains(MinionId, VillainId) VALUES(4, 2),(1, 1),(5, 7),(3, 5),(2, 6),(11, 5),(8, 4),(9, 7),(7, 1),(1, 3),(7, 3),(5, 3),(4, 3),(1, 2),(2, 1),(2, 7)"
            };
            return result;
        }

        private static string[] GetCreateTableStatements()
        {
            var result = new string[]
            {
                "CREATE TABLE Countries (Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50))",

                "CREATE TABLE Towns(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50), CountryCode INT FOREIGN KEY REFERENCES Countries(Id))",

                "CREATE TABLE Minions(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(30), Age INT, TownId INT FOREIGN KEY REFERENCES Towns(Id))",

                "CREATE TABLE EvilnessFactors(Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50))",

                "CREATE TABLE Villains (Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50), EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id))",

                "CREATE TABLE MinionsVillains (MinionId INT FOREIGN KEY REFERENCES Minions(Id),VillainId INT FOREIGN KEY REFERENCES Villains(Id),CONSTRAINT PK_MinionsVillains PRIMARY KEY (MinionId, VillainId))"
            };
            return result;
        }
    }
}
