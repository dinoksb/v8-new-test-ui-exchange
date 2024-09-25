namespace G2
{
    public static class Config
    {
        public const string SDK_VERSION = "0.1.0";
        public const bool LOCAL_VERSE_SERVICE = false;
        public const bool LOCAL_GAME_SERVICE = false;
        public const string CONSOLE_URL = "https://console.g2platform.com";
        private const string _DATA_ENDPOINT = "https://api.g2platform.com";
        public const string DATA_SERVICE_ENDPOINT = _DATA_ENDPOINT;
        public const string VERSE_SERVICE_ENDPOINT = _DATA_ENDPOINT;
        
        public static class LayerName
        {
            public const string UI = "UI";
        }

        public static class FileExtensions
        {
            public const string PNG = ".png";
        }
    }
}
