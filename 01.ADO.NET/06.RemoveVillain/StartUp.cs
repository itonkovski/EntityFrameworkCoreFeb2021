using System;
using Microsoft.Data.SqlClient;

namespace _06.RemoveVillain
{
    public class StartUp
    {
        const string SqlConnectionString =
            "Server=localhost;User Id = SA;Password = Qawsed12;Database=MinionsDB";

        public static void Main(string[] args)
        {
            using var connection = new SqlConnection(SqlConnectionString);
            connection.Open();

            int value = int.Parse(Console.ReadLine());

            string villainNameQuery = @"SELECT Name FROM Villains WHERE Id = @villainId";
            using var sqlCommand = new SqlCommand(villainNameQuery, connection);
            sqlCommand.Parameters.AddWithValue("@villainId", value);
            var name = (string)sqlCommand.ExecuteScalar();

            if (name == null)
            {
                Console.WriteLine("No such villain was found.");
                return;
            }

            var deleteMinionsVillainsQuery = @"DELETE FROM MinionsVillains 
                                                WHERE VillainId = @villainId";

            using var sqlDeleteMVCommand = new SqlCommand(deleteMinionsVillainsQuery, connection);
            sqlDeleteMVCommand.Parameters.AddWithValue("@villainId", value);
            var affectedRows = sqlDeleteMVCommand.ExecuteNonQuery();

            var deleteVillainQuery = @"DELETE FROM Villains
                                        WHERE Id = @villainId";

            using var sqlDeleteVCommand = new SqlCommand(deleteVillainQuery, connection);
            sqlDeleteVCommand.Parameters.AddWithValue("@villainId", value);
            sqlDeleteVCommand.ExecuteNonQuery();

            Console.WriteLine($"{name} was deleted.");
            Console.WriteLine($"{affectedRows} minions were released.");
        }
    }
}
