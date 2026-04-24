const BASE = "http://localhost:5160/api/blockchain";

export const api = {
  getBlocks: () => fetch(BASE).then((r) => r.json()),
  getPending: () => fetch(`${BASE}/pending`).then((r) => r.json()),
  verify: () => fetch(`${BASE}/verify`).then((r) => r.json()),
  save: () => fetch(`${BASE}/save`, { method: "POST" }).then((r) => r.json()),
  addTransaction: (data) =>
    fetch(`${BASE}/transaction`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }).then((r) => r.json()),
};
