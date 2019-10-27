using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace HowLong.Models
{
	public class TimeAccount
	{
		[Key]
		public Guid AccountId { get; set; }
        public DateTime WorkDate { get; set; } = DateTime.Today;
		public TimeSpan StartWorkTime { get; set; } = DateTime.Now.TimeOfDay;
        public TimeSpan EndWorkTime { get; set; } = DateTime.Now.TimeOfDay;
        public ObservableCollection<Break> Breaks { get; set; } = new ObservableCollection<Break>();
        public double OverWork { get; set; }
		public bool IsClosed { get; set; }
        public bool IsStarted { get; set; }
        public bool IsWorking { get; set; } = true;
    }
}
