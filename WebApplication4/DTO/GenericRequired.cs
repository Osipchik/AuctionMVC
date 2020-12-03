using System.ComponentModel.DataAnnotations;

namespace WebApplication4.DTO
{
    public class GenericRequired : RequiredAttribute
    {
        public GenericRequired()
        {
            ErrorMessage = "{0} is required";
        }
    }
}