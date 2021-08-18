using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    class Receive
    {
        //--------------------------Receive Channel Name----------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------
        public static void ReceiveChannelName(object sender, MessageEventArgs e)
        {
            try
            {
                var message = e.Message;
                if (message == null)
                    return;
                if (message.Type == MessageType.Text)
                {
                    Regex reg = new Regex("^[a-zA-Z0-9@]*$");
                    //לתקן כי הוא לא מקבל קו תחתון או רווח
                    if (reg.Match(message.Text).Success && message.Text.Contains("@"))
                    {
                        SQL.ReceiveChannel(message, message.Text.ToString());
                        MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "נוסף בהצלחה !\n" + "/start  - לחץ לחזרה");
                        MainClass.Bot.OnMessage -= ReceiveChannelName;

                    }
                    else
                    {
                        MainClass.Bot.SendTextMessageAsync(message.Chat.Id, " לרשום את שם הערוץ/קבוצה באנגלית ו@ בהתחלה\n" + "/start  - לחץ לחזרה ");
                        MainClass.Bot.OnMessage -= ReceiveChannelName;
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        //--------------------------Receive Post--------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------

        private static string _postName = "";
        public static void ReceivePost(object sender, MessageEventArgs e)
        {
            try
            {
                var message = e.Message;
                if (message == null)
                    return;
                if (message.Type == MessageType.Text)
                {
                    Regex reg = new Regex("(^[a-zA-Z0-9_]*$)");
                    if (reg.Match(message.Text).Success)
                    {
                        _postName = message.Text;
                        MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "העבר לכאן את התמונה/סרטון עם הכפתורים");
                    }
                    else
                    {
                        MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "שם המודעה באנלית, במקום_רווח_מקף_תחתון\n" + "/start  - לחץ לחזרה ");
                        MainClass.Bot.OnMessage -= ReceivePost;
                    }
                }


                if (message.Photo != null && _postName != "")
                {
                    SQL.ReceiveMessage(message, _postName);
                    MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "נוסף בהצלחה !\n" + "/start  - לחץ לחזרה");
                    MainClass.Bot.OnMessage -= ReceivePost;

                }
                if (message.Animation != null && _postName != "")
                {
                    SQL.ReceiveMessage(message, _postName);
                    MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "נוסף בהצלחה !\n" + "/start  - לחץ לחזרה");
                    MainClass.Bot.OnMessage -= ReceivePost;

                }
                else if (message.Video != null && _postName != "")
                {
                    SQL.ReceiveMessage(message, _postName);
                    MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "נוסף בהצלחה !\n" + "/start  - לחץ לחזרה");
                    MainClass.Bot.OnMessage -= ReceivePost;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }


        //--------------------------Recieve Time------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------
        public static void ReceiveTime(object sender, MessageEventArgs e)
        {
            try
            {
                var message = e.Message;
                if (message == null)
                    return;
                if (message.Type == MessageType.Text)
                {
                    Regex reg = new Regex(@"^\d+$");
                    if (reg.Match(message.Text).Success)
                    {
                        SQL.AddTime(message, Int32.Parse(message.Text));
                        MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "זמן פרסום נשמר בהצלחה !\n" + "/start  - לחץ לחזרה");
                        MainClass.Bot.OnMessage -= ReceiveTime;
                    }
                    else
                    {
                        MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "זמן הפרסום לא תקין אנא חזרו על הפעולה\n" + "/start  - לחץ לחזרה");
                        MainClass.Bot.OnMessage -= ReceiveTime;
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        //--------------------------Delete Post------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------
        public static void ReceivePostName(object sender, MessageEventArgs e)
        {
            try
            {
                var message = e.Message;
                if (message == null)
                    return;
                if (message.Type == MessageType.Text && message.Text != "חזרה לתפריט הראשי")
                {
                    SQL.DeletePost(message);
                    MainClass.Bot.OnMessage -= ReceivePostName;
                    MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "המודעה הוסרה בהצלחה!\n" + "/start  - לחץ לחזרה ", replyMarkup: new ReplyKeyboardRemove());

                }
                else
                {
                    MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "חזרו לתפריט הראשי!\n" + "/start  - לחץ לחזרה ", replyMarkup: new ReplyKeyboardRemove());
                    MainClass.Bot.OnMessage -= ReceivePostName;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        //-------------------------Add User------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------
        public static void ReceiveUser(object sender, MessageEventArgs e)
        {
            try
            {
                var message = e.Message;
                if (message == null)
                    return;
                if (message.Type == MessageType.Text)
                {
                    Regex reg = new Regex("^[a-zA-Z0-9]+$");
                    if (reg.Match(message.Text).Success)
                    {
                        SQL.AddUser(message);
                        MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "המשתמש נוסף למערכת בהצחלה !\n" + "/start  - לחץ לחזרה ", replyMarkup: new ReplyKeyboardRemove());
                        MainClass.Bot.OnMessage -= ReceiveUser;

                    }
                    else
                    {
                        MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "המשתמש בנוי מאותיות באנגלית מספרים וקו תחתון !\n" + "/add  - לחץ לנסות שוב ", replyMarkup: new ReplyKeyboardRemove());
                        MainClass.Bot.OnMessage -= ReceiveUser;
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        //-------------------------Delete User------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------
        public static void RemoveUser(object sender, MessageEventArgs e)
        {
            try
            {
                var message = e.Message;
                if (message == null)
                    return;
                if (message.Type == MessageType.Text)
                {
                    Regex reg = new Regex("^[a-zA-Z0-9]+$");
                    if (reg.Match(message.Text).Success)
                    {
                        SQL.DeleteUser(message);
                        MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "המשתמש הוסר מהמערכת בהצחלה !\n" + "/start  - לחץ לחזרה ", replyMarkup: new ReplyKeyboardRemove());
                        MainClass.Bot.OnMessage -= RemoveUser;

                    }
                    else
                    {
                        MainClass.Bot.SendTextMessageAsync(message.Chat.Id, "המשתמש בנוי מאותיות באנגלית מספרים וקו תחתון !\n" + "/delete  - לחץ לנסות שוב ", replyMarkup: new ReplyKeyboardRemove());
                        MainClass.Bot.OnMessage -= RemoveUser;
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        //----------------------Bot Receive Error---------------------------------------------------------------------------------

        public static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }






    }
}
