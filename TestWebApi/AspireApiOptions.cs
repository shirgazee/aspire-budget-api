namespace TestWebApi
{
    public class AspireApiOptions
    {
        /// <summary>
        /// Google Sheet Id
        /// </summary>
        public string SheetId { get; set; }
        
        /// <summary>
        /// Base64-encoded credentials file
        /// </summary>
        public string ApiCredentialsBase64 { get; set; }
    }
}