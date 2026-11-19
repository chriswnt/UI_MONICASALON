# AGENTS.md

**Repo:** Monica Salon — aplikasi .NET MAUI 10 untuk customer, admin, dan kasir.
**Fokus aturan ini:** dokumentasi. Setiap fitur yang dikerjakan wajib meninggalkan dokumentasi yang sinkron di `docs/`.

> Baca file ini sebelum mengubah kode. Aturan ini berlaku untuk AI agent dan kontributor manusia.
> Kalau dokumentasi lama bertentangan dengan kode, **kode adalah sumber kebenaran** — perbaiki dokumentasinya di task yang sama, jangan mengubah kode agar cocok dengan dokumen lama tanpa alasan.

---

## 1. Profil proyek

| Aspek | Keterangan |
|---|---|
| Framework | .NET MAUI 10 (`net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst`, `net10.0-windows10.0.19041.0`) |
| Bahasa | C# dengan nullable enabled dan implicit usings |
| UI | UraniumUI 3.0 (`UraniumUI`, `UraniumUI.Material`, Material Symbols) + InputKit.Maui |
| MVVM | CommunityToolkit.Mvvm 8.4.2 (`[ObservableProperty]`, `[RelayCommand]`) |
| XAML | `MauiXamlInflator=SourceGen` |
| Data | HTTP API backend lewat `Services/Api/`; sebagian fitur masih memakai mock (mis. feedback) |
| Target demo | Windows; Android juga didukung. iOS/MacCatalyst hanya bisa di-build di Mac |
| Bahasa UI | Indonesia. Identifier kode, endpoint, dan nama class tetap bahasa Inggris |
| Test | Belum ada automated test project; verifikasi dilakukan manual lewat `qa-test.md` |

### Peta repo

```
UAS_APP_MONICASALON/
├── AppShell.xaml(.cs)        # Shell root + registrasi route
├── MauiProgram.cs            # DI, UraniumUI, font, HttpClient, API services
├── Controls/                 # Komponen UI bersama (customer + Admin/)
├── Models/                   # Model UI + Admin/ (store & data mock admin/kasir)
├── Services/                 # Session, cache, katalog, formatter
│   └── Api/                  # ApiClient, TokenStore, Http*Api, DTO
├── ViewModels/               # MVVM (customer + Admin/)
├── Views/                    # Halaman XAML (customer + Admin/)
├── Resources/Styles/         # Colors, Styles, AdminStyles
└── prd_monica_salon_phase_1.md
```

| Dokumen | Fungsi |
|---|---|
| `README.md` | Gambaran produk, cara run, catatan UI. Bisa tertinggal dari kode — sinkronkan kalau menyentuh flow/fitur/dependency |
| `prd_monica_salon_phase_1.md` | PRD Phase 1, bersifat historis. Jangan dianggap dokumentasi kondisi terkini |
| `docs/` | Dokumentasi per fitur + known issues (lihat bagian 3) |
| `AGENTS.md` | Aturan kerja dan standar dokumentasi ini |

---

## 2. Perintah penting

Jalankan dari root repo:

```powershell
# Restore dependency
dotnet restore

# Build target Windows (target demo utama)
dotnet build UAS_APP_MONICASALON/UAS_APP_MONICASALON.csproj -f net10.0-windows10.0.19041.0

# Run di Windows
dotnet build UAS_APP_MONICASALON/UAS_APP_MONICASALON.csproj -t:Run -f net10.0-windows10.0.19041.0

# Build Android
dotnet build UAS_APP_MONICASALON/UAS_APP_MONICASALON.csproj -f net10.0-android
```

- Tidak ada `dotnet test` di repo ini. Jangan menulis "tests passed" kalau yang dilakukan hanya build atau QA manual.
- Kalau menambah test project, update `AGENTS.md`, `README.md`, dan template QA pada task yang sama.
- iOS/MacCatalyst butuh Mac. Kegagalan build target itu di Windows bukan bug aplikasi.

---

## 3. Aturan dokumentasi (WAJIB)

### 3.1 Prinsip

