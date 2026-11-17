# Monica Salon — Aplikasi Mobile UI (Phase 1)

Aplikasi **UI-only** untuk Monica Salon, dibangun dengan **.NET MAUI 10** dan **UraniumUI 3.0**. Proyek ini adalah tugas UAS mata kuliah Pemrograman Visual.

> **Scope Phase 1:** aplikasi **UI-only**. Belum ada backend, database, halaman admin, atau pembayaran nyata. Data salon, jadwal, booking, dan ulasan disediakan oleh **mock API layer** (`IBookingApi`, `IFeedbackApi`, `IScheduleApi`) yang disimpan di memori melalui `MockSalonDatabase`. Layer ini bisa diganti dengan HTTP API tanpa mengubah halaman atau ViewModel. Booking baru langsung berstatus **Terkonfirmasi** — tidak ada status "Menunggu Konfirmasi".

---

## 📱 Halaman yang Tersedia

| Halaman | File | Isi |
|---------|------|-----|
| Login | `Views/LoginPage.xaml` | Hero coral dengan ilustrasi kontur, transisi gelombang ke panel putih, form email/password, "Ingat saya", lupa password, dan tautan daftar |
| Register | `Views/RegisterPage.xaml` | Form pendaftaran customer (nama, email, telepon, password, konfirmasi, checkbox S&K) |
| Home Customer | `Views/HomeCustomerPage.xaml` | Dashboard customer: sapaan, pencarian, banner promo, kategori, daftar layanan, bottom navigation, bottom sheet detail layanan |
| Katalog Layanan | `Views/CataloguePage.xaml` | Daftar semua layanan dengan pencarian dan filter kategori |
| Detail Layanan | `Views/ServiceDetailPage.xaml` | Foto, kategori, harga, durasi, deskripsi, dan tombol booking |
| Jadwal | `Views/SchedulePage.xaml` | Pilih layanan tambahan, tanggal, slot waktu bergaya bioskop, hairstyle, add-on, dan ringkasan total |
| Konfirmasi Booking | `Views/ConfirmBookingPage.xaml` | Ringkasan layanan/jadwal, hairstyle, add-on, kontak, catatan, dan rincian harga |
| Booking Berhasil | `Views/BookingSuccessPage.xaml` | Kode booking, status **Terkonfirmasi**, jadwal, layanan, add-on, dan total |
| Riwayat Booking | `Views/BookingHistoryPage.xaml` | Daftar booking dengan filter status (Semua/Aktif/Selesai/Dibatalkan) dan shortcut beri ulasan |
| Detail Booking | `Views/BookingDetailsPage.xaml` | Status, jadwal, layanan, add-on, catatan, total, dan tombol beri ulasan |
| Ulasan | `Views/BookingFeedbackPage.xaml` | Rating bintang, tag ulasan, komentar, dan kirim ulasan |
| Buku Gaya | `Views/HairstyleBookPage.xaml` | Katalog hairstyle dari admin dengan filter kategori |
| Detail Hairstyle | `Views/HairstyleDetailPage.xaml` | Deskripsi gaya, rekomendasi layanan, dan tombol booking |

Alur navigasi utama:

`LoginPage` → (Login / Daftar) → `RegisterPage` / `HomeCustomerPage` → `CataloguePage` / `HairstyleBookPage` → `ServiceDetailPage` / `HairstyleDetailPage` → `SchedulePage` → `ConfirmBookingPage` → `BookingSuccessPage` → `BookingHistoryPage` → `BookingDetailsPage` → `BookingFeedbackPage`.

Bottom navigation (`Controls/BottomNavBar.cs`) menghubungkan **Home**, **Layanan**, **Booking**, **Riwayat**, dan **Gaya**.

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

> 💡 Login bersifat demo: email dan password apa pun yang tidak kosong akan membuka Home. "Ingat saya" menyimpan identifier melalui `Preferences` agar terisi otomatis pada pembukaan berikutnya.

---

## 🧱 Struktur Proyek

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
│   ├── BottomNavBar.cs         # Bottom navigation bersama (5 tab)
│   └── ButtonView.cs           # Shim: <uranium:ButtonView> → UraniumUI Material
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
│   └── RatingStarItem.cs
│
├── Services/                   # Mock API, session, katalog, formatter
│   ├── AppServices.cs          # Service locator sederhana untuk halaman/VM
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
│   └── HairstyleDetailViewModel.cs
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
│   └── HairstyleDetailPage.xaml(.cs)
│
├── Icons/
│   └── MaterialRegular.cs      # Kumpulan nama ikon Material Symbols
│
├── Platforms/                  # Konfigurasi per-platform (Android/iOS/Windows/Mac)
└── Resources/                  # Font, gambar, icon, splash, style warna
    └── Images/
        ├── login_hero.svg      # Ilustrasi kontur coral untuk hero login
        ├── style_*.png         # Ilustrasi hairstyle
        └── service_*.jpg       # Foto layanan
```

---

## 🎨 Catatan UI

### Login

- Hero coral memakai ilustrasi kontur `login_hero.svg` dan transisi gelombang putih (`Path`) ke panel form.
- Form dibatasi maksimal **440 px** dan tetap terpusat pada layar lebar; pada layar sempit konten dapat di-scroll.
- Validasi login dilakukan di `LoginViewModel` dan ditampilkan sebagai banner error di atas tombol **Masuk** (bukan ikon validasi di dalam field).
- "Ingat saya" menyimpan identifier lewat `Preferences`; password tidak pernah disimpan.
- Warna mengikuti token coral/blush yang sudah ada (`#D9534F`, `#8B2626`, `#FDECEC`, `#FAF7F5`).

### Booking

- Slot waktu mengikuti pola **cinema booking**: admin/API menyediakan slot tersedia, slot terisi tidak bisa dipilih, dan tidak ada status "Menunggu Konfirmasi".
- Halaman jadwal menggabungkan pemilihan tanggal, slot, hairstyle, add-on (mis. shampoo), dan total yang diperbarui langsung.
- Setelah submit, booking disimpan lewat `IBookingApi` dengan status **Terkonfirmasi**.

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

---

## 📄 Lisensi & Catatan Akademik

Proyek ini dibuat untuk keperluan **tugas UAS mata kuliah Pemrograman Visual**. Desain dan spesifikasi mengikuti dokumen `prd_monica_salon_phase_1.md` di dalam repository ini.

Referensi UraniumUI: <https://enisn-projects.io/docs/en/uranium/latest>
