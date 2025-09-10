using System.Text.Json;

namespace LaboGrocery.Services
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var str = session.GetString(key);
            return str == null ? default : JsonSerializer.Deserialize<T>(str);
        }
    }
}