1. **Dokumentasi adalah bagian dari deliverable**, bukan pekerjaan setelahnya.
2. **Satu fitur = satu folder** di `docs/features/<slug>/`, berisi tepat dua file: `technical-design.md` dan `qa-test.md`.
3. **Jangan membuat dokumen per halaman.** Satu alur fitur (mis. booking wizard) mencakup beberapa halaman dalam satu folder.
4. **Tulis fakta dari kode.** Sebelum menulis endpoint, state, atau file, baca implementasinya. Jangan mengarang perilaku.
5. **Dokumentasi berbahasa Indonesia.** Nama file, class, endpoint, dan istilah teknis tetap apa adanya.
6. **Tanggal memakai format `YYYY-MM-DD`.** Setiap dokumen wajib punya status dan tanggal terakhir diperbarui.
7. **Gunakan link relatif.** Jangan copy-paste isi dokumen lain; ringkas lalu link.
8. **Jangan pernah menulis secret** (token, password, API key, data pribadi) ke dokumentasi.

### 3.2 Kapan wajib membuat atau memperbarui dokumen

| Jenis pekerjaan | `technical-design.md` | `qa-test.md` | `known-issues.md` | `docs/README.md` | `README.md` root |
|---|---|---|---|---|---|
| Fitur baru / alur baru | Buat | Buat | Tambah kalau ada batasan | Tambah/ubah baris index | Update kalau flow/fitur/dependency berubah |
| Perubahan perilaku, state, navigasi, API, atau aturan bisnis | Update | Tambah/ubah test case | Update | Update status | Update kalau terlihat user |
| Bugfix | Update kalau logic/state berubah | Tambah test regresi + hasil | Tutup issue terkait atau tambah baru | — | — |
| Refactor tanpa perubahan perilaku | Update daftar file/arsitektur | — | Tambah kalau menimbulkan risiko | — | — |
| Typo/copy/style saja | — | Opsional | — | — | — |

Aturan tambahan:

- Kalau menyentuh fitur yang **belum punya dokumentasi**, buat dokumentasinya sekalian (kecuali perubahan benar-benar trivial seperti typo).
- Kalau fitur dihapus atau diganti, ubah status dokumen menjadi `Deprecated` dan jelaskan penggantinya. Jangan hapus riwayat.
- Jangan membuat nama file versi (`technical-design-v2.md`). Update file yang sama.
- Jangan menyembunyikan keterbatasan hanya di chat atau komentar commit. Semua keterbatasan yang diketahui wajib masuk `docs/known-issues.md`.

### 3.3 Struktur dokumentasi

```
docs/
├── README.md                     # index fitur, status, dan backlog dokumentasi
├── known-issues.md               # satu-satunya daftar fitur belum sempurna
├── templates/
│   ├── technical-design.md
│   └── qa-test.md
└── features/
    ├── README.md
    └── <slug>/
        ├── technical-design.md
        └── qa-test.md
```

**Aturan slug:**

- lowercase kebab-case, bahasa Inggris, stabil, dan berbasis kemampuan user — bukan nama page.
- Contoh: `auth-login`, `catalog-browse`, `booking-wizard`, `booking-history`, `feedback`, `style-book`, `admin-dashboard`, `admin-bookings`, `cashier-checkout`, `transaction-history`.
- Jangan rename folder tanpa memperbarui semua link dan index.

**Setiap dokumen wajib punya header metadata:**

```markdown
> Status: Draft | In Progress | Implemented | Needs Work | Deprecated
> Terakhir diperbarui: YYYY-MM-DD
> QA: [qa-test.md](./qa-test.md)
> Known issues: [../../known-issues.md](../../known-issues.md)
```

### 3.4 Alur kerja per fitur

