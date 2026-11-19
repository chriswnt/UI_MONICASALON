# QA Test — <Nama Fitur>

> Status QA: Not Run
> Terakhir diuji: YYYY-MM-DD
> Build/commit: `<target build / hash>`
> Platform: Windows / Android
> Technical design: [technical-design.md](./technical-design.md)
> Known issues: [../known-issues.md](../known-issues.md)
> <!-- Saat disalin ke docs/features/<slug>/, ubah link known issues menjadi ../../known-issues.md. -->

<!--
Panduan singkat:
- Jalankan test case satu per satu; jangan menandai Pass tanpa eksekusi.
- Fail wajib punya entri di ../../known-issues.md atau alasan eksplisit kenapa bukan issue.
- Blocked/Skipped wajib menulis alasannya di kolom Bukti.
- Hapus komentar panduan setelah dokumen diisi.
-->

## 1. Scope pengujian

Diuji:

- <fitur/halaman yang diuji>

Tidak diuji:

- <hal di luar scope + alasan>

## 2. Environment & prasyarat

| Item | Nilai |
|---|---|
| Target build | `net10.0-windows10.0.19041.0` / `net10.0-android` |
| Akun/role | <customer/staff/owner> |
| Koneksi API | <base URL / mode mock> |
| Data awal | <state yang harus disiapkan> |
| Device/emulator | <Windows / Android> |

## 3. Test data

| Data | Nilai | Catatan |
|---|---|---|
| <data> | <nilai> | <catatan> |

Jangan memakai token, password asli, atau data pribadi di dokumen ini.

## 4. Test matrix

| ID | Tipe | Skenario | Prasyarat | Langkah | Hasil yang diharapkan | Status | Bukti |
|---|---|---|---|---|---|---|---|
| TC-<slug>-001 | Functional | Happy path | <prasyarat> | 1. <langkah> 2. <langkah> | <hasil> | Not Run | <build/langkah/hasil> |
| TC-<slug>-002 | Validation | Input kosong/tidak valid | <prasyarat> | 1. <langkah> | Pesan validasi tampil, submit tidak jalan | Not Run | |
| TC-<slug>-003 | Error | API gagal/timeout/offline | <prasyarat> | 1. <langkah> | Error state tampil, user bisa retry | Not Run | |
| TC-<slug>-004 | State | Back/navigasi ulang | <prasyarat> | 1. <langkah> | State sesuai desain, tidak ada data nyangkut | Not Run | |
| TC-<slug>-005 | Regression | Alur terkait | <prasyarat> | 1. <langkah> | Alur lama tetap berjalan | Not Run | |
| TC-<slug>-006 | Accessibility | Kontrol baru | <prasyarat> | 1. <langkah> | Deskripsi/hint terbaca, urutan fokus wajar | Not Run | |

Status yang dipakai: `Not Run`, `Pass`, `Fail`, `Blocked`, `Skipped`.

## 5. Regression checklist

- [ ] <alur lama yang berbagi state/navigasi>
- [ ] <alur lain yang memakai service/API yang sama>

## 6. Accessibility checklist

- [ ] Semua kontrol interaktif baru punya `SemanticProperties.Description`/`Hint`.
- [ ] Judul halaman memakai `HeadingLevel` Level 1; judul seksi/kartu Level 2.
- [ ] Satu kartu/item = satu swipe (label dekoratif `IsInAccessibleTree="False"`).
- [ ] Perubahan penting diumumkan lewat `SemanticScreenReader.Default.Announce`.
- [ ] Konten bottom sheet tertutup tidak terbaca pembaca layar.

## 7. Catatan platform

| Platform | Catatan |
|---|---|
| Windows | <hasil/perbedaan> |
| Android | <hasil/perbedaan atau alasan tidak diuji> |

## 8. Known failures & blocker

| Test ID | Status | Alasan | Known issue |
|---|---|---|---|
| <TC-...> | Fail/Blocked/Skipped | <alasan> | [KI-XXX](../known-issues.md) <!-- ubah ke ../../known-issues.md saat disalin ke folder fitur --> |

## 9. Sign-off

| Item | Nilai |
|---|---|
| Build diverifikasi | `<command>` — <sukses/gagal> |
| Tanggal pengujian | YYYY-MM-DD |
| Penguji | <nama/agent> |
| Hasil akhir | Not Run / Pass / Fail / Blocked |
| Catatan | <catatan> |
