using System.Linq;

namespace Airslip.Identity.MongoDb.Contracts
{
    public static class CompositeId
    {
        public static string Build(params string[] keys)
        {
            if (keys.Length == 0)
                return string.Empty;

            string result = keys.Aggregate(
                string.Empty,
                (current, key) => current + $"|{key}");

            return result[1..];
        }
    }
}