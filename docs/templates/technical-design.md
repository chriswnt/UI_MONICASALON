# Technical Design — <Nama Fitur>

> Status: Draft
> Terakhir diperbarui: YYYY-MM-DD
> Slug: `<slug>`
> Halaman/flow: `<halaman atau alur yang dicakup>`
> QA: [qa-test.md](./qa-test.md)
> Known issues: [../known-issues.md](../known-issues.md)
> <!-- Saat disalin ke docs/features/<slug>/, ubah link known issues menjadi ../../known-issues.md. -->

<!--
Panduan singkat:
- Isi dengan fakta dari kode, bukan asumsi atau rencana yang belum diimplementasikan.
- Sinkronkan dokumen ini setelah implementasi selesai; status Implemented hanya kalau QA sudah dijalankan.
- Hapus komentar panduan setelah dokumen diisi.
-->

## 1. Ringkasan & tujuan

<!-- 2–4 kalimat: masalah yang dipecahkan, siapa penggunanya, hasil yang diharapkan. -->

## 2. Scope

### Termasuk

- <alur/kemampuan yang dicakup>

### Tidak termasuk

- <hal yang sengaja tidak dikerjakan>

## 3. User flow & navigasi

| Langkah | Aksi user | Hasil | Route/Shell |
|---|---|---|---|
| 1 | <aksi> | <hasil> | `<route>` |

Kondisi masuk/keluar:

- Masuk: <prasyarat, role, state>
- Keluar: <navigasi, reset state>

## 4. Arsitektur & file terkait

| Layer | File | Peran |
|---|---|---|
| View | `Views/<...>.xaml` | <peran> |
| ViewModel | `ViewModels/<...>.cs` | <peran> |
| Service/API | `Services/Api/<...>` | <peran> |
| Model/State | `<...>` | <peran> |
| Control | `Controls/<...>` | <peran> |

## 5. Kontrak data & API

| Endpoint | Method | Auth | Request | Response | Dipakai di |
|---|---|---|---|---|---|
| `<path>` | GET/POST/PUT | Bearer/Anon | `<DTO>` | `<DTO>` | `<VM/page>` |

Catatan:

- DTO: `Services/Api/Dtos/<...>.cs`
- Mapping: `<mapper>`
- Mock (kalau ada): `<MockXxxApi>` + alasan pemakaian
- Format waktu: UTC dari API, dikonversi lewat `SalonTime` untuk tampilan WIB.

## 6. Desain state

### 6.1 Sumber data

<!-- API / mock / session statis / cache / input user. Jelaskan mana yang menjadi sumber kebenaran. -->

### 6.2 Kepemilikan state

| State | Pemilik | Lifetime | Reset/invalidasi |
|---|---|---|---|
| <state> | <VM/session/service> | <sesi/halaman/app> | <trigger reset> |

### 6.3 UI state

| State | Pemicu | Tampilan | Aksi user |
|---|---|---|---|
| Loading | <pemicu> | <spinner/skeleton/banner> | <aksi> |
| Empty | <pemicu> | <empty state + CTA> | <aksi> |
| Error | <pemicu> | <banner pesan> | <aksi> |
| Success | <pemicu> | <data/konfirmasi> | <aksi> |
| Disabled | <pemicu> | <kontrol nonaktif> | <aksi> |

### 6.4 Perilaku penting

- Buka ulang halaman / back: <perilaku>
- Submit ganda: <pencegahan>
- Timeout/offline: <perilaku>
- 401/refresh token: <perilaku>
- Cache: <kapan invalidasi>
- App restart: <apa yang persist, apa yang hilang>

## 7. Aturan bisnis & validasi

| Aturan | Detail | Sumber |
|---|---|---|
| <aturan> | <detail> | <file/endpoint> |

## 8. Edge case & failure handling

| Kondisi | Perilaku yang diharapkan | Catatan |
|---|---|---|
| <kondisi> | <perilaku> | <catatan> |

## 9. Keputusan teknis

| Keputusan | Alasan | Alternatif yang ditolak |
|---|---|---|
| <keputusan> | <alasan> | <alternatif> |

## 10. Dependensi & risiko

| Item | Jenis | Dampak/risiko |
|---|---|---|
| <API/package/platform> | <jenis> | <dampak> |

## 11. Riwayat perubahan

| Tanggal | Perubahan | Referensi |
|---|---|---|
| YYYY-MM-DD | <perubahan> | <commit/PR/dokumen> |
