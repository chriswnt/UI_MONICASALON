# Monica Salon — Aplikasi Mobile

Aplikasi Monica Salon dengan **.NET MAUI 10** dan **UraniumUI 3.0**. Proyek ini adalah tugas UAS mata kuliah Pemrograman Visual.

> **Scope:** Phase 1 mencakup **Customer UI** (login, register, home, katalog, booking, riwayat, ulasan, buku gaya). Phase 2 (Admin) menambahkan **Admin Dashboard**, **Admin Booking**, dan **Kasir** sebagai perluasan aplikasi.
>
> **Status integrasi API (2026-11-19):** auth, katalog, jadwal, dan booking customer sudah memakai backend production `https://monica-salon-be.onrender.com` lewat `Services/Api/`. **Admin Dashboard, Admin Booking, Kasir, dan feedback masih memakai mock layer** (`AdminBookingStore`, `CashierSession`, `MockFeedbackApi`). Booking customer baru berstatus **Menunggu Konfirmasi (`Pending`)** sampai admin mengonfirmasi. Detail integrasi ada di [`docs/features/api-integration/`](docs/features/api-integration/) dan keterbatasan di [`docs/known-issues.md`](docs/known-issues.md).

---

## 📱 Halaman yang Tersedia

| Halaman | File | Isi |
|---------|------|-----|
| Login | `Views/LoginPage.xaml` | Hero coral dengan ilustrasi kontur, transisi gelombang ke panel putih, form email/password, "Ingat saya", lupa password, tautan daftar, dan tautan **Masuk sebagai Admin** |
| Register | `Views/RegisterPage.xaml` | Form pendaftaran customer (nama, email, telepon, password, konfirmasi, checkbox S&K) |
| Home Customer | `Views/HomeCustomerPage.xaml` | Dashboard customer: sapaan, pencarian, banner promo, kategori, daftar layanan, bottom navigation, bottom sheet detail layanan |
| Katalog Layanan | `Views/CataloguePage.xaml` | Daftar semua layanan dengan pencarian dan filter kategori |
| Detail Layanan | `Views/ServiceDetailPage.xaml` | Foto, kategori, harga, durasi, deskripsi, dan tombol booking |
| Jadwal | `Views/SchedulePage.xaml` | Pilih layanan tambahan, tanggal, slot waktu bergaya bioskop, hairstyle, dan ringkasan total |
| Konfirmasi Booking | `Views/ConfirmBookingPage.xaml` | Ringkasan layanan/jadwal, hairstyle, kontak, catatan, dan rincian harga |
| Booking Berhasil | `Views/BookingSuccessPage.xaml` | Kode booking, status awal **Menunggu Konfirmasi**, jadwal, layanan, dan total |
| Riwayat Booking | `Views/BookingHistoryPage.xaml` | Daftar booking dengan filter status (Semua/Aktif/Selesai/Dibatalkan) dan shortcut beri ulasan |
| Detail Booking | `Views/BookingDetailsPage.xaml` | Status, jadwal, layanan, catatan, total, dan tombol beri ulasan |
| Ulasan | `Views/BookingFeedbackPage.xaml` | Rating bintang, tag ulasan, komentar, dan kirim ulasan |
| Buku Gaya | `Views/HairstyleBookPage.xaml` | Katalog hairstyle dari admin dengan filter kategori |
| Detail Hairstyle | `Views/HairstyleDetailPage.xaml` | Deskripsi gaya, rekomendasi layanan, dan tombol booking |
| Admin Dashboard | `Views/Admin/AdminDashboardPage.xaml` | Ringkasan operasional: statistik, booking mingguan, pendapatan, booking terbaru |
| Admin Booking | `Views/Admin/AdminBookingsPage.xaml` | Manajemen booking: pencarian, filter tanggal/status, kartu booking, bottom sheet detail & jadwal ulang |
| Admin Booking Baru | `Views/Admin/AdminCreateBookingPage.xaml` | Form booking baru: pelanggan, layanan, stylist, tanggal/jam, catatan, cek ketersediaan slot |
| Admin Detail Booking | `Views/Admin/AdminBookingDetailsPage.xaml` | Detail booking satu halaman penuh + aksi konfirmasi / tolak / selesai / batal / jadwal ulang / bayar di kasir |
| Admin Jadwal Ulang | `Views/Admin/AdminReschedulePage.xaml` | Pindahkan jadwal: pemilihan tanggal, pilih jam, cek bentrok slot stylist |
| Kasir | `Views/Admin/CashierPage.xaml` | Katalog layanan & produk, pencarian, filter kategori, keranjang (bottom sheet) |
| Review Transaksi | `Views/Admin/TransactionReviewPage.xaml` | Data pelanggan & stylist, ubah jumlah item, kode promo, ringkasan total |
| Pembayaran | `Views/Admin/PaymentPage.xaml` | Metode bayar (Tunai/EDC/QRIS/E-Wallet), nominal cepat, kembalian, status "memproses" |
| Pembayaran Berhasil | `Views/Admin/TransactionSuccessPage.xaml` | Status sukses + ringkasan struk + aksi lanjutan |
| Riwayat Transaksi | `Views/Admin/TransactionHistoryPage.xaml` | Statistik hari ini, filter cari/periode/metode, tombol kembali ke Kasir, daftar transaksi |
| Detail Transaksi | `Views/Admin/TransactionDetailsPage.xaml` | Struk lengkap, cetak/bagikan (stub), pengembalian dana |

