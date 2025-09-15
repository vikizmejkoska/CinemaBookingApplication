using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingApplication.Domain.Dtos
{
    public class MovieImportDto
    {
        public string Title { get; set; } = default!;
        public string? Overview { get; set; }
        public int Runtime { get; set; }
        public double Rating { get; set; }
        public string? PosterUrl { get; set; }
    }
}
