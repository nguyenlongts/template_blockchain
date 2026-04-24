using Microsoft.AspNetCore.Mvc;
using BlockchainDiemAPI.Services;

namespace BlockchainDiemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlockchainController : ControllerBase
    {
        private readonly BlockchainService _svc;
        public BlockchainController(BlockchainService svc) => _svc = svc;

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _svc.GetBlocks().Select(b => new {
                b.BlockNumber,
                b.BlockHash,
                b.PreviousBlockHash,
                b.CreatedDate,
                Transactions = b.Transaction.Select(t => new {
                    t.MaSinhVien,
                    t.MaMonHoc,
                    t.Diem,
                    t.LanThi,
                    t.NgayLuu,
                    Hash = t.CalculateTransactionHash()
                })
            });
            return Ok(result);
        }

        [HttpPost("transaction")]
        public IActionResult Add([FromBody] AddTxRequest req)
        {
            var txn = new Transaction(req.MaSinhVien, req.MaMonHoc, req.Diem, req.LanThi, DateTime.UtcNow);
            var r = _svc.AddTransaction(txn);
            return Ok(new
            {
                r.BlockCreated,
                r.PendingCount,
                message = r.BlockCreated
                    ? "Block mới được tạo!"
                    : $"Đã thêm ({r.PendingCount}/{3})"
            });
        }

        [HttpGet("pending")]
        public IActionResult GetPending() => Ok(_svc.GetPending());


        [HttpGet("verify")]
        public IActionResult Verify()
        {
            _svc.Verify(out var chainErrors);
            var blocks = _svc.GetBlocks();

            var blockResults = blocks.Select(block => {
                var tamperedIdx = block.GetTamperedTransactions();
                bool blockValid = tamperedIdx.Count == 0;

                return new
                {
                    blockNumber = block.BlockNumber,
                    isValid = blockValid,
                    status = blockValid ? "✅ Hợp lệ" : "⚠️ Có transaction bị thay đổi",
                    transactions = block.Transaction.Select((t, i) => new {
                        index = i,
                        maSinhVien = t.MaSinhVien,
                        maMonHoc = t.MaMonHoc,
                        diem = t.Diem,
                        isValid = !tamperedIdx.Contains(i),
                        status = tamperedIdx.Contains(i) ? "⚠️ Đã bị thay đổi" : "✅ Hợp lệ"
                    })
                };
            }).ToList();

            // ✅ chainValid phải tính cả 2 nguồn
            bool chainValid = chainErrors.Count == 0 && blockResults.All(b => b.isValid);

            return Ok(new
            {
                isValid = chainValid,
                message = chainValid ? "Blockchain hợp lệ" : "Phát hiện dữ liệu bị thay đổi!",
                details = chainErrors,
                blocks = blockResults
            });
        }
        [HttpPost("save")]
        public IActionResult Save() { _svc.Save(); return Ok(new { message = "Đã lưu" }); }
    }

    public record AddTxRequest(string MaSinhVien, string MaMonHoc, double Diem, int LanThi);
}