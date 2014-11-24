using System;
using System.Collections.Generic;
using System.Text;

using SofiaPublicTransport.DataModel;

namespace SofiaPublicTransport.ViewModels
{
    public class ViewModelSchedule : ViewModelBase
    {
        private ScheduleDataModel schedule;
        private string test;

        public ViewModelSchedule()
            : this(new ScheduleDataModel())
        {
        }

        public ViewModelSchedule(ScheduleDataModel schedule)
        {
            this.schedule = schedule;
            this.Test = "Please work";
        }

        public ScheduleDataModel Schedule
        {
            get
            {
                return this.schedule;
            }
            set
            {
                this.schedule = value;
                this.OnPropertyChanged("Schedule");
            }
        }

        public string Test
        {
            get
            {
                return this.test;
            }
            set
            {
                this.test = value;
                this.OnPropertyChanged("Test");
            }
        }
    }
}
