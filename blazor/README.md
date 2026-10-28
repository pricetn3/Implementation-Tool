# Code handoff: the five new tabs in Blazor

This folder is the real code for the five new tabs: **API Order Batching**, **Student Store**, **Lost & Damaged**, **Country** and **Users**. It's written for the tool's stack: **Blazor + Telerik UI for Blazor + Bootstrap 5.1**.

The clickable prototype in `../prototype/` is a design-tool export. Its markup uses a template syntax (`{{…}}`, `<sc-if>`, `<sc-for>`) and depends on a bundled runtime (`support.js`), so **don't copy code from it**. Use it only to see how the pages should behave. Copy the code from here instead.

What's here:
- **Pages** that behave like the prototype: same fields, copy, validation, confirmations, toasts, unsaved-changes prompts and focus moves.
- **Shared components** used by all five pages: User dropdown, institution search, sticky Submit bar, "Are you sure?" and "Leave without saving?" dialogs.
- **Service interfaces** for the data each page reads and writes, plus **in-memory sample implementations** seeded with the prototype's example data (1111 set up, 2222 new). The pages run before any database work exists.

What's left for you:
1. Implement each `I…Service` against the real database (section 4).
2. Wire up the Actions Log and institution lookup to the existing ones.
3. Check the styling against the live tool (section 5).

---

## 1. Drop it into the app

| Step | What to do |
|---|---|
| 1 | Copy `Models/`, `Services/`, `Shared/`, `Pages/` into the app. Rename the `ClientImplementation` namespace to the app's root namespace (find and replace). |
| 2 | Merge `_Imports.razor` into the app's `_Imports.razor`. |
| 3 | Copy `ServiceRegistration.cs`. In `Program.cs`, after `builder.Services.AddTelerikBlazor();`, add `builder.Services.AddNewTabs();` |
| 4 | Copy `wwwroot/css/new-tabs.css` and `wwwroot/js/new-tabs.js`. In the host page (`_Host.cshtml` / `App.razor`), add `<link href="css/new-tabs.css" rel="stylesheet" />` after the Telerik theme, and `<script src="js/new-tabs.js"></script>` after the Blazor script. |
| 5 | Add the five nav links from `snippets/NavMenu-links.razor.txt` to `NavMenu.razor`. |
| 6 | Run it. Search **1111** (set up) or **2222** (new) on Batching, Student Store or Lost & Damaged. |

