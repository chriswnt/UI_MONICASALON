# Technical Design — Integrasi API Client

> Status: In Progress
> Terakhir diperbarui: 2026-11-19
> Slug: `api-integration`
> Halaman/flow: Login, Register, Home, Katalog, Buku Gaya, Jadwal, Konfirmasi Booking, Riwayat Booking, Detail Booking, shell admin
> QA: [qa-test.md](./qa-test.md)
> Known issues: [../../known-issues.md](../../known-issues.md)

## 1. Ringkasan & tujuan

Aplikasi sebelumnya 100% UI-only: login menerima kredensial apa pun, katalog dan jadwal berasal dari data statis, dan booking disimpan di memori. Perubahan ini menghubungkan aplikasi ke backend Monica Salon (`https://monica-salon-be.onrender.com`) untuk auth, katalog, ketersediaan jadwal, dan booking customer, lengkap dengan penyimpanan token, auto-refresh, serta fallback katalog lokal saat API tidak dapat dijangkau.

Hasil yang diharapkan: customer dapat login/register dengan akun nyata, melihat layanan dan gaya dari API, memilih slot yang benar-benar tersedia, membuat booking yang tersimpan di PostgreSQL, melihat riwayat, membatalkan booking yang masih `Pending`, dan tetap bisa membuka aplikasi ketika API sedang cold start atau offline (dengan data fallback).

## 2. Scope

### Termasuk

- Konfigurasi API dari `.env`/`.env.example` yang di-embed saat build.
- `ApiConfig`, `ApiClient`, `TokenStore`, `ApiException`, `SalonTime`.
- DTO dan implementasi HTTP untuk Auth, Catalog, Booking, Schedule, Admin Booking, Transaction, Inventory, dan Finance.
- Registrasi DI di `MauiProgram` dan service locator `AppServices`.
- Wiring UI: login, register, logout admin, role routing, katalog, buku gaya, jadwal, create booking, riwayat, detail, cancel.
- Status booking `Pending`, tampilan status, dan aturan customer hanya bisa cancel saat `Pending`.
- Fallback katalog lokal ketika API gagal, serta penyesuaian teks UI yang masih menyebut mode demo.
- Penyembunyian section add-on karena backend belum punya endpoint add-on.

### Tidak termasuk

- Wiring UI Admin Dashboard, Admin Booking, Admin Create/Reschedule, Kasir, dan Riwayat Transaksi ke API. HTTP service-nya sudah ada, tetapi UI masih memakai mock.
- Endpoint/feedback backend. Feedback customer masih memakai `MockFeedbackApi`.
- Add-on booking. Section add-on di `SchedulePage` disembunyikan.
- Halaman Inventory, Finance, Reports, Settings, dan profil customer.
- Auto-login saat app dibuka ulang.
- Perubahan pada project `monica-salon-api`.
- Automated test project.

## 3. User flow & navigasi

| Langkah | Aksi user | Hasil | Route/Shell |
|---|---|---|---|
| 1 | Buka aplikasi | Halaman login tampil | `LoginRoot/LoginPage` |
| 2 | Login customer | Token disimpan, masuk Home | `//HomeRoot/HomeCustomerPage` |
| 3 | Login Staff/Owner | Token disimpan, masuk dashboard admin | `AdminDashboardPage` |
| 4 | Register customer | Akun dibuat di API, token disimpan, masuk Home | `//HomeRoot/HomeCustomerPage` |
| 5 | Buka katalog/buku gaya | Layanan & hairstyle dimuat dari API (cache sesi) | `CataloguePage`, `HairstyleBookPage` |
| 6 | Pilih layanan/gaya | Draft booking disimpan di `BookingSession` | `ServiceDetailPage`, `HairstyleDetailPage` |
| 7 | Pilih jadwal | Availability 14 hari dan slot diambil dari API | `SchedulePage` |
| 8 | Konfirmasi booking | `POST /bookings`, status awal `Pending` | `ConfirmBookingPage` → `BookingSuccessPage` |
| 9 | Riwayat/detail | `GET /bookings/mine` dan `GET /bookings/{id}` | `BookingHistoryPage`, `BookingDetailsPage` |
| 10 | Batalkan booking | `POST /bookings/{id}/cancel` hanya untuk `Pending` | `BookingDetailsPage` |
| 11 | Buka tautan admin tanpa role | Alert, tidak masuk dashboard | `LoginPage` |

