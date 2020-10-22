using CoreCodeCamp.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Models
{
    public class CampModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string Moniker { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;

        [Range(1,100)]
        public int Length { get; set; } = 1;

        [Required]
        public string Venue { get; set; }
        public string LocationAddressLine1 { get; set; }
        public string LocationAddressLine2 { get; set; }
        public string LocationAddressLine3 { get; set; }
        public string LocationCityTown { get; set; }
        public string LocationStateProvince { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }

        public ICollection<TalkModel> Talks;
    }
}