1. **Baca konteks:** `README.md`, dokumen fitur terkait, `docs/known-issues.md`, dan kode yang relevan.
2. **Tentukan slug** dan buat/update `technical-design.md` minimal sampai bagian scope + desain state (status `Draft`/`In Progress`).
3. **Implementasi** fitur.
4. **Build** target yang terdampak. Perbaiki error sebelum lanjut.
5. **Jalankan QA** mengikuti `qa-test.md`. Isi status, bukti, dan tanggal.
6. **Finalisasi desain:** sinkronkan `technical-design.md` dengan implementasi final (file, endpoint, state, edge case, keputusan).
7. **Update known issues:** tutup yang sudah beres, tambah yang baru. Fitur yang belum sempurna wajib punya entri.
8. **Update index:** `docs/README.md`; update `README.md` root kalau flow/fitur/dependency berubah.
9. **Laporkan** ringkasan, dokumen yang diubah, hasil QA, dan known issues (format di bagian 11).

Kalau langkah 4–7 belum selesai, task **belum boleh** dinyatakan selesai.

### 3.5 Status yang dipakai

| Jenis | Nilai | Arti |
|---|---|---|
| Dokumen | `Draft` | Skeleton/desain awal, implementasi belum lengkap |
| Dokumen | `In Progress` | Sedang dikerjakan |
| Dokumen | `Implemented` | Sesuai kode dan QA sudah dijalankan |
| Dokumen | `Needs Work` | Ada bagian yang belum beres atau QA gagal |
| Dokumen | `Deprecated` | Fitur dihapus/diganti |
| QA | `Not Run` | Belum dijalankan |
| QA | `Pass` | Sesuai harapan |
| QA | `Fail` | Tidak sesuai harapan; wajib punya known issue |
| QA | `Blocked` | Tidak bisa diuji; tulis alasannya |
| QA | `Skipped` | Sengaja dilewati; tulis alasannya |
| Known issue | `Belum Dikerjakan` | Diketahui, belum ada implementasi |
| Known issue | `Sebagian` | Sudah ada, tetapi belum lengkap/terbatas |
| Known issue | `Perlu Perbaikan` | Ada bug atau kualitas belum memadai |
| Known issue | `Diblokir` | Menunggu pihak lain/dependency |
| Known issue | `Selesai` | Sudah diperbaiki; pindahkan ke riwayat selesai |
| Severity | `P0 Blocker` | Menghalangi demo/pemakaian utama |
| Severity | `P1 High` | Alur utama terganggu, tetapi masih ada workaround terbatas |
| Severity | `P2 Medium` | Mengganggu, tidak memblokir demo |
| Severity | `P3 Low` | Kosmetik/keterbatasan kecil |

---

## 4. Isi wajib `technical-design.md`

Gunakan template di [`docs/templates/technical-design.md`](docs/templates/technical-design.md). Bagian yang wajib ada:

1. **Ringkasan & tujuan** — masalah yang dipecahkan dan hasil yang diharapkan.
2. **Scope** — apa yang termasuk dan apa yang sengaja tidak termasuk.
3. **User flow & navigasi** — langkah user, route Shell, dan kondisi masuk/keluar.
4. **Arsitektur & file terkait** — tabel layer → file → peran.
5. **Kontrak data & API** — endpoint, method, auth, request, response, DTO/mapping. Kalau memakai mock, tulis mock-nya dan alasannya.
6. **Desain state** — bagian paling penting; lihat 4.1.
7. **Aturan bisnis & validasi** — aturan harga, slot, status booking, promo, dan sebagainya.
8. **Edge case & failure handling** — kondisi gagal dan perilaku yang diharapkan.
9. **Keputusan teknis** — keputusan, alasan, dan alternatif yang ditolak.
10. **Dependensi & risiko** — API, package, platform, dan risiko yang diketahui.
11. **Riwayat perubahan** — tanggal, perubahan, commit/PR jika ada.

### 4.1 Standar desain state

Bagian desain state harus menjawab eksplisit:

- Dari mana data berasal: HTTP API, mock, session statis, cache, atau input user?
- Siapa pemilik state dan berapa lama hidupnya? Ingat: **Shell membuat instance halaman baru setiap navigasi**, jadi state lintas halaman tidak boleh disimpan di field page/VM.
- Apa yang terjadi saat halaman dibuka ulang, di-back, atau user menekan tombol kembali?
- Bagaimana state `loading`, `empty`, `error`, `success`, dan `disabled` ditampilkan?
- Bagaimana penanganan submit ganda, timeout, offline, 401/refresh token, dan respons API yang tidak valid?
- Kapan cache di-invalidate, dan data apa yang di-refresh?
- Apa yang tetap ada setelah app restart, dan apa yang hilang?

