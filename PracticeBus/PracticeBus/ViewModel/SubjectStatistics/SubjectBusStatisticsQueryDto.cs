namespace PracticeBus.ViewModel.SubjectStatistics
{
    public class SubjectBusStatisticsQueryDto
    {
        public string SubjectName { get; set; } = string.Empty;

        public decimal? FinishPercent { get; set; }

        public decimal? PassPercent { get; set; }
    }
}
