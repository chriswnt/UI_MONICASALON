# PRODUCT REQUIREMENTS DOCUMENT (PRD) — MONICA SALON MOBILE UI (PHASE 1)

## 1. Project Overview & Scope
- **Project Name:** Monica Salon Mobile App
- **Framework:** .NET MAUI (.NET 8 / .NET 9)
- **UI Presentation Framework:** UraniumUI (`https://enisn-projects.io/docs/en/uranium/latest`)
- **Strict Scope Boundary:** 
  Build ONLY the Customer UI for three (3) screens:
  1. `LoginPage.xaml` (Onboarding + Login Screen)
  2. `RegisterPage.xaml` (Customer Registration Screen)
  3. `HomeCustomerPage.xaml` (Customer Home Dashboard)
  *Do NOT generate backend database connections, admin screens, or checkout flows in this phase. Focus 100% on pixel-accurate UI, UraniumUI component compliance, and mock data binding.*

---

## 2. Strict Library & Dependency Rules (NO DEVIATION)
The AI Agent MUST exclusively use **UraniumUI** ecosystem packages and namespaces. Do NOT use third-party UI toolkits outside of UraniumUI/InputKit.

### 2.1 Required NuGet Packages
Run or ensure the following packages are installed in the `.csproj` file:
```bash
dotnet add package UraniumUI
dotnet add package UraniumUI.Material
dotnet add package UraniumUI.Icons.MaterialSymbols
dotnet add package CommunityToolkit.Mvvm
```
*(Note: `InputKit.Maui` is automatically included via `UraniumUI.Material` for form validations and advanced input controls).*

### 2.2 Mandatory `MauiProgram.cs` Configuration
Ensure `MauiProgram.cs` registers UraniumUI, its Material theme, and Material Symbols fonts:
```csharp
using UraniumUI;
using UraniumUI.Icons.MaterialSymbols;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("PlusJakartaSans-Regular.ttf", "PlusJakartaSansRegular");
                fonts.AddFont("PlusJakartaSans-Medium.ttf", "PlusJakartaSansMedium");
                fonts.AddFont("PlusJakartaSans-SemiBold.ttf", "PlusJakartaSansSemiBold");
                fonts.AddFont("PlusJakartaSans-Bold.ttf", "PlusJakartaSansBold");
                fonts.AddMaterialSymbolsFonts(); // Mandatory for UraniumUI Material Symbols
            });

        return builder.Build();
    }
}
```

### 2.3 Mandatory `App.xaml` Resource Dictionary
In `App.xaml`, initialize UraniumUI Material resources so all inputs, buttons, and surfaces inherit the Monica Salon Coral/Terracotta theme automatically:
```xml
<?xml version = "1.0" encoding = "UTF-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"
             xmlns:local="clr-namespace:MonicaSalon"
             x:Class="MonicaSalon.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary x:Name="appColors" Source="Resources/Styles/Colors.xaml" />
                <ResourceDictionary x:Name="appStyles" Source="Resources/Styles/Styles.xaml" />
                <material:StyleResource ColorsOverride="{x:Reference appColors}" BasedOn="{x:Reference appStyles}" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

---

## 3. Design System & Theme Specifications (Coral / Terracotta Salon Theme)

### 3.1 Color Palette (`Resources/Styles/Colors.xaml`)
Override the default MAUI/UraniumUI colors with the exact palette from the Monica Salon reference mockup:
- **Primary (`#D9534F`):** Coral Red / Terracotta (Used for primary CTA buttons, active tab icons, filter button, badges, active borders)
- **PrimaryDark (`#8B2626`):** Deep Berry / Maroon (Used for banner headings, dark accents)
- **Secondary / Surface Tint (`#FDECEC`):** Soft Blush Pink (Used for circular category icon backgrounds, soft card highlights)
- **Background (`#FAF7F5`):** Warm Alabaster / Off-White (Main page background)
- **Surface (`#FFFFFF`):** Pure White (For cards, search bar, form container, bottom navigation bar)
- **TextPrimary (`#1E1E1E`):** Charcoal Black (Headings, service titles, prices)
- **TextSecondary (`#757575`):** Warm Grey (Subtitles, duration `45 mnt`, location label, placeholders)
- **Accent / Rating Star (`#F59E0B`):** Warm Amber (Star icon in `4.8/5.0` badge)
- **Outline / Border (`#EFE6E4`):** Soft Warm Border (Subtle border for inputs and cards)

