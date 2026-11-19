# Fitur

> Status: Implemented
> Terakhir diperbarui: 2026-11-19
> Known issues: [../known-issues.md](../known-issues.md)
> Aturan kerja: [AGENTS.md](../../AGENTS.md)

Satu folder = satu kemampuan user, bukan satu halaman. Setiap folder wajib berisi:

```
<slug>/
├── technical-design.md   # desain teknis + desain state
└── qa-test.md            # skenario QA + hasil
```

Aturan:

- Slug lowercase kebab-case, bahasa Inggris, stabil. Contoh: `booking-wizard`, `cashier-checkout`.
- Jangan membuat folder per halaman. Beberapa halaman dalam satu alur masuk ke folder yang sama.
- Jangan membuat file versi (`technical-design-v2.md`); update file yang sama.
- Screenshot bukti QA (opsional) disimpan di `<slug>/evidence/` tanpa data sensitif.
- Setelah dokumen dibuat, tambahkan barisnya ke index di [`../README.md`](../README.md).
- Keterbatasan fitur dicatat di [`../known-issues.md`](../known-issues.md), bukan di folder fitur.

Template:

- [`../templates/technical-design.md`](../templates/technical-design.md)
- [`../templates/qa-test.md`](../templates/qa-test.md)
