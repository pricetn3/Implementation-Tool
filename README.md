# Client Implementation Tool: new user stories

Clickable prototype and dev handoff for five new pages in the Paradigm Client Implementation tool, built in the current tool's style (Blazor + Telerik UI for Blazor + Bootstrap 5.1).

## New pages

| Page | File |
|---|---|
| API Order Batching | `prototype/ClassicBatching.dc.html` |
| Student Store (paper ordering, pricing, cert type description and packaging) | `prototype/ClassicStore.dc.html` |
| Lost & Damaged | `prototype/ClassicLostDamaged.dc.html` |
| Country | `prototype/ClassicCountry.dc.html` |
| Users (add or delete the names in the User dropdown) | `prototype/ClassicUsers.dc.html` |

The existing tabs (Ced/CeC, Reorder Management, Hold Management, MyHub, Org Display Name, Actions Log) are included so the sidebar works end to end. Their layouts are unchanged; they only gained the shared confirmation, toast and grid behavior.

## How to view

Open `index.html` in a browser (double-click it): a short start page, then **Access prototype** opens the Ced/CeC tab. Use the sidebar for the other pages. No install or server needed.

Example institutions: **1111** Virginia Hanock University (set up) and **2222** Virginia Hanock University Graduate School (new, nothing set up). Any other ID shows "not found".

## For developers

**The real code is in [blazor/](blazor/README.md).** It has Blazor + Telerik pages for the five new tabs, ready to copy into the app, and a step-by-step handoff. Don't copy code from `prototype/`: it's a design-tool export that runs on a bundled runtime.

For the design details, see **[docs/dev-handoff.md](docs/dev-handoff.md)**: which Telerik component or Bootstrap class each part maps to, custom pieces, theme values measured from the live tool, behavior rules and open questions.

The same content is in `prototype/ClassicDevHandoff.dc.html`.
