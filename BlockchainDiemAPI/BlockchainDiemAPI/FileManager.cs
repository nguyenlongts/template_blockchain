using Newtonsoft.Json;

namespace BlockchainDiemAPI.Services
{
    public static class FileManager
    {
        private static readonly string _path = "data/blockchain.json";

        public static void SaveBlockchain(BlockChain chain)
        {
            Directory.CreateDirectory("data");
            var json = JsonConvert.SerializeObject(chain.Blocks, Formatting.Indented);
            File.WriteAllText(_path, json);
        }

        public static BlockChain LoadBlockchain()
        {
            var chain = new BlockChain();
            if (!File.Exists(_path)) return chain;

            var json = File.ReadAllText(_path);
            var blocks = JsonConvert.DeserializeObject<List<Block>>(json);
            if (blocks == null || blocks.Count == 0) return chain;

            // ✅ Rebuild HeadBlock + CurrentBlock + NextBlock links từ Blocks[]
            foreach (var block in blocks.OrderBy(b => b.BlockNumber))
                chain.AcceptBlock(block);

            chain.RebuildChainLinks();
            return chain;
        }
    }
}