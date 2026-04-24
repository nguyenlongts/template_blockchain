namespace BlockchainDiemAPI.Interfaces
{
    public interface ITransaction
    {
        string MaSinhVien { get; set; }
        string MaMonHoc { get; set; }
        double Diem { get; set; }
        int LanThi { get; set; }
        DateTime NgayLuu { get; set; }

        string CalculateTransactionHash();
    }
}