### 3.2 Typography & Spacing
- **Headings (H1/H2):** Bold (`FontAttributes="Bold"`), `22-26sp` for Auth Titles, `18sp` for Section Headers (`Layanan`, `Layanan Favorit`).
- **Body / Card Titles:** SemiBold (`14-15sp`), `#1E1E1E`.
- **Captions / Meta:** Regular (`12sp`), `#757575`.
- **Page Padding:** `20` horizontal, `16` vertical.
- **Corner Radius:**
  - Primary Buttons: `CornerRadius="24"` (Full Pill shape)
  - Banner & Cards: `CornerRadius="16"`
  - Search Bar & Form Inputs: `CornerRadius="12"`
  - Category Icons: Circular (`WidthRequest="58" HeightRequest="58" StrokeShape="RoundRectangle 29"`)

---

## 4. Strict UraniumUI Component Mapping
Do NOT use raw unstyled MAUI `<Entry>` or `<CheckBox>` when a UraniumUI Material equivalent exists:

| UI Element | Mandatory UraniumUI / XAML Control | Namespace |
| :--- | :--- | :--- |
| **Base Page** | `<uranium:UraniumContentPage>` | `xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui"` |
| **Form Container** | `<input:FormView>` | `xmlns:input="clr-namespace:InputKit.Shared.Controls;assembly=InputKit.Maui"` |
| **Text / Email / Phone Input** | `<material:TextField>` | `xmlns:material="http://schemas.enisn-projects.io/dotnet/maui/uraniumui/material"` |
| **Password Input** | `<material:TextField IsPassword="True">` with `<material:TextFieldPasswordShowHideAttachment />` | `UraniumUI.Material.Attachments` |
| **Input Validation** | `<validation:RequiredValidation />`, `<validation:MinLengthValidation />`, `<validation:DigitsOnlyValidation />`, `<validation:EmailValidation />` | `xmlns:validation="clr-namespace:InputKit.Shared.Validations;assembly=InputKit.Maui"` |
| **Checkbox (Terms)** | `<material:CheckBox>` | `UraniumUI.Material.Controls` |
| **Interactive Ripple/Click** | `<uranium:ButtonView>` or `<Button StyleClass="FilledButton">` | `UraniumUI` / `UraniumUI.Material` |
| **Icons** | `<FontImageSource FontFamily="MaterialOutlined" Glyph="{x:Static m:MaterialRegular.*}" />` | `xmlns:m="clr-namespace:UraniumUI.Icons.MaterialSymbols;assembly=UraniumUI.Icons.MaterialSymbols"` |
| **Bottom Sheet / Modal** | `<material:BottomSheetView>` (inside `UraniumContentPage.Attachments`) | `UraniumUI.Material.Attachments` |

---

## 5. Screen-by-Screen Detailed Requirements

### SCREEN 1: `LoginPage.xaml` (Onboarding + Login Screen)
**Concept:** Combines the welcoming salon visual from Screen 1 of the mockup (`Your Ultimate Salon Booking App` / `A little care, a lovely you.`) with the customer login form.
1. **Root Element:** `<uranium:UraniumContentPage>` with `BackgroundColor="#FAF7F5"`, `NavigationPage.HasNavigationBar="False"`.
2. **Top Visual / Header Section:**
   - Rounded illustration or banner container at the top (`HeightRequest="220"`, `StrokeShape="RoundRectangle 20"`, `BackgroundColor="#FDECEC"`) featuring the salon illustration or `banner_aplikasi.png`.
   - Heading: `"Your Ultimate Salon Booking App"` (`24sp`, Bold, `#1E1E1E`, centered, `HorizontalTextAlignment="Center"`).
   - Subtitle tagline: `"A little care, a lovely you."` (`14sp`, `#757575`, centered).
3. **Login Form (`<input:FormView SubmitCommand="{Binding LoginCommand}" Spacing="14">`):**
   - **Email / No. HP Input:**
     - `<material:TextField Title="Email atau No. HP" Text="{Binding Identifier}" Icon="{FontImageSource FontFamily=MaterialOutlined, Glyph={x:Static m:MaterialRegular.Person}}" AccentColor="#D9534F" BorderColor="#EFE6E4" CornerRadius="12">`
     - Include `<validation:RequiredValidation Message="Email atau Nomor HP wajib diisi" />`.
   - **Password Input:**
     - `<material:TextField Title="Password" Text="{Binding Password}" IsPassword="True" Icon="{FontImageSource FontFamily=MaterialOutlined, Glyph={x:Static m:MaterialRegular.Lock}}" AccentColor="#D9534F" BorderColor="#EFE6E4" CornerRadius="12">`
     - Attach `<material:TextFieldPasswordShowHideAttachment />`.
     - Include `<validation:RequiredValidation Message="Password wajib diisi" />`.
   - **Forgot Password Link:** Right-aligned label `"Lupa Password?"` (`TextColor="#D9534F"`, `FontSize="13"`, `FontAttributes="Bold"`).