Kondisi masuk/keluar:

- Masuk customer: login/register berhasil dan role `Customer`.
- Masuk admin: login berhasil dan role `Staff`/`Owner`; tombol **Masuk sebagai Admin** hanya membuka dashboard jika `UserSession.IsStaffOrOwner`.
- Keluar: `AdminLayout` memanggil `POST /auth/logout`, menghapus token, lalu kembali ke `LoginRoot/LoginPage`.
- Navigasi kembali tidak menyimpan state di page/VM; state lintas halaman memakai `BookingSession`, `CatalogCache`, `TokenStore`, dan `UserSession`.

## 4. Arsitektur & file terkait

| Layer | File | Peran |
|---|---|---|
| Config | `.env`, `.env.example` | Base URL, prefix, timeout, environment; di-embed ke assembly lewat `UAS_APP_MONICASALON.csproj`. |
| Config | `Services/Api/ApiConfig.cs` | Membaca resource `UAS_APP_MONICASALON.appconfig`, override environment variable, memilih `API_BASE_URL_ANDROID` di Android, menyusun URL. |
| HTTP core | `Services/Api/ApiClient.cs` | GET/POST/PUT, bearer token, refresh proaktif, retry sekali saat 401, parsing `ProblemDetails`, timeout. |
| HTTP core | `Services/Api/TokenStore.cs` | Menyimpan access/refresh token + user di `SecureStorage`, fallback `Preferences`. |
| HTTP core | `Services/Api/ApiException.cs` | Exception dengan status code dan pesan API. |
| HTTP core | `Services/Api/SalonTime.cs` | Konversi waktu UTC API ↔ WIB (Asia/Jakarta). |
| DTO | `Services/Api/Dtos/*.cs` | `AuthDtos`, `CatalogDtos`, `BookingDtos`, `CashierDtos`, `InventoryDtos`, `FinanceDtos`, `CommonDtos`. |
| API | `Services/Api/HttpAuthApi.cs` | Login, register, me, logout, change password. |
| API | `Services/Api/HttpCatalogApi.cs` + `CatalogMapper.cs` | Services/hairstyles + mapping API DTO ke model UI. |
| API | `Services/Api/CatalogCache.cs` | Cache layanan/hairstyle selama sesi aplikasi. |
| API | `Services/Api/HttpScheduleApi.cs` | Availability per tanggal + mapping slot. |
| API | `Services/Api/HttpBookingApi.cs` | Create, history, detail, cancel, reschedule (method tersedia, belum dipakai UI). |
| API | `Services/Api/HttpAdminBookingApi.cs` | Endpoint admin booking (belum dipakai UI). |
| API | `Services/Api/HttpTransactionApi.cs` | Checkout dan riwayat transaksi (belum dipakai UI). |
| API | `Services/Api/HttpInventoryApi.cs` | Inventori dan movement (belum dipakai UI). |
| API | `Services/Api/HttpFinanceApi.cs` | Summary, entries, export (belum dipakai UI). |
| DI | `MauiProgram.cs` | Registrasi `HttpClient`, `TokenStore`, `ApiClient`, semua API, `CatalogCache`; memanggil `AppServices.Configure(app.Services)`. |
| DI | `Services/AppServices.cs` | Service locator statis agar halaman/lama tidak perlu refactor constructor. |
| Model | `Models/ServiceItem.cs` | Tambah `ApiId`, `IsActive`. |
| Model | `Models/HairstyleItem.cs` | Tambah `ApiId`. |
| Model | `Models/BookingRecord.cs` | Tambah `ApiId`; `IsActive` mencakup `Pending`; `CanCancel` hanya `Pending`; warna/teks status `Pending`. |
| Model | `Models/BookingStatus.cs` | Tambah `Pending` + teks "Menunggu Konfirmasi". |
| Model | `Services/UserSession.cs` | Tambah `UserId`, `Role`, `IsStaffOrOwner`, `ApplyUser`; logout mereset identitas. |
| ViewModel | `ViewModels/LoginViewModel.cs` | Login API, simpan user, route berdasarkan role, `IsBusy`. |
| ViewModel | `ViewModels/RegisterViewModel.cs` | Register API, simpan user, `IsBusy`. |
| ViewModel | `HomeCustomerViewModel.cs` | Load layanan/hairstyle dari `CatalogCache`, fallback statis. |
| ViewModel | `CatalogueViewModel.cs` | Load layanan API, rebuild opsi kategori, fallback statis. |
| ViewModel | `HairstyleBookViewModel.cs` | Load hairstyle API, rebuild kategori, fallback statis. |
| ViewModel | `ScheduleViewModel.cs` | Load katalog API, availability API, slot API. |
| ViewModel | `ConfirmBookingViewModel.cs` | Submit `BookingRequest` ke `HttpBookingApi`. |
| ViewModel | `BookingHistoryViewModel.cs`, `BookingDetailsViewModel.cs` | Riwayat/detail/cancel lewat `HttpBookingApi`. |
| ViewModel | `BookingFeedbackViewModel.cs` | Booking dari API, feedback masih `MockFeedbackApi`. |
| View | `Views/LoginPage.xaml(.cs)`, `RegisterPage.xaml(.cs)` | Teks login/register disesuaikan; guard tautan admin. |
| View | `Views/*Page.xaml.cs` | `OnAppearing` memanggil load API pada Home, Katalog, Buku Gaya, Jadwal. |
| View | `Views/SchedulePage.xaml` | Section add-on `IsVisible="False"`; teks konfirmasi diperbarui. |
| Mock | `Services/MockFeedbackApi.cs` + `MockSalonDatabase.cs` | Feedback lokal karena backend belum punya endpoint. |
| Mock | `Models/Admin/AdminMockData.cs`, `AdminBookingStore.cs` | Admin booking masih in-memory. |
| Mock | `Models/Admin/CashierMockData.cs`, `CashierSession.cs` | Kasir/transaksi masih in-memory. |