Minimal sertakan dua tabel:

**Kepemilikan state**

| State | Pemilik | Lifetime | Reset/invalidasi |
|---|---|---|---|
| contoh | `BookingSession` | Sesi aplikasi | `ClearDraft()` setelah sukses |

**UI state**

| State | Pemicu | Tampilan | Aksi user |
|---|---|---|---|
| Loading | Request berjalan | Spinner/RefreshView | Menunggu |
| Empty | Respons kosong | Empty state + CTA | Ulangi/cari lain |
| Error | `ApiException` | Banner pesan `ex.Message` | Coba lagi |
| Success | Respons sukses | Data + konfirmasi | Lanjut |

State yang sudah ada di proyek dan wajib dipertimbangkan:

| State | Lokasi | Sifat |
|---|---|---|
| `UserSession` | `Services/UserSession.cs` | Static, in-memory, identitas customer/staff |
| `TokenStore` | `Services/Api/TokenStore.cs` | Singleton; token di SecureStorage, fallback Preferences |
| `CatalogCache` | `Services/Api/CatalogCache.cs` | Singleton; cache katalog selama sesi, punya `Invalidate()` |
| `BookingSession` | `Services/BookingSession.cs` | Static; draft booking lintas halaman |
| `CashierSession` | `Models/Admin/CashierSession.cs` | Singleton; cart, riwayat, dan state pembayaran |
| `AdminBookingStore` | `Models/Admin/AdminBookingStore.cs` | Static; data booking admin, in-memory |
| `Preferences` | Login | Identifier "Ingat saya"; password tidak pernah disimpan |

---

## 5. Isi wajib `qa-test.md`

Gunakan template di [`docs/templates/qa-test.md`](docs/templates/qa-test.md). Wajib memuat:

1. **Scope pengujian** — fitur/halaman yang diuji dan yang tidak.
2. **Environment & prasyarat** — target build, akun/role, data awal, koneksi API.
3. **Test data** — data yang dipakai (jangan data sensitif).
4. **Test matrix** — tabel test case.
5. **Regression checklist** — alur lama yang berpotensi terdampak.
6. **Accessibility checklist** — untuk kontrol baru atau yang diubah.
7. **Catatan platform** — perbedaan Windows/Android bila relevan.
8. **Known failures & blocker** — link ke `known-issues.md`.
9. **Sign-off** — build/commit, tanggal, penguji, hasil akhir.

**Format ID test case:** `TC-<slug>-NNN`, contoh `TC-booking-wizard-003`.

**Kolom test matrix:**

| ID | Tipe | Skenario | Prasyarat | Langkah | Hasil yang diharapkan | Status | Bukti |
|---|---|---|---|---|---|---|---|

**Tipe:** `Functional`, `Validation`, `Error`, `Regression`, `Accessibility`, `Platform`, `State`.

**Cakupan minimum untuk setiap fitur:**

- happy path end-to-end;
- validasi input kosong/salah;
- empty state;
- error state (API gagal/timeout/offline/401);
- submit ganda dan navigasi kembali;
- data yang seharusnya persist setelah app restart (kalau relevan);
- regresi alur yang berbagi state/navigasi;
- aksesibilitas untuk kontrol interaktif baru;
- Windows wajib; Android kalau perubahan menyentuh platform/device.

**Aturan status:**

- `Fail` wajib disertai entri di `known-issues.md` atau alasan eksplisit kenapa bukan issue.
- `Blocked`/`Skipped` wajib menulis alasan.
- Jangan menandai `Pass` tanpa benar-benar menjalankan langkahnya.
- Bukti minimal: build/target yang dipakai, langkah yang dijalankan, dan hasil. Screenshot opsional disimpan di `docs/features/<slug>/evidence/` tanpa data sensitif.

