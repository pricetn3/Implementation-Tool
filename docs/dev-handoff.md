# Dev handoff: component mapping

For the new pages: Student Store, API Order Batching, Lost & Damaged, Country and Users. Stack: **Blazor** app, **Telerik UI for Blazor** components (confirmed), **Bootstrap 5.1** styles. Rule: reuse the Telerik component or Bootstrap class the tool already uses before building anything custom.

## 1. Telerik components

| In the prototype | Telerik component | Settings | Pages |
|---|---|---|---|
| Cert type and country tables (filter row, funnel menu, sorting, footer) | `TelerikGrid` | `FilterMode="GridFilterMode.FilterRow"`, `Sortable="true"`, default text operator Contains, empty text "No records available." Student Store: paging off, or `PageSize` 50+, so "Apply packaging to all" covers every filtered row. Row stripes and hover come from the theme. | Student Store, Country, Users (same as Actions Log) |
| Gray dropdowns and their menus | `TelerikDropDownList` | Resting background `#f5f5f5`, selected option `#3f6fa9` | All |
| Checkboxes | `TelerikCheckBox` | Checked `#3f6fa9`. In Country, only the checkbox and its Enabled/Disabled label toggle, not the row | Lost & Damaged, Country, Student Store dialog, Batching (twice a week) |
| Radio buttons | `TelerikRadioGroup` | Inline secondary text for "Daily  Monday–Friday" (see section 2) | Batching, Student Store (additional price) |
| Date fields with calendar | `TelerikDatePicker` | Already used on Ced/CeC and Actions Log filters | Existing pages |
| "[Page] Updated" toast after OK | `TelerikNotification` | Reuse the MyHub setup: closable, auto-closes about 5 seconds. Text: Student Store Updated, Batching Schedule Updated, Lost & Damaged Updated, Countries Updated, Users Updated | All |
| Paper description dialog | `TelerikWindow` (modal) | Centered, same header, footer and buttons as the "Are you sure?" confirmation. Return focus to the Edit button on close | Student Store |

## 2. Bootstrap 5.1 classes

| In the prototype | Class | Notes |
|---|---|---|
| Submit (primary) and secondary buttons, incl. Discard changes | `btn btn-primary`, `btn btn-secondary` | Theme override: primary `#1274AC`, hover `#0f6292`; secondary `#f2f2f2` |
| Blue institution box; Country note | `alert alert-info` |  |
| Field errors | `is-invalid` + `invalid-feedback` | Shown only after Submit. No error summary box. Submit moves focus to the first invalid field and shows "Nothing was saved. Fix the fields marked in red." in red next to Submit. |
| Badges | `badge bg-secondary` (Unsaved) | Mark changed rows only |
| Hint text under fields | `form-text` |  |
| "Monday–Friday" next to Daily | `small text-muted` |  |
| $ before price fields | `input-group` + `input-group-text` |  |
| Gray read-only boxes (next 3 batches, rush orders) | `bg-light border rounded p-3` |  |
| Dialog close × | `btn-close` |  |

## 3. Custom pieces (no ready-made component)

- Sticky Submit bar (`position: sticky; bottom: 0`): **Submit**, then **Discard changes** (secondary), then the save note and unsaved changes. Student Store lists what changed, e.g. "Unsaved: Pricing · Description (2 cert types) · Packaging (3 cert types)".
- Student Store: every cert type needs its own paper description (300 characters max). No shared generic text. Missing descriptions block Submit while paper ordering is on, including when turning it on.
- Student Store: "Copy this description to other cert types" inside the paper description dialog: a search box and a checkbox list ("No description" marks empty ones), "Select shown" and "Clear". Each cert type gets its own copy of the text, not a shared link.
- Student Store: packaging is required per cert type. Each row starts on "Select Packaging" (missing). "No packaging required" is an explicit choice (CEDs, microcredentials), stored as its own value, not as empty.
- Student Store: "Apply packaging to all": a dropdown plus "Apply to N cert types", applied to the grid’s currently filtered rows. Then edit exceptions in their rows.
- Student Store: Paper Description uses the standard text filter (Contains). The Packaging "Not selected" filter keeps matching rows in view while they’re being fixed, until the filter changes. On Submit, filters clear if needed and focus goes to the first row to fix.

## 4. Theme values (from the live tool)

