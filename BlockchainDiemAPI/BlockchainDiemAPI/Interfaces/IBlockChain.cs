namespace BlockchainDiemAPI.Interfaces
{
    public interface IBlockChain
    {
        void AcceptBlock(Block block);
        int NextBlockNumber { get; }
        void VerifyChain();
    }
}
