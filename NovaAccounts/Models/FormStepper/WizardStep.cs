namespace NovaAccounts.Models.FormStepper;

public class WizardStep
{
    public int StepNumber { get; set; }
    public string Title { get; set; } = "";
    public string Icon { get; set; } = "fa-check";
    public string TabId { get; set; } = "";
    public string Description { get; set; } = "";
    public List<WizardField> Fields { get; set; } = new();
    public bool IsCompleted { get; set; }
    public bool IsActive { get; set; }
}