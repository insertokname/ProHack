using System.Xml.Serialization;

namespace Infrastructure.Database
{
    public static class Database
    {
        public static DatabaseTables Tables = new();

        public static event Action? OnUpdate;

        public static string FileName { get { return nameof(Database) + ".xml"; } }

        private static readonly XmlSerializer _xmlSerializer = new(typeof(DatabaseTables));

        public static void Save()
        {
            StreamWriter sw = new(FileName);
            try
            {
                _xmlSerializer.Serialize(sw, Tables);
            }
            catch (Exception e)
            {
                sw.Close();
                throw new Exception("An error occured during serialization of the database tables.", e);
            }
            sw.Close();
            OnUpdate?.Invoke();
        }

        public static void Load()
        {
            StreamReader sr = new(FileName);
            try
            {
                Tables = (DatabaseTables)_xmlSerializer.Deserialize(sr)!;
            }
            catch (Exception e)
            {
                sr.Close();
                throw new Exception("An error occured during deserialization of the database tables.", e);
            }
            sr.Close();
            OnUpdate?.Invoke();
        }
    }
}