## 5. Kontrak data & API

Base URL production: `https://monica-salon-be.onrender.com`. Prefix: `/api/v1`. Semua endpoint selain auth dan health memakai Bearer token.

| Endpoint | Method | Auth | Request | Response | Dipakai di |
|---|---|---|---|---|---|
| `/auth/login` | POST | Anon | `LoginRequest { emailOrUsername, password }` | `AuthResponse { accessToken, refreshToken, expiresAt, user }` | `HttpAuthApi`, `LoginViewModel` |
| `/auth/register` | POST | Anon | `RegisterRequest { name, email, phone, password }` | `AuthResponse` | `HttpAuthApi`, `RegisterViewModel` |
| `/auth/refresh` | POST | Anon | `RefreshRequest { refreshToken }` | `AuthResponse` | `ApiClient.RefreshTokensAsync` |
| `/auth/logout` | POST | Bearer | — | 204 | `AdminLayout` lewat `HttpAuthApi` |
| `/auth/me` | GET | Bearer | — | `UserDto` | `HttpAuthApi` (belum dipakai UI) |
| `/auth/change-password` | POST | Bearer | `ChangePasswordRequest` | 204 | `HttpAuthApi` (belum dipakai UI) |
| `/services` | GET | Bearer | `includeInactive?` | `ServiceDto[]` | `HttpCatalogApi` → Home/Katalog/Jadwal |
| `/hairstyles` | GET | Bearer | `includeInactive?` | `HairstyleDto[]` | `HttpCatalogApi` → Home/Buku Gaya/Jadwal |
| `/bookings/availability?date=` | GET | Bearer | `date=yyyy-MM-dd` | `AvailabilityResponse { date, slots[] }` | `HttpScheduleApi` |
| `/bookings` | POST | Bearer | `CreateBookingRequest { customerId?, serviceIds[], startAt, notes?, hairstyleId?, hairstyleNote?, customerName?, customerPhone? }` | `BookingDto` | `HttpBookingApi.CreateAsync` |
| `/bookings/mine` | GET | Bearer | — | `BookingDto[]` | `HttpBookingApi.GetHistoryAsync` |
| `/bookings/{id}` | GET | Bearer | — | `BookingDto` | `HttpBookingApi.GetByIdAsync` |
| `/bookings/{id}/cancel` | POST | Bearer | `CancelBookingRequest { reason? }` | `BookingDto` | `HttpBookingApi.CancelAsync` |
| `/bookings/{id}/reschedule` | POST | Bearer | `RescheduleBookingRequest { startAt }` | `BookingDto` | `HttpBookingApi.RescheduleAsync` (belum dipakai UI) |
| `/bookings` | GET | Staff/Owner | `date?, status?, search?, page?, pageSize?` | `PagedResult<BookingDto>` | `HttpAdminBookingApi` (belum dipakai UI) |
| `/bookings/{id}/confirm` | POST | Staff/Owner | — | `BookingDto` | `HttpAdminBookingApi` (belum dipakai UI) |
| `/transactions` | POST | Staff/Owner | `CheckoutRequest` | `TransactionDto` | `HttpTransactionApi` (belum dipakai UI) |
| `/transactions` | GET | Staff/Owner | `date?, page?, pageSize?` | `PagedResult<TransactionDto>` | `HttpTransactionApi` (belum dipakai UI) |
| `/transactions/{id}` | GET | Staff/Owner | — | `TransactionDto` | `HttpTransactionApi` (belum dipakai UI) |
| `/inventory`, `/inventory/{id}`, `/inventory/{id}/movements` | GET/POST/PUT | Staff/Owner | `InventoryUpsertRequest`, `StockMovementRequest` | DTO terkait | `HttpInventoryApi` (belum ada UI) |
| `/finance/summary`, `/finance/entries`, `/finance/export` | GET/POST | Owner | `CreateFinanceEntryRequest` | DTO/Excel | `HttpFinanceApi` (belum ada UI) |