4. **Action Buttons:**
   - **Primary CTA:** `<Button Text="Mulai Sekarang / Masuk" StyleClass="FilledButton" input:FormView.IsSubmitButton="True" BackgroundColor="#D9534F" TextColor="White" CornerRadius="24" HeightRequest="52" FontAttributes="Bold" />`
   - **Footer Navigation:** Centered horizontal stack: `"Belum punya akun? "` (`#757575`) + `"Daftar"` (`#D9534F`, Bold, TapGestureRecognizer navigating to `RegisterPage`).

---

### SCREEN 2: `RegisterPage.xaml` (Customer Registration Screen)
**Concept:** Clean card-based registration form matching the Monica Salon User Requirements (`Nama Lengkap`, `No. HP`, `Email`, `Password`, `Konfirmasi Password`, `Syarat & Ketentuan`).
1. **Root Element:** `<uranium:UraniumContentPage BackgroundColor="#FAF7F5" NavigationPage.HasNavigationBar="False">` with `<ScrollView>`.
2. **Header:**
   - Back button using `<uranium:ButtonView>` (`WidthRequest="40" HeightRequest="40"`) with `MaterialRegular.Arrow_back` icon (`#1E1E1E`).
   - Title: `"Buat Akun Cantikmu."` (`24sp`, Bold, `#1E1E1E`).
   - Subtitle: `"Isi data di bawah untuk mulai booking dan melihat buku gaya."` (`14sp`, `#757575`).
3. **Form Card Container:**
   - White `<Border>` (`BackgroundColor="#FFFFFF"`, `StrokeShape="RoundRectangle 16"`, `Padding="20"`, `Stroke="#EFE6E4"`).
   - Inside `<input:FormView SubmitCommand="{Binding RegisterCommand}" Spacing="14">`:
     1. **Nama Lengkap:** `<material:TextField Title="Nama lengkap (cth: Bella Anisa)" Text="{Binding FullName}" Icon="{FontImageSource FontFamily=MaterialOutlined, Glyph={x:Static m:MaterialRegular.Badge}}" AccentColor="#D9534F" CornerRadius="12">` + `<validation:RequiredValidation Message="Nama lengkap wajib diisi" />`.
     2. **No. HP:** `<material:TextField Title="No. HP (08xx xxxx xxxx)" Keyboard="Telephone" Text="{Binding PhoneNumber}" Icon="{FontImageSource FontFamily=MaterialOutlined, Glyph={x:Static m:MaterialRegular.Phone_iphone}}" AccentColor="#D9534F" CornerRadius="12">` + `<validation:RequiredValidation Message="Nomor HP wajib diisi" />` + `<validation:DigitsOnlyValidation Message="Hanya boleh angka" />`.
     3. **Email:** `<material:TextField Title="Email (nama@email.com)" Keyboard="Email" Text="{Binding Email}" Icon="{FontImageSource FontFamily=MaterialOutlined, Glyph={x:Static m:MaterialRegular.Mail}}" AccentColor="#D9534F" CornerRadius="12">` + `<validation:RequiredValidation Message="Email wajib diisi" />` + `<validation:EmailValidation Message="Format email tidak valid" />`.
     4. **Password:** `<material:TextField Title="Password (Minimal 6 karakter)" IsPassword="True" Text="{Binding Password}" Icon="{FontImageSource FontFamily=MaterialOutlined, Glyph={x:Static m:MaterialRegular.Lock}}" AccentColor="#D9534F" CornerRadius="12">` + `<material:TextFieldPasswordShowHideAttachment />` + `<validation:MinLengthValidation MinLength="6" Message="Minimal 6 karakter" />`.
     5. **Konfirmasi Password:** `<material:TextField Title="Konfirmasi password" IsPassword="True" Text="{Binding ConfirmPassword}" Icon="{FontImageSource FontFamily=MaterialOutlined, Glyph={x:Static m:MaterialRegular.Lock_reset}}" AccentColor="#D9534F" CornerRadius="12">` + `<material:TextFieldPasswordShowHideAttachment />` + `<validation:RequiredValidation Message="Ulangi password Anda" />`.
     6. **Terms & Conditions:** `<material:CheckBox Text="Saya setuju dengan syarat &amp; ketentuan." Color="#D9534F" TextColor="#1E1E1E" IsChecked="{Binding IsTermsAccepted}" />`.
     7. **Submit Button:** `<Button Text="Daftar" StyleClass="FilledButton" input:FormView.IsSubmitButton="True" BackgroundColor="#D9534F" TextColor="White" CornerRadius="24" HeightRequest="52" FontAttributes="Bold" Margin="0,8,0,0" />`.