---

## 6. `docs/known-issues.md`

Satu file untuk seluruh proyek. Ini **satu-satunya tempat** mencatat fitur yang belum sempurna.

Aturan:

- Setiap fitur yang belum lengkap, masih mock, placeholder, atau QA-nya gagal **wajib** punya entri.
- Satu baris = satu masalah. Jangan menggabungkan beberapa masalah berbeda.
- Sertakan dampak user, workaround, langkah berikutnya, dan link dokumen fitur.
- Update tanggal setiap kali status berubah.
- Kalau sudah selesai, pindahkan ke tabel **Selesai** dengan tanggal dan link commit/dokumen. Jangan hapus riwayat.
- Jangan membuat file known issues terpisah per fitur.

Kolom: `ID`, `Fitur`, `Status`, `Severity`, `Deskripsi`, `Dampak`, `Workaround`, `Langkah berikutnya`, `Docs`, `Terakhir diperbarui`.

---

## 7. Konvensi implementasi

### 7.1 MVVM

- Logic ada di ViewModel. Code-behind hanya untuk lifecycle, navigasi, dan interaksi UI yang tidak bisa di-bind.
- Gunakan `[ObservableProperty]` dengan partial property dan `[RelayCommand]` sesuai pola file terdekat.
- Nama method async berakhiran `Async`; field private memakai `_camelCase`.
- Ikuti gaya namespace file yang diedit (sebagian file memakai file-scoped namespace, sebagian block-scoped).

### 7.2 API & state

- Semua HTTP lewat `ApiClient`; jangan membuat `HttpClient` baru di page/VM.
- Interface API (`IAuthApi`, `ICatalogApi`, dst.) di-resolve lewat `AppServices`; registrasi ada di `MauiProgram`.
- DTO ada di `Services/Api/Dtos/`; mapping ke model UI lewat `CatalogMapper`/mapper terkait. Jangan bind DTO langsung ke XAML.
- `ApiClient` sudah menangani bearer token, refresh sekali saat 401, timeout, dan pesan `ProblemDetails`. Jangan menduplikasi logic refresh di VM.
- Simpan state lintas halaman di session/service yang sesuai (bagian 4.1), bukan di field page.
- Tangani error di batas UI: tampilkan `ApiException.Message`, jangan menelan exception tanpa jejak.

### 7.3 XAML & UI

- Root halaman memakai `<uranium:UraniumContentPage>`; kontrol mengikuti pola halaman terdekat.
- Jangan memakai `uranium:CalendarView` di halaman yang ditutup dengan `GoToAsync("..")` — menyebabkan crash di Windows. Gunakan `DatePicker`.
- Tombol aksi penting memakai `Button` asli (bukan `ButtonView`/gesture) agar bisa diakses pembaca layar.
- Warna diambil dari `Resources/Styles/Colors.xaml`/`AdminStyles.xaml`; jangan hardcode kalau token sudah ada.
- Font customer `PlusJakartaSans*`; font admin `OpenSans*`.
- Format uang: `Models/Admin/Rupiah.cs` (admin/kasir) atau `Services/CurrencyFormatter.cs` (customer). Jangan format manual.
- Waktu API dalam UTC; konversi ke WIB memakai `Services/Api/SalonTime.cs`.
- Teks UI berbahasa Indonesia; identifier tetap bahasa Inggris.

### 7.4 Aksesibilitas

Setiap kontrol interaktif baru wajib:

- punya `SemanticProperties.Description` dan `SemanticProperties.Hint` bila perlu;
- memakai `SemanticProperties.HeadingLevel` untuk judul halaman (Level 1) dan judul seksi/kartu (Level 2);
- menandai label dekoratif di dalam kartu dengan `AutomationProperties.IsInAccessibleTree="False"` agar satu item = satu swipe;
- mengumumkan perubahan penting dengan `SemanticScreenReader.Default.Announce`;
- menyembunyikan konten bottom sheet yang tertutup dari pohon aksesibilitas.

### 7.5 Dependency

Jangan menambah package baru tanpa kebutuhan jelas. Kalau menambah:

