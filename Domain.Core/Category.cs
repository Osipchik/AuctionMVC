using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Core
{
    public class Category : Entity
    {
        [DataType(DataType.Text)]
        public string Name { get; set; }
        
        public List<Lot> Lots { get; set; }
    }
}