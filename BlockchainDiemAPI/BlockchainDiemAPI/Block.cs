using BlockchainDiemAPI;
using BlockchainDiemAPI.Cryptography;
using BlockchainDiemAPI.Interfaces;
using BlockchainDiemAPI.Merkle;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
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
    public string MerkleRoot { get; set; }

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
        MerkleRoot = merkleTree.RootNode.ToString();
    }

    public bool IsValidChain(string prevBlockHash, bool verbose, List<string> errors = null)
    {
        bool isValid = true;

        // Verify Merkle root independently before checking block hash
        var verifyTree = new MerkleTree();
        foreach (var txn in Transaction)
            verifyTree.AppendLeaf(MerkleHash.Create(txn.StoredHash));
        verifyTree.BuildTree();

        if (verifyTree.RootNode.ToString() != MerkleRoot)
        {
            isValid = false;
            errors?.Add($"Block #{BlockNumber}: Merkle root không khớp (transaction bị thay đổi hoặc sao chép)");
        }

        BuildMerkleTree();
        string newBlockHash = CalculateBlockHash(prevBlockHash);

        if (newBlockHash != BlockHash)
        {
            isValid = false;
            errors?.Add($"Block #{BlockNumber}: hash không khớp (dữ liệu bị thay đổi)");
        }
        else if (PreviousBlockHash != prevBlockHash)
        {
            isValid = false;
            errors?.Add($"Block #{BlockNumber}: previousHash không khớp (chuỗi bị phá vỡ)");
        }

        if (NextBlock != null)
            return NextBlock.IsValidChain(newBlockHash, verbose, errors) && isValid;

        return isValid;
    }

    public List<int> GetTamperedTransactions()
    {
        var tampered = new List<int>();

        // Check 1: each transaction's current data must match its own stored hash
        for (int i = 0; i < Transaction.Count; i++)
        {
            if (Transaction[i].CalculateTransactionHash() != Transaction[i].StoredHash)
                tampered.Add(i);
        }

        // Check 2: rebuild Merkle tree from StoredHash values and compare to the
        // block's committed MerkleRoot. This catches copy-paste attacks where an
        // attacker duplicates an entire transaction (including its StoredHash),
        // which fools the per-field check above but changes the tree structure.
        var verifyTree = new MerkleTree();
        foreach (var txn in Transaction)
            verifyTree.AppendLeaf(MerkleHash.Create(txn.StoredHash));
        verifyTree.BuildTree();

        if (verifyTree.RootNode.ToString() != MerkleRoot)
        {
            // Identify duplicated StoredHash values (signature of a copy-paste attack)
            var seen = new Dictionary<string, int>();
            for (int i = 0; i < Transaction.Count; i++)
            {
                var h = Transaction[i].StoredHash;
                if (seen.ContainsKey(h))
                {
                    if (!tampered.Contains(seen[h])) tampered.Add(seen[h]);
                    if (!tampered.Contains(i))        tampered.Add(i);
                }
                else
                {
                    seen[h] = i;
                }
            }

            // No duplicates but root still mismatches (e.g. reordering) — flag all
            if (tampered.Count == 0)
                for (int i = 0; i < Transaction.Count; i++)
                    tampered.Add(i);
        }

        return tampered.Distinct().OrderBy(x => x).ToList();
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