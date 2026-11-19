# Dokumentasi Monica Salon

> Status: Implemented
> Terakhir diperbarui: 2026-11-19
> Known issues: [known-issues.md](./known-issues.md)
> Aturan kerja: [AGENTS.md](../AGENTS.md)

Dokumentasi fitur disimpan per kemampuan user, bukan per halaman. Setiap fitur punya dua dokumen wajib:

- [`technical-design.md`](./templates/technical-design.md) — template desain teknis, kontrak API, dan **desain state**.
- [`qa-test.md`](./templates/qa-test.md) — template skenario pengujian manual, status, dan bukti.

Semua keterbatasan yang diketahui dicatat di satu tempat: [`known-issues.md`](./known-issues.md).

---

## Struktur

```
docs/
├── README.md                     # file ini
├── known-issues.md               # daftar fitur belum sempurna
├── templates/
│   ├── technical-design.md
│   └── qa-test.md
└── features/
    └── <slug>/
        ├── technical-design.md
        └── qa-test.md
```

Template disalin ke folder fitur baru, lalu diisi. Jangan mengubah template menjadi dokumen fitur.

---

## Index fitur

| Slug | Cakupan | Technical design | QA test | Status dokumen | QA terakhir |
|---|---|---|---|---|---|
| `api-integration` | Konfigurasi `.env`, `ApiClient`, `TokenStore`, auth, katalog, jadwal, booking customer, fallback katalog, status `Pending`. Admin/kasir/feedback masih mock. | [technical-design.md](./features/api-integration/technical-design.md) | [qa-test.md](./features/api-integration/qa-test.md) | In Progress | In Progress (2026-11-19) |

> `api-integration` mendokumentasikan lapisan API lintas fitur; backlog di bawah tetap berlaku untuk desain per alur. Index ini diperbarui setiap kali dokumen fitur dibuat atau statusnya berubah.

---

## Backlog dokumentasi

Fitur yang sudah ada di kode tetapi belum punya folder dokumentasi. Saat menyentuh salah satunya, buat dokumennya sekalian memakai slug berikut:

| Slug yang disarankan | Cakupan | Status |
|---|---|---|
| `auth-login` | Login, register, session, "Ingat saya", redirect role | Tercakup sebagian di `api-integration`; dokumen khusus belum ada |
| `home-customer` | Home customer, banner promo, pencarian, bottom navigation | Tercakup sebagian di `api-integration`; dokumen khusus belum ada |
| `catalog-browse` | Katalog layanan + detail layanan | Tercakup sebagian di `api-integration`; dokumen khusus belum ada |
| `booking-wizard` | Pilih jadwal → konfirmasi → booking sukses | Tercakup sebagian di `api-integration`; dokumen khusus belum ada |
| `booking-history` | Riwayat booking + detail booking | Tercakup sebagian di `api-integration`; dokumen khusus belum ada |
| `feedback` | Ulasan customer (masih mock) | Belum ada; lihat KI-001 |
| `style-book` | Buku gaya + detail hairstyle | Tercakup sebagian di `api-integration`; dokumen khusus belum ada |
| `admin-dashboard` | Ringkasan operasional admin | Belum ada; UI masih mock (KI-008) |
| `admin-bookings` | Daftar, tambah, detail, dan jadwal ulang booking | Belum ada; UI masih mock (KI-008) |
| `cashier-checkout` | Kasir → review → pembayaran → transaksi sukses | Belum ada; UI masih mock (KI-009) |
| `transaction-history` | Riwayat transaksi + detail/struk | Belum ada; UI masih mock (KI-009) |

Slug boleh berubah selama belum ada dokumen; setelah dokumen dibuat, slug harus stabil.

---

## Status dokumen

| Status | Arti |
|---|---|
| `Draft` | Skeleton/desain awal, implementasi belum lengkap |
| `In Progress` | Sedang dikerjakan |
| `Implemented` | Sesuai kode dan QA sudah dijalankan |
| `Needs Work` | Ada bagian yang belum beres atau QA gagal |
| `Deprecated` | Fitur dihapus/diganti; jelaskan penggantinya |

## Status QA

| Status | Arti |
|---|---|
| `Not Run` | Belum dijalankan |
| `In Progress` | Sebagian test case sudah dijalankan |
| `Pass` | Semua test case relevan lulus |
| `Fail` | Ada test case gagal; wajib punya known issue |
| `Blocked` | Tidak bisa diuji; tulis alasannya |
| `Skipped` | Sengaja dilewati; tulis alasannya |

---

## Cara menambah dokumentasi fitur

1. Tentukan slug (lowercase kebab-case, bahasa Inggris, berbasis kemampuan user).
2. Buat folder `docs/features/<slug>/`.
3. Salin `docs/templates/technical-design.md` → `technical-design.md`, isi minimal scope + desain state.
4. Salin `docs/templates/qa-test.md` → `qa-test.md`, isi scope + test matrix.
5. Implementasi fitur, lalu sinkronkan desain dan isi hasil QA.
6. Update baris index di atas dan `docs/known-issues.md`.
7. Update `README.md` root kalau flow, fitur, atau dependency berubah.

Detail aturan dan Definition of Done ada di [AGENTS.md](../AGENTS.md).
