using CardService.Application.Common.Attributes;
using CardService.Application.Common.Enums;
using System.Reflection;

namespace CardService.Application.Common.Extensions
{
    public static class ResponseFormatExtension
    {

        public static int GetStatusCode(this ResponseCodes value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            object[] attribs = field.GetCustomAttributes(typeof(ServiceStatusAttribute), true);
            if (attribs.Length > 0)
            {
                return ((ServiceStatusAttribute)attribs[0]).ResponseCode;
            }

            return 9999;
        }

        public static string GetStatusMessage(this ResponseCodes value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            object[] attribs = field.GetCustomAttributes(typeof(ServiceStatusAttribute), true);
            if (attribs.Length > 0)
            {
                return ((ServiceStatusAttribute)attribs[0]).Message;
            }

            return value.ToString();
        }
    }
}
