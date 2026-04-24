import { useState } from "react";
import { api } from "../api/api";

export default function VerifyPanel() {
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);

  const verify = async () => {
    setLoading(true);
    setResult(null);
    try {
      setResult(await api.verify());
    } catch (e) {
      setResult({ isValid: false, message: "Lỗi kết nối server.", blocks: [] });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card">
      <span className="card-title">Xác minh chuỗi</span>
      <p style={{ color: "#6b7280", fontSize: 13, margin: "0.5rem 0 1rem" }}>
        Kiểm tra tính toàn vẹn của toàn bộ blockchain.
      </p>
      <button className="btn-primary" onClick={verify} disabled={loading}>
        {loading ? "Đang xác minh..." : "Xác minh ngay"}
      </button>

      {result && (
        <div style={{ marginTop: "1rem" }}>

          {/* Banner tổng */}
          <div style={{
            display: "flex",
            alignItems: "center",
            gap: 10,
            padding: "10px 14px",
            borderRadius: 8,
            marginBottom: "1rem",
            background: result.isValid ? "#f0fdf4" : "#fef2f2",
            border: `1px solid ${result.isValid ? "#bbf7d0" : "#fecaca"}`,
          }}>
            <span style={{ fontSize: 20 }}>{result.isValid ? "✅" : "⚠️"}</span>
            <div>
              <div style={{
                fontWeight: 600,
                fontSize: 14,
                color: result.isValid ? "#15803d" : "#b91c1c",
              }}>
                {result.message}
              </div>
              {/* Chi tiết lỗi hash chain nếu có */}
              {result.details?.length > 0 && (
                <ul style={{ margin: "4px 0 0", padding: "0 0 0 16px", fontSize: 12, color: "#b91c1c" }}>
                  {result.details.map((d, i) => <li key={i}>{d}</li>)}
                </ul>
              )}
            </div>
          </div>

          {/* Chi tiết từng block */}
          {result.blocks?.map((block) => (
            <div key={block.blockNumber} style={{
              border: `1px solid ${block.isValid ? "#e5e7eb" : "#fecaca"}`,
              borderRadius: 8,
              marginBottom: 10,
              overflow: "hidden",
            }}>
              {/* Header block */}
              <div style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                padding: "8px 12px",
                background: block.isValid ? "#f9fafb" : "#fff5f5",
                borderBottom: "1px solid #e5e7eb",
              }}>
                <span style={{ fontWeight: 600, fontSize: 13 }}>
                  Block #{block.blockNumber}
                </span>
                <span style={{
                  fontSize: 12,
                  fontWeight: 500,
                  color: block.isValid ? "#15803d" : "#b91c1c",
                }}>
                  {block.status}
                </span>
              </div>

              {/* Bảng transactions */}
              <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12 }}>
                <thead>
                  <tr style={{ background: "#f3f4f6" }}>
                    <th style={thStyle}>#</th>
                    <th style={thStyle}>Mã SV</th>
                    <th style={thStyle}>Môn học</th>
                    <th style={thStyle}>Điểm</th>
                    <th style={thStyle}>Trạng thái</th>
                  </tr>
                </thead>
                <tbody>
                  {block.transactions?.map((tx) => (
                    <tr key={tx.index} style={{
                      background: tx.isValid ? "transparent" : "#fff5f5",
                    }}>
                      <td style={tdStyle}>{tx.index}</td>
                      <td style={tdStyle}>{tx.maSinhVien}</td>
                      <td style={tdStyle}>{tx.maMonHoc}</td>
                      <td style={tdStyle}>{tx.diem}</td>
                      <td style={{ ...tdStyle, fontWeight: 500, color: tx.isValid ? "#15803d" : "#b91c1c" }}>
                        {tx.status}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

const thStyle = {
  padding: "6px 10px",
  textAlign: "left",
  fontWeight: 600,
  color: "#6b7280",
  borderBottom: "1px solid #e5e7eb",
};

const tdStyle = {
  padding: "6px 10px",
  borderBottom: "1px solid #f3f4f6",
  color: "#111",
};