Catatan kontrak:

- DTO ada di `Services/Api/Dtos/`.
- Mapping katalog ada di `CatalogMapper`; mapping booking ada di `HttpBookingApi.ToRecordAsync`.
- Waktu API adalah UTC. `SalonTime` mengonversi ke WIB untuk `BookingRecord.Date/Time`, `AdminBooking.AppointmentAt`, dan transaksi.
- Backend **belum punya** endpoint feedback dan add-on. `IFeedbackApi` tetap `MockFeedbackApi`; section add-on disembunyikan.

## 6. Desain state

### 6.1 Sumber data

| Data | Sumber | Catatan |
|---|---|---|
| Token & user login | API + `TokenStore` | Token di `SecureStorage`; fallback `Preferences`. |
| Identitas tampilan (`UserSession`) | `AuthResponse.User` | In-memory; tidak direstore saat app restart. |
| Layanan & hairstyle | API `/services`, `/hairstyles` | Fallback ke `ServiceCatalog`/`HairstyleCatalog` jika API gagal. |
| Availability & slot | API `/bookings/availability` | Tidak ada fallback slot palsu; error ditampilkan. |
| Booking customer | API `/bookings` | Persisten di PostgreSQL; status awal `Pending`. |
| Feedback | `MockSalonDatabase` | In-memory; hilang saat app ditutup. |
| Admin booking | `AdminBookingStore` + `AdminMockData` | In-memory; belum memakai API. |
| Kasir/transaksi | `CashierSession` + `CashierMockData` | In-memory; belum memakai API. |

### 6.2 Kepemilikan state

| State | Pemilik | Lifetime | Reset/invalidasi |
|---|---|---|---|
| Access token, refresh token, expiry | `TokenStore` singleton | Lintas navigasi dan app restart | `ClearAsync` saat logout, refresh gagal, atau 401 tanpa refresh token. |
| User aktif & role | `UserSession` static | Proses aplikasi | `SignOut`; tidak otomatis direstore saat restart (KI-013). |
| Cache layanan/hairstyle | `CatalogCache` singleton | Proses aplikasi | `Invalidate()`; tidak auto-refresh setelah perubahan admin (KI-018). |
| Draft booking | `BookingSession` static | Proses aplikasi | `ClearDraft()` setelah booking sukses; `Begin()`/`StartBookingFrom()` saat mulai draft baru. |
| Booking terakhir / ID terpilih | `BookingSession` static | Proses aplikasi | Ditimpa saat membuka booking lain; dipakai untuk navigasi detail. |
| Identifier "Ingat saya" | `Preferences` | Persisten | Checkbox login; password tidak pernah disimpan. |
| Data booking server | PostgreSQL lewat API | Persisten | Refetch saat halaman riwayat/detail dibuka. |
| Feedback | `MockSalonDatabase` static | Proses aplikasi | Hilang saat app ditutup (KI-001). |
| Admin booking & kasir | `AdminBookingStore`/`CashierSession` static | Proses aplikasi | Hilang saat app ditutup (KI-007). |

### 6.3 UI state