- update `README.md` (tabel dependency + catatan build bila perlu);
- catat alasan di `technical-design.md` bagian keputusan teknis;
- jalankan build target Windows dan Android bila memungkinkan.

---

## 8. Jebakan teknis yang sudah diketahui

- **`uranium:CalendarView` + `GoToAsync("..")` = crash di Windows.** Semua halaman yang bisa di-back memakai `DatePicker`/dropdown.
- **`Controls/ButtonView.cs` adalah shim** dari `UraniumUI.Material.Controls.ButtonView`, didaftarkan lewat `AssemblyInfo.cs` dan handler di `MauiProgram.cs`. Jangan hapus tanpa mengganti seluruh XAML.
- **`Platforms/Windows/SafeStatefulButtonHandler.cs`** menggantikan handler Uranium untuk `Button` di Windows. Jangan hapus; handler asli crash saat halaman di-pop.
- **Halaman baru dibuat setiap navigasi.** Jangan menyimpan draft/keranjang/state lintas halaman di field page atau VM.
- **`.env` di-embed saat build** lewat `UAS_APP_MONICASALON.csproj`. Setelah mengubah `.env`, rebuild. Environment variable menang atas isi file. `.env` di-ignore Git; ubah `.env.example` untuk default yang dibagikan.
- **Android memakai `API_BASE_URL_ANDROID`**, platform lain memakai `API_BASE_URL`.
- **Feedback masih mock** (`MockFeedbackApi`, didaftarkan di `MauiProgram`): data hilang saat app restart. Jangan mengklaim feedback persisten.
- **Data admin/kasir in-memory** (`AdminBookingStore`, `CashierSession`): booking/transaksi baru hilang saat app restart.
- **Jangan commit** `bin/`, `obj/`, `.env`, token, atau data pribadi.

---

## 9. Definition of Done

Sebuah task baru boleh dinyatakan selesai kalau:

- [ ] Build target yang terdampak sukses (minimal Windows; Android bila relevan).
- [ ] Fitur berjalan sesuai scope di `technical-design.md`.
- [ ] `docs/features/<slug>/technical-design.md` sudah sinkron dengan kode final.
- [ ] `docs/features/<slug>/qa-test.md` terisi; tidak ada `Fail` tanpa known issue, dan `Blocked`/`Skipped` punya alasan.
- [ ] `docs/known-issues.md` sudah diperbarui (issue baru ditambah, yang selesai dipindah ke riwayat).
- [ ] `docs/README.md` index sudah diperbarui.
- [ ] `README.md` root diperbarui kalau flow/fitur/dependency berubah.
- [ ] Link dokumentasi valid dan tanggal sudah diperbarui.
- [ ] Tidak ada secret atau build artifact yang ikut berubah.

---

## 10. Git & keamanan

- Jangan membuat commit kecuali diminta.
- Sertakan perubahan dokumentasi dalam commit/PR yang sama dengan kode fitur.
- Jangan pernah commit `.env`, token, password, atau data pribadi.
- Jangan menghapus file dokumentasi lama tanpa alasan; ubah statusnya menjadi `Deprecated`.
- Jangan memakai `git reset --hard` atau force-push untuk menyelesaikan konflik dokumentasi.
- Ikuti gaya pesan commit yang sudah ada di repo; sebut nama fitur/slug bila relevan.

---

## 11. Format laporan akhir

Setelah menyelesaikan task, laporkan singkat:

```markdown
## Ringkasan
<apa yang dikerjakan dan hasilnya>

## Dokumentasi
- `docs/features/<slug>/technical-design.md` — <status>
- `docs/features/<slug>/qa-test.md` — <hasil>
- `docs/known-issues.md` — <issue yang ditambah/ditutup>

## QA
- Build: <target + hasil>
- Test: <jumlah pass/fail/blocked> (detail di qa-test.md)

## Catatan
<Keterbatasan, risiko, atau pekerjaan lanjutan>
```

Kalau ada yang belum selesai, katakan terus terang dan pastikan sudah tercatat di `docs/known-issues.md`.
