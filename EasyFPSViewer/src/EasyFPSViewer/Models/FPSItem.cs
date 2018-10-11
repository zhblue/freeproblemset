
namespace EasyFPSViewer.Models
{
    public class FPSItem
    {
        public string Title { get; set; } = "No Title";
        public int TimeLimit { get; set; } = 1;
        public string TimeLimitUnit { get; set; } = "s";
        public int MemoryLimit { get; set; } = 256;
        public string MemoryLimitUnit { get; set; } = "mb";
        public string Description { get; set; } = "No Description";
        public string Input { get; set; } = "";
        public string Output { get; set; } = "";
        public string SampleInput { get; set; } = "";
        public string SampleOutput { get; set; } = "";
        public string Hint { get; set; } = "";
        public string Source { get; set; } = "";
        public FPSItemSolution[] Solutions { get; set; }
        public string[] TestInput { get; set; }
        public string[] TestOutput { get; set; }
        public FPSItemImage[] Images { get; set; }
        public FPSItemSpecialJudge[] SpecialJudge { get; set; }

        public int TestDataSize { get; set; }
    }
}