> **Status data (2026-11-19):** halaman auth, katalog, buku gaya, jadwal, dan booking customer memakai API production. Halaman **Admin Dashboard**, **Admin Booking**, **Kasir**, dan **Riwayat/Detail Transaksi** masih memakai data mock.

Alur navigasi customer:

`LoginPage` → (Login / Daftar) → `RegisterPage` / `HomeCustomerPage` → `CataloguePage` / `HairstyleBookPage` → `ServiceDetailPage` / `HairstyleDetailPage` → `SchedulePage` → `ConfirmBookingPage` → `BookingSuccessPage` → `BookingHistoryPage` → `BookingDetailsPage` → `BookingFeedbackPage`.

Bottom navigation (`Controls/BottomNavBar.cs`) menghubungkan **Home**, **Layanan**, **Booking**, **Riwayat**, dan **Gaya**.

Alur navigasi Admin:

`LoginPage` → **Masuk sebagai Admin** → bottom navigation **Dashboard ↔ Booking ↔ Kasir ↔ Reports ↔ Settings** (Reports & Settings masih placeholder).

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

> 💡 Login memakai API production. Gunakan akun seed `owner / Admin123!` atau `staff / Staff123!` untuk admin, atau daftar akun customer baru lewat halaman **Register**. "Ingat saya" hanya menyimpan identifier lewat `Preferences`; password tidak pernah disimpan.

---

## 🌐 Integrasi API

Aplikasi terhubung ke backend Monica Salon melalui `Services/Api/`. Sumber kebenaran kontrak ada di project `D:\monicasalon\monica-salon-api`.

### Konfigurasi `.env`

- Template ada di [`.env.example`](.env.example); salin menjadi `.env` untuk development lokal.
- `API_BASE_URL` dipakai Windows/iOS/Mac, `API_BASE_URL_ANDROID` dipakai Android.
- Keduanya di-embed ke assembly saat build (`EmbeddedResource` di `UAS_APP_MONICASALON.csproj`), sehingga tidak perlu file runtime di perangkat.
- Environment variable dengan nama yang sama menimpa nilai `.env`.
- Production saat ini: `https://monica-salon-be.onrender.com`; timeout 60 detik karena Render free tier bisa cold start.

```dotenv
API_BASE_URL=https://monica-salon-be.onrender.com
API_BASE_URL_ANDROID=https://monica-salon-be.onrender.com
API_PREFIX=/api/v1
API_TIMEOUT_SECONDS=60
API_ENVIRONMENT=Production
```

### Yang sudah memakai API

