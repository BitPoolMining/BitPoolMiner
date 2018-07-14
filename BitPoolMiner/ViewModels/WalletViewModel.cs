using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Windows;
using ToastNotifications.Messages;

namespace BitPoolMiner.ViewModels
{
    public class WalletViewModel : ViewModelBase
    {
        /// <summary>
        /// Constrtuctor
        /// </summary>
        public WalletViewModel()
        {
            // Instantiate new List of wallet addresses
            AccountWalletList = new List<AccountWallet>();

            // Wire up commands for Account Wallets
            CommandSaveAccountWallet = new RelayCommand(PersistAccountWallet);
            CommandInitAccountWallet = new RelayCommand(InitAccountWallet);

            // Load initial wallet data
            InitAccountWallet(null);
        }

        #region Wallet Settings

        // Account wallet property to bind to UI
        private List<AccountWallet> accountWalletList;
        public List<AccountWallet> AccountWalletList
        {
            get
            {
                return accountWalletList;
            }
            set
            {
                accountWalletList = value;
                OnPropertyChanged();

                // Set global variable for Wallet Addresses
                Application.Current.Properties["AccountWalletList"] = accountWalletList;
            }
        }

        // Relay commands used for command binding to the view
        public RelayCommand CommandSaveAccountWallet { get; set; }
        public RelayCommand CommandInitAccountWallet { get; set; }

        /// <summary>
        /// Load list of wallet addresses account
        /// </summary>
        private void InitAccountWallet(object param)
        {
            try
            {
                if (Application.Current.Properties["AccountID"] != null)
                {
                    // Load list of wallet addresses account
                    AccountWalletAPI accountWalletAPI = new AccountWalletAPI();
                    AccountWalletList = accountWalletAPI.GetAccountWalletList();

                    // Notify UI of change
                    OnPropertyChanged("AccountWalletList");

                    // Set global variable for Wallet Address
                    Application.Current.Properties["AccountWalletList"] = accountWalletList;
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error loading wallet address"), e);
            }
        }

        public void PersistAccountWallet(object param)
        {
            try
            {
                if (Application.Current.Properties["AccountID"] != null)
                {
                    // Insert wallet addresses account
                    // API will override existing records
                    AccountWalletAPI accountWalletAPI = new AccountWalletAPI();
                    accountWalletAPI.PostAccountWalletList(accountWalletList);

                    // Notify UI of change
                    OnPropertyChanged("AccountWalletList");

                    // Set global variable for Wallet Addresses
                    Application.Current.Properties["AccountWalletList"] = accountWalletList;

                    notifier.ShowSuccess("Wallet settings saved");
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error saving wallet addresses"), e);
            }
        }

        #endregion
    }
}
