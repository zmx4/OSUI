using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Services;

namespace OSUI.ViewModels;

public partial class ChangePasswordViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    public Action? OnPasswordChanged { get; set; }
    public Action? OnCancel { get; set; }

    public ChangePasswordViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangePasswordCommand))]
    private string _currentPassword = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangePasswordCommand))]
    private string _newPassword = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangePasswordCommand))]
    private string _confirmNewPassword = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    private bool CanChangePassword() =>
        !string.IsNullOrWhiteSpace(CurrentPassword)
        && !string.IsNullOrWhiteSpace(NewPassword)
        && !string.IsNullOrWhiteSpace(ConfirmNewPassword);

    [RelayCommand(CanExecute = nameof(CanChangePassword))]
    private void ChangePassword()
    {
        HasError = false;
        ErrorMessage = string.Empty;

        if (!string.Equals(NewPassword, ConfirmNewPassword, StringComparison.Ordinal))
        {
            HasError = true;
            ErrorMessage = "两次输入的新密码不一致。";
            return;
        }

        if (_authService.ChangePassword(CurrentPassword, NewPassword, out var errorMessage))
        {
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
            OnPasswordChanged?.Invoke();
            return;
        }

        HasError = true;
        ErrorMessage = string.IsNullOrWhiteSpace(errorMessage)
            ? "修改失败，请稍后重试。"
            : errorMessage;
    }

    [RelayCommand]
    private void Cancel()
    {
        OnCancel?.Invoke();
    }
}
