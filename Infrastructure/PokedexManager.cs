using Domain;
using System.Text.Json;

namespace Infrastructure
{
    public class PokedexManager
    {
        public static string DataPath { get; } = "DATA";
        public static string PokedexPath { get; } = Path.Combine(DataPath, "pokedex.json");
        public static string SpritesPath { get; } = Path.Combine(DataPath, "sprites");
        public static string ShinySpritesPath { get; } = Path.Combine(DataPath, "shiny_sprites");

        public int? MaxIndex { get; private set; }

        public PokedexModel? Pokedex { get; private set; } = null;

        private Dictionary<PokedexModel.PokedexEntryModel, Bitmap>? _loadedSprites = null;

        public void Load()
        {
            try
            {
                Pokedex = _getPokedex();
                _loadedSprites = _loadSprites(Pokedex);
                MaxIndex = Pokedex.Entries.MaxBy(e => e.ID)?.ID;
            }
            catch (Exception e)
            {
                Unload();
                throw new Exception("An error occured while loading the pokedex!", e);
            }
        }

        public void Unload()
        {

            Pokedex = null;
            _unloadSprites();
            MaxIndex = null;
        }

        public Bitmap? GetAsset(PokedexModel.PokedexEntryModel entry)
        {
            var sprite = _loadedSprites?[entry];
            if (sprite == null)
                return null;
            return new Bitmap(sprite);
        }

        public List<Bitmap> GetShinyAssets(PokedexModel.PokedexEntryModel entry)
        {
            List<Bitmap> output = [];
            var files = Directory.GetFiles(ShinySpritesPath)
                .Select(f => Path.GetFileName(f))
                .Where(f => f.StartsWith($"{entry.ID}_") || f == $"{entry.ID}.png" || f == $"{entry.ID}f.png");
            foreach (var file in files)
            {
                var fileName = Path.Combine(ShinySpritesPath, file);
                var bmp = new Bitmap(fileName);
                output.Add(bmp);
            }
            return output;
        }

        public List<Bitmap> GetEventAssets(PokedexModel.PokedexEntryModel entry)
        {
            List<Bitmap> output = [];
            var files = Directory.GetFiles(SpritesPath)
                .Select(f => Path.GetFileName(f))
                .Where(f => f.StartsWith($"{entry.ID}_"));
            foreach (var file in files)
            {
                var fileName = Path.Combine(SpritesPath, file);
                var bmp = new Bitmap(fileName);
                output.Add(bmp);
            }
            return output;
        }


        private static PokedexModel _getPokedex()
        {
            var content = File.ReadAllText(PokedexPath);
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            var entries = JsonSerializer.Deserialize<List<PokedexModel.PokedexEntryModel>>(content, options)
                ?? throw new Exception("Couldn't parse pokedex json!");

            return new PokedexModel { Entries = entries };
        }

        private static Dictionary<PokedexModel.PokedexEntryModel, Bitmap> _loadSprites(PokedexModel pokedex)
        {
            Dictionary<PokedexModel.PokedexEntryModel, Bitmap> output = [];

            List<PokedexModel.PokedexEntryModel> badEntries = [];

            foreach (var entry in pokedex.Entries)
            {
                try
                {
                    var fileName = Path.Combine(SpritesPath, $"{entry.ID}.png");

                    var bmp = new Bitmap(fileName);
                    output.Add(entry, bmp);
                }
                catch (Exception)
                {
                    badEntries.Add(entry);
                }
            }

            foreach (var entry in badEntries)
            {
                pokedex.Entries.Remove(entry);
            }

            return output;
        }

        private void _unloadSprites()
        {
            if (_loadedSprites != null)
            {
                foreach (var sprite in _loadedSprites.Values)
                {
                    sprite.Dispose();
                }
            }
            _loadedSprites = null;
        }
    }
}
