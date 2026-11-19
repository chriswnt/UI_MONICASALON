# QA Test — Integrasi API Client

> Status QA: In Progress
> Terakhir diuji: 2026-11-19
> Build/commit: `dotnet build -f net10.0-windows10.0.19041.0`; `dotnet build -f net10.0-android`; `dotnet publish -f net10.0-android -c Release`
> Platform: Windows + Android
> Technical design: [technical-design.md](./technical-design.md)
> Known issues: [../../known-issues.md](../../known-issues.md)

## 1. Scope pengujian

Diuji:

- Konfigurasi API dari `.env` yang di-embed.
- Build Windows dan Android.
- Publish + install + launch APK Release di perangkat Android via USB.
- Startup aplikasi sampai halaman login tampil, tanpa crash.
- Endpoint production: `/health`, `/auth/login`, `/services`, `/hairstyles`, `/bookings/availability`.
- Verifikasi teks footer login terbaru.

Tidak diuji:

- Login/register customer lewat UI aplikasi.
- Alur Home → Katalog → Buku Gaya → Jadwal → Konfirmasi → Booking sukses lewat UI.
- Riwayat, detail, dan cancel booking lewat UI.
- Refresh token 401, offline, timeout, dan fallback katalog lewat UI.
- Feedback, admin dashboard, admin booking, kasir, dan transaksi (masih mock).
- Accessibility dengan TalkBack.
- Login Staff/Owner lewat UI aplikasi.

Alasan: pengujian UI end-to-end membutuhkan akun customer dan interaksi manual di perangkat; sesi ini fokus pada lapisan API, build, dan startup. Endpoint auth/katalog sudah diverifikasi langsung lewat HTTP.

## 2. Environment & prasyarat

| Item | Nilai |
|---|---|
| Target build Windows | `net10.0-windows10.0.19041.0` |
| Target build Android | `net10.0-android` |
| API base URL | `https://monica-salon-be.onrender.com` (dari `.env`) |
| Akun API | `owner / Admin123!`, `staff / Staff123!` (seed backend) |
| Perangkat Android | USB, `adb devices` = `26aa927d device` |
| APK yang dipakai | `bin/Release/net10.0-android/publish/com.companyname.uas_app_monicasalon-Signed.apk` |
| Tanggal data uji | `2026-11-19` |
| Koneksi | Internet perangkat + PC untuk `adb` |

## 3. Test data

| Data | Nilai | Catatan |
|---|---|---|
| Akun owner | `owner` / `Admin123!` | Seed backend; dipakai untuk verifikasi HTTP login. |
| Akun staff | `staff` / `Staff123!` | Seed backend; belum diuji. |
| Tanggal availability | `2026-11-19` | Response 24 slot, kapasitas 3. |
| Layanan API | 9 item | `Potong Rambut`, `Cuci Blow`, `Cuci Catok`, dll. |
| Hairstyle API | 8 item | `Bob Classic` s/d `Mocha Lob`. |
| Customer uji | Tidak dibuat | Menghindari data produksi baru. |
| Token/password | Hanya di env/seed | Tidak ditulis di dokumen ini. |

## 4. Test matrix

