using BlockchainDiemAPI.Interfaces;
using System;
using System.Collections.Generic;
namespace BlockchainDiemAPI {
    public class BlockChain : IBlockChain
    {
        public Block CurrentBlock { get; private set; }
        public Block HeadBlock { get; private set; }

        public List<Block> Blocks { get; }

        public BlockChain()
        {
            Blocks = new List<Block>();
        }

        public void AcceptBlock(Block block)
        {
            // This is the first block, so make it the genesis block.

            if (HeadBlock == null)
            {
                HeadBlock = block;
                HeadBlock.PreviousBlockHash = null;
            }

            CurrentBlock = block;
            Blocks.Add(block);
        }
public void RebuildChainLinks()
{
    if (Blocks == null || Blocks.Count == 0) return;
    
    var sorted = Blocks.OrderBy(b => b.BlockNumber).ToList();
    
    for (int i = 0; i < sorted.Count - 1; i++)
    {
        sorted[i].NextBlock = sorted[i + 1];
    }
    
    HeadBlock = sorted.First();
    CurrentBlock = sorted.Last();
}
        public int NextBlockNumber
        {
           
                get
    {
                    return Blocks.Count;
                }
            
        }

        public void VerifyChain()
        {
            if (HeadBlock == null)
            {
                throw new InvalidOperationException("Genesis block not set.");
            }

            bool isValid = HeadBlock.IsValidChain(null, true);

            if (isValid)
            {
                Console.WriteLine("Blockchain integrity intact.");
            }
            else
            {
                Console.WriteLine("Blockchain integrity NOT intact.");
            }
        }
    }
}