4. **Bottom Link:** Centered horizontal stack `"Sudah punya akun? "` + `"Login"` (`#D9534F` Bold, navigating back to `LoginPage`).

---

### SCREEN 3: `HomeCustomerPage.xaml` (Customer Home Dashboard)
**Concept:** Replica of the middle phone screen in the reference mockup (`Location header`, `Search + Filter button`, `Hero Banner Carousel with pagination dots`, `5 Circular Service Categories`, `Popular Services Cards`, and `Bottom Navigation Bar`).

1. **Root Element:** `<uranium:UraniumContentPage BackgroundColor="#FAF7F5" NavigationPage.HasNavigationBar="False">` with a main `<Grid RowDefinitions="*, Auto">`.
2. **Top Header & Location Bar:**
   - Left column: Small label `"Location"` (`12sp`, `#757575`), below it a horizontal row with Pin icon (`MaterialRegular.Location_on`, `#D9534F`) + Text `"Pasar Kemis, Tangerang"` (`15sp`, Bold, `#1E1E1E`).
   - Right column: Notification bell icon button (`MaterialRegular.Notifications`, `#1E1E1E`) with a small coral dot indicator inside a circular `<uranium:ButtonView>`.
3. **Search Bar & Filter Row (`Grid ColumnDefinitions="*, Auto" ColumnSpacing="12"`):**
   - Column 0: White rounded search container (`StrokeShape="RoundRectangle 12"`, `HeightRequest="48"`, `BackgroundColor="White"`, `Stroke="#EFE6E4"`) with Search icon (`MaterialRegular.Search`, `#757575`) and placeholder `"Cari layanan, paket, atau gaya..."`.
   - Column 1: Square rounded filter `<uranium:ButtonView>` (`WidthRequest="48"`, `HeightRequest="48"`, `StrokeShape="RoundRectangle 12"`, `BackgroundColor="#D9534F"`) with white filter/tune icon (`MaterialRegular.Tune`).
4. **Hero Banner Carousel:**
   - `<CarouselView ItemsSource="{Binding Banners}" HeightRequest="160" IndicatorView="bannerIndicator" PeekAreaInsets="0,0,20,0" Loop="False">`
   - Bind to 2 Hero Banner items:
     - **Banner 1:** Image `"banner_aplikasi.png"` (Fallback card with `#FDECEC` background, title `"Kini Monica Salon Sudah Ada Aplikasi!"` in `#8B2626`, subtitle `"Booking perawatan favoritmu sekarang lebih mudah, cepat, dan praktis!"`, badge `"# Buka Setiap Hari"`).
     - **Banner 2:** Image `"banner_booking24jam.png"` (Fallback card with `#FDECEC` background, title `"Kini Bisa Booking 24 Jam!"` in `#8B2626`, subtitle `"Atur jadwal perawatanmu kapan saja tanpa antre."`, badge `"# Buka 24/7"`).
   - Below Carousel: `<IndicatorView x:Name="bannerIndicator" IndicatorColor="#E0CFCB" SelectedIndicatorColor="#D9534F" IndicatorSize="8" HorizontalOptions="Center" Margin="0,6,0,0" />`.
5. **Section: Kategori Layanan (`Layanan`):**
   - Header Row: Left `"Layanan"` (`18sp`, Bold, `#1E1E1E`), Right `"Lihat Semua"` (`13sp`, `#D9534F`, SemiBold).
   - Horizontal `<CollectionView ItemsSource="{Binding Categories}" ItemsLayout="HorizontalList" HeightRequest="98">`:
     - Each item uses `<uranium:ButtonView>` containing a circular `<Border WidthRequest="58" HeightRequest="58" StrokeShape="RoundRectangle 29" BackgroundColor="#FDECEC" StrokeThickness="0">` with centered icon (`#D9534F`) and label below (`12sp`, `#1E1E1E`, `HorizontalTextAlignment="Center"`):
       1. `Potong Rambut` (`MaterialRegular.Content_cut`)
       2. `Creambath` (`MaterialRegular.Spa`)
       3. `Smoothing` (`MaterialRegular.Auto_awesome`)
       4. `Pewarnaan` (`MaterialRegular.Palette`)
       5. `Facial` (`MaterialRegular.Face_retouching_natural`)