| ID | Tipe | Skenario | Prasyarat | Langkah | Hasil yang diharapkan | Status | Bukti |
|---|---|---|---|---|---|---|---|
| TC-api-integration-001 | Functional | Build Windows | .NET 10 + MAUI workload | `dotnet build -f net10.0-windows10.0.19041.0` | Build sukses, 0 warning/error | Pass | Build sukses 0 warning, 0 error. |
| TC-api-integration-002 | Functional | Build Android Debug | Android workload + SDK | `dotnet build -f net10.0-android` | Build sukses, 0 warning/error | Pass | Build sukses 0 warning, 0 error. |
| TC-api-integration-003 | Functional | Publish Android Release | Android workload + SDK | `dotnet publish -f net10.0-android -c Release` | APK Release berhasil dibuat | Pass | APK `bin/Release/net10.0-android/publish/...-Signed.apk`. |
| TC-api-integration-004 | Functional | APK Release berisi assemblies | APK Release ada | `tar -tf ...-Signed.apk \| findstr UAS_APP_MONICASALON.dll.so` | Ada `libaot-UAS_APP_MONICASALON.dll.so` | Pass | Entry `lib/arm64-v8a/libaot-UAS_APP_MONICASALON.dll.so` ditemukan. |
| TC-api-integration-005 | Functional | Install APK Release | Device `26aa927d` terhubung | `adb uninstall`, lalu `adb install -r ...Signed.apk` | Install sukses | Pass | `Success` dari `adb install`. |
| TC-api-integration-006 | Functional | Launch aplikasi | APK ter-install | `adb shell am start -W -n ...MainActivity` | Aplikasi start, proses hidup | Pass | `Status: ok`, `LaunchState: COLD`, PID aktif. |
| TC-api-integration-007 | Functional | Startup tidak crash | APK Release ter-install | Launch, tunggu 8 detik, cek `pidof` + logcat | PID masih hidup, tidak ada `No assemblies found`/FATAL | Pass | PID hidup (contoh 23024); logcat tidak memuat abort Fast Deployment. |
| TC-api-integration-008 | Functional | Halaman login tampil | Aplikasi start | Screenshot + `uiautomator dump` | Field email, password, tombol Masuk, footer terlihat | Pass | [login-screen.png](./evidence/login-screen.png). |
| TC-api-integration-009 | Functional | Footer login terbaru | Aplikasi start | `uiautomator dump` cari teks footer | Teks `Customer baru? Daftar dulu.` | Pass | UI dump memuat teks tersebut; teks admin sudah tidak ada. |
| TC-api-integration-010 | Functional | API health | Internet | `GET /health` | `status: healthy` | Pass | Response `{"status":"healthy",...}`. |
| TC-api-integration-011 | Functional | API login owner | Internet | `POST /api/v1/auth/login` body owner | 200 + `AuthResponse`, role `Owner` | Pass | Response user `{id:1, name:"owner", role:"Owner"}`. |
| TC-api-integration-012 | Functional | API services | Internet + token | `GET /api/v1/services` | Array 9 layanan dengan id/name/category/price/isActive | Pass | 9 item; contoh `Potong Rambut` 50000. |
| TC-api-integration-013 | Functional | API hairstyles | Internet + token | `GET /api/v1/hairstyles` | Array 8 hairstyle | Pass | 8 item; `Bob Classic` s/d `Mocha Lob`. |
| TC-api-integration-014 | Functional | API availability | Internet + token | `GET /api/v1/bookings/availability?date=2026-11-19` | `date` + slot 09:00–20:30, capacity 3 | Pass | 24 slot; `booked/capacity/remaining` sesuai. |
| TC-api-integration-015 | Functional | Login owner lewat UI | Akun owner | Isi email/username + password, tap Masuk | Masuk dashboard admin | Not Run | Belum dijalankan; verifikasi login baru lewat HTTP. |
| TC-api-integration-016 | Functional | Register customer lewat UI | Data customer baru | Isi form register, setujui S&K, submit | Akun dibuat, masuk Home | Not Run | Belum dijalankan; menghindari pembuatan data produksi. |
| TC-api-integration-017 | Functional | Home memuat katalog API | Login customer | Buka Home | Layanan favorit dari API | Not Run | Butuh login customer. |
| TC-api-integration-018 | Functional | Katalog + filter API | Login customer | Buka Katalog, cari/filter | Data API tampil, kategori dinamis | Not Run | Butuh login customer. |
| TC-api-integration-019 | Functional | Buku Gaya API | Login customer | Buka Buku Gaya | Hairstyle API tampil, filter kategori | Not Run | Butuh login customer. |
| TC-api-integration-020 | Functional | Jadwal API | Login customer | Buka Jadwal, pilih tanggal | Tanggal & slot dari availability API | Not Run | Butuh login customer. |
| TC-api-integration-021 | Functional | Create booking | Login customer + slot tersedia | Pilih layanan, tanggal, slot, konfirmasi | Booking `Pending`, tersimpan di API | Not Run | Butuh login customer. |
| TC-api-integration-022 | Functional | Riwayat & detail booking | Punya booking | Buka Riwayat, buka detail | Data booking dari `GET /bookings/mine`/`{id}` | Not Run | Butuh login customer. |
| TC-api-integration-023 | Functional | Cancel booking Pending | Booking `Pending` | Buka detail, tap Batalkan | Status menjadi `Cancelled` di API | Not Run | Butuh login customer. |
| TC-api-integration-024 | Validation | Login kosong/salah | Halaman login | Submit kosong, lalu kredensial salah | Pesan validasi/error, tidak masuk | Not Run | Belum dijalankan. |
| TC-api-integration-025 | Validation | Submit booking tanpa layanan/tanggal/slot | Halaman jadwal/konfirmasi | Kosongkan salah satu field, submit | Pesan validasi, tidak mengirim request | Not Run | Belum dijalankan. |
| TC-api-integration-026 | Error | Offline/timeout | Jaringan dimatikan | Buka Home/Katalog/Jadwal | Fallback katalog atau pesan error sesuai halaman | Not Run | Belum dijalankan. |
| TC-api-integration-027 | Error | 401 + refresh token | Token mendekati/melewati expiry | Buka halaman yang memanggil API | Refresh otomatis, request berhasil | Not Run | Belum dijalankan; logic ada di `ApiClient`. |
| TC-api-integration-028 | State | Back/reopen halaman | Login customer | Back dari detail/jadwal, buka ulang | Draft/state sesuai desain, tidak ada data nyangkut | Not Run | Belum dijalankan. |
| TC-api-integration-029 | State | App restart | Sudah login | Tutup paksa, buka lagi | Kembali ke login (auto-login belum ada) | Not Run | Diharapkan mengikuti KI-013. |
| TC-api-integration-030 | Regression | Tautan admin tanpa role | Belum login Staff/Owner | Tap "Masuk sebagai Admin" | Alert, tidak masuk dashboard | Not Run | Belum dijalankan lewat UI. |
| TC-api-integration-031 | Accessibility | Kontrol login | TalkBack aktif | Navigasi field/tombol | Deskripsi/hint terbaca, urutan fokus wajar | Not Run | Belum diuji dengan TalkBack. |
| TC-api-integration-032 | Regression | Build Windows startup | Build Windows sukses | Jalankan exe, tunggu 10 detik | Proses tetap hidup, tidak crash | Pass | Smoke test proses hidup; tidak ada output crash. |
| TC-api-integration-033 | Platform | Fast Deployment Debug | APK Debug manual | Install APK hasil `dotnet build`, launch | App crash "No assemblies found" | Pass | Reproduksi; workaround Release publish. Lihat KI-017. |
| TC-api-integration-034 | Skipped | Section add-on | Halaman jadwal | Lihat section Tambahan | Section tidak tampil | Skipped | Backend belum punya endpoint add-on; `IsVisible="False"`. Lihat KI-010. |
| TC-api-integration-035 | Skipped | Feedback | Booking selesai | Kirim ulasan | Ulasan tersimpan di backend | Skipped | Backend belum punya endpoint feedback; masih mock. Lihat KI-001. |
| TC-api-integration-036 | Skipped | Admin/kasir API | Login Staff/Owner | Buka dashboard/kasir | Data dari API | Skipped | UI masih mock; HTTP service sudah ada. Lihat KI-007/KI-008/KI-009. |

