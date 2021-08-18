using Newtonsoft.Json;

namespace TelegramBot
{
    class Json
    {
        //----------------------photo-------------------------------
        //Set photo properties for deserialization
        public class PhotoProperties
        {
            [JsonProperty("width")]
            public string Width { get; set; }

            [JsonProperty("height")]
            public string Height { get; set; }

            [JsonProperty("file_id")]
            public string FileId { get; set; }

            [JsonProperty("file_size")]
            public string FileSize { get; set; }
        }
        //----------------------video------------------------------
        //Set video properties for deserialization
        public class VideoProperties
        {
            [JsonProperty("duration")]
            public string Duration { get; set; }

            [JsonProperty("file_id")]
            public string FileId { get; set; }

            [JsonProperty("file_size")]
            public string File_size { get; set; }

            [JsonProperty("height")]
            public string Height { get; set; }

            [JsonProperty("mime_type")]
            public string Mime_type { get; set; }

            [JsonProperty("width")]
            public string Width { get; set; }

            [JsonProperty("thumb")]

            public PhotoProperties thumb = new PhotoProperties();

        }


    }
}
