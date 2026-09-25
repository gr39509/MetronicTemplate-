# Nova Nsawa web redesign and restructure

Date: 2026-09-25
Status: approved to build (owner said "go ahead and build it")

## Goal

Turn the Blazor organiser portal and the public donation page into a consistent, production-ready product: one design system, small feature-focused pages, a service layer over the NSwag client, safe error handling, and protected routes. The API contract and the generated `ApiClient.cs` stay unchanged so the client can be regenerated at any time.

## Decisions

- **Approach A**: a small custom component library on plain CSS. Bootstrap is removed.
- **Visual direction "Emerald & Charcoal"**: charcoal sidebar (from the logo), deep emerald `#0E6B53` for primary and money actions, warm neutral canvas `#F4F4F1`. Jost for display (echoes the logo's geometric caps), Figtree for UI text. The logo is kept.
- **Render mode**: Interactive Server with prerendering **off**. The JWT lives in `localStorage`, so prerendered HTML could never make authenticated calls; turning prerender off removes double API calls and the token-less first request.
- **Auth guard**: the app shell wraps content in `AuthorizeView` and redirects to `/?returnUrl=…` when signed out. We do not put `[Authorize]` on components, because the app has no server-side auth middleware and endpoint metadata would throw.
- **Public event endpoint** `GET api/Event/{id}` allows anonymous access (confirmed by owner).

## Structure

```
NsawaWeb.Services/                 (unchanged generated client + auth plumbing)
  Services/ApiClient.cs            generated, untouched
  Services/ApiResultContracts.cs   partial classes implement IApiResult<T>
  Services/AuthService.cs          token storage, claims; no navigation side effects
NsawaWeb/NsawaWeb/
  Application/                     services the UI calls; never touches markup
    Result.cs, ApiRunner.cs        maps ApiException/HttpRequestException to friendly messages, logs detail
    EventsService, DonationsService, AffiliatesService, GroupsService, AccountService
    LookupService                  event types, payment types, networks, roles (cached per circuit)
    QrCodeService                  QR PNG generation (QRCoder + SkiaSharp)
    ToastService, Format           money/date/phone helpers
  Components/
    App.razor, Routes.razor
    Layout/  AppShell, AuthLayout, PublicLayout, RedirectToLogin
    UI/      Button, Card, PageHeader, StatCard, DataTable pieces, Modal, ConfirmDialog,
             Field, Alert, Badge, EmptyState, ErrorState, Skeleton, Tabs, Toasts, Icon, Spinner
  Features/
    Auth/        Login (/), Register (/register), ResetPassword (/admin/resetpassword), ChangePasswordDialog
    Dashboard/   /home
    Events/      /my-events, /affiliate-events, /create-event, /events/{id} hub with tabs
    Donations/   tab /events/{id}/donations, POS page /events/{id}/donate
    Withdrawals/ tab /events/{id}/withdraw
    Groups/      tab /events/{id}/groups
    Affiliates/  tab /events/{id}/affiliates
    Public/      /donate/{id} donor wizard
  wwwroot/css/  tokens.css, app.css (base + shared component styles)
```

Existing URLs keep working. `/events/{id}/edit` becomes the edit form inside the event hub.

## Behaviour preserved (from the inventory)

Everything in the current pages is kept: login, sign-up, set-new-password link flow, change password, dashboard totals, event list and actions (view, edit, donations, withdraw, public donate link, QR download, deactivate), create/edit with banner upload, groups CRUD, affiliates CRUD with roles and notification toggles, donation summary and filters with detail view, POS donation with payment-type-specific fields, MoMo verification and withdrawal, the public OTP donation wizard redirecting to `PayUrl`.

## Fixes included

- Withdrawal success navigated to a missing route; logout fallback pointed to `/login`.
- Donations list used reflection on `_httpClient`; now uses `DonationsAsync`.
- Raw exception text shown to users; now friendly messages, details go to `ILogger`.
- Hard-coded network lists replaced by `api/NetworkType/All`; hard-coded avatar and placeholder images removed.
- Deactivation success shown as an error; affiliate modal reset on every re-render; public wizard inputs only updating on blur.
- `eval` JS removed; sidebar and menus are pure Blazor.
- Open redirect after login: only local return URLs are accepted.
- Logout sent an unassigned device-token body; now sends an empty token and always clears `fullName`.
- One shared `HttpClient` handler with pooled connection lifetime instead of a new handler per circuit.
- Image proxy validates the file name and sets cache headers.
- Dead code removed: Test, Weather, Counter, ReceiveDonations, NavMenu, dashboard.html, the WASM client project, the unused Data project, commented-out page copies.
- Repository: add `.gitignore` and stop tracking `bin/`, `obj/`, `.DS_Store`, `.idea`.

## Known limits (backend, out of scope)

- `Api:BaseUrl` is plain HTTP to an IP address; move the API behind HTTPS before production.
- Lat/long are not collected by the UI. Create keeps sending the old placeholder (10, 10) as a named constant; edit resends the stored values. A location picker is a follow-up.
- Set-new-password security depends on the backend's link id.

## Verification

- `dotnet build` with zero errors.
- Run the app and load each route; signed-out access to shell routes redirects to login.