| Area | Endpoint utama | Status |
|---|---|---|
| Login/register/logout | `POST /auth/login`, `/auth/register`, `/auth/logout` | Tersambung |
| Katalog layanan & gaya | `GET /services`, `GET /hairstyles` | Tersambung (cache sesi + fallback lokal) |
| Jadwal & slot | `GET /bookings/availability?date=` | Tersambung |
| Booking customer | `POST /bookings`, `GET /bookings/mine`, `GET /bookings/{id}`, `POST /bookings/{id}/cancel` | Tersambung |
| Admin booking, kasir, inventory, finance | `HttpAdminBookingApi`, `HttpTransactionApi`, `HttpInventoryApi`, `HttpFinanceApi` | Service HTTP tersedia; UI masih mock |
| Feedback & add-on | — | Belum ada endpoint backend |

### Akun demo backend

| Role | Login | Password |
|---|---|---|
| Owner | `owner` | `Admin123!` |
| Staff | `staff` | `Staff123!` |

Customer harus **register** terlebih dahulu. Ganti password seed sebelum demo publik.

### Menjalankan di Android via USB

```bash
adb devices
dotnet build -t:Run -f net10.0-android
```

Kalau instalasi diblokir HP (`INSTALL_FAILED_USER_RESTRICTED`) atau ingin install manual:

```bash
dotnet publish -f net10.0-android -c Release
adb install -r "UAS_APP_MONICASALON/bin/Release/net10.0-android/publish/com.companyname.uas_app_monicasalon-Signed.apk"
adb shell monkey -p com.companyname.uas_app_monicasalon -c android.intent.category.LAUNCHER 1
```

> Jangan install APK Debug hasil `dotnet build` secara manual: APK itu memakai **Fast Deployment** dan akan crash `No assemblies found ... Fast Deployment`. Lihat [KI-017](docs/known-issues.md).

---

## 🧱 Struktur Proyek

Di root repository juga tersedia `.env.example` (template konfigurasi API) dan folder `docs/` (dokumentasi fitur + known issues).

