// FileManager.cs
using BlockchainDiemAPI;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

public static class FileManager
{
    // Đổi tên file theo từng bài
    private static readonly string FILE_PATH = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "data", "blockchain.json"
    );

    public static void SaveBlockchain(BlockChain blockchain)
    {
        if (blockchain == null) return;
        Directory.CreateDirectory(Path.GetDirectoryName(FILE_PATH)!);
        string json = JsonConvert.SerializeObject(blockchain, Formatting.Indented);
        File.WriteAllText(FILE_PATH, json);
    }

public static BlockChain LoadBlockchain()
{
    if (!File.Exists(FILE_PATH)) return new BlockChain();
    string json = File.ReadAllText(FILE_PATH);
    if (string.IsNullOrWhiteSpace(json)) return new BlockChain();

    var blockchain = JsonConvert.DeserializeObject<BlockChain>(json) ?? new BlockChain();

    blockchain.RebuildChainLinks();

    return blockchain;
}
}