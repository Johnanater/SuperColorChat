using System;
using MySql.Data.MySqlClient;
using Rocket.Core.Logging;

namespace SuperColorChat
{
    public class MySQLUtils
    {
        internal MySQLUtils()
        {
            new I18N.West.CP1250();
            MySqlConnection connection = CreateConnection();
            try
            {
                connection.Open();
                connection.Close();

                CreateCheckSchema();
            }
            catch (MySqlException e)
            {
                Logger.LogException(e);
                Main.Instance.UnloadPlugin();
            }
        }

        private static MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", Main.Config.DatabaseAddress, Main.Config.DatabaseName, Main.Config.DatabaseUsername, Main.Config.DatabasePassword, Main.Config.DatabasePort));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
            return connection;
        }

        private void CreateCheckSchema()
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                connection.Open();
                command.CommandText = "SHOW TABLES LIKE '" + Main.Config.DatabaseTableName + "';";

                object check = command.ExecuteScalar();

                if (check == null)
                {
                    Logger.Log("Tables not found, creating!");
                    command.CommandText = "CREATE TABLE `" + Main.Config.DatabaseTableName + "` ( `steamId` VARCHAR(50) NULL DEFAULT NULL, `color` VARCHAR(50) NULL DEFAULT NULL) COLLATE = 'utf8_general_ci' ENGINE = InnoDB;";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        public static bool CheckExists(String steamId)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = new MySqlCommand("SELECT 1 FROM `" + Main.Config.DatabaseTableName + "` WHERE `steamId` = @SteamId;", connection);
                command.Parameters.AddWithValue("@SteamId", steamId);
                connection.Open();
                object result = command.ExecuteScalar();
                connection.Close();

                if (result != null)
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
            return false;
        }

        public static string GetColor(String steamId)
        {
            string output = null;
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = new MySqlCommand("SELECT * FROM " + Main.Config.DatabaseTableName + " WHERE steamId = @SteamId;", connection);
                command.Parameters.AddWithValue("@SteamId", steamId);

                connection.Open();
                MySqlDataReader dataReader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                while (dataReader.Read())
                {
                    output = Convert.ToString(dataReader["color"]);
                }
                dataReader.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
            return output;
        }

        public static void SetColor(String steamId, String color)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = new MySqlCommand("INSERT INTO " + Main.Config.DatabaseTableName + "(steamId, color) VALUES (@SteamID, @Color);", connection);
                command.Parameters.AddWithValue("@SteamID", steamId);
                command.Parameters.AddWithValue("@Color", color);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        public static void UpdateColor(String steamId, String color)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = new MySqlCommand("UPDATE " + Main.Config.DatabaseTableName + " SET `color` = @Color WHERE  `steamId` = @SteamId;", connection);
                command.Parameters.AddWithValue("@SteamId", steamId);
                command.Parameters.AddWithValue("@Color", color);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        public static void RemoveColor(String steamId)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = new MySqlCommand("DELETE FROM " + Main.Config.DatabaseTableName + " WHERE `steamId` = @SteamId;", connection);
                command.Parameters.AddWithValue("@SteamId", steamId);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }
    }
}