| State | Pemicu | Tampilan | Aksi user |
|---|---|---|---|
| Loading login/register | Request auth berjalan | `IsBusy` mencegah submit ganda; indikator visual belum di-bind ke tombol | Tunggu |
| Error login/register | API 401/409/validasi | Banner `ErrorMessage` berisi pesan API | Perbaiki input, submit ulang |
| Loading Home/Katalog/Buku Gaya | `OnAppearing` memuat katalog | Data fallback lokal tampil lebih dulu; tidak ada spinner khusus | Tetap bisa melihat data fallback |
| Empty Katalog | Filter tidak menemukan layanan | `IsEmpty` + teks "Layanan tidak ditemukan." | Ubah pencarian/kategori |
| Empty Buku Gaya | Filter tidak menemukan gaya | `CollectionView.EmptyView` | Ubah pencarian/kategori |
| Error katalog | API gagal | Fallback lokal dipakai; error tidak selalu terlihat | Coba buka ulang halaman |
| Loading jadwal | `IsLoadingDates` / `IsLoadingSlots` | Indikator tanggal/slot | Tunggu |
| Empty slot | Tidak ada slot tersedia | `ShowSlotEmpty` + teks penjelasan | Pilih tanggal lain |
| Error slot | API gagal | `SlotErrorMessage` + tombol retry | Retry |
| Submit booking | `IsSubmitting` | Teks tombol "Memproses booking..." | Tunggu |
| Error booking | API 409/400/offline | `ErrorMessage` di halaman konfirmasi | Perbaiki pilihan/tanggal |
| Riwayat loading/empty/error | Load API | `IsLoading`, `IsEmpty`, `ErrorMessage` | Refresh / booking baru |
| Detail booking loading/error | Load API | `IsLoading`, `ErrorMessage` | Kembali / retry |
| Cancel booking | `IsActionBusy` | Dialog konfirmasi, lalu status berubah | Tunggu |
| Admin link tanpa role | Tap "Masuk sebagai Admin" | Alert "Masuk dengan akun Staff atau Owner" | Login dulu |

### 6.4 Perilaku penting

- **Buka ulang halaman/back:** Shell membuat instance page/VM baru. Data diambil ulang dari `CatalogCache` (jika sudah ada) atau API. Draft booking tetap ada di `BookingSession`.
- **Submit ganda:** `LoginViewModel.IsBusy`, `RegisterViewModel.IsBusy`, `ConfirmBookingViewModel.IsSubmitting`, dan `BookingDetailsViewModel.IsActionBusy` mencegah command berjalan dua kali. Tombol belum tentu disabled secara visual.
- **Timeout/offline:** `ApiClient` melempar `ApiException` dengan pesan Indonesia. Home/Katalog/Buku Gaya memakai fallback lokal; halaman yang butuh data real-time (jadwal, riwayat, detail, submit) menampilkan error.
- **401/refresh token:** Access token yang kedaluwarsa direfresh proaktif. Saat 401, `ApiClient` mencoba refresh sekali lalu mengulang request. Jika refresh gagal, token dibersihkan dan pesan "Sesi kamu sudah berakhir" ditampilkan.
- **Cache:** `CatalogCache` menyimpan layanan/hairstyle selama sesi. Tidak ada invalidasi otomatis setelah admin mengubah katalog; app perlu restart atau `Invalidate()` manual.
- **App restart:** Token tetap di `SecureStorage`/`Preferences`, tetapi `UserSession` tidak direstore, jadi user kembali ke halaman login. Belum ada auto-login.
- **Android deployment:** APK Debug hasil `dotnet build` memakai Fast Deployment; jika di-install manual, app crash karena assemblies tidak ikut. Gunakan `dotnet build -t:Run` atau `dotnet publish -f net10.0-android -c Release` (KI-017).

## 7. Aturan bisnis & validasi

