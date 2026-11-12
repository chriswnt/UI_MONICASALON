# Monica Salon — Aplikasi Mobile UI (Phase 1)

Aplikasi **UI-only** untuk Monica Salon, dibangun dengan **.NET MAUI 10** dan **UraniumUI 3.0** sebagai framework presentasi UI. Proyek ini adalah tugas UAS mata kuliah Pemrograman Visual.

> **Scope:** Phase 1 mencakup 3 halaman Customer UI. Phase 2 (Admin) menambahkan **Admin Dashboard** dan **Admin Booking** sebagai perluasan aplikasi. Tidak ada koneksi database/backend; semua data masih *mock data*.

---

## 📱 Halaman yang Tersedia

| Halaman | File | Isi |
|---------|------|-----|
| Login | `Views/LoginPage.xaml` | Onboarding + form login (email & password) dengan validasi |
| Register | `Views/RegisterPage.xaml` | Form pendaftaran customer (nama, email, telepon, password, konfirmasi, checkbox S&K) |
| Home Customer | `Views/HomeCustomerPage.xaml` | Dashboard customer: header, search, banner promo, kategori layanan, daftar layanan, bottom navigation, bottom sheet detail layanan |
| Admin Dashboard | `Views/Admin/AdminDashboardPage.xaml` | Ringkasan operasional: statistik, booking mingguan, pendapatan, booking terbaru |
| Admin Booking | `Views/Admin/AdminBookingsPage.xaml` | Manajemen booking: pencarian, filter tanggal/status, kartu booking, bottom sheet detail & jadwal ulang |
| Admin Booking Baru | `Views/Admin/AdminCreateBookingPage.xaml` | Form booking baru: pelanggan, layanan, stylist, tanggal/jam, catatan, cek ketersediaan slot |
| Admin Detail Booking | `Views/Admin/AdminBookingDetailsPage.xaml` | Detail booking satu halaman penuh + aksi konfirmasi / tolak / selesai / batal / jadwal ulang / bayar di kasir |
| Admin Jadwal Ulang | `Views/Admin/AdminReschedulePage.xaml` | Pindahkan jadwal: kalender, pilih jam, cek bentrok slot stylist |
| Kasir | `Views/Admin/CashierPage.xaml` | Katalog layanan & produk, pencarian, filter kategori, keranjang (bottom sheet) |
| Review Transaksi | `Views/Admin/TransactionReviewPage.xaml` | Data pelanggan & stylist, ubah jumlah item, kode promo, ringkasan total |
| Pembayaran | `Views/Admin/PaymentPage.xaml` | Metode bayar (Tunai/EDC/QRIS/E-Wallet), nominal cepat, kembalian, status "memproses" |
| Pembayaran Berhasil | `Views/Admin/TransactionSuccessPage.xaml` | Status sukses + ringkasan struk + aksi lanjutan |
| Riwayat Transaksi | `Views/Admin/TransactionHistoryPage.xaml` | Statistik hari ini, filter cari/periode/metode, tombol kembali ke Kasir, daftar transaksi |
| Detail Transaksi | `Views/Admin/TransactionDetailsPage.xaml` | Struk lengkap, cetak/bagikan (stub), pengembalian dana |

Alur navigasi customer: `LoginPage` → (Login / Daftar) → `RegisterPage` / `HomeCustomerPage`.

Alur navigasi Admin: `LoginPage` → tautan **Masuk sebagai Admin** → bottom navigation 5 tab:
**Dashboard ↔ Booking ↔ Kasir ↔ Reports ↔ Settings** (Reports & Settings masih placeholder).
Tab `Kasir` menggantikan tab placeholder `Customers` yang sebelumnya belum punya halaman.

Alur modul Booking: `AdminBookingsPage` → tombol **+** → `AdminCreateBookingPage` (kembali ke daftar),
atau kartu booking → bottom sheet detail → **Buka Halaman Detail** → `AdminBookingDetailsPage` →
**Jadwal Ulang** → `AdminReschedulePage`.

