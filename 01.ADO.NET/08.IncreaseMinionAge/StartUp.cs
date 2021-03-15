using System;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace _08.IncreaseMinionAge
{
    public class StartUp
    {
        const string SqlConnectionString =
            "Server=localhost;User Id = SA;Password = Qawsed12;Database=MinionsDB";

        public static void Main(string[] args)
        {
            using var connection = new SqlConnection(SqlConnectionString);
            connection.Open();

            
            var minionsIds = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();

            var updateMinionsQuery = @"UPDATE Minions
                                          SET Age += 1, Name = UPPER(SUBSTRING(Name, 1, 1)) + SUBSTRING(Name, 2, LEN(Name))
                                          WHERE Id = @Id";

            foreach (var id in minionsIds)
            {
                using var sqlCommand = new SqlCommand(updateMinionsQuery, connection);
                sqlCommand.Parameters.AddWithValue("@Id", id);
                sqlCommand.ExecuteNonQuery();
            }

            var selectMinions = "SELECT Name, Age FROM Minions";
            using var selectCommand = new SqlCommand(selectMinions, connection);
            using var reader = selectCommand.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]} {reader["Age"]}");
            }
        }
    }
}