```
UAS_APP_MONICASALON/
├── App.xaml                    # Registrasi resource + tema UraniumUI
├── App.xaml.cs                 # Window + DesktopWindowSizer (Windows)
├── AppShell.xaml               # Shell navigation, route, dan penyembunyian back button
├── MauiProgram.cs              # Registrasi UraniumUI, Material theme, font
├── AssemblyInfo.cs             # XmlnsDefinition untuk shim ButtonView
├── prd_monica_salon_phase_1.md # Dokumen spesifikasi (PRD) Phase 1
│
├── Controls/
│   ├── BottomNavBar.cs         # Bottom navigation customer (5 tab)
│   ├── ButtonView.cs           # Shim: <uranium:ButtonView> → UraniumUI Material
│   └── Admin/
│       ├── AdminLayout.xaml    # Header + konten + bottom navigation admin
│       ├── AdminNavigationItem.xaml
│       ├── StatCard.xaml
│       ├── SectionCard.xaml
│       ├── StatusBadge.xaml
│       └── PaymentBadge.xaml
│
├── Models/                     # Model data (dummy / DTO)
│   ├── BannerItem.cs
│   ├── CategoryItem.cs
│   ├── ServiceItem.cs
│   ├── ServiceSelectionItem.cs
│   ├── BookingRecord.cs
│   ├── BookingRequest.cs
│   ├── BookingStatus.cs
│   ├── BookingTimeSlot.cs
│   ├── DateOption.cs
│   ├── DayAvailability.cs
│   ├── HairstyleItem.cs
│   ├── SalonAddOn.cs
│   ├── AddOnSelectionItem.cs
│   ├── FeedbackRecord.cs
│   ├── FeedbackRequest.cs
│   ├── FilterChip.cs
│   ├── RatingStarItem.cs
│   └── Admin/                  # Model & mock store modul admin/kasir
│       ├── AdminBooking.cs
│       ├── AdminBookingStore.cs
│       ├── AdminMockData.cs
│       ├── CashierSession.cs
│       ├── CashierMockData.cs
│       ├── CashierTransaction.cs
│       ├── CashierPricing.cs
│       ├── PaymentMethod.cs
│       └── Rupiah.cs
│
├── Services/                   # API client, session, katalog, formatter
│   ├── Api/                    # ApiConfig, ApiClient, TokenStore, DTO, Http*Api
│   ├── AppServices.cs          # Service locator (DI) untuk halaman/VM
│   ├── IBookingApi.cs
│   ├── IFeedbackApi.cs
│   ├── IScheduleApi.cs
│   ├── MockSalonDatabase.cs    # State in-memory: booking, feedback, slot terisi
│   ├── MockBookingApi.cs
│   ├── MockFeedbackApi.cs
│   ├── MockScheduleApi.cs
│   ├── ServiceCatalog.cs
│   ├── HairstyleCatalog.cs
│   ├── AddOnCatalog.cs
│   ├── BookingSession.cs       # Draft booking antar-halaman
│   ├── UserSession.cs
│   └── CurrencyFormatter.cs
│
├── ViewModels/                 # MVVM (CommunityToolkit.Mvvm)
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── HomeCustomerViewModel.cs
│   ├── CatalogueViewModel.cs
│   ├── ServiceDetailViewModel.cs
│   ├── ScheduleViewModel.cs
│   ├── ConfirmBookingViewModel.cs
│   ├── BookingSuccessViewModel.cs
│   ├── BookingHistoryViewModel.cs
│   ├── BookingDetailsViewModel.cs
│   ├── BookingFeedbackViewModel.cs
│   ├── HairstyleBookViewModel.cs
│   ├── HairstyleDetailViewModel.cs
│   └── Admin/                  # ViewModel modul admin/kasir
│       ├── AdminDashboardViewModel.cs
│       ├── AdminBookingsViewModel.cs
│       ├── AdminCreateBookingViewModel.cs
│       ├── AdminBookingDetailsViewModel.cs
│       ├── AdminRescheduleViewModel.cs
│       ├── CashierViewModel.cs
│       ├── TransactionReviewViewModel.cs
│       ├── PaymentViewModel.cs
│       ├── TransactionSuccessViewModel.cs
│       ├── TransactionHistoryViewModel.cs
│       └── TransactionDetailsViewModel.cs
│
├── Views/
│   ├── LoginPage.xaml(.cs)
│   ├── RegisterPage.xaml(.cs)
│   ├── HomeCustomerPage.xaml(.cs)
│   ├── CataloguePage.xaml(.cs)
│   ├── ServiceDetailPage.xaml(.cs)
│   ├── SchedulePage.xaml(.cs)
│   ├── ConfirmBookingPage.xaml(.cs)
│   ├── BookingSuccessPage.xaml(.cs)
│   ├── BookingHistoryPage.xaml(.cs)
│   ├── BookingDetailsPage.xaml(.cs)
│   ├── BookingFeedbackPage.xaml(.cs)
│   ├── HairstyleBookPage.xaml(.cs)
│   ├── HairstyleDetailPage.xaml(.cs)
│   └── Admin/                  # Halaman admin, booking, dan kasir
│       ├── AdminDashboardPage.xaml(.cs)
│       ├── AdminBookingsPage.xaml(.cs)
│       ├── AdminCreateBookingPage.xaml(.cs)
│       ├── AdminBookingDetailsPage.xaml(.cs)
│       ├── AdminReschedulePage.xaml(.cs)
│       ├── CashierPage.xaml(.cs)
│       ├── TransactionReviewPage.xaml(.cs)
│       ├── PaymentPage.xaml(.cs)
│       ├── TransactionSuccessPage.xaml(.cs)
│       ├── TransactionHistoryPage.xaml(.cs)
│       └── TransactionDetailsPage.xaml(.cs)
│
├── Icons/
│   ├── MaterialRegular.cs      # Kumpulan nama ikon Material Symbols
│   └── AdminIcons.cs           # Ikon khusus layar admin
│
├── Platforms/                  # Konfigurasi per-platform (Android/iOS/Windows/Mac)
└── Resources/                  # Font, gambar, icon, splash, style warna
    ├── Styles/
    │   ├── Colors.xaml         # Token warna customer + admin
    │   ├── Styles.xaml         # Style dasar MAUI/customer
    │   └── AdminStyles.xaml    # Style layar admin
    └── Images/
        ├── login_hero.svg      # Ilustrasi kontur coral untuk hero login
        ├── banner_aplikasi.png # Banner login alternatif
        ├── style_*.png         # Ilustrasi hairstyle
        └── service_*.jpg       # Foto layanan
```

