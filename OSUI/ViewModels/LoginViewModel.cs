using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Views.Windows;
using System.Windows;
using System.Windows.Controls;
using OSUI.Services;

namespace OSUI.ViewModels
{
    public partial class LoginViewModel(IAuthService authService) : ObservableObject
    {
        public Action? OnLoginSuccess { get; set; }
        public Action? OnGoToRegister { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private partial string Username { get; set; } = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        public partial string Password { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ErrorMessage { get; private set; } = string.Empty;

        [ObservableProperty]
        public partial bool HasError { get; private set; }

        private bool CanLogin() =>
            !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        /// <summary>
        /// 以访客身份进入。不进行任何身份验证，直接开启只读浏覧模式
        /// AuthService.CurrentUser 保持 null，CurrentRole 自动为 Guest
        /// </summary>
        [RelayCommand]
        private void EnterAsGuest() => OnLoginSuccess?.Invoke();

        [RelayCommand]
        private void GoToRegister() => OnGoToRegister?.Invoke();

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private void Login()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            if (authService.Login(Username, Password))
            {
                OnLoginSuccess?.Invoke();
            }
            else
            {
                HasError = true;
                ErrorMessage = "用户名或密码错误，请重试。";
            }
        }
    }

}
