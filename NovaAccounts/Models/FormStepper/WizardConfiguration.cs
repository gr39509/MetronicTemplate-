namespace NovaAccounts.Models.FormStepper;

public class WizardConfiguration
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string Title { get; set; } = "Form Wizard";
    public string CssClass { get; set; } = "portlet light bordered";
    public bool ShowProgressBar { get; set; } = true;
    public bool ShowStepNumbers { get; set; } = true;
    public string BackButtonText { get; set; } = "Back";
    public string NextButtonText { get; set; } = "Continue";
    public string FinishButtonText { get; set; } = "Finish";
    public string BackButtonIcon { get; set; } = "icon-control-rewind";
    public string NextButtonIcon { get; set; } = "icon-control-forward";
    public string FinishButtonIcon { get; set; } = "icon-check";
    public bool ShowSuccessMessage { get; set; } = true;
    public bool ShowErrorMessage { get; set; } = true;
    public string SuccessMessage { get; set; } = "Your form validation is successful!";
    public string ErrorMessage { get; set; } = "You have some form errors. Please check below.";
}