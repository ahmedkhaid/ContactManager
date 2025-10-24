using System.Security.AccessControl;

namespace CustomException
{
    public class InvalidPersonIdException:Exception
    {
        
        public InvalidPersonIdException():base()
        {
            
        }
        public InvalidPersonIdException(string?message):base(message: message)
        {
            
        }
        public InvalidPersonIdException(string?message,Exception?innerException)
        {
            
        }

    }
}
