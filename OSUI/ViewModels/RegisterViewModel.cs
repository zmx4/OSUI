using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Services;

namespace OSUI.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly Action _onRegisterSuccess;
        private readonly Action _onCancel;

        public RegisterViewModel(Action onRegisterSuccess, Action onCancel)
        {
            _onRegisterSuccess = onRegisterSuccess;
            _onCancel = onCancel;
        }

        public RegisterViewModel()
        {
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
                ErrorMessage = "两次输入的密码不一致。";
                return;
            }

            if (AuthService.Instance.Register(Username, Password))
            {
                _onRegisterSuccess();
            }
            else
            {
                HasError = true;
                ErrorMessage = "用户名已存在，请重试。";
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            _onCancel();
        }
    }
}
