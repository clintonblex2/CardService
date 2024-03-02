namespace CardService.Application.Common.Helpers
{
    public static class BaseStrings
    {
        // cache keys
        public const string USER_TBL = "Users";
        public const string CARD_TBL = "Cards";

        //Successful
        public const string SUCCESSFUL = "Request was successful";
        public const string SUCCESSFUL_CARD_CREATION = "Card created successfully";
        public const string SUCCESSFUL_CARD_UPDATE = "Card updated successfully";
        public const string SUCCESSFUL_CARD_DELETE = "Card deleted successfully";
        public const string SUCCESSFUL_CODE = "200";

        //User request error
        public const string BAD_REQUEST = "Request cannot be completed at the moment, please try again.";
        public const string RECORD_NOT_FOUND = "Record not found, please try again.";
        public const string NOT_FOUND_CODE = "404";
        public const string VALIDATION_ERROR_CODE = "400";
        public const string UNAUTHORIZED_ERROR_CODE = "401";
        public const string FORBIDDEN_ERROR_CODE = "403";
        public const string CARD_ALREADY_EXIST = "Card already exist with the same name. Kindly update the card name and retry!";
        public const string CARD_NOT_EXIST = "Card does not exist.";

        //Unexpected error
        public const string SERVER_ERROR_CODE = "500";
        public const string REQUEST_TIMEOUT_CODE = "509";

        public const int REST_REQUEST_TIMEOUT = 60;
    }
}
