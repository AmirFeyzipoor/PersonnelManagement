using System.Linq.Expressions;

namespace PersonnelManagement.IntegrationTest.Infrastructure
{
    public static class Runner
    {
        public static void RunScenario(
            params Expression<Action<object>>[] steps)
        {
            var textContext = new { };
            steps.Select(_ => _.Compile()).ForEach(_ => _.Invoke(textContext));
        }
    }
}