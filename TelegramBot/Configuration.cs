public static class Configuration
{
    public readonly static string BotToken = "1830456578:AAFm-d1TughhJOvyNYMvw_cnk3m9U7TRohM";

#if USE_PROXY
        public static class Proxy
        {
            public readonly static string Host = "{PROXY_ADDRESS}";
            public readonly static int Port = 8080;
        }
#endif
}