Status yang dipakai: `Not Run`, `Pass`, `Fail`, `Blocked`, `Skipped`.

## 5. Regression checklist

- [x] Build Windows `net10.0-windows10.0.19041.0` sukses.
- [x] Build Android `net10.0-android` sukses.
- [x] Publish Release Android sukses dan APK ter-install.
- [x] Aplikasi Release start dan halaman login tampil.
- [x] API production health/login/services/hairstyles/availability merespons.
- [ ] Login/register customer lewat UI.
- [ ] Home/Katalog/Buku Gaya memuat data API.
- [ ] Jadwal + create booking + riwayat + cancel.
- [ ] Fallback katalog saat API offline.
- [ ] Refresh token 401.
- [ ] Admin dashboard/booking/kasir (masih mock).
- [ ] Feedback (masih mock).
- [ ] Accessibility TalkBack.

## 6. Accessibility checklist

- [ ] Kontrol login (email, password, tombol, tautan) punya `SemanticProperties.Description`/`Hint`.
- [ ] Judul halaman memakai `HeadingLevel` Level 1.
- [ ] Satu kartu layanan/hairstyle = satu swipe.
- [ ] Perubahan status booking diumumkan lewat `SemanticScreenReader`.
- [ ] Bottom sheet yang tertutup tidak terbaca pembaca layar.

Catatan: checklist aksesibilitas belum dijalankan pada sesi ini.

## 7. Catatan platform

| Platform | Catatan |
|---|---|
| Windows | Build dan startup sukses; alur UI API belum dijalankan di Windows. |
| Android | Release APK sukses install/launch. APK Debug manual gagal karena Fast Deployment (KI-017). Production memakai HTTPS sehingga tidak butuh `usesCleartextTraffic`. |
| iOS/MacCatalyst | Tidak diuji; build hanya bisa di Mac. |

## 8. Known failures & blocker

| Test ID | Status | Alasan | Known issue |
|---|---|---|---|
| TC-api-integration-015 s/d 031 | Not Run | Pengujian UI end-to-end butuh login customer dan interaksi manual di perangkat; sesi ini fokus API/build/startup. | — |
| TC-api-integration-029 | Not Run | Auto-login setelah restart memang belum diimplementasikan. | [KI-013](../../known-issues.md) |
| TC-api-integration-033 | Pass (reproduksi) | APK Debug manual tidak membawa assemblies Fast Deployment. | [KI-017](../../known-issues.md) |
| TC-api-integration-034 | Skipped | Backend belum punya endpoint add-on. | [KI-010](../../known-issues.md) |
| TC-api-integration-035 | Skipped | Backend belum punya endpoint feedback. | [KI-001](../../known-issues.md) |
| TC-api-integration-036 | Skipped | UI admin/kasir masih mock; HTTP service sudah tersedia. | [KI-007](../../known-issues.md), [KI-008](../../known-issues.md), [KI-009](../../known-issues.md) |

## 9. Sign-off

| Item | Nilai |
|---|---|
| Build diverifikasi | Windows, Android Debug, Android Release publish — sukses |
| Tanggal pengujian | 2026-11-19 |
| Penguji | AI agent + verifikasi perangkat pengguna (`26aa927d`) |
| Hasil akhir | In Progress |
| Catatan | Lapisan API, build, install, dan startup Android sudah verified. Alur UI customer end-to-end, refresh token, offline fallback, admin/kasir, feedback, dan accessibility belum dijalankan. |
