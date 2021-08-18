using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace TelegramBot
{
    public static class MainClass
    {
        //A client to use Telegram bot API
        public static TelegramBotClient Bot;
        //Declaration of task list
        private static readonly List<Task> _tasks = new List<Task>();
        //Declaration of dictionary<token,username>
        private static readonly Dictionary<CancellationTokenSource, string> _taskDictionary = new Dictionary<CancellationTokenSource, string>();

        [Obsolete]
        public static async Task StartListening()
        {
#if USE_PROXY
            var Proxy = new WebProxy(Configuration.Proxy.Host, Configuration.Proxy.Port) { UseDefaultCredentials = true };
            Bot = new TelegramBotClient(Configuration.BotToken, webProxy: Proxy);
#else
            Bot = new TelegramBotClient(Configuration.BotToken);
#endif
            var user = await Bot.GetMeAsync();
            Console.Title = user.Username;
            //Start receiving
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnReceiveError += Receive.BotOnReceiveError;
            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{user.Username}");

            //stop receiving
            Console.ReadLine();
            Bot.StopReceiving();
        }


        //-----------------------------BotOnMessage----------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------
        private const string case1 = "התחל פרסום🟢/הפסק פרסום⛔️";
        private const string case2 = "זמן פרסום 🕚";
        private const string case3 = "הסר ערוץ,קבוצה➖";
        private const string case4 = "הוסף ערוץ,קבוצה ➕";
        private const string case5 = "הסר מודעה➖";
        private const string case6 = "הוסף מודעה➕";

        [Obsolete]
        public static async void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                var replyKeyboardMarkup = new ReplyKeyboardMarkup();


                var message = e.Message;
                if (message == null || message.Type != MessageType.Text)
                    return;

                var valid = Task.Run(() => SQL.CheckUser(message)).Result;

                if (valid != "")
                {
                    switch (message.Text)
                    {
                        //----------------------------Start/Stop post------------------------------------------------
                        case case1:
                            //------------------------Delete task if exists----------------------------------------
                            //checking if user have posts to post
                            var checkpost = Task.Run(() => SQL.CheckPost(message)).Result;
                            //checking if user already have post running    
                            int exists = 0;
                            foreach (var taskX in _taskDictionary)
                            {
                                if (taskX.Value.Contains(valid))
                                {
                                    exists = 1;
                                    taskX.Key.Cancel();
                                    _taskDictionary.Remove(taskX.Key);

                                    await Bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "הפרסומים הופסקו!\n" + "/start  - לחץ לחזרה", replyMarkup: new ReplyKeyboardRemove());
                                }
                            }

                            //-------------------Else Start posting-----------------------------------------------------------

                            if (checkpost != "" && exists == 0)
                            {
                                var sendwhere = Task.Run(() => SQL.WhatChannel(message)).Result;
                                var time = Task.Run(() => SQL.WhatTime(message)).Result;

                                if (sendwhere == "")
                                {
                                    await Bot.SendTextMessageAsync(e.Message.Chat.Id, "אין לך ערוץ/קבוצה רשומsים במערכת");
                                }
                                if (time == 0)
                                {
                                    await Bot.SendTextMessageAsync(e.Message.Chat.Id, "הגדר זמן פרסום");

                                }
                                if (sendwhere != "" && time != 0)
                                {
                                    // Define the cancellation token.
                                    CancellationTokenSource source = new CancellationTokenSource();
                                    CancellationToken token = source.Token;
                                    //New task
                                    Task MyTask = new Task(async () => await StartPosting(token, sendwhere, time, message));
                                    //Add task to dictionary
                                    _taskDictionary.Add(source, valid);
                                    //Add thread and to ThreadList
                                    _tasks.Add(MyTask);
                                    //start task
                                    MyTask.Start();
                                    await Bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "הפרסום התחיל!\n" + "/start  - לחץ לחזרה", replyMarkup: new ReplyKeyboardRemove());

                                }
                            }
                            else if (checkpost == "")
                            {
                                await Bot.SendTextMessageAsync(e.Message.Chat.Id, "אין לך מודעות !");
                                break;
                            }

                            break;

                        //----------------------------Add Time-----------------------------------------
                        case case2:
                            await Bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "הגדר זמן בדקות", replyMarkup: new ReplyKeyboardRemove());
                            Bot.OnMessage += Receive.ReceiveTime;

                            break;
                        //----------------------------Remove channel-----------------------------------
                        case case3:
                            SQL.RemoveChannel(message);
                            await Bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "הערוץ/קבוצה הוסר בהצלחה!");
                            break;
                        //----------------------------Add Channel---------------------------------------
                        case case4:
                            await Bot.SendTextMessageAsync(message.Chat.Id, "שם הערוץ/קבוצה עם-@ בהתחלה", replyMarkup: new ReplyKeyboardRemove());
                            Bot.OnMessage += Receive.ReceiveChannelName;

                            break;
                        //----------------------------Remove Posts---------------------------------------
                        case case5:
                            SQL.ShowPosts(message);
                            Bot.OnMessage += Receive.ReceivePostName;
                            break;
                        //----------------------------Add Post-------------------------------------------
                        case case6:
                            await Bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "שם המודעה?", replyMarkup: new ReplyKeyboardRemove());
                            Bot.OnMessage += Receive.ReceivePost;
                            break;

                        //----------------------------Keyboard---------------------------------------------

                        case "/start":
                            await Bot.SendTextMessageAsync(e.Message.Chat.Id, "בחרו אפשרות:", replyMarkup:
                                 replyKeyboardMarkup = new ReplyKeyboardMarkup(
                            new KeyboardButton[][]
                            {
                        new KeyboardButton[]
                        {
                             new  KeyboardButton(case1),
                        },
                        new KeyboardButton[]
                        {
                             new KeyboardButton(case2)
                        },
                         new KeyboardButton[]
                        {
                            new KeyboardButton(case3),
                            new KeyboardButton(case4)
                        },
                          new KeyboardButton[]
                        {
                            new KeyboardButton(case5),
                            new KeyboardButton(case6)
                        },
                            },
                            resizeKeyboard: true
                            ));
                            break;
                        case "/add":
                            if (valid == "" || valid == "")
                            {
                                await Bot.SendTextMessageAsync(chatId: message.Chat.Id, text: " רשום את שם המשתמש אותו תרצו להוסיף בלי @ ", replyMarkup: new ReplyKeyboardRemove());
                                Bot.OnMessage += Receive.ReceiveUser;
                            }
                            break;
                        case "/delete":
                            if (valid == "" || valid == "")
                            {
                                await Bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "  רשום את שם המשתמש אותו תרצו להסיר בלי @", replyMarkup: new ReplyKeyboardRemove());
                                Bot.OnMessage += Receive.RemoveUser;
                            }
                            break;
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        //------------------------------Start Posting-----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static async Task<string> StartPosting(CancellationToken ct, string sendwhere, int time, Message message)
        {
            while (true)
            {
                try
                {
                    String photo_video = "", caption = "", replymarkup = "";
                    int result = 0;

                    DB db = new DB();
                    using (db.connection)
                    {
                        Console.WriteLine("Start posting for'" + message.From.Username + "'");
                        db.connection.Open();

                        using (var command = db.connection.CreateCommand())
                        {
                            command.CommandText = "select * from Messages where username='" + message.From.Username + "' ";
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    photo_video = reader["postvalue"].ToString();
                                    caption = reader["caption"].ToString();
                                    replymarkup = reader["replymarkup"].ToString();

                                    InlineKeyboardMarkup keyboard = JsonConvert.DeserializeObject<InlineKeyboardMarkup>(replymarkup);
                                    if (photo_video.Contains("animation"))
                                    {
                                        var video = JsonConvert.DeserializeObject<Json.VideoProperties>(photo_video);
                                        result = Bot.SendAnimationAsync(
                                               chatId: sendwhere,
                                               animation: video.FileId,
                                               caption: caption,
                                               replyMarkup: keyboard
                                             ).GetAwaiter().GetResult().MessageId;
                                    }
                                    else if (photo_video.Contains("video"))
                                    {
                                        var video = JsonConvert.DeserializeObject<Json.VideoProperties>(photo_video);
                                        result = Bot.SendVideoAsync(
                                               chatId: sendwhere,
                                               video: video.FileId,
                                               caption: caption,
                                               replyMarkup: keyboard
                                             ).GetAwaiter().GetResult().MessageId;
                                    }
                                    else
                                    {
                                        var photo = JsonConvert.DeserializeObject<List<Json.PhotoProperties>>(photo_video);
                                        result = Bot.SendPhotoAsync(
                                                   chatId: sendwhere,
                                                   photo: photo[2].FileId,
                                                   caption: caption,
                                                   replyMarkup: keyboard
                                                 ).GetAwaiter().GetResult().MessageId;
                                    }
                                    //pinning message
                                    //await Bot.PinChatMessageAsync(sendwhere, result);
                                    await Task.Delay((time * 60000), ct);
                                    if (ct.IsCancellationRequested)
                                    {
                                        Console.WriteLine("task canceled");
                                        command.CommandText = "INSERT INTO Messages (lastpost) VALUES(@param1)";
                                        command.Parameters.AddWithValue("@param1", "last");
                                        await command.ExecuteNonQueryAsync();
                                        break;
                                    }
                                }
                            }
                            Console.WriteLine("'" + message.From.Username + "'stopped posting");
                        }
                    }
                }
                catch (Telegram.Bot.Exceptions.ApiRequestException err)
                {
                    if (err.Message.Contains("bot is not a member of the channel"))
                        await Bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "הבוט לא נמצא בערוץ/קבוצה שלך", replyMarkup: new ReplyKeyboardRemove());
                    Console.WriteLine(err.Message);
                    return err.Message;
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    return err.Message;
                }
            }
        }





    }
}