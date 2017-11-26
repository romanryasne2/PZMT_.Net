using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using ColorInteraction.Common;
using Newtonsoft.Json;

namespace ColorInteraction.Database
{
    public class Database
    {
        private readonly string ConnectionString;

        public Database()
        {
            ConnectionString = "Server=RZR2PC\\RZR2SQL;Database=DbForDotnet;User Id = sa; password=sa0123Roma;";
        }

        public async void Log(ColorMessage colorMessage)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            string color = JsonConvert.SerializeObject(colorMessage.Color);

            string script = $"INSERT INTO Logs VALUES ('{colorMessage.MachineName}', '{color}', GETDATE());";

            SqlCommand sqlCommand = new SqlCommand(script, sqlConnection);

            sqlConnection.Open();

            await sqlCommand.ExecuteNonQueryAsync();

            sqlConnection.Close();
        }

        public async Task<List<ColorMessage>> GetColorMessages(DateTime lastUpdate)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            string script = @"SELECT MachineName, Color FROM Logs
                              WHERE Id IN (
                                  SELECT MAX(Id) FROM Logs
                                  WHERE LastUpdate > @lastUpdate
                                  GROUP BY MachineName);";

            SqlCommand sqlCommand = new SqlCommand(script, sqlConnection);

            sqlCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@lastUpdate",
                SqlDbType = SqlDbType.DateTime,
                Value = lastUpdate
            });

            sqlConnection.Open();

            SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

            List<ColorMessage> result = new List<ColorMessage>();

            while (reader.Read())
            {
                result.Add(new ColorMessage
                {
                    Color = JsonConvert.DeserializeObject<Color>(reader.GetString(reader.GetOrdinal("Color"))),
                    MachineName = reader.GetString(reader.GetOrdinal("MachineName"))
                });
            }

            sqlConnection.Close();

            return result;
        }
    }
}