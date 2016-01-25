#region

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using SimpleMvvmToolkit;

using Umehluko.Tools.DataModel;
using Umehluko.Tools.UI.Helper;
using Umehluko.Tools.UI.Models;
using Umehluko.Tools.Utils.Common;
using Umehluko.Tools.Utils.Extensions;

#endregion

namespace Umehluko.Tools.UI.ViewModels
{
    /// <summary>
    /// The tarrif management view model.
    /// </summary>
    public class TarrifManagementViewModel : ViewModelDetailBase<TarrifManagementViewModel, BusyModel>
    {
        /// <summary>
        /// The _file name.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// The _financial start date.
        /// </summary>
        private DateTime _financialStartDate;

        /// <summary>
        /// The _financial end date.
        /// </summary>
        private DateTime _financialEndDate;

        /// <summary>
        /// The tariff types.
        /// </summary>
        private ObservableCollection<string> tariffTypes;

        /// <summary>
        /// The selected tariff type.
        /// </summary>
        private string selectedTariffType;

        /// <summary>
        /// The is busy.
        /// </summary>
        private bool isBusy;

        /// <summary>
        /// The progress text.
        /// </summary>
        private string progressText;

        /// <summary>
        /// The provinces.
        /// </summary>
        private ObservableCollection<string> provinces;

        /// <summary>
        /// The selected province.
        /// </summary>
        private string selectedProvince;

        /// <summary>
        /// </summary>
        private static readonly GenericUnitOfWork uow = new GenericUnitOfWork();

        /// <summary>
        /// Gets or sets the tariff types.
        /// </summary>
        public ObservableCollection<string> TariffTypes
        {
            get
            {
                return this.tariffTypes;
            }

            set
            {
                this.tariffTypes = value;
                this.NotifyPropertyChanged(t => t.TariffTypes);
            }
        }

        /// <summary>
        /// Gets or sets the provinces.
        /// </summary>
        public ObservableCollection<string> Provinces
        {
            get
            {
                return this.provinces;
            }

            set
            {
                this.provinces = value;
                this.NotifyPropertyChanged(t => t.Provinces);
            }
        }

        /// <summary>
        /// Gets or sets the selected province.
        /// </summary>
        public string SelectedProvince
        {
            get
            {
                return this.selectedProvince;
            }

            set
            {
                this.selectedProvince = value;
                this.NotifyPropertyChanged(p => p.SelectedProvince);
            }
        }

        /// <summary>
        /// Gets or sets the selected tariff type.
        /// </summary>
        public string SelectedTariffType
        {
            get
            {
                return this.selectedTariffType;
            }

            set
            {
                this.selectedTariffType = value;
                this.NotifyPropertyChanged(s => s.SelectedTariffType);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.isBusy = value;
                this.NotifyPropertyChanged(s => s.IsBusy);
            }
        }

