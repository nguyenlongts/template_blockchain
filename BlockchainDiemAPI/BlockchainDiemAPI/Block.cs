using BlockchainDiemAPI;
using BlockchainDiemAPI.Cryptography;
using BlockchainDiemAPI.Interfaces;
using BlockchainDiemAPI.Merkle;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text;



public class Block : IBlock
{
    public List<Transaction> Transaction { get;  set; }

    // Set as part of the block creation process.
    public int BlockNumber { get;  set; }
    public DateTime CreatedDate { get; set; }
    public string BlockHash { get;  set; }
    public string PreviousBlockHash { get; set; }
    public IBlock NextBlock { get; set; }
    private MerkleTree merkleTree = new MerkleTree();

    public Block(int blockNumber)
    {
        BlockNumber = blockNumber;

        CreatedDate = DateTime.UtcNow;
        Transaction = new List<Transaction>();
    }

    public void AddTransaction(Transaction transaction)
    {
        Transaction.Add(transaction);
    }

    public string CalculateBlockHash(string previousBlockHash)
    {
        string blockheader = BlockNumber + CreatedDate.ToString() + previousBlockHash;
        string combined = merkleTree.RootNode + blockheader;

        return Convert.ToBase64String(HashData.ComputeHashSha256(Encoding.UTF8.GetBytes(combined)));
    }

    // Set the block hash
    public void SetBlockHash(IBlock parent)
    {
        if (parent != null)
        {
            PreviousBlockHash = parent.BlockHash;
            parent.NextBlock = this;
        }
        else
        {
            // Previous block is the genesis block.
            PreviousBlockHash = null;
        }

        BuildMerkleTree();

        BlockHash = CalculateBlockHash(PreviousBlockHash);
    }

    private void BuildMerkleTree()
    {
        if (Transaction == null || Transaction.Count == 0)
            throw new Exception("Block must contain at least one transaction.");
        merkleTree = new MerkleTree();

        foreach (ITransaction txn in Transaction)
        {
            merkleTree.AppendLeaf(MerkleHash.Create(txn.CalculateTransactionHash()));
        }

        merkleTree.BuildTree();
    }

    public bool IsValidChain(string prevBlockHash, bool verbose)
{
    bool isValid = true;
    BuildMerkleTree();

    string newBlockHash = CalculateBlockHash(prevBlockHash);

    if (newBlockHash != BlockHash)
    {
        isValid = false;
    }
    else
    {
        isValid = (PreviousBlockHash == prevBlockHash);
    }

    PrintVerificationMessage(verbose, isValid);

    if (NextBlock != null)
        return NextBlock.IsValidChain(newBlockHash, verbose);

    return isValid; 
}

    private void PrintVerificationMessage(bool verbose, bool isValid)
    {
        if (verbose)
        {
            if (!isValid)
            {
                Console.WriteLine("Block Number " + BlockNumber + " : FAILED VERIFICATION");
            }
            else
            {
                Console.WriteLine("Block Number " + BlockNumber + " : PASS VERIFICATION");
            }
        }
    }
}