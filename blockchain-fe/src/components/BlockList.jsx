import { useEffect, useState } from "react";
import { api } from "../api/api";

export default function BlockList({ refresh }) {
  const [blocks, setBlocks] = useState(null);
  const [error, setError] = useState(null);
  const [expanded, setExpanded] = useState({});
  const [showHash, setShowHash] = useState({});

  useEffect(() => {
    load();
  }, [refresh]);

  const load = async () => {
    setError(null);
    try {
      setBlocks(await api.getBlocks());
    } catch {
      setError("Không thể tải dữ liệu. Kiểm tra kết nối server.");
      setBlocks([]);
    }
  };

  const toggleBlock = (bn) => setExpanded((e) => ({ ...e, [bn]: !e[bn] }));
  const toggleHash = (key) => setShowHash((h) => ({ ...h, [key]: !h[key] }));

  if (blocks === null) return <div className="empty">Đang tải...</div>;
  if (error) return <div className="alert alert-error">{error}</div>;
  if (blocks.length === 0)
    return <div className="empty">Chưa có block nào</div>;

  return (
    <div>
      {blocks.map((b) => (
        <div
          key={b.blockNumber}
          className="card"
          style={{ marginBottom: "1rem" }}
        >
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              marginBottom: "0.5rem",
            }}
          >
            <div
              style={{ display: "flex", alignItems: "center", gap: "0.75rem" }}
            >
              <span className="block-num">Block #{b.blockNumber}</span>
              <span style={{ fontSize: 12, color: "#6b7280" }}></span>
            </div>
            <button
              className="btn"
              onClick={() => toggleBlock(b.blockNumber)}
              style={{ fontSize: 12 }}
            >
              {expanded[b.blockNumber] ? "Thu gọn ▲" : "Chi tiết ▼"}
            </button>
          </div>

          <div
            style={{
              fontSize: 11,
              color: "#9ca3af",
              marginBottom: "0.5rem",
              fontFamily: "monospace",
              wordBreak: "break-all",
            }}
          >
            <span style={{ color: "#6b7280" }}>Hash: </span>
            {b.blockHash}
            {/* {b.previousBlockHash && (
              <span style={{ marginLeft: 8 }}>
                <span style={{ color: "#6b7280" }}>Prev: </span>
                {b.previousBlockHash}
              </span>
            )} */}
          </div>

          <table className="tx-table">
            <thead>
              <tr>
                <th>STT</th>
                <th>Mã SV</th>
                <th>Họ tên</th>
                <th>Ngành học</th>
                <th>Điểm TK</th>
                <th>Loại TN</th>
                <th>Ngày TN</th>
              </tr>
            </thead>
            <tbody>
              {b.transactions?.map((t) => (
                <tr key={t.index}>
                  <td>{t.index + 1}</td>
                  <td>{t.maSinhVien}</td>
                  <td>{t.hoTen}</td>
                  <td>{t.nganhHoc}</td>
                  <td>
                    <span className="diem-badge">{t.diemTongKet}</span>
                  </td>
                  <td>{t.loaiTotNghiep}</td>
                  <td style={{ fontSize: 12, color: "#6b7280" }}>
                    {t.ngayTotNghiep}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {expanded[b.blockNumber] && (
            <div
              style={{
                marginTop: "1rem",
                borderTop: "1px solid #e5e7eb",
                paddingTop: "1rem",
              }}
            >
              <div
                style={{
                  fontSize: 13,
                  fontWeight: 500,
                  marginBottom: "0.5rem",
                  color: "#374151",
                }}
              >
                Chi tiết Merkle Root & Chữ ký số
              </div>
              <div
                style={{
                  fontSize: 11,
                  color: "#6b7280",
                  marginBottom: "0.75rem",
                  fontFamily: "monospace",
                  wordBreak: "break-all",
                }}
              >
                <b>Merkle Root:</b> {b.merkleRoot}
              </div>

              {b.transactions?.map((t) => {
                const key = `${b.blockNumber}-${t.index}`;
                return (
                  <div
                    key={t.index}
                    style={{
                      background: "#f9fafb",
                      borderRadius: 8,
                      padding: "10px 12px",
                      marginBottom: 8,
                      border: "1px solid #e5e7eb",
                    }}
                  >
                    <div
                      style={{
                        display: "flex",
                        justifyContent: "space-between",
                        alignItems: "center",
                      }}
                    >
                      <span style={{ fontSize: 13, fontWeight: 500 }}>
                        TX #{t.index} — {t.maSinhVien} · {t.hoTen}
                      </span>
                      <button
                        className="btn"
                        style={{ fontSize: 11 }}
                        onClick={() => toggleHash(key)}
                      >
                        {showHash[key] ? "Ẩn ▲" : "Xem Hash & Chữ ký ▼"}
                      </button>
                    </div>
                    {showHash[key] && (
                      <div
                        style={{
                          marginTop: 8,
                          fontSize: 11,
                          fontFamily: "monospace",
                          wordBreak: "break-all",
                          lineHeight: 1.8,
                        }}
                      >
                        <div>
                          <span style={{ color: "#6b7280" }}>Hash: </span>
                          <span style={{ color: "#1d4ed8" }}>{t.hash}</span>
                        </div>
                        <div style={{ marginTop: 4 }}>
                          <span style={{ color: "#6b7280" }}>Chữ ký số: </span>
                          <span style={{ color: "#7c3aed" }}>
                            {t.signature || "Chưa có chữ ký"}
                          </span>
                        </div>
                      </div>
                    )}
                  </div>
                );
              })}
            </div>
          )}
        </div>
      ))}
    </div>
  );
}
