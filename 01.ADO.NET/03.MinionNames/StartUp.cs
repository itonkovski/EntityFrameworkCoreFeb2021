using System;
using Microsoft.Data.SqlClient;

namespace _03.MinionNames
{
    public class StartUp
    {
        const string SqlConnectionString =
            "Server=localhost;User Id = SA;Password = Qawsed12;Database=MinionsDB";

        public static void Main(string[] args)
        {
            using var connection = new SqlConnection(SqlConnectionString);
            connection.Open();

            int villainId = int.Parse(Console.ReadLine());

            string villainNameQuery = @"SELECT Name FROM Villains WHERE Id = @Id";

            using var command = new SqlCommand(villainNameQuery, connection);
            command.Parameters.AddWithValue("@Id", villainId);
            var result = command.ExecuteScalar();

            string minionsQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                             m.Name, 
                                             m.Age
                                        FROM MinionsVillains AS mv
                                        JOIN Minions As m ON mv.MinionId = m.Id
                                       WHERE mv.VillainId = @Id
                                    ORDER BY m.Name";

            if (result == null)
            {
                Console.WriteLine($"No villain with ID {villainId} exists in the database.");
            }
            else
            {
                Console.WriteLine($"Villain: {result}");

                using (var minionCommand = new SqlCommand(minionsQuery, connection))
                {
                    minionCommand.Parameters.AddWithValue("@Id", villainId);

                    using (var reader = minionCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine($"(no minions)");
                        }

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader[0]}. {reader[1]} {reader[2]}");
                        }
                    }
                }
            }
        }
    }
}
