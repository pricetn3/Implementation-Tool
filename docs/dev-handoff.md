# Dev handoff: component mapping

For the four new pages: Store, API Order Batching, Lost & Damaged and Country. Stack: **Blazor** app, **Telerik UI for Blazor** components (confirmed), **Bootstrap 5.1** styles. Rule: reuse the Telerik component or Bootstrap class the tool already uses before building anything custom.

## 1. Telerik components

| In the prototype | Telerik component | Settings | Pages |
|---|---|---|---|
| Cert type and country tables (filter row, funnel menu, sorting, footer) | `TelerikGrid` | `FilterMode="GridFilterMode.FilterRow"`, `Sortable="true"`, default text operator Contains, empty text "No records available." Store: paging off, or `PageSize` 50+, so "Set packaging for all shown" covers every filtered row. Row stripes and hover come from the theme. | Store, Country (same as Actions Log) |
| Gray dropdowns and their menus | `TelerikDropDownList` | Resting background `#f5f5f5`, selected option `#3f6fa9` | All |
| Checkboxes | `TelerikCheckBox` | Checked `#3f6fa9`. In Country, only the checkbox and its Enabled/Disabled label toggle, not the row | Lost & Damaged, Country, Store dialog, Batching (twice a week) |
| Radio buttons | `TelerikRadioGroup` | Inline secondary text for "Daily  Monday–Friday" (see section 2) | Batching, Store (additional price) |
| Date fields with calendar | `TelerikDatePicker` | Already used on Ced/CeC and Actions Log filters | Existing pages |
| "[Page] Updated" toast after OK | `TelerikNotification` | Reuse the MyHub setup: closable, auto-closes about 5 seconds. Text: Store Updated, Batching Schedule Updated, Lost & Damaged Updated, Countries Updated | All |
| Paper description dialog | `TelerikWindow` (modal) | Centered, same header, footer and buttons as the "Are you sure?" confirmation. Return focus to the Edit button on close | Store |

## 2. Bootstrap 5.1 classes

| In the prototype | Class | Notes |
|---|---|---|
| Submit (primary) and secondary buttons | `btn btn-primary`, `btn btn-secondary` | Theme override: primary `#1274AC`, hover `#0f6292`; secondary `#f2f2f2` |
| Blue institution box; Country note | `alert alert-info` |  |
| Red error box after Submit ("Nothing was saved. Fix this:") | `alert alert-danger` | Links move focus to the field |
| Field errors | `is-invalid` + `invalid-feedback` | Shown only after Submit |
| Badges | `badge bg-secondary` (Unsaved), `badge bg-primary` (Custom), `badge bg-light text-dark` (Generic) | Mark exceptions only |
| Hint text under fields | `form-text` |  |
| "Monday–Friday" next to Daily | `small text-muted` |  |
| $ before price fields | `input-group` + `input-group-text` |  |
| Gray read-only boxes (generic description, next 3 batches, rush orders) | `bg-light border rounded p-3` |  |
| Dialog close × | `btn-close` |  |

## 3. Custom pieces (no ready-made component)

- Sticky Submit bar (`position: sticky; bottom: 0`) showing "N unsaved changes" when there are any.
- Store: "Copy custom description to other cert types" inside the paper description dialog: a search box and a checkbox list with Generic/Custom badges, "Select shown" and "Clear". Each cert type gets its own copy of the text, not a shared link.
- Store: "Set packaging for all shown": a dropdown (including "No packaging required") plus "Apply to N cert types", applied to the grid’s currently filtered rows.
- Store: packaging is optional. "No packaging required" is a normal choice in each row’s dropdown and a Packaging filter option. While that filter option is on, changed rows stay in view until the filter changes.

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

- Submit → "Are you sure?" (reuse the MyHub confirmation; OK first, then Cancel) → OK → toast. Nothing changed: show "No changes to save." and skip the confirmation.
- Confirmation copy is one sentence naming only the new value, e.g. "Updating client 1234 (Virginia Hanock University) API order batching to Daily, Monday–Friday. First batch: Wed, Oct 21." Use bullets only for several changes.
- Loading a different institution reloads its saved values and drops unsaved edits.
- Hidden sections keep their saved values (Store paper ordering off, Lost & Damaged reporting off).
- Every save writes an Actions Log entry with the institution, the old value and the new value.

## 6. Open questions

- Batching: what time batches run and in which time zone, and which orders each batch includes.
- Lost & Damaged: do approval changes apply to requests already waiting, or new requests only?
- Store: is the generic description one fixed text for every cert type and institution?
- Store: grid paging off, or the page size to use.
- Store: should some cert types require packaging later? If so, it needs a per-cert-type flag; then packaging becomes required only for those.
