# Known Issues — Monica Salon

> Status: Implemented
> Terakhir diperbarui: 2026-11-19
> Index fitur: [docs/README.md](./README.md)
> Aturan kerja: [AGENTS.md](../AGENTS.md)

File ini adalah **satu-satunya** daftar fitur yang belum lengkap, masih memakai mock, placeholder, atau gagal QA.

Aturan:

- Satu baris = satu masalah. Jangan menggabungkan beberapa masalah berbeda.
- Setiap fitur yang belum sempurna wajib punya entri di sini, bukan hanya di chat atau komentar kode.
- Sertakan dampak user, workaround, langkah berikutnya, dan link dokumen fitur (kalau sudah ada).
- Update kolom `Terakhir diperbarui` setiap kali status berubah.
- Kalau selesai, pindahkan baris ke tabel **Selesai** dengan tanggal dan referensi commit/dokumen. Jangan hapus riwayat.
- Jangan membuat file known issues terpisah per fitur.

Status: `Belum Dikerjakan`, `Sebagian`, `Perlu Perbaikan`, `Diblokir`, `Selesai`.

Severity: `P0 Blocker`, `P1 High`, `P2 Medium`, `P3 Low`.

---

## Issue aktif

| ID | Fitur | Status | Severity | Deskripsi | Dampak | Workaround | Langkah berikutnya | Docs | Terakhir diperbarui |
|---|---|---|---|---|---|---|---|---|---|
| KI-001 | Feedback / ulasan | Sebagian | P2 | Backend belum punya endpoint feedback; aplikasi memakai `MockFeedbackApi` sehingga ulasan hanya tersimpan di memori. | Ulasan customer tidak persisten, hilang saat app ditutup, dan tidak terlihat oleh admin. | Tidak ada; input ulang setelah app dibuka. | Tambah endpoint feedback di backend, buat `HttpFeedbackApi`, daftarkan di `MauiProgram`, lalu migrasikan QA. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-002 | Admin Reports & Settings | Belum Dikerjakan | P3 | Menu **Reports** dan **Settings** di bottom navigation admin masih placeholder. | Admin belum bisa melihat laporan atau mengubah pengaturan. | Tidak ada. | Definisikan kebutuhan, buat `technical-design.md` + `qa-test.md`, lalu implementasi. | — | 2026-11-19 |
| KI-003 | Struk transaksi | Sebagian | P3 | Aksi cetak/bagikan struk di `TransactionDetailsPage` masih stub. | Struk tidak bisa dicetak atau dibagikan dari perangkat. | Tunjukkan struk di layar. | Integrasikan print/share platform atau sembunyikan aksi sampai siap. | — | 2026-11-19 |
| KI-004 | Pembayaran non-tunai | Sebagian | P3 | Metode non-tunai hanya mencatat nomor referensi, tanpa gateway pembayaran. | Pembayaran tidak benar-benar diproses; status sukses hanya simulasi UI. | Gunakan mode Tunai untuk demo pembayaran penuh. | Integrasikan payment gateway dan dokumentasikan kontraknya di technical design kasir. | — | 2026-11-19 |
| KI-005 | QA & otomatisasi | Belum Dikerjakan | P2 | Belum ada project automated test; verifikasi masih manual. | Regresi hanya tertangkap lewat QA manual. | Ikuti `qa-test.md` per fitur. | Tambah test project (unit untuk VM/pricing, integration untuk `ApiClient`), lalu update `AGENTS.md`, `README.md`, dan template QA. | [docs/templates/qa-test.md](./templates/qa-test.md) | 2026-11-19 |
| KI-007 | Booking admin & kasir | Sebagian | P2 | `AdminBookingStore` dan `CashierSession` menyimpan data di memori. | Booking/transaksi baru hilang saat app restart. | Selesaikan alur dalam satu sesi dan catat bukti QA. | Pindahkan state ke API atau storage persisten; update desain state fitur terkait. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-008 | Admin Dashboard & Admin Booking | Sebagian | P1 | `HttpAdminBookingApi` sudah tersedia, tetapi `AdminDashboardViewModel`, `AdminBookingsViewModel`, `AdminCreateBookingViewModel`, dan `AdminBookingDetailsViewModel` masih membaca/menulis `AdminBookingStore`/`AdminMockData`. Backend juga belum punya entity stylist, payment status, atau status `Rejected`/`NoShow`. | Admin melihat dan mengubah data mock; perubahan tidak masuk database dan tidak sinkron dengan booking customer. | Tidak ada; selesaikan alur dalam satu sesi. | Petakan `AdminBooking` ke DTO/endpoint, sambungkan dashboard/list/create/detail/reschedule, dan putuskan kontrak stylist/payment status dengan backend. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-009 | Kasir & transaksi | Sebagian | P1 | `HttpTransactionApi` sudah tersedia, tetapi `CashierSession`, `CashierMockData`, `PaymentViewModel`, dan halaman transaksi masih memakai data lokal. Metode pembayaran UI (5) tidak sama dengan API (`Cash`, `Transfer`, `Qris`), dan backend belum punya endpoint refund/void. | Transaksi tidak masuk database, stok tidak berkurang, finance tidak tercatat, dan refund hanya mengubah status lokal. | Tidak ada; transaksi hilang saat app ditutup. | Sambungkan katalog kasir ke `GET /services` + `GET /inventory`, checkout ke `POST /transactions`, riwayat ke `GET /transactions`, dan tambahkan endpoint refund. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-010 | Add-on booking | Sebagian | P2 | Backend belum punya endpoint add-on/booking item tambahan; section add-on di `SchedulePage` disembunyikan (`IsVisible="False"`). | Customer tidak bisa memilih add-on seperti sampo/hair oil. | Tidak ada. | Tambah entity + endpoint add-on di backend, lalu tampilkan kembali section dan kirim add-on ke `CreateBookingRequest`. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-011 | Field katalog API | Sebagian | P2 | `GET /services` hanya mengembalikan `id/name/category/price/isActive`; `GET /hairstyles` hanya `id/name/description/isActive`. Durasi, rating, gambar, deskripsi layanan, kategori gaya, panjang, face shape, maintenance, harga, tags, dan gambar masih diisi dari katalog lokal (`CatalogMapper`). | Tampilan bisa berbeda dengan data yang diubah admin; layanan baru tanpa metadata lokal memakai nilai default. | Fallback `CatalogMapper`/katalog lokal. | Lengkapi schema dan endpoint katalog, lalu hapus fallback metadata. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-012 | Profil customer | Sebagian | P2 | `UserDto` dari `/auth/me` tidak memuat nomor HP; belum ada endpoint profil customer. Setelah login, `UserSession.Phone` kosong. | Halaman konfirmasi booking menampilkan nomor HP kosong; customer belum bisa melihat/mengubah profil. | Nomor HP tidak dibutuhkan API untuk booking customer. | Tambah endpoint `GET/PUT` profil customer dan isi `UserSession` dari response tersebut. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-013 | Auto-login | Belum Dikerjakan | P3 | Token disimpan di `TokenStore`, tetapi `UserSession` tidak direstore saat app dibuka ulang. | User harus login lagi meskipun token masih valid. | Login ulang. | Saat startup, baca token lalu panggil `/auth/me`; kalau valid, restore `UserSession` dan arahkan ke Home/Dashboard. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-014 | Role guard admin | Sebagian | P2 | Guard role hanya ada di tautan "Masuk sebagai Admin" (`LoginPage.xaml.cs`). Route/page admin belum memeriksa role sendiri. | Customer yang tahu route admin masih bisa membuka halaman admin lewat navigasi kode/deep link. | Jangan bagikan route admin; logout setelah demo. | Tambah guard di `AppShell`/page admin atau middleware navigasi berbasis `UserSession.Role`. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-015 | Inventory & Finance UI | Belum Dikerjakan | P2 | `HttpInventoryApi` dan `HttpFinanceApi` sudah tersedia, tetapi belum ada halaman Inventory, Finance, atau laporan. | Fitur backend belum bisa dipakai dari aplikasi. | Gunakan Swagger/Bruno untuk sementara. | Buat halaman Inventory dan Finance/Reports, lalu dokumentasikan di `docs/features/`. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-016 | Logout customer | Belum Dikerjakan | P3 | Belum ada menu profil/logout di UI customer. Logout baru tersedia di layout admin. | Customer tidak bisa berganti akun tanpa menghapus data aplikasi. | Hapus data aplikasi / install ulang. | Tambah menu profil + tombol logout yang memanggil `IAuthApi.LogoutAsync` dan `UserSession.SignOut`. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-017 | Android debug deployment | Sebagian | P2 | APK Debug hasil `dotnet build` memakai Fast Deployment; jika di-install manual, app crash `No assemblies found ... Fast Deployment`. | Developer bisa mengira aplikasi rusak padahal APK-nya tidak lengkap. | Gunakan `dotnet build -t:Run -f net10.0-android` atau `dotnet publish -f net10.0-android -c Release` lalu install APK Release. | Dokumentasikan alur run yang benar dan/atau tambahkan konfigurasi build yang aman untuk instalasi manual. | [api-integration](./features/api-integration/qa-test.md) | 2026-11-19 |
| KI-018 | Catalog cache | Sebagian | P3 | `CatalogCache` menyimpan layanan/hairstyle selama sesi dan tidak punya invalidasi otomatis setelah admin mengubah katalog. | Perubahan katalog tidak terlihat sampai app restart atau `Invalidate()` manual. | Restart aplikasi. | Tambahkan refresh saat halaman katalog dibuka ulang atau setelah aksi admin. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-019 | Lupa password | Belum Dikerjakan | P3 | Aksi "Lupa password?" hanya menampilkan instruksi menghubungi resepsionis; belum ada endpoint reset password. | Customer tidak bisa reset password mandiri. | Hubungi admin salon. | Tambah alur reset password (endpoint + email/OTP) atau dokumentasikan sebagai fitur di luar scope. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |
| KI-020 | Data fallback katalog | Sebagian | P3 | Katalog lokal masih dipakai saat API gagal. Harga/kategori lokal tidak sama dengan seed API (mis. Smoothing 250k vs 350k). | Saat offline/cold start, harga yang tampil bisa berbeda dari data backend. | Muat ulang saat API pulih. | Samakan data fallback dengan seed API atau hapus fallback setelah backend stabil. | [api-integration](./features/api-integration/technical-design.md) | 2026-11-19 |

---

## Selesai

| ID | Fitur | Deskripsi | Diselesaikan | Referensi |
|---|---|---|---|---|
| KI-006 | Dokumentasi | `README.md` root sudah disinkronkan dengan arsitektur API, cara login, konfigurasi `.env`, status mock/API, dan cara run Android. | 2026-11-19 | [README.md](../README.md), [api-integration](./features/api-integration/technical-design.md) |

---

## Cara menambah issue

1. Ambil ID berikutnya (`KI-XXX`).
2. Isi semua kolom; `Docs` diisi link `docs/features/<slug>/technical-design.md` kalau sudah ada, atau `—`.
3. Kalau issue berasal dari QA yang `Fail`, tautkan test case-nya di kolom `Docs` atau catatan issue.
4. Setelah diperbaiki dan diverifikasi, pindahkan baris ke tabel **Selesai**, isi tanggal dan link commit/dokumen, lalu update tanggal file ini.
