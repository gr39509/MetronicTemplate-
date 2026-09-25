namespace NsawaWeb.Components.UI;

/// <summary>Field-level validation messages for a form, keyed by field name.</summary>
public sealed class FormErrors
{
    private readonly Dictionary<string, string> _errors = new(StringComparer.Ordinal);

    public bool Any => _errors.Count > 0;

    public string? this[string field] => _errors.GetValueOrDefault(field);

    public bool Has(string field) => _errors.ContainsKey(field);

    public void Clear() => _errors.Clear();

    public void Clear(string field) => _errors.Remove(field);

    /// <summary>Records <paramref name="message"/> when <paramref name="invalid"/> is true.</summary>
    public FormErrors Check(bool invalid, string field, string message)
    {
        if (invalid && !_errors.ContainsKey(field))
        {
            _errors[field] = message;
        }
        return this;
    }
}
