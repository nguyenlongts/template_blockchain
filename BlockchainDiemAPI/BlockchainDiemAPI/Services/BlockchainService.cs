using BlockchainDiemAPI.Cryptography;

namespace BlockchainDiemAPI.Services
{
    public class BlockchainService
    {
        private readonly BlockChain _blockchain;
        private readonly List<Transaction> _pending = new();
        private readonly DigitalSignature _ds;
        private const int TX_PER_BLOCK = 4;

        public BlockchainService()
        {
            _blockchain = FileManager.LoadBlockchain();
            _ds = new DigitalSignature();
            _ds.LoadOrCreateKey(); 
        }

        public AddResult AddTransaction(Transaction txn)
        {
            txn.Sign(_ds);
            _pending.Add(txn);

            if (_pending.Count >= TX_PER_BLOCK)
            {
                var block = new Block(_blockchain.NextBlockNumber);
                foreach (var t in _pending) block.AddTransaction(t);
                block.SetBlockHash(_blockchain.CurrentBlock);
                _blockchain.AcceptBlock(block);
                _pending.Clear();
                FileManager.SaveBlockchain(_blockchain);
                return new AddResult(true, 0);
            }

            return new AddResult(false, _pending.Count);
        }

        public List<Block> GetBlocks() => _blockchain.Blocks;

        public List<Transaction> GetPending() => _pending;

        public bool Verify() => Verify(out _);

        public bool Verify(out List<string> errors)
        {
            errors = new List<string>();
            if (_blockchain.HeadBlock == null) return true;
            return _blockchain.HeadBlock.IsValidChain(null, true, errors);
        }

        public void Save() => FileManager.SaveBlockchain(_blockchain);

        public record AddResult(bool BlockCreated, int PendingCount);
    }
}