---

## 🎨 Catatan UI

### Login

- Hero coral memakai ilustrasi kontur `login_hero.svg` dan transisi gelombang putih (`Path`) ke panel form.
- Form dibatasi maksimal **440 px** dan tetap terpusat pada layar lebar; pada layar sempit konten dapat di-scroll.
- Login/register memanggil `IAuthApi`; error API (401/409/koneksi) ditampilkan sebagai banner di atas tombol **Masuk**. `UserSession.Role` menentukan Home atau Admin Dashboard.
- "Ingat saya" menyimpan identifier lewat `Preferences`; password tidak pernah disimpan.
- Warna mengikuti token coral/blush yang sudah ada (`#D9534F`, `#8B2626`, `#FDECEC`, `#FAF7F5`).

### Booking

- Slot waktu mengikuti pola **cinema booking**: admin/API menyediakan slot tersedia, slot terisi tidak bisa dipilih. Booking customer baru berstatus **Menunggu Konfirmasi (`Pending`)** sampai admin mengonfirmasi.
- Halaman jadwal menggabungkan pemilihan tanggal, slot, hairstyle, dan total yang diperbarui langsung. Section add-on disembunyikan sampai backend punya endpoint add-on.
- Setelah submit, booking dikirim ke `POST /bookings` lewat `HttpBookingApi`; status awal `Pending`.

### Chip & filter

Chip yang bisa dipilih (filter riwayat, tag ulasan, kategori buku gaya) memakai `Border` + `TapGestureRecognizer` dengan warna yang dihitung di model, bukan `uranium:ButtonView`. Pola ini membuat state terpilih benar-benar terlihat dan tetap konsisten di Windows/Android.

### Validasi form

- **Login:** validasi di ViewModel (banner error).
- **Register:** `<input:FormView>` + `validation:RequiredValidation`, `RegexValidation`, `MinLengthValidation`, dan `DigitsOnlyValidation`.

---

## 🧩 Tentang UraniumUI

Seluruh layout dan komponen UI memakai **UraniumUI** (bukan kontrol MAUI polos), sesuai aturan PRD:

| Kebutuhan | Komponen yang Dipakai |
|-----------|----------------------|
| Root halaman | `<uranium:UraniumContentPage>` |
| Input teks / email / telepon | `<material:TextField>` + `material:TextField.Icon` |
| Toggle show/hide password | `material:TextFieldPasswordShowHideAttachment` |
| Checkbox form | `<material:CheckBox>` (Register) / `<CheckBox>` (Login) |
| Bottom sheet detail layanan | `<material:BottomSheetView>` |
| CTA utama | `<Button StyleClass="FilledButton">` (style dari UraniumUI.Material) |
| Validasi form Register | `<input:FormView>` + `validation:*` |
| Tema global | `<material:StyleResource ColorsOverride="..." />` di `App.xaml` |

### Catatan: kenapa ada `Controls/ButtonView.cs`?

Di **UraniumUI 3.0**, kontrol `ButtonView` pindah dari namespace `UraniumUI` ke `UraniumUI.Material.Controls`. Sementara PRD menulis sintaks `<uranium:ButtonView>`.

Agar XAML tetap sesuai PRD **tanpa** mengubah satu baris pun, dibuat *shim*:

```csharp
public class ButtonView : UraniumUI.Material.Controls.ButtonView { }
```

