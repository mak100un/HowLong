using System;
using System.ComponentModel.DataAnnotations;

namespace HowLong.Models
{
    public class Break
    {
        [Key]
        public Guid BreakId { get; set; }
        public double StartBreakTime { get; set; }
        public double EndBreakTime { get; set; }
        public TimeAccount TimeAccount { get; set; }
    }
}