| Aturan | Detail | Sumber |
|---|---|---|
| Login | `emailOrUsername` dan password wajib; API memvalidasi kredensial. Pesan 401: "Email/username atau password salah." | `LoginViewModel`, `AuthEndpoints` |
| Register | Nama, email, telepon, password wajib; password minimal 6 karakter; email harus unik; S&K wajib dicentang. | `RegisterViewModel`, `AuthEndpoints` |
| Role routing | `Owner`/`Staff` → `AdminDashboardPage`; `Customer` → Home. | `LoginViewModel`, `UserSession` |
| Tautan admin | Hanya membuka dashboard jika `UserSession.IsStaffOrOwner`; jika tidak, tampil alert. | `LoginPage.xaml.cs` |
| Katalog | Hanya layanan aktif yang ditampilkan. Field yang tidak ada di API diisi dari katalog lokal (durasi, rating, gambar, deskripsi). | `CatalogMapper` |
| Jadwal | 14 hari ke depan; slot mengikuti `SalonSettings` backend (buka 09:00, tutup 21:00, slot 30 menit, kapasitas 3). Slot dengan `Remaining == 0` = "Penuh"; slot ≤ 1 jam dari sekarang = "Lewat". | `HttpScheduleApi`, `BookingService` |
| Create booking | Minimal satu layanan; tanggal dan jam wajib; `StartAt` dikirim dalam UTC hasil konversi WIB; `HairstyleId` dikirim jika ada; `HairstyleNote` dipakai jika gaya tidak punya ID; catatan di-trim. Status awal `Pending`. | `HttpBookingApi`, `BookingService` |
| Cancel booking | Customer hanya boleh cancel saat status `Pending`. `BookingRecord.CanCancel` mengikuti aturan ini. | `BookingRecord`, `BookingService` |
| Add-on | Tidak dikirim ke API; section add-on disembunyikan. | `SchedulePage.xaml` |
| Feedback | Disimpan lokal; tidak dikirim ke backend. | `MockFeedbackApi` |
| Admin/kasir | Masih memakai aturan dan data mock lama; belum terhubung ke endpoint API. | `AdminBookingStore`, `CashierSession` |

## 8. Edge case & failure handling

| Kondisi | Perilaku yang diharapkan | Catatan |
|---|---|---|
| Access token expired | Refresh proaktif sebelum request | `ApiClient.SendCoreAsync` |
| 401 saat request | Refresh sekali, retry request | Jika refresh gagal, token dihapus dan error sesi tampil |
| Refresh token invalid/expired | `TokenStore.ClearAsync`, user diarahkan login ulang | `ApiClient.RefreshTokensAsync` |
| 403 | Pesan "Kamu tidak punya akses ke fitur ini." | `ApiClient.EnsureSuccessAsync` |
| 404 booking | `GetByIdAsync`/`CancelAsync` mengembalikan `null` | UI menampilkan "Booking tidak ditemukan" |
| 409 slot penuh | Pesan dari `ProblemDetails.Detail` | UI menampilkan error di halaman konfirmasi |
| 429 rate limit | Pesan "Terlalu banyak percobaan..." | `ApiClient` |
| Timeout/offline | `ApiException` dengan pesan koneksi | Fallback katalog untuk Home/Katalog/Buku Gaya |
| Respons bukan JSON | `ApiException` 500 "Respons API tidak dapat dibaca" | `ApiClient` |
| API cold start (Render) | Timeout 60 detik | Request pertama bisa lambat |
| Katalog API gagal | Fallback `ServiceCatalog`/`HairstyleCatalog` | Data fallback bisa beda harga/kategori dengan API (KI-020) |
| Hairstyle lookup gagal saat mapping booking | Booking tetap dimuat tanpa gambar gaya | `HttpBookingApi.ToRecordAsync` menangkap exception |
| Satu request availability gagal | Seluruh `GetAvailabilityAsync` gagal (Task.WhenAll) | UI menampilkan error dan tombol retry |
| Slot kosong | `ShowSlotEmpty` + "Semua slot di tanggal ini sudah penuh" | `ScheduleViewModel` |
| Submit ganda | Dicegah oleh flag `IsBusy`/`IsSubmitting` | Tombol belum disabled secara visual |
| Back saat request berjalan | Request tidak dibatalkan eksplisit; hasil diabaikan oleh page baru | Tidak ada cancellation token dari UI |
| App restart | Token persist, sesi user tidak; kembali ke login | KI-013 |
| SecureStorage tidak tersedia | Token disimpan di `Preferences` | Fallback di `TokenStore` |
| APK Debug di-install manual | Crash "No assemblies found ... Fast Deployment" | KI-017; gunakan Release publish atau `-t:Run` |
| Deep link ke halaman admin tanpa role | Belum ada guard di route/page | KI-014 |

## 9. Keputusan teknis