Shim ini didaftarkan lewat `AssemblyInfo.cs` (`XmlnsDefinition`) dan handler-nya didaftarkan di `MauiProgram.cs`. Karena mewarisi (`inherit`) kontrol Uranium asli, seluruh perilaku dan tampilan tetap milik UraniumUI.

### Catatan: `Platforms/Windows/SafeStatefulButtonHandler.cs`

UraniumUI 3.0 mendaftarkan `StatefulButtonHandler` untuk setiap `Button` polos. Saat halaman di-pop dari navigation stack, handler tersebut mereset visual state setelah MAUI menghapus platform view, sehingga muncul error `PlatformView cannot be null here` dan aplikasi tertutup. Di Windows, handler diganti dengan `SafeStatefulButtonHandler` yang mempertahankan efek hover/pressed tetapi melewati reset state saat proses disconnect.

### Admin UI (Phase 2)

- Layout **mobile-first**: header admin, konten bertumpuk, dan bottom navigation Dashboard / Booking / Kasir / Reports / Settings.
- Komponen bersama di `Controls/Admin/`: `AdminLayout`, `AdminNavigationItem`, `StatCard`, `SectionCard`, `StatusBadge`, `PaymentBadge`.
- Style bersama di `Resources/Styles/AdminStyles.xaml`; warna tetap memakai token dari `Colors.xaml`.
- Data mock di `Models/Admin/`; ViewModel di `ViewModels/Admin/`.
- Alur login/register customer tidak diubah; hanya ditambahkan tautan **Masuk sebagai Admin** di bagian bawah halaman login.
- Font admin memakai `OpenSansRegular` / `OpenSansSemibold`, sedangkan halaman customer memakai `PlusJakartaSans*`; keduanya didaftarkan di `MauiProgram.cs`.

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

- `SemanticProperties.Description` + `SemanticProperties.Hint` pada kontrol interaktif
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
| Coral dark | `#8B2626` | Judul dan teks brand |
| Blush | `#FDECEC` | Latar lembut, chip aktif, aksen |
| Background | `#FAF7F5` | Latar halaman |
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

**Build gagal karena file `.exe` terkunci** → tutup aplikasi yang sedang berjalan, lalu build ulang.

**Error Android SDK / emulator** → buka **Android Device Manager** di Visual Studio, buat/boot emulator dulu sebelum `dotnet build -t:Run -f net10.0-android`.

---

## 📦 Dependency

| Package | Versi | Fungsi |
|---------|-------|--------|
| `UraniumUI` | 3.0.0 | Core framework UI |
| `UraniumUI.Material` | 3.0.0 | Tema & komponen Material (TextField, BottomSheet, dll) |
| `UraniumUI.Icons.MaterialSymbols` | 3.0.0 | Ikon Material Symbols |
| `InputKit.Maui` | 4.6.0 | FormView & validasi form |
| `CommunityToolkit.Mvvm` | 8.4.2 | MVVM: `ObservableObject`, `RelayCommand` |

> Integrasi API tidak menambah package NuGet; memakai `HttpClient`, `System.Text.Json`, `SecureStorage`, dan `Preferences` bawaan MAUI/.NET.

---

## 📚 Dokumentasi

- [docs/README.md](docs/README.md) — index fitur dan backlog dokumentasi
- [docs/features/api-integration/technical-design.md](docs/features/api-integration/technical-design.md) — desain teknis integrasi API
- [docs/features/api-integration/qa-test.md](docs/features/api-integration/qa-test.md) — test matrix dan hasil QA
- [docs/known-issues.md](docs/known-issues.md) — daftar fitur yang belum sempurna

---

## 📄 Lisensi & Catatan Akademik

Proyek ini dibuat untuk keperluan **tugas UAS mata kuliah Pemrograman Visual**. Desain dan spesifikasi mengikuti dokumen `prd_monica_salon_phase_1.md` di dalam repository ini.

Referensi UraniumUI: <https://enisn-projects.io/docs/en/uranium/latest>
