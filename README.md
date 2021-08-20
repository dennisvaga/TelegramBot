# TelegramBot

1. Create SQL Server database, then use this query to create tabels.<br />                                                                                
   CREATE TABLE Messages (username varchar(30),postname varchar(30),postvalue nvarchar(MAX),caption nvarchar(MAX),replymarkup nvarchar(MAX))
   CREATE TABLE Channels (username varchar(30) PRIMARY KEY,channel varchar(30))<br />
   CREATE TABLE Times (username varchar(30) PRIMARY KEY, timevalue INTEGER)<br />
   CREATE TABLE Users (username varchar(30) PRIMARY KEY)<br />
   
2. In DB class, change SqlConnection to your own connection.

3. In configuration class, change BotToken to your bot token.
   For additional information visit: https://core.telegram.org/bots
