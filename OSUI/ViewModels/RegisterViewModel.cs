using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Services;

namespace OSUI.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        public Action? OnRegisterSuccess { get; set; }
        public Action? OnCancel { get; set; }

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        public partial string Username { get; set; } = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        public partial string Password { get; set; } = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        public partial string ConfirmPassword { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ErrorMessage { get; private set; } = string.Empty;

        [ObservableProperty]
        public partial bool HasError { get; private set; }

        private bool CanRegister() =>
            !string.IsNullOrWhiteSpace(Username) && 
            !string.IsNullOrWhiteSpace(Password) &&
            !string.IsNullOrWhiteSpace(ConfirmPassword);

        [RelayCommand(CanExecute = nameof(CanRegister))]
        private void Register()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            if (Password != ConfirmPassword)
            {
                HasError = true;
                ErrorMessage = LocalizationService.Instance.GetString("Register.Error.PasswordMismatch");
                return;
            }

            if (_authService.Register(Username, Password))
            {
                OnRegisterSuccess?.Invoke();
            }
            else
            {
                HasError = true;
                ErrorMessage = LocalizationService.Instance.GetString("Register.Error.UsernameExists");
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            OnCancel?.Invoke();
        }
    }
}
