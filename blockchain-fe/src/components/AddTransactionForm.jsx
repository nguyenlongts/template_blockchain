import { useState } from "react";
import { api } from "../api/api";
import Alert from "./Alert";
export default function AddTransactionForm({ onAdded }) {
  const [form, setForm] = useState({
    MaSinhVien: "",
    MaMonHoc: "",
    Diem: "",
    LanThi: "1",
  });

  const [msg, setMsg] = useState(null);

  const handleChange = (k, v) => {
    setForm((f) => ({ ...f, [k]: v }));
  };

  const handleSubmit = async () => {
    if (!form.MaSinhVien || !form.MaMonHoc || form.Diem === "") {
      setMsg({ text: "Vui lòng nhập đầy đủ", type: "error" });
      return;
    }

    try {
      const data = await api.addTransaction({
        ...form,
        Diem: parseFloat(form.Diem),
        LanThi: parseInt(form.LanThi),
      });

      setMsg({ text: data.message, type: "success" });

      setForm({
        MaSinhVien: "",
        MaMonHoc: "",
        Diem: "",
        LanThi: "1",
      });

      onAdded();
    } catch (e) {
      setMsg({ text: e.message, type: "error" });
    }
  };

  return (
    <div className="card">
      <h2>Thêm giao dịch</h2>

      <div className="form-grid">
        <div>
          <label>Mã sinh viên</label>
          <input
            className="input"
            value={form.MaSinhVien}
            onChange={(e) => handleChange("MaSinhVien", e.target.value)}
          />
        </div>

        <div>
          <label>Mã môn học</label>
          <input
            className="input"
            value={form.MaMonHoc}
            onChange={(e) => handleChange("MaMonHoc", e.target.value)}
          />
        </div>

        <div>
          <label>Điểm</label>
          <input
            className="input"
            type="number"
            value={form.Diem}
            onChange={(e) => handleChange("Diem", e.target.value)}
          />
        </div>

        <div>
          <label>Lần thi</label>
          <input
            className="input"
            type="number"
            value={form.LanThi}
            onChange={(e) => handleChange("LanThi", e.target.value)}
          />
        </div>
      </div>

      <button className="btn-primary" onClick={handleSubmit}>
        Thêm
      </button>

      {msg && <Alert msg={msg.text} type={msg.type} />}
    </div>
  );
}
