using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static TelegramBot.MainClass;

namespace TelegramBot
{
    public class SQL
    {
        //-----------------------------------------------------------------------------------------------------------------------
        //----------------------Receive Post-------------------------------------------------------------------------------------
        public static async void ReceiveMessage(Message message, string postname)
        {
            try
            {
                if (message.Photo != null || message.Video != null || message.Animation != null)
                {
                    DB db = new DB();
                    using (db.connection)
                    {
                        db.OpenConnection();
                        using (var command = db.connection.CreateCommand())
                        {
                            string photo_video = "";

                            if (message.Photo != null)
                            {
                                photo_video = JsonConvert.SerializeObject(message.Photo);
                            }
                            else if (message.Video != null)
                            {
                                photo_video = JsonConvert.SerializeObject(message.Video);
                                photo_video += "//video";
                            }
                            else
                            {
                                photo_video = JsonConvert.SerializeObject(message.Animation);
                                photo_video += "//animation";
                            }
                            var caption = message.Caption;
                            var replymarkup = JsonConvert.SerializeObject(message.ReplyMarkup);
                            command.CommandText = "INSERT INTO Messages (username ,postname,postvalue,caption,replymarkup) VALUES(@param1, @param2,@param3,@param4,@param5)";
                            command.Parameters.AddWithValue("@param1", message.From.Username);
                            command.Parameters.AddWithValue("@param2", postname);
                            command.Parameters.AddWithValue("@param3", photo_video);
                            command.Parameters.AddWithValue("@param4", caption);
                            command.Parameters.AddWithValue("@param5", replymarkup);

                            int rowCount = await command.ExecuteNonQueryAsync();
                            Console.WriteLine(String.Format("Number of rows inserted into Messages={0}", rowCount));
                        }
                    }
                    // connection will be closed by the 'using' block
                    Console.WriteLine("Closing connection");
                }

            }

            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        //----------------------------------------------------------------------------------------------------------------------
        //----------------------Remove Post-------------------------------------------------------------------------------------
        public static async void RemovePost(Message message)
        {
            try
            {
                DB db = new DB();
                using (db.connection)
                {
                    db.OpenConnection();
                    using (var command = db.connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM Messages WHERE postname = '" + message.Text + "'AND username= '" + message.From.Username + "'";
                        command.Parameters.AddWithValue("@name", "orange");
                        int rowCount = await command.ExecuteNonQueryAsync();
                        Console.WriteLine(String.Format("Number of rows deleted from Messages ={0}", rowCount));
                    }
                    Console.WriteLine("Closing connection");
                }
                Console.WriteLine("Closing connection");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        //----------------------------------------------------------------------------------------------------------------------
        //----------------------Check if user have posts-------------------------------------------------------------------------------------
        public static async Task<string> CheckPost(Message message)
        {
            try
            {
                DB db = new DB();
                String checkpost = "";
                using (db.connection)
                {
                    db.OpenConnection();
                    using (var command = db.connection.CreateCommand())
                    {
                        command.CommandText = "select * from Messages where username='" + message.From.Username + "' ";

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                checkpost = reader["username"].ToString();
                            }
                        }
                    }
                }

                return checkpost;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            return "";
        }

        //----------------------------------------------------------------------------------------------------------------------
        //----------------------Add Channel-------------------------------------------------------------------------------------

        public static async void ReceiveChannel(Message message, string channel)
        {
            try
            {
                DB db = new DB();
                using (db.connection)
                {
                    db.OpenConnection();
                    using (var command = db.connection.CreateCommand())
                    {
                        string query = @"IF EXISTS(SELECT * FROM Channels WHERE username = @param1)
                                                   UPDATE Channels
                                                   SET channel = @param2
                                                    WHERE username = @param1
                                                     ELSE
                                                     INSERT INTO Channels (username,channel) VALUES(@param1, @param2)";
                        command.CommandText = query;
                        command.Parameters.AddWithValue("@param1", message.From.Username);
                        command.Parameters.AddWithValue("@param2", channel);

                        int rowCount = await command.ExecuteNonQueryAsync();
                        Console.WriteLine(String.Format("Number of rows inserted into Channels={0}", rowCount));
                    }
                }
                // connection will be closed by the 'using' block
                Console.WriteLine("Closing connection to update channel for '" + message.From.Username + "' ");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        //----------------------------------------------------------------------------------------------------------------------
        //----------------------Remove Channel----------------------------------------------------------------------------------

        public static async void RemoveChannel(Message message)
        {
            try
            {
                DB db = new DB();
                using (db.connection)
                {
                    db.OpenConnection();
                    using (var command = db.connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM Channels where username='" + message.From.Username + "' ";

                        int rowCount = await command.ExecuteNonQueryAsync();
                        Console.WriteLine(String.Format("Number of rows removed from Channels={0}", rowCount));
                    }
                }
                // connection will be closed by the 'using' block
                Console.WriteLine("closing connection to remove channel for '" + message.From.Username + "' ");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        //----------------------------------------------------------------------------------------------------------------------
        //----------------------What Channel------------------------------------------------------------------------------------
        public static async Task<string> WhatChannel(Message message)
        {
            try
            {
                DB db = new DB();
                String sendwhere = "";
                using (db.connection)
                {
                    db.OpenConnection();
                    using (var command = db.connection.CreateCommand())
                    {
                        command.CommandText = "select * from Channels where username='" + message.From.Username + "' ";

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                sendwhere = reader["channel"].ToString();
                            }
                        }
                    }
                }

                return sendwhere;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            return "";
        }
        //----------------------------------------------------------------------------------------------------------------------
        //----------------------Add Time----------------------------------------------------------------------------------------
        public static async void AddTime(Message message, int time)
        {
            try
            {
                DB db = new DB();
                using (db.connection)
                {
                    db.OpenConnection();
                    using (var command = db.connection.CreateCommand())
                    {
                        string query = @"IF EXISTS(SELECT * FROM Times WHERE username = @param1)
                                                   UPDATE Times
                                                   SET timevalue = @param2
                                                    WHERE username = @param1
                                                     ELSE
                                                     INSERT INTO Times (username, timevalue) VALUES(@param1, @param2)";
                        command.CommandText = query;
                        command.Parameters.AddWithValue("@param1", message.From.Username);
                        command.Parameters.AddWithValue("@param2", time);


                        int rowCount = await command.ExecuteNonQueryAsync();
                        Console.WriteLine("close connection to add time for '" + message.From.Username + "' ");
                    }
                }
                // connection will be closed by the 'using' block
                Console.WriteLine("Closing connection");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        //----------------------------------------------------------------------------------------------------------------------
        //----------------------What Time---------------------------------------------------------------------------------------
        public static async Task<int> WhatTime(Message message)
        {
            try
            {
                DB db = new DB();
                string time = "";

                using (db.connection)
                {
                    db.OpenConnection();
                    using (var command = db.connection.CreateCommand())
                    {
                        command.CommandText = "select * from Times where username='" + message.From.Username + "' ";

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                time = reader["timevalue"].ToString();
                            }
                        }
                    }
                }
                if (time != "")
                    return Int16.Parse(time);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            return 0;
        }

        //--------------------------------------------------------------------------------------------------------------------
        //----------------------Show Posts------------------------------------------------------------------------------------
        public static async void ShowPosts(Message message)
        {
            try
            {
                //need to check if post list is empty
                DB db = new DB();
                List<string> postnames = new List<string>();

                using (db.connection)
                {
                    db.OpenConnection();
                    using (var command = db.connection.CreateCommand())
                    {

                        command.CommandText = "select * from Messages where username='" + message.From.Username + "' ";

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                postnames.Add(reader["postname"].ToString());
                            }
                        }
                    }
                    Console.WriteLine("close connection to show posts for '" + message.From.Username + "' ");
                }

                postnames.Add("חזרה לתפריט הראשי");
                var buttons = postnames.Select(category => new[] { new KeyboardButton(category) })
                    .ToArray();
                var replyMarkup = new ReplyKeyboardMarkup(buttons);

                await Bot.SendTextMessageAsync(
                       chatId: message.Chat.Id,
                       text: "בחר את המודעה אותה תרצו להסיר:",
                       replyMarkup: replyMarkup
                       );
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        //----------------------------------------------------------------------------------------------------------------------
        //----------------------Delete Post-------------------------------------------------------------------------------------
        public static async void DeletePost(Message message)
        {
            try
            {
                DB db = new DB();
                using (db.connection)
                {
                    db.OpenConnection();
                    using (var command = db.connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM Messages WHERE postname = '" + message.Text + "' AND username= '" + message.From.Username + "'";

                        int rowCount = await command.ExecuteNonQueryAsync();
                        Console.WriteLine(String.Format("Number of rows removed from Messages={0}", rowCount));
                    }
                }
                // connection will be closed by the 'using' block
                Console.WriteLine("close connection to delete post for '" + message.From.Username + "' ");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------
        //----------------------Check if user is valid-------------------------------------------------------------------------------------
        public static async Task<string> CheckUser(Message message)
        {
            try
            {
                DB db = new DB();
                String username = "";

                using (db.connection)
                {
                    db.OpenConnection();
                    using (var command = db.connection.CreateCommand())
                    {
                        command.CommandText = "select username from Users where username ='" + message.From.Username + "'";

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                username = reader["username"].ToString();
                            }
                        }
                    }
                }
                return username;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            return "";
        }
        //--------------------------------------------------------------------------------------------------------------------------------
        //----------------------Add User--------------------------------------------------------------------------------------------------
        public static async void AddUser(Message message)
        {
            try
            {
                DB db = new DB();
                using (db.connection)
                {
                    db.OpenConnection();
                    using (var command = db.connection.CreateCommand())
                    {
                        command.CommandText = " INSERT INTO Users(username) VALUES(@param1)";
                        command.Parameters.AddWithValue("@param1", message.Text);

                        int rowCount = await command.ExecuteNonQueryAsync();
                        Console.WriteLine("close connection to add user , from admin:'" + message.From.Username + "' ");
                    }
                }
                // connection will be closed by the 'using' block
                Console.WriteLine("Closing connection");
            }

            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        //--------------------------------------------------------------------------------------------------------------------------------
        //---------------------Remove User------------------------------------------------------------------------------------------------
        public static async void DeleteUser(Message message)
        {
            try
            {
                DB db = new DB();
                using (db.connection)
                {
                    db.OpenConnection();
                    using (var command = db.connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM Users WHERE username = '" + message.Text + "'";

                        int rowCount = await command.ExecuteNonQueryAsync();
                        Console.WriteLine(String.Format("Number of rows removed from Messages={0}", rowCount));
                    }
                }
                Console.WriteLine("close connection to delete user , from admin:'" + message.From.Username + "' ");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }









    }
}