**Requirements:** .NET 7 or later (for `NavigationLock`) and Telerik UI for Blazor. Verified with .NET 8 and Telerik 15.1. `MainLayout` must already wrap the app in `<TelerikRootComponent>` (it does if MyHub's toast works).

## 2. File map: prototype → code

| Prototype page | Blazor page | Route | Model | Service |
|---|---|---|---|---|
| `ClassicBatching.dc.html` | `Pages/ApiOrderBatching.razor` | `/api-order-batching` | `Models/BatchingSchedule.cs` | `IBatchingService` |
| `ClassicStore.dc.html` | `Pages/StudentStore.razor` + `Pages/PaperDescriptionWindow.razor` | `/student-store` | `Models/StudentStoreSettings.cs` | `IStudentStoreService` |
| `ClassicLostDamaged.dc.html` | `Pages/LostDamaged.razor` | `/lost-damaged` | `Models/LostDamagedSettings.cs` | `ILostDamagedService` |
| `ClassicCountry.dc.html` | `Pages/Country.razor` | `/country` | `Models/ShippingCountry.cs` | `ICountryService` |
| `ClassicUsers.dc.html` | `Pages/Users.razor` | `/users` | (names only) | `IUsersService` |

Shared pieces:

| File | What it is |
|---|---|
| `Shared/EditPageBase.cs` | Base class for every page. Handles the Submit flow (field errors and focus, "No changes to save.", required user, confirmation, save, Actions Log and toast) and Discard changes. |
| `Shared/PageHeader.razor` | Page title, TESTING note and User dropdown |
| `Shared/UserSelect.razor` | User dropdown (`TelerikDropDownList`). "Paradigm User" is the placeholder only. |
| `Shared/InstitutionSearch.razor` | Institution ID search and the blue info box (`alert alert-info`) |
| `Shared/SubmitBar.razor` | Sticky bar: Submit, Discard changes, the save note and the unsaved summary |
| `Shared/ConfirmDialog.razor` | "Are you sure?" (`TelerikDialog`), with OK first, then Cancel |
| `Shared/LeaveDialog.razor` | "Leave without saving?" with Leave and Stay |
| `Shared/UnsavedChangesGuard.razor` | `NavigationLock`: our dialog for sidebar links, the browser prompt for tab close or refresh |
| `Models/Common.cs` | `Institution`, `ActionsLogEntry`, `FieldError` |
| `Services/CommonServices.cs` | `IInstitutionService`, `IActionsLogService`, `IUsersService`, `CurrentUserState` |

## 3. How a page works

Every page follows the same pattern, so after reading one (start with `Pages/LostDamaged.razor`, the smallest) the others read the same way:

```razor
@page "/lost-damaged"
@inherits EditPageBase

<PageHeader Title="Lost & Damaged" />
<InstitutionSearch Loaded="@institution" IsDirty="@IsDirty" OnLoad="LoadAsync" />
... fields bound to `draft` ...
<SubmitBar OnSubmit="SubmitAsync" OnDiscard="DiscardChanges" Note="@SubmitNote" UnsavedText="@UnsavedText" />
<ConfirmDialog Visible="@ConfirmVisible" Text="@ConfirmText" OnOk="ConfirmAsync" OnCancel="CancelConfirmAsync" />
<UnsavedChangesGuard IsDirty="@IsDirty" />
<TelerikNotification @ref="Toast" Class="cl-toast" ... />
```

- The page keeps a **`saved`** copy (as loaded) and a **`draft`** copy (being edited). `IsDirty` compares the two. The models are records, so `draft != saved` compares by value.
- Each edit handler calls **`MarkEdited()`**.
- The page overrides **`Validate()`**, which returns `FieldError(fieldId, message)` in page order. The base class adds the user-required error, shows errors only after Submit, and focuses the first error.
- The page overrides **`RevertToSaved()`**, which puts the draft back to `saved` (usually `draft = saved;`). The **Discard changes** button calls the base class's `DiscardChanges()`, which runs it and clears errors and the save note. There's no confirmation.
- The page overrides **`CommitAsync()`**, which saves through the service, writes Actions Log entries with `LogEntry(institution, area, old, new)`, then sets `saved = draft`.

## 4. Services to implement

Each interface has a sample implementation in `Services/Sample/`. Replace it with a real one and change the registration in `ServiceRegistration.cs` (use `AddScoped` for database-backed services).

| Interface | Notes |
|---|---|
| `IInstitutionService` | Same lookup the Ced/CeC search uses. Return `null` for an unknown ID. |
| `IActionsLogService` | The existing Actions Log. Each save writes the selected user (as text), institution, area, old value and new value. |
| `IUsersService` | Names for the User dropdown, A–Z. Deleting a user doesn't change past log entries. |
| `IBatchingService` | Per institution. A new institution has no schedule ("Current schedule: Not set"). |
| `IStudentStoreService` | Per institution. Paper ordering is an **organization-level flag**; don't infer it from paper-product records. Turning it off keeps every saved value. Packaging stores "No packaging required" as its own value, not as empty. |
| `ILostDamagedService` | Per institution. Reporting is its **own** org-level flag, separate from paper ordering. `ApproveLost` and `ApproveDamaged` aren't shown yet (later release) but are in the model. |
| `ICountryService` | One global list, used by the Student Store only. |

`CurrentUserState` is Scoped (one per user session), so the selected user stays selected across pages. To keep it across a browser refresh too, persist it with `ProtectedLocalStorage`. If Windows authentication becomes possible, the signed-in user replaces the dropdown.

## 5. Styling

`wwwroot/css/new-tabs.css` holds the theme values measured from the live tool (accent `#3f6fa9`, primary button `#1274AC`, 30px inputs, grid colors). They're defined as CSS variables at the top. Telerik class names differ slightly between theme versions, so check the grid, dropdown and dialog overrides against the live tool, and drop any rule the app's theme already covers. Component-to-class mapping: `../docs/dev-handoff.md`.

## 6. Page notes

### API Order Batching (`Pages/ApiOrderBatching.razor`)
- The schedule rules (describe the schedule, next 3 batch dates, 1st–28th ordinals) are pure functions in `BatchingScheduleText` in `Models/BatchingSchedule.cs`, so they're easy to unit test.
- Only the values the chosen frequency uses are compared and saved (`Normalized()`). In the prototype, going Weekly → Twice a week → Weekly showed a false "1 unsaved change"; that's fixed here.
- "Next 3 batches" and "First batch" use the server's `DateTime.Today`. **Decide:** use Eastern Time instead, since batches run at 1:00 p.m. ET?

### Student Store (`Pages/StudentStore.razor`, `Pages/PaperDescriptionWindow.razor`)
- The grid uses `OnRead`, so "Apply packaging to all" applies to **every filtered row**, not only the current page.
- "No packaging required" is stored as `"none"` (`CertTypeSetting.NoPackagingRequired`). Not selected is `null`.
- On Submit, `BeforeFocusErrorAsync` clears the grid filters (or switches Packaging to "Not selected") so the first row to fix is visible before it gets focus.
- Each change writes its own Actions Log entry, e.g. "Student Store Packaging (8511-SPEC)".
- Prices use `TelerikNumericTextBox` with `Min=0`, so a negative amount can't be typed (the prototype showed an error instead).
- Small differences from the prototype: searching another institution resets the Packaging filter; turning paper ordering off and on resets the text filters.
- **Decide:** grid paging. It's set to 50 rows per page; the bulk apply covers every filtered row either way.

### Lost & Damaged (`Pages/LostDamaged.razor`)
- Only the reporting checkbox shows. `ApproveLost` and `ApproveDamaged` are in the model for the later approval release and are kept as saved.

### Country (`Pages/Country.razor`)
- One global list, no institution search. Only the checkbox and its Enabled/Disabled label toggle. Changed rows show "Unsaved".
- The Enabled filter counts follow unsaved choices. While that filter is set, a toggled row leaves the grid right away, as in the prototype.
- The Actions Log writes one entry per country, area "Country: Brazil (BR)".
- **Decide:** what happens to existing Student Store carts and orders when a country is turned off?

### Users (`Pages/Users.razor`)
- Names only. After Submit, every User dropdown reloads (`CurrentUserState.UsersChanged`). If the selected user was deleted, it resets to the placeholder.


## 7. Status

**Compiled and click-tested.** The code builds with 0 errors against .NET 8 and Telerik UI for Blazor 15.1 (the only warnings are about the missing license key). Each page was clicked through in a running app with the sample data:

| Page | Checked |
|---|---|
| Lost & Damaged | Search 1111 → toggle → Submit without a user (error, focus on dropdown) → pick user → "Are you sure?" → OK → toast |
| Student Store | Load 1111 grid; open the description window and its copy list; load 2222, turn paper ordering on, Submit → every missing price, description and packaging flagged |
| API Order Batching | Load 1111 (next 3 batches) → Daily → confirmation copy with "First batch" |
| Country | Toggle a country → "Unsaved" badge and count; a sidebar link with unsaved changes → "Leave without saving?" → Leave |
| Users | Add, block a duplicate, delete → confirmation with both changes → toast; the User dropdown picks up the new list |

**Discard changes** reverts every page to its saved values and clears red errors (checked on all five pages, in both the prototype and the Blazor app).

**Not checked:**
- Styling against the live tool. The theme CSS was written from the measured values, not compared side by side.
- The real data services (they don't exist yet).
- Older Telerik versions. If the app is on an older version than 15.1, a parameter name may differ. Blazor reports that at runtime as "does not have a property matching the name …", so open each page once after copying.

**Open questions** (also in `../docs/dev-handoff.md`): Windows authentication in place of the User dropdown; Student Store grid paging.

