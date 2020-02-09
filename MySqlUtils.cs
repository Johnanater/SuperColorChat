using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Rocket.Core.Logging;

namespace SuperColorChat
{
    public class MySqlUtils
    {
        internal MySqlUtils()
        {
            new I18N.West.CP1250();
            var connection = CreateConnection();
            try
            {
                connection.Open();
                connection.Close();

                CreateCheckSchema();
            }
            catch (MySqlException ex)
            {
                Logger.LogException(ex);
                Main.Instance.UnloadPlugin();
            }
        }

        private MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection($"SERVER={Main.Config.DatabaseAddress};DATABASE={Main.Config.DatabaseName};UID={Main.Config.DatabaseUsername};PASSWORD={Main.Config.DatabasePassword};PORT={Main.Config.DatabasePort};");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        private void CreateCheckSchema()
        {
            using (var connection = CreateConnection())
            {
                try
                {
                    var command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = $"SHOW TABLES LIKE '{Main.Config.DatabaseTableName}';";

                    var check = command.ExecuteScalar();

                    if (check == null)
                    {
                        Logger.Log("Tables not found, creating!");
                        command.CommandText = $"CREATE TABLE `{Main.Config.DatabaseTableName}` ( `steamId` VARCHAR(50) NULL DEFAULT NULL, `color` VARCHAR(50) NULL DEFAULT NULL) COLLATE = 'utf8_general_ci' ENGINE = InnoDB;";
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public async Task<bool> CheckExists(string steamId)
        {
            using (var connection = CreateConnection())
            {
                try
                {
                    var command = new MySqlCommand
                    (
                        $@"SELECT EXISTS(SELECT 1 FROM `{Main.Config.DatabaseTableName}`
                        WHERE `steamId` = @SteamId);", connection
                    );

                    command.Parameters.AddWithValue("@SteamId", steamId);
                    await connection.OpenAsync();

                    var status = Convert.ToInt32(await command.ExecuteScalarAsync());

                    return status > 0;
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    return false;
                }
            }
        }

        public async Task<string> GetColor(string steamId)
        {
            using (var connection = CreateConnection())
            {
                string output = null;

                try
                {
                    var command = new MySqlCommand
                    (
                        $@"SELECT * FROM {Main.Config.DatabaseTableName}
                        WHERE steamId = @SteamId;", connection
                    );

                    command.Parameters.AddWithValue("@SteamId", steamId);

                    await connection.OpenAsync();
                    var dataReader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.SingleRow);
                    
                    while (await dataReader.ReadAsync())
                    {
                        output = Convert.ToString(dataReader["color"]);
                    }
                    
                    dataReader.Close();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                return output;
            }
        }

        public async Task SetColor(string steamId, string color)
        {
            using (var connection = CreateConnection())
            {
                try
                {
                    var command = new MySqlCommand
                    (
                        $@"INSERT INTO {Main.Config.DatabaseTableName} (steamId, color)
                        VALUES (@SteamId, @Color);", connection
                    );

                    command.Parameters.AddWithValue("@SteamId", steamId);
                    command.Parameters.AddWithValue("@Color", color);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public async Task UpdateColor(string steamId, string color)
        {
            using (var connection = CreateConnection())
            {
                try
                {
                    var command = new MySqlCommand
                    (
                        $@"UPDATE {Main.Config.DatabaseTableName}
                        SET `color` = @Color WHERE  `steamId` = @SteamId;", connection
                    );

                    command.Parameters.AddWithValue("@SteamId", steamId);
                    command.Parameters.AddWithValue("@Color", color);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public async Task RemoveColor(string steamId)
        {
            using (var connection = CreateConnection())
            {
                try
                {
                    var command = new MySqlCommand
                    (
                        $@"DELETE FROM {Main.Config.DatabaseTableName}
                        WHERE `steamId` = @SteamId;", connection
                    );
                    
                    command.Parameters.AddWithValue("@SteamId", steamId);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }
    }
}
