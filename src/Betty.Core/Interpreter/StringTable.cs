namespace Betty.Core.Interpreter
{
    public static class StringTable
    {
        private static readonly List<string> _strings = new List<string>();
        private static readonly Dictionary<string, int> _stringToId = new Dictionary<string, int>();

        public static int AddString(string str)
        {
            if (_stringToId.TryGetValue(str, out var id))
            {
                return id;
            }
            else
            {
                _strings.Add(str);
                int newId = _strings.Count - 1;
                _stringToId[str] = newId;
                return newId;
            }
        }

        public static string GetString(int id)
        {
            return _strings[id];
        }
    }
}