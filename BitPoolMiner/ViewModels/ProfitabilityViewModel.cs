using BitPoolMiner.Models.MinerPayments;
using BitPoolMiner.Models.WhatToMine;
using BitPoolMiner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPoolMiner.ViewModels
{
    class ProfitabilityViewModel : ViewModelBase
    {
        #region Properties

        private MainWindowViewModel _mainWindowViewModel;

        public WhatToMineData WhatToMineData
        {
            get
            {
                return _mainWindowViewModel.WhatToMineData;
            }
        }

        public MinerPaymentsData MinerPaymentsData
        {
            get
            {
                return _mainWindowViewModel.MinerPaymentsData;
            }
        }

        #endregion

        #region Init

        public ProfitabilityViewModel(MainWindowViewModel mainWindowViewModel)
        {
            // Get a reference back to the main window view model
            _mainWindowViewModel = mainWindowViewModel;
        }

        #endregion

    }
}