        /// <summary>
        /// Gets or sets the progress text.
        /// </summary>
        public string ProgressText
        {
            get
            {
                return this.progressText;
            }

            set
            {
                this.progressText = value;
                this.NotifyPropertyChanged(s => s.ProgressText);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TarrifManagementViewModel"/> class.
        /// </summary>
        public TarrifManagementViewModel()
        {
            this.FinancialStartDate = DateTime.Now;
            this.TariffTypes = new ObservableCollection<string>();
            this.IsBusy = false;
            this.ProgressText = "Please Wait!";

            this.TariffTypes = new ObservableCollection<string>(TariffTypeCommonConstant.GetTariffTypes());
            this.Provinces = new ObservableCollection<string>(ProvinceNames.GetProvinces());

            this.selectedTariffType = this.TariffTypes.FirstOrDefault();
        }

        /// <summary>
        /// Gets the upload file command.
        /// </summary>
        public ICommand UploadFileCommand
        {
            get
            {
                return new DelegateCommand(this.UploadFile);
            }
        }

        /// <summary>
        /// Gets the upload file command.
        /// </summary>
        public ICommand CompareCommand
        {
            get
            {
                return new DelegateCommand(this.CompareTariffsProcessor);
            }
        }

        /// <summary>
        /// Gets the upload file command.
        /// </summary>
        public ICommand GenerateCommand
        {
            get
            {
                return new DelegateCommand(this.GenerateScriptProcessor);
            }
        }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName
        {
            get
            {
                return this._fileName;
            }

            set
            {
                this._fileName = value;
                this.NotifyPropertyChanged(f => f.FileName);
            }
        }

        /// <summary>
        /// Gets or sets the financial start date.
        /// </summary>
        public DateTime FinancialStartDate
        {
            get
            {
                return this._financialStartDate;
            }

            set
            {
                this._financialStartDate = value;
                this.NotifyPropertyChanged(f => f.FinancialStartDate);
            }
        }

        /// <summary>
        /// Gets or sets the financial end date.
        /// </summary>
        public DateTime FinancialEndDate
        {
            get
            {
                return this._financialEndDate;
            }

            set
            {
                this._financialEndDate = value;
                this.NotifyPropertyChanged(f => f.FinancialStartDate);
            }
        }

        /// <summary>
        /// The upload file.
        /// </summary>
        public async void CompareTariffsProcessor()
        {
            this.IsBusy = true;
            var financialStart = this.FinancialStartDate.ToString(Constant.DateFormat);
            var financialEnd = FinancialYearHelper.GetFinancialYearEnd(this.FinancialStartDate);

            // this.FinancialEndDate = DateTime.Parse(financialEnd);

            // this.NotifyPropertyChanged(f => f.FinancialEndDate);
            // TariffsDataAccessHelper.ProcessTariffs(financialStart, this.FileName);
            if (this.SelectedTariffType == TariffTypeCommonConstant.BaseCodeTariffs)
            {
                this.ProcessBaseUnitTariffs(financialStart, financialEnd);
                this.IsBusy = false;
                return;
            }

            await
                Task.Run(
                    () => TariffProcessorHelper.ProcessTariffs(financialStart, this.FileName, TariffAction.Compare));

            this.IsBusy = false;
        }

        /// <summary>
        /// The upload file.
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// The file cannot be found. 
        /// </exception>
        public async void GenerateScriptProcessor()
        {
            this.IsBusy = true;
            var financialStart = this.FinancialStartDate.ToString(Constant.DateFormat);
            var financialEnd = FinancialYearHelper.GetFinancialYearEnd(this.FinancialStartDate);

            // this.FinancialEndDate = DateTime.Parse(financialEnd);

            // this.NotifyPropertyChanged(f => f.FinancialEndDate);
            // TariffsDataAccessHelper.ProcessTariffs(financialStart, this.FileName);
            switch (this.SelectedTariffType)
            {
                case TariffTypeCommonConstant.BaseCodeTariffs:
                    {
                        this.ProcessBaseUnitTariffs(financialStart, financialEnd);
                        this.IsBusy = false;
                    }

                    break;
                case TariffTypeCommonConstant.Upfs:
                    {
                        await Task.Run(() => PublicHospitalTariffCodeHelper.ProcessUpfs(this.FileName));
                        this.IsBusy = false;
                    }

                    break;
                case TariffTypeCommonConstant.NormalTariffs:
                    {
                        await
                            Task.Run(
                                () =>
                                TariffProcessorHelper.ProcessTariffs(
                                    financialStart,
                                    this.FileName,
                                    TariffAction.GenerateScript));
                        this.IsBusy = false;
                    }

                    break;
            }
        }

        /// <summary>
        /// The upload file.
        /// </summary>
        public void UploadFile()
        {
            this.IsBusy = true;
            this.FileName = FileHelper.GetFileName();
            this.IsBusy = false;
        }

        /// <summary>
        /// The get tariffs.
        /// </summary>
        /// <param name="financialStart">
        /// The financial start.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task<Tariff[]> GetTariffs(string financialStart)
        {
            var tariffs =
                uow.Repository<Tariff>()
                    .GetAll(
                        p =>
                        p.ValidFrom.GetFormatedDate() == financialStart
                        && p.TariffBaseUnitCostID.ToString().Length == Constant.TbucIdLength
                        && p.TariffTypeID == Constant.CoidTariffTypeId);

            var tariffsArray = tariffs as Tariff[] ?? tariffs.ToArray();
            return tariffsArray;
        }

        /// <summary>
        /// The process base unit tariffs.
        /// </summary>
        /// <param name="financialStart">
        /// The financial start.
        /// </param>
        /// <param name="financialEnd">
        /// The financial End.
        /// </param>
        private void ProcessBaseUnitTariffs(string financialStart, string financialEnd)
        {
            var basecodeunits =
                uow.Repository<TariffBaseUnitCost>().GetAll(
                    p => p.ValidFrom.GetFormatedDate() == financialStart &&

                         // p.ValidTo.GetFormatedDate() == financialEnd &&
                         p.TariffBaseUnitCostID.ToString().Length == Constant.TbucIdLength
                         && p.TariffTypeID == Constant.CoidTariffTypeId).ToList();

            FileHelper.BaseCodeTariffsUpload(FileType.CsvFile, this.FileName, basecodeunits);

            FileHelper.GenerateBaseCodeCript(
                FileType.TextFile,
                this.FileName,
                basecodeunits,
                financialStart,
                financialEnd);
        }
    }
}