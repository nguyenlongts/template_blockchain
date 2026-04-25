using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class IgnorePropertiesResolver : DefaultContractResolver
{
    private readonly HashSet<string> _ignore;

    public IgnorePropertiesResolver(params string[] props)
    {
        _ignore = new HashSet<string>(props);
    }

    protected override JsonProperty CreateProperty(
        System.Reflection.MemberInfo member,
        MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);
        if (_ignore.Contains(prop.PropertyName!))
            prop.ShouldSerialize = _ => false;
        return prop;
    }
}
namespace BlockchainDiemAPI.Services
{
    public static class FileManager
    {
        private static readonly string _path = "data/blockchain.json";

        public static void SaveBlockchain(BlockChain chain)
        {
            Directory.CreateDirectory("data");

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new IgnorePropertiesResolver("NextBlock")
            };

            var json = JsonConvert.SerializeObject(chain.Blocks, settings);
            File.WriteAllText(_path, json);
        }

        public static BlockChain LoadBlockchain()
        {
            var chain = new BlockChain();
            if (!File.Exists(_path)) return chain;

            var json = File.ReadAllText(_path);

            var blocks = JsonConvert.DeserializeObject<List<Block>>(json);
            if (blocks == null || blocks.Count == 0) return chain;

            foreach (var block in blocks.OrderBy(b => b.BlockNumber))
                chain.AcceptBlock(block);

            return chain;
        }
    }
}