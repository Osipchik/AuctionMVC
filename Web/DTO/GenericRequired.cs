using System.ComponentModel.DataAnnotations;

namespace Web.DTO
{
    public class GenericRequired : RequiredAttribute
    {
        public GenericRequired()
        {
            ErrorMessage = "{0} is required";
        }
    }
}