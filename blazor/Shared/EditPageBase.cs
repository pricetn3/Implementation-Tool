using ClientImplementation.Models;
using ClientImplementation.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace ClientImplementation.Shared;

/// <summary>
/// Submit flow shared by every new page:
/// Submit → field errors (focus first) → "No changes to save." → user required → "Are you sure?" → OK → save, log, toast.
/// Discard changes → back to the saved values.
/// A page supplies IsDirty, Validate(), the confirmation copy, CommitAsync() and RevertToSaved().
/// </summary>
public abstract class EditPageBase : ComponentBase, IDisposable
{
    [Inject] protected CurrentUserState CurrentUser { get; set; } = default!;
    [Inject] protected IActionsLogService ActionsLog { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    /// <summary>Bind with @ref="Toast" on the page's TelerikNotification.</summary>
    protected TelerikNotification? Toast;

    /// <summary>Set after a Submit that had errors. Errors then show live until the next successful save.</summary>
    protected bool Submitted { get; set; }
    protected bool NoChange { get; private set; }
    protected bool JustSaved { get; private set; }
    protected bool ConfirmVisible { get; set; }

    /// <summary>"Users Updated", "Batching Schedule Updated", …</summary>
    protected abstract string ToastText { get; }

    protected abstract bool IsDirty { get; }

    /// <summary>Field errors, in page order. The first one gets focus on Submit.</summary>
    protected virtual IEnumerable<FieldError> Validate() => [];

    /// <summary>Saves, writes the Actions Log, and makes the draft the new saved state.</summary>
    protected abstract Task CommitAsync();

    /// <summary>Puts every field back to its saved value (Discard changes).</summary>
    protected abstract void RevertToSaved();

    /// <summary>Errors currently shown (empty until the first failed Submit).</summary>
    protected IReadOnlyList<FieldError> Errors => Submitted ? AllErrors() : [];

    protected bool HasError(string fieldId) => Errors.Any(e => e.FieldId == fieldId);
    protected string? ErrorFor(string fieldId) => Errors.FirstOrDefault(e => e.FieldId == fieldId)?.Message;
    protected string InvalidClass(string fieldId) => HasError(fieldId) ? "is-invalid" : "";

    /// <summary>Text next to Submit.</summary>
    protected string SubmitNote =>
        NoChange ? "No changes to save."
        : Errors.Count > 0 ? "Nothing was saved. Fix the fields marked in red."
        : "";

    protected string SubmitNoteClass => NoChange ? "text-secondary" : Errors.Count > 0 ? "text-danger" : "text-secondary";

    protected override void OnInitialized() => CurrentUser.Changed += OnUserChanged;

    private void OnUserChanged() => InvokeAsync(StateHasChanged);

    /// <summary>Call from every edit handler, so "No changes to save." and the saved state clear.</summary>
    protected void MarkEdited()
    {
        NoChange = false;
        JustSaved = false;
    }

    /// <summary>Clears submit state, e.g. after loading a different institution.</summary>
    protected void ResetSubmitState()
    {
        Submitted = false;
        NoChange = false;
        JustSaved = false;
        ConfirmVisible = false;
    }

    private List<FieldError> AllErrors()
    {
        var errors = Validate().ToList();
        if (IsDirty && !CurrentUser.HasUser)
            errors.Insert(0, new FieldError(UserSelect.ElementId, "Select your name before submitting."));
        return errors;
    }

    protected async Task SubmitAsync()
    {
        var errors = AllErrors();
        if (errors.Count > 0)
        {
            Submitted = true;
            NoChange = false;
            JustSaved = false;
            if (errors[0].FieldId == UserSelect.ElementId)
            {
                CurrentUser.ShowRequiredError = true;
                CurrentUser.NotifyChanged();
            }
            await BeforeFocusErrorAsync(errors[0]);
            await FocusAsync(errors[0].FieldId);
            return;
        }

        if (!IsDirty)
        {
            NoChange = true;
            JustSaved = false;
            Submitted = false;
            return;
        }

        NoChange = false;
        ConfirmVisible = true;
    }

    /// <summary>Hook for pages that must reveal a field before focusing it (e.g. clear a grid filter).</summary>
    protected virtual Task BeforeFocusErrorAsync(FieldError first) => Task.CompletedTask;

    protected async Task ConfirmAsync()
    {
        ConfirmVisible = false;
        await CommitAsync();
        Submitted = false;
        NoChange = false;
        JustSaved = true;
        Toast?.Show(new NotificationModel
        {
            Text = ToastText,
            ThemeColor = ThemeConstants.Notification.ThemeColor.Primary,
            Closable = true,
            CloseAfter = 5000
        });
        await JS.InvokeVoidAsync("implTool.scrollTop");
    }

    /// <summary>
    /// "Discard changes": back to the saved values right away, no confirmation, red errors cleared.
    /// Nothing changed: does nothing.
    /// </summary>
    protected void DiscardChanges()
    {
        if (!IsDirty) return;
        RevertToSaved();
        ResetSubmitState();
        if (CurrentUser.ShowRequiredError)
        {
            CurrentUser.ShowRequiredError = false;
            CurrentUser.NotifyChanged();
        }
    }

    protected async Task CancelConfirmAsync()
    {
        ConfirmVisible = false;
        await FocusAsync(SubmitBar.SubmitButtonId);
    }

    protected ValueTask FocusAsync(string elementId) => JS.InvokeVoidAsync("implTool.focus", elementId);

    /// <summary>Actions Log entry for the selected user.</summary>
    protected ActionsLogEntry LogEntry(Institution? institution, string area, string oldValue, string newValue) => new()
    {
        User = CurrentUser.UserName!,
        InstitutionId = institution?.Id,
        InstitutionName = institution?.Name,
        Area = area,
        OldValue = oldValue,
        NewValue = newValue
    };

    /// <summary>"A", "A and B", "A, B, and C".</summary>
    protected static string JoinList(IReadOnlyList<string> items) => items.Count switch
    {
        0 => "",
        1 => items[0],
        2 => $"{items[0]} and {items[1]}",
        _ => string.Join(", ", items.Take(items.Count - 1)) + ", and " + items[^1]
    };

    protected static string Plural(int n, string one, string many) => n == 1 ? $"{n} {one}" : $"{n} {many}";

    public virtual void Dispose() => CurrentUser.Changed -= OnUserChanged;
}