6. **Section: Layanan Favorit (`Layanan Favorit`):**
   - Header Row: `"Layanan Favorit"` (`18sp`, Bold, `#1E1E1E`).
   - 2-Column Grid (`<CollectionView ItemsSource="{Binding FavoriteServices}" ItemsLayout="VerticalGrid, 2">`) or Horizontal List of Service Cards:
     - White card (`<Border StrokeShape="RoundRectangle 16" BackgroundColor="White" Padding="10" Stroke="#EFE6E4" Margin="0,0,10,12">`) wrapped in `<uranium:ButtonView TappedCommand="{Binding Source={RelativeSource AncestorType={x:Type vm:HomeCustomerViewModel}}, Path=SelectServiceCommand}" CommandParameter="{Binding .}">`.
     - Top image container (`HeightRequest="115"`, `StrokeShape="RoundRectangle 12"`) with:
       - Top-right circular favorite heart icon (`MaterialRegular.Favorite`, `#D9534F` inside white circle).
       - Bottom-right rating pill badge (`★ 4.8/5.0`, white background, amber star `#F59E0B`, `11sp` bold).
     - Below image:
       - Service Name: `{Binding Name}` (`14sp`, Bold, `#1E1E1E`)
       - Price & Duration row: `{Binding FormattedPrice}` (`13sp`, Bold, `#D9534F`) + `" | "` + `{Binding Duration}` (`12sp`, `#757575`).
     - Pre-populate ViewModel with Monica Salon services:
       1. `Potong Rambut` — `Rp 50.000` | `45 mnt` (`4.8/5.0`)
       2. `Smoothing` — `Rp 250.000` | `2-3 jam` (`4.8/5.0`)
       3. `Pewarnaan Rambut` — `Rp 200.000` | `2-3 jam` (`4.9/5.0`)
       4. `Creambath` — `Rp 80.000` | `45 mnt` (`4.7/5.0`)
       5. `Rebonding` — `Rp 300.000` | `3-4 jam` (`4.8/5.0`)
       6. `Facial` — `Rp 120.000` | `60 mnt` (`4.7/5.0`)
7. **Bottom Navigation Bar (Grid Row 1):**
   - White bar (`BackgroundColor="White"`, `HeightRequest="65"`, top border `#EFE6E4`) with 5 equal columns (`Grid ColumnDefinitions="*,*,*,*,*"`) using `<uranium:ButtonView>`:
     1. **Home** (Active: `MaterialRegular.Home`, `#D9534F`, Bold label `"Home"`)
     2. **Explore** (`MaterialRegular.Explore`, `#757575`, label `"Explore"`)
     3. **Booking** (`MaterialRegular.Calendar_month`, `#757575`, label `"Booking"`)
     4. **Chat / Riwayat** (`MaterialRegular.Chat_bubble_outline`, `#757575`, label `"Chat"`)
     5. **Profil** (`MaterialRegular.Person_outline`, `#757575`, label `"Profil"`)
8. **UraniumUI BottomSheet Attachment (Service Quick Preview):**
   - Inside `<uranium:UraniumContentPage.Attachments>`, include a `<material:BottomSheetView x:Name="serviceBottomSheet" IsPresented="{Binding IsServiceDetailOpen}">` showing the selected service's title, price, duration, description (`"Perawatan dengan bahan premium, konsultasi gaya rambut gratis dengan stylist Monica Salon."`), and a full-width `"Booking Sekarang"` pill button (`BackgroundColor="#D9534F"`, `CornerRadius="24"`).

---

## 6. Deliverables & File Structure Expected from AI Agent
1. `MauiProgram.cs` — Configured with `.UseUraniumUI()`, `.UseUraniumUIMaterial()`, and `.AddMaterialSymbolsFonts()`.
2. `Resources/Styles/Colors.xaml` — Updated with the Monica Salon Coral/Blush color tokens.
3. `App.xaml` & `App.xaml.cs` — Merged `<material:StyleResource>` and `MainPage = new NavigationPage(new LoginPage());`.
4. `Models/ServiceItem.cs`, `Models/CategoryItem.cs`, `Models/BannerItem.cs`.
5. `ViewModels/LoginViewModel.cs`, `ViewModels/RegisterViewModel.cs`, `ViewModels/HomeCustomerViewModel.cs`.
6. `Views/LoginPage.xaml` (+ `.cs`), `Views/RegisterPage.xaml` (+ `.cs`), `Views/HomeCustomerPage.xaml` (+ `.cs`).