import { useEffect, useState } from "react";
import { api } from "../api/api";

export default function BlockList({ refresh }) {
  const [blocks, setBlocks] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => { load(); }, [refresh]);

  const load = async () => {
    setError(null);
    try {
      setBlocks(await api.getBlocks());
    } catch (e) {
      setError("Không thể tải dữ liệu. Kiểm tra kết nối server.");
      setBlocks([]);
    }
  };

  if (blocks === null) return <div className="empty">Đang tải...</div>;
  if (error) return <div className="alert alert-error">{error}</div>;
  if (blocks.length === 0) return <div className="empty">Chưa có block nào</div>;

  return (
    <div>
      {blocks.map((b) => (
        <div key={b.blockNumber} className="card">
          <div className="block-header">
            <span className="block-num">Block #{b.blockNumber}</span>
            <span className="block-hash" title={b.blockHash}>{b.blockHash}</span>
          </div>

          <table className="tx-table">
            <thead>
              <tr>
                <th>Mã SV</th>
                <th>Môn học</th>
                <th>Điểm</th>
                <th>Lần thi</th>
                <th>Ngày lưu</th>
              </tr>
            </thead>
            <tbody>
              {b.transactions?.map((t, i) => (
                <tr key={i}>
                  <td>{t.maSinhVien}</td>
                  <td>{t.maMonHoc}</td>
                  <td><span className="diem-badge">{t.diem}</span></td>
                  <td>{t.lanThi}</td>
                  <td>{new Date(t.ngayLuu).toLocaleString("vi-VN")}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ))}
    </div>
  );
}