Alur modul Kasir: `CashierPage` → keranjang / **Review** → `TransactionReviewPage` → `PaymentPage` →
`TransactionSuccessPage` → **Lihat Struk** → `TransactionDetailsPage`, atau **Riwayat Transaksi** →
`TransactionHistoryPage` → kartu transaksi → `TransactionDetailsPage`.

---

## ✅ Prasyarat (Requirements)

Pastikan sudah terinstal di komputer:

1. **Visual Studio 2022/2026** dengan workload **.NET Multi-platform App UI development (.NET MAUI)**
   — atau **VS Code** + ekstensi **.NET MAUI**.
2. **.NET SDK 10.0** (proyek ini memakai `net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst`, dan `net10.0-windows10.0.19041.0`).
3. **Workload MAUI** terpasang. Cek dengan:
   ```bash
   dotnet workload list
   ```
   Kalau belum ada `maui` / `maui-windows`, install:
   ```bash
   dotnet workload install maui
   ```

> Cek versi SDK: `dotnet --version` harus **10.x**.

---

## 🚀 Cara Menjalankan (Clone → Run)

### 1. Clone repository

```bash
git clone https://github.com/chriswnt/UI_MONICASALON.git
cd UI_MONICASALON
```

### 2. Restore dependency (NuGet)

```bash
dotnet restore
```

### 3. Jalankan aplikasi

**Windows** (paling cepat untuk demo):
```bash
dotnet build -t:Run -f net10.0-windows10.0.19041.0
```

**Android** (butuh emulator atau HP dengan USB debugging aktif):
```bash
dotnet build -t:Run -f net10.0-android
```

**Lewat Visual Studio:**
1. Buka `UAS_APP_MONICASALON.slnx`.
2. Di toolbar atas, pilih target **Windows Machine** (atau emulator Android).
3. Tekan **F5**.

> ⚠️ Proyek iOS/MacCatalyst **tidak bisa** di-build di Windows tanpa Mac — ini normal, abaikan saja target tersebut.

---

## 🧱 Struktur Proyek

```
UAS_APP_MONICASALON/
├── App.xaml                    # Registrasi resource + tema UraniumUI
├── App.xaml.cs
├── AppShell.xaml               # Shell navigation (entry point: LoginPage)
├── MauiProgram.cs              # Registrasi UraniumUI, Material theme, font
├── AssemblyInfo.cs             # XmlnsDefinition untuk shim ButtonView
├── prd_monica_salon_phase_1.md # Dokumen spesifikasi (PRD) Phase 1
│
├── Controls/
│   └── ButtonView.cs           # Shim: <uranium:ButtonView> → UraniumUI Material
│
├── Models/                     # Model data (dummy)
│   ├── BannerItem.cs
│   ├── CategoryItem.cs
│   └── ServiceItem.cs
│
├── ViewModels/                 # MVVM (CommunityToolkit.Mvvm)
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   └── HomeCustomerViewModel.cs
│
├── Views/
│   ├── LoginPage.xaml(.cs)
│   ├── RegisterPage.xaml(.cs)
│   └── HomeCustomerPage.xaml(.cs)
│
├── Icons/
│   └── MaterialRegular.cs      # Kumpulan nama ikon Material Symbols
│
├── Platforms/                  # Konfigurasi per-platform (Android/iOS/Windows/Mac)
└── Resources/                  # Font, gambar, icon, splash, style warna
```

---

## 🎨 Tentang UraniumUI

Seluruh layout dan komponen UI memakai **UraniumUI** (bukan kontrol MAUI polos), sesuai aturan PRD:

| Kebutuhan | Komponen yang Dipakai |
|-----------|----------------------|
| Root halaman | `<uranium:UraniumContentPage>` |
| Input teks / email / telepon | `<material:TextField>` + `material:TextField.Icon` |
| Toggle show/hide password | `material:TextFieldPasswordShowHideAttachment` |
| Checkbox | `<material:CheckBox>` |
| Tombol interaktif (back, notif, filter, kategori, kartu, bottom nav) | `<uranium:ButtonView>` |
| Bottom sheet detail layanan | `<material:BottomSheetView>` |
| CTA utama | `<Button StyleClass="FilledButton">` (style dari UraniumUI.Material) |
| Validasi form | `<input:FormView>` + `validation:RequiredValidation` / `RegexValidation` / `MinLengthValidation` / `DigitsOnlyValidation` |
| Tema global | `<material:StyleResource ColorsOverride="..." />` di `App.xaml` |

### Catatan: kenapa ada `Controls/ButtonView.cs`?

Di **UraniumUI 3.0**, kontrol `ButtonView` pindah dari namespace `UraniumUI` ke `UraniumUI.Material.Controls`. Sementara PRD menulis sintaks `<uranium:ButtonView>`.

Agar XAML tetap sesuai PRD **tanpa** mengubah satu baris pun, dibuat *shim*:

```csharp
public class ButtonView : UraniumUI.Material.Controls.ButtonView { }
```

Shim ini didaftarkan lewat `AssemblyInfo.cs` (`XmlnsDefinition`) dan handler-nya didaftarkan di `MauiProgram.cs`. Karena mewarisi (`inherit`) kontrol Uranium asli, seluruh perilaku dan tampilan tetap milik UraniumUI.

### Admin UI (Phase 2)

- Layout **mobile-first**: header admin, konten bertumpuk, dan bottom navigation Dashboard / Booking.
- Komponen bersama di `Controls/Admin/`: `AdminLayout`, `AdminNavigationItem`, `StatCard`, `SectionCard`, `StatusBadge`.
- Style bersama di `Resources/Styles/AdminStyles.xaml`; warna tetap memakai token dari `Colors.xaml`.
- Data mock di `Models/Admin/AdminMockData.cs`; ViewModel di `ViewModels/Admin/`.
- Alur login/register customer tidak diubah; hanya ditambahkan tautan admin di footer login.

### Modul Kasir & Booking (Phase 3)

**Kasir (UI-only, tanpa backend):**

- Katalog layanan & produk dari `Models/Admin/CashierMockData.cs`; keranjang dan riwayat transaksi disimpan di
  `Models/Admin/CashierSession.cs` (singleton), karena Shell membuat instance halaman baru setiap navigasi.
- Harga: subtotal → diskon (kode promo demo `MONICA10`, `HEMAT20`, `NEWCUST15`) → PPN 11% → total
  (aturan di `Models/Admin/CashierPricing.cs`). Semua nominal diformat lewat `Models/Admin/Rupiah.cs`.
- Metode pembayaran: Tunai, Kartu Debit, Kartu Kredit, QRIS, E-Wallet (`Models/Admin/PaymentMethod.cs`).
  Tunai memakai nominal cepat dan menghitung kembalian; non-tunai hanya mencatat nomor referensi (tanpa gateway).

**Booking:**

- Booking baru / jadwal ulang memakai `Models/Admin/AdminBookingStore.cs`, store in-memory bersama
  `AdminMockData`, sehingga booking yang baru dibuat langsung terlihat di daftar Booking dan Dashboard.
- Pengecekan bentrok slot memakai aturan yang sama dengan bottom sheet yang sudah ada
  (slot terkunci + status Dikonfirmasi + stylist sama + jadwal bertumpuk).
- Detail booking tetap tersedia sebagai bottom sheet (pintasan) **dan** halaman penuh
  `AdminBookingDetailsPage`; tidak ada kode lama yang dihapus.

**State UI:**

- *Loading*: `RefreshView` di katalog kasir & riwayat transaksi, `ActivityIndicator` saat memproses pembayaran.
- *Empty*: katalog tanpa hasil, keranjang kosong, riwayat tanpa hasil, booking/transaksi tidak ditemukan.
- *Success*: halaman Pembayaran Berhasil, banner sukses (kode promo, booking dibuat, status booking berubah).
- *Error*: banner galat untuk uang diterima kurang, kode promo salah, field wajib kosong, slot bentrok,
  waktu sudah lewat, dan nomor transaksi/booking tidak ditemukan.