| Keputusan | Alasan | Alternatif yang ditolak |
|---|---|---|
| Meng-embed `.env` sebagai `EmbeddedResource` | MAUI tidak membaca `.env` native; resource tersedia di semua platform tanpa file IO runtime. | Membaca file dari disk (tidak reliable di Android/iOS), hardcode URL di source. |
| Environment variable menimpa nilai `.env` | Memudahkan CI/override tanpa rebuild resource. | Hanya mengandalkan file. |
| Base URL per platform | Android emulator butuh `10.0.2.2` untuk API lokal; production memakai HTTPS. | Satu URL untuk semua platform. |
| `ApiClient` singleton + `HttpClient` singleton | Cukup untuk app mobile; tidak perlu `IHttpClientFactory`. | Menambah package `Microsoft.Extensions.Http`. |
| Refresh token ditangani di `ApiClient` | Semua request otomatis mendapat perilaku yang sama. | Refresh manual di setiap ViewModel. |
| `TokenStore` memakai `SecureStorage` + fallback `Preferences` | `SecureStorage` tidak selalu tersedia di Windows unpackaged. | Hanya `Preferences` (kurang aman), hanya `SecureStorage` (bisa gagal). |
| Mempertahankan `AppServices` static locator | Menghindari refactor constructor di seluruh halaman. | Inject service ke setiap page/VM (perubahan besar). |
| Fallback katalog lokal | Aplikasi tetap bisa dipakai saat API cold start/offline. | Menampilkan error kosong di Home/Katalog. |
| `SalonTime` WIB | Backend memakai Asia/Jakarta; tampilan harus konsisten meski timezone device berbeda. | Memakai `DateTimeOffset.ToLocalTime()` device. |
| Menambah status `Pending` | Backend membuat booking baru sebagai `Pending`. | Memaksa backend langsung `Confirmed` atau memetakan `Pending` ke `Confirmed` (menyesatkan). |
| Menyembunyikan add-on | Backend belum punya endpoint add-on; total akan berbeda jika tetap ditampilkan. | Mengirim add-on sebagai layanan palsu, menampilkan add-on tanpa dikirim (menyesatkan). |
| Feedback tetap mock | Backend belum punya endpoint feedback. | Menghapus fitur ulasan dari UI. |
| Admin/kasir tetap mock | Backend belum punya stylist/payment status/Rejected/NoShow/refund; UI terikat ke model mock. | Refactor besar sebelum kontrak backend siap. |
| Tanpa package baru | `HttpClient`, `System.Text.Json`, `SecureStorage`, `Preferences` sudah tersedia di MAUI. | Menambah library HTTP/JSON/dotenv. |

## 10. Dependensi & risiko

| Item | Jenis | Dampak/risiko |
|---|---|---|
| .NET 10 + .NET MAUI | Platform | Target Windows/Android; iOS/Mac hanya bisa build di Mac. |
| `System.Net.Http`, `System.Text.Json` | Bawaan .NET | Tidak ada package tambahan. |
| `SecureStorage`/`Preferences` | MAUI Essentials | Perilaku berbeda per platform; fallback sudah disediakan. |
| `https://monica-salon-be.onrender.com` | API production | Render free tier bisa cold start; timeout 60 detik. |
| Kontrak API belum lengkap | Backend | Field katalog, feedback, add-on, stylist, payment status, refund belum tersedia. |
| Release AOT/trimming | Build | APK Release yang diuji sudah berjalan; perubahan reflection di masa depan perlu QA ulang. |
| Fast Deployment Debug | Build/device | APK Debug manual crash; gunakan `-t:Run` atau Release publish. |
| Tidak ada automated test | QA | Regresi hanya tertangkap lewat QA manual. |
| Fallback katalog lokal | Data | Harga/kategori fallback bisa berbeda dari data API. |

## 11. Riwayat perubahan

| Tanggal | Perubahan | Referensi |
|---|---|---|
| 2026-11-19 | Integrasi awal: config `.env`, ApiClient, TokenStore, HTTP auth/catalog/schedule/booking, DI, wiring customer flow, status `Pending`, fallback katalog, perbaikan teks UI. | Dokumen ini + [qa-test.md](./qa-test.md) |
| 2026-11-19 | Verifikasi API production (`/health`, login owner, services, hairstyles, availability) dan APK Release di perangkat Android. | [qa-test.md](./qa-test.md) |