| Token | Value |
|---|---|
| Body and control text | 14px Helvetica Neue, `#212529` |
| Input and dropdown height | 30px (28px inside the 1px border); label 15px above the field |
| Input border / focus | `#e1e1e1` / `#949494` border + 2px `#ebebeb` ring (darker than live for 3:1 contrast) |
| Accent (checked, selected option, OK button, toast) | `#3f6fa9` |
| Hover gray (buttons, menus, rows, headers) | `#e9e9e9` |
| Focus ring (buttons, checkboxes, radios) | 2px `#c5d4e5` |
| Grid header / stripes / borders | `#fafafa` / `#f5f5f5` / `#e3e3e3`; active sort arrow gray `#6c757d` |

## 5. Behavior rules

- **Discard changes** (every page, next to Submit): puts every field back to its last saved values right away. No confirmation, red errors and the save note clear, and the page is no longer "unsaved" (no leave prompt). With nothing changed it does nothing. Grid filters and sorting stay as they are. On Users it also clears the New User Name field.
- Submit → "Are you sure?" (reuse the MyHub confirmation; OK first, then Cancel) → OK → toast. Nothing changed: show "No changes to save." and skip the confirmation.
- Confirmation copy is one sentence naming only the new value, e.g. "Updating client 1111 (Virginia Hanock University) API order batching to Daily, Monday–Friday. First batch: Wed, Oct 21." Use bullets only for several changes.
- Searching a different institution loads its saved values. With unsaved changes, it asks "Leave without saving?" first.
- New institution (nothing set up, example 2222): Student Store paper ordering is off with the note "Paper ordering isn’t set up for this institution yet. Turn it on to add pricing, descriptions and packaging." (a disabled client with saved settings says its values are kept instead). Batching shows "Current schedule: Not set" with no frequency picked. Lost & Damaged reporting is off. Turning Student Store on requires both prices, a description and a packaging choice for every cert type.
- Leaving a page with unsaved changes (sidebar link) asks "Leave without saving?" with Leave and Stay. In Blazor use `NavigationLock` with `ConfirmExternalNavigation` for tab close and refresh.
- Batching: switching Weekly ↔ Twice a week keeps the chosen day.
- Student Store: the row button reads "Add" when a cert type has no description, "Edit" when it has one.
- Users: names only, no other fields. Add, or Delete (row shows "Will be deleted" with Undo until Submit). A name added but not yet submitted has Remove. Duplicate names are blocked. Deleting a user doesn’t change past Actions Log entries, which store the name as text.
- User dropdown (every page): "Paradigm User" is only the placeholder (empty value), not a choice. Options are every user on the Users page, A–Z, updated on Submit there. Submitting any change without a user is blocked: "Select your name before submitting." under the dropdown; focus moves to the dropdown. The chosen user stays selected across pages and is written to the Actions Log with each save. If that user is deleted, the dropdown resets to the placeholder.
- Hidden sections keep their saved values (Student Store paper ordering off, Lost & Damaged reporting off).
- Every save writes an Actions Log entry with the user selected in the User dropdown, the institution, the old value and the new value. Country and Users have the User dropdown too.
- Batching: monthly days are 1st–28th (weekends included); daily is Monday–Friday. Note under the current schedule: "Universal API batches run at 1:00 p.m. Eastern Time, for both standard and rush orders." Rush is fixed daily Monday–Friday, read-only.
- Lost & Damaged (February launch): only the reporting checkbox shows, with the hint "Requests are approved automatically and become replacement orders." (off: "Students can’t report lost or damaged credentials."). Requests are auto-approved into an order. Approval settings (lost, damaged) and photo review are built later; keep the data model ready. Reporting needs its own organization-level flag, separate from the paper ordering flag.
- Country: one list for every institution, used by the Student Store only (not Reorder). Operations maintains it from UPS and USPS notices. Future: address verification (type a few letters, pick a validated address).
- Student Store pricing: "Per cert type" is listed first and marked Recommended; new clients default to it. "Per cart" stays for HBSO.
- Student Store paper ordering: an organization-level on/off flag. Off keeps every saved value; on restores them. Don’t infer it from paper-product records.

## 6. Open questions

- Users: is Windows authentication possible in the delivery window? If so, the signed-in user replaces the User dropdown.
- Student Store: grid paging off, or the page size to use.