**Aksesibilitas:**

- `SemanticProperties.Description` + `SemanticProperties.Hint` pada semua kontrol interaktif
  (tombol, tombol ikon `ButtonView`, chip kategori, kartu katalog, dropdown, field teks).
- `SemanticProperties.HeadingLevel` (Level 1 untuk judul halaman, Level 2 untuk judul kartu/seksi) di
  `AdminStyles.xaml`, jadi urutan heading konsisten di seluruh layar admin.
- Setiap kartu daftar (booking & transaksi) punya satu kalimat ringkas untuk pembaca layar, dan label
  di dalamnya ditandai `AutomationProperties.IsInAccessibleTree="False"` supaya satu item = satu swipe.
- Ikon dekoratif dikeluarkan dari pohon aksesibilitas; `SemanticScreenReader.Default.Announce` dipakai untuk
  mengumumkan perubahan penting (item masuk keranjang, hasil pembayaran, galat validasi, perubahan status booking).
- Bottom sheet yang tertutup ditandai tidak terlihat oleh pembaca layar, sehingga konten tersembunyi tidak
  ikut terbaca.

**Catatan teknis: `uranium:CalendarView` dan navigasi kembali**

UraniumUI `CalendarView` tidak boleh dipakai di halaman yang akan ditutup dengan `GoToAsync("..")`:
menutup halaman yang berisi `CalendarView` (walaupun kalendernya tidak pernah dibuka) membuat proses
aplikasi mati mendadak di Windows. Karena itu semua halaman yang bisa di-back memakai `DatePicker` biasa
(atau dropdown periode), dan tidak ada lagi `CalendarView` di proyek ini. Tombol aksi yang penting
(tombol kembali, tombol **+** Booking Baru, shortcut Riwayat) juga memakai `Button` asli, bukan
`ButtonView`/gesture, supaya bisa diaktifkan pembaca layar.

### Palet Warna

| Warna | Hex | Pemakaian |
|-------|-----|-----------|
| Coral (primer) | `#D9534F` | Tombol utama, ikon aktif, aksen |
| Background | `#FAF7F5` | Latar semua halaman |
| Teks utama | `#1E1E1E` | Judul, label |
| Border / divider | `#EFE6E4` | Garis kartu dan pemisah |

---

## 🛠️ Build & Troubleshooting

**Bersihkan hasil build (kalau ada error aneh):**
```bash
dotnet clean
rm -rf bin obj
dotnet restore
```

**Error `NU1101` / package tidak ketemu** → cek koneksi internet, lalu `dotnet restore` ulang.

**Error XAML `ButtonView tidak dikenal`** → pastikan `AssemblyInfo.cs` masih ada dan `MauiProgram.cs` masih memanggil `UseUraniumUI()` + `UseUraniumUIMaterial()`.

**Error Android SDK / emulator** → buka **Android Device Manager** di Visual Studio, buat/boot emulator dulu sebelum `dotnet build -t:Run -f net10.0-android`.

---

## 📦 Dependency

| Package | Versi | Fungsi |
|---------|-------|--------|
| `UraniumUI` | 3.0.0 | Core framework UI |
| `UraniumUI.Material` | 3.0.0 | Tema & komponen Material (TextField, ButtonView, BottomSheet, dll) |
| `UraniumUI.Icons.MaterialSymbols` | 3.0.0 | Ikon Material Symbols |
| `InputKit.Maui` | 4.6.0 | FormView & validasi form |
| `CommunityToolkit.Mvvm` | 8.4.2 | MVVM: `ObservableObject`, `RelayCommand` |

---

## 📄 Lisensi & Catatan Akademik

Proyek ini dibuat untuk keperluan **tugas UAS mata kuliah Pemrograman Visual**. Desain dan spesifikasi mengikuti dokumen `prd_monica_salon_phase_1.md` di dalam repository ini.

Referensi UraniumUI: <https://enisn-projects.io/docs/en/uranium/latest>