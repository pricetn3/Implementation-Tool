# Client Implementation Tool: new user stories

Clickable prototype and dev handoff for four new pages in the Paradigm Client Implementation tool, built in the current tool's style (Blazor + Telerik UI for Blazor + Bootstrap 5.1).

## New pages

| Page | File |
|---|---|
| API Order Batching | `prototype/ClassicBatching.dc.html` |
| Store (paper ordering, pricing, cert type description and packaging) | `prototype/ClassicStore.dc.html` |
| Lost & Damaged | `prototype/ClassicLostDamaged.dc.html` |
| Country | `prototype/ClassicCountry.dc.html` |

The existing tabs (Ced/CeC, Reorder Management, Hold Management, MyHub, Org Display Name, Actions Log) are included so the sidebar works end to end. Their layouts are unchanged; they only gained the shared confirmation, toast and grid behavior.

## How to view

Open `index.html` in a browser (double-click it). No install or server needed; every page loads `prototype/support.js`.

Example institution: Virginia Hanock University, ID **1234** (any other ID shows "not found").

## For developers

Start with **[docs/dev-handoff.md](docs/dev-handoff.md)**: which Telerik component or Bootstrap class each part maps to, the three custom pieces, theme values measured from the live tool, behavior rules and open questions.

The same content is in `prototype/ClassicDevHandoff.dc.html`.
