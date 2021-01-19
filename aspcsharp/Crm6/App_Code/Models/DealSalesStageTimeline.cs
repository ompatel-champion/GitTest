using System;

namespace Models
{
    public class DealSalesStageTimeline
    {
        public int DealId { get; set; }
        public int SalesStageId { get; set; }
        public string SalesStage { get; set; }
        public DateTime? AddedDate { get; set; }
        public double DaysInStage { get; set; }
        public int AddedBy { get; set; }
        public string AddedName { get; set; }
    }
}