import { useState } from "react";
import AddTransactionForm from "./components/AddTransactionForm";
import BlockList from "./components/BlockList";
import PendingList from "./components/PendingList";
import VerifyPanel from "./components/VerifyPanel";
import "./App.css";

const TABS = [
  { id: "blocks", label: "Blocks" },
  { id: "pending", label: "Chờ lưu" },
  { id: "verify", label: "Xác minh" },
];

export default function App() {
  const [tab, setTab] = useState("blocks");
  const [refreshKey, setRefreshKey] = useState(0);

  return (
    <div className="app">
      <div className="app-header">
        <span className="app-title">
          Blockchain quản lý thông tin tốt nghiệp của sinh viên
        </span>
      </div>

      <AddTransactionForm onAdded={() => setRefreshKey((k) => k + 1)} />

      <div className="tabs">
        {TABS.map((t) => (
          <button
            key={t.id}
            className={`tab ${tab === t.id ? "active" : ""}`}
            onClick={() => setTab(t.id)}
          >
            {t.label}
          </button>
        ))}
      </div>

      {tab === "blocks" && <BlockList refresh={refreshKey} />}
      {tab === "pending" && <PendingList />}
      {tab === "verify" && <VerifyPanel />}
    </div>
  );
}
