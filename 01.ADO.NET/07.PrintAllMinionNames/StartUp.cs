using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace _07.PrintAllMinionNames
{
    public class StartUp
    {
        const string SqlConnectionString =
            "Server=localhost;User Id = SA;Password = Qawsed12;Database=MinionsDB";

        public static void Main(string[] args)
        {
            using var connection = new SqlConnection(SqlConnectionString);
            connection.Open();

            var queryGetAllMinions = @"SELECT Name FROM Minions";
            using var getAllMinionsCommand = new SqlCommand(queryGetAllMinions, connection);
            using var reader = getAllMinionsCommand.ExecuteReader();

            var listOfMinions = new List<string>();

            while (reader.Read())
            {
                listOfMinions.Add(reader["Name"].ToString());
            }

            for (int i = 0; i < listOfMinions.Count / 2; i++)
            {
                Console.WriteLine(listOfMinions[i]);
                Console.WriteLine(listOfMinions[listOfMinions.Count - i - 1]);
            }

            if (listOfMinions.Count % 2 != 0)
            {
                Console.WriteLine(listOfMinions[listOfMinions.Count / 2]);
            }
        }
    }
}
