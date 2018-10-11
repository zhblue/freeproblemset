using System.Text;
using EasyFPSViewer.Models;

namespace EasyFPSViewer.Core
{
    public static class FPSViewBuilder
    {
        public static string Build(FPSItem item)
        {
            StringBuilder sb = new StringBuilder(Properties.Resources.FPSWebView);
            sb.Replace("[[[Title]]]", item.Title);
            sb.Replace("[[[Description]]]", item.Description);
            sb.Replace("[[[TimeLimit]]]", item.TimeLimit + item.TimeLimitUnit);
            sb.Replace("[[[MemoryLimit]]]", item.MemoryLimit + item.MemoryLimitUnit);
            sb.Replace("[[[Input]]]", item.Input);
            sb.Replace("[[[Output]]]", item.Output);
            sb.Replace("[[[Sample Input]]]", item.SampleInput);
            sb.Replace("[[[Sample Output]]]", item.SampleOutput);
            sb.Replace("[[[Hint]]]", item.Hint);
            sb.Replace("[[[Source]]]", item.Source);
            return sb.ToString();
        }
    }
}
