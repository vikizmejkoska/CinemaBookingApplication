using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingApplication.Domain.Dtos
{
    public class CreateReservationViewModel
    {
        [Required]
        public Guid ScreeningId { get; set; }

        [Range(1, 20)]
        public int Quantity { get; set; } = 1;
    }
}
