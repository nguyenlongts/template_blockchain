using BlockchainDiemAPI;
using System;
using System.Collections.Generic;

namespace BlockchainDiemAPI.Interfaces
{
    public interface IBlock
    {
        // List of transactions
        List<Transaction> Transaction { get; }

        // Block header data
        int BlockNumber { get; }
        DateTime CreatedDate { get; set; }
        string BlockHash { get; }
        string PreviousBlockHash { get; set; }

        void AddTransaction(Transaction transaction);
        string CalculateBlockHash(string previousBlockHash);
        void SetBlockHash(IBlock parent);
        IBlock NextBlock { get; set; }
        bool IsValidChain(string prevBlockHash, bool verbose);
    }
}
