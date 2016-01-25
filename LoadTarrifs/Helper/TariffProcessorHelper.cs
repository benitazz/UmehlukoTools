#region

using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;

using Umehluko.Tools.DataModel;
using Umehluko.Tools.DataModel.DataAccessHelper;
using Umehluko.Tools.Utils.Common;
using Umehluko.Tools.Utils.Extensions;

#endregion

namespace Umehluko.Tools.UI.Helper
{
    /// <summary>
    /// The tariff processor helper.
    /// </summary>
    public class TariffProcessorHelper
    {
        /// <summary>
        /// </summary>
        private static readonly GenericUnitOfWork uow = new GenericUnitOfWork();

        /// <summary>
        /// The get tariffs.
        /// </summary>
        /// <param name="startYearAndMonth">
        /// The start Year And Month.
        /// </param>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        /// <param name="tariffAction">
        /// The tariff Action.
        /// </param>
        public static void ProcessTariffs(string startYearAndMonth, string fileName, TariffAction tariffAction)
        {
            using (var dataAccessHelper = new TariffsDataAccessHelper())
            {
                var tariffs = dataAccessHelper.GetTariff(startYearAndMonth, fileName, tariffAction);
                Process(tariffs, fileName, tariffAction, dataAccessHelper, startYearAndMonth);
            }
        }

        /*/// <summary>
        /// The process tariffs async.
        /// </summary>
        /// <param name="tariffs">
        /// The tariffs.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="tariffAction">
        /// The tariff Action.
        /// </param>
        private static async Task ProcessTariffsAsync(
            IQueryable<Tariff> tariffs, 
            string fileName, 
            TariffAction tariffAction)
        {
            Process(tariffs, fileName, tariffAction)
          // await Task.Run(() => Process(tariffs, fileName, tariffAction));
        }*/

        /// <summary>
        /// Processes the specified tariffs.
        /// </summary>
        /// <param name="tariffs">
        /// The tariffs.
        /// </param>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="tariffAction">
        /// The tariff action.
        /// </param>
        /// <param name="dataAccessHelper">
        /// The data access helper.
        /// </param>
        /// <param name="startYearAndMonth">
        /// The start year and month.
        /// </param>
        private static void Process(
            IQueryable<Tariff> tariffs, 
            string fileName, 
            TariffAction tariffAction, 
            TariffsDataAccessHelper dataAccessHelper, 
            string startYearAndMonth)
        {
            if (!tariffs.Any())
            {
                return;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            switch (tariffAction)
            {
                case TariffAction.Compare:
                    CompareTariffs(fileName, tariffs, dataAccessHelper, startYearAndMonth);
                    break;
                case TariffAction.GenerateScript:
                    CreateTariffScript(fileName, tariffs, startYearAndMonth);
                    break;
            }
        }

        /// <summary>
        /// Compares the tariffs.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="tariffs">
        /// The tariffs.
        /// </param>
        /// <param name="dataAccessHelper">
        /// The data access helper.
        /// </param>
        /// <param name="startYearAndMonth">
        /// The start year and month.
        /// </param>
        private static void CompareTariffs(
            string fileName, 
            IQueryable<Tariff> tariffs, 
            TariffsDataAccessHelper dataAccessHelper, 
            string startYearAndMonth)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            if (new FileInfo(fileName).Length == 0)
            {
                return;
            }

            var file = new StreamReader(fileName);

            // var fileExtension = fileType.GetDescription();
            var compareFilePath = fileName + "Compare" + "_" + DateTime.Now.ToString("MM_dd_yyyy")
                                  + FileType.CsvFile.GetDescription();

            var scriptFilePath = fileName + "Script" + "_" + DateTime.Now.ToString("MM_dd_yyyy")
                                 + FileType.TextFile.GetDescription();

            var newTariffNotLoadedPath = fileName + "TariffNotUploaded" + "_" + DateTime.Now.ToString("MM_dd_yyyy")
                                         + FileType.TextFile.GetDescription();

            if (File.Exists(compareFilePath))
            {
                File.Delete(compareFilePath);
            }

            if (File.Exists(scriptFilePath))
            {
                File.Delete(scriptFilePath);
            }

            if (File.Exists(newTariffNotLoadedPath))
            {
                File.Delete(newTariffNotLoadedPath);
            }

            string line;
            var isHeaderCreated = false;

            while ((line = file.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var lineValues = line.Split(';');

                if (!lineValues.Any())
                {
                    continue;
                }

                if (lineValues.Count() <= 14)
                {
                    continue;
                }

                var itemCode = lineValues[0];

                if (itemCode.Contains("Code", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var unitType = lineValues[1];
                var specialistunits = lineValues[2];
                var gpUnits = lineValues[3];
                var specialistAmount = lineValues[9];
                var gpAmount = lineValues[10];
                var specialistAmountVatInclusive = lineValues[12];
                var gpAmountVatInclusive = lineValues[13];

                if (string.IsNullOrEmpty(itemCode) || string.IsNullOrEmpty(unitType) || specialistAmount == "*"
                    || gpUnits == "*" || specialistAmount == "#VALUE!" || gpAmount == "#VALUE!")
                {
                    continue;
                }

                if ((string.IsNullOrEmpty(specialistunits) && string.IsNullOrEmpty(gpUnits))
                    && (string.IsNullOrEmpty(specialistAmount) && string.IsNullOrEmpty(gpAmount))
                    && (string.IsNullOrEmpty(specialistAmountVatInclusive) && string.IsNullOrEmpty(gpAmountVatInclusive)))
                {
                    continue;
                }

                var fileItemCode = itemCode.Trim();
                var fileUnitType = unitType.Trim();

                decimal fileSpecialistRecommendedUnit = 0;

                if (!string.IsNullOrEmpty(specialistunits))
                {
                    fileSpecialistRecommendedUnit = decimal.Parse(specialistunits.Trim());
                }

                decimal fileDoctorRecommendedUnit = 0;

                if (!string.IsNullOrEmpty(gpUnits))
                {
                    fileDoctorRecommendedUnit = decimal.Parse(gpUnits.Trim());
                }

               /* if (!string.IsNullOrEmpty(specialistAmount))
                {
                    var fileSpecialistAmountVatExclusive = specialistAmount.Trim();
                }

                if (!string.IsNullOrEmpty(gpAmount))
                {
                    var fileDoctAmountVatExclusive = gpAmount.Trim();
                }

                if (!string.IsNullOrEmpty(specialistAmountVatInclusive))
                {
                    var fileSpecialistAmountVatInclusive = specialistAmountVatInclusive.Trim();
                }

                var fileDoctorAmountVatInclusive = gpAmountVatInclusive.Trim();*/

                var originalGazetteName = fileUnitType;
                var correctName = fileUnitType.ToUpper().Trim();

                correctName = MapFileName(fileUnitType, correctName);

                //var correctBaseUnitCost1 = DataAccessHelper.GetTbuc(correctName, startYearAndMonth);

                var correctBaseUnitCost1 =
                    uow.Repository<TariffBaseUnitCost>()
                        .Get(
                            baseUnitCost =>
                            (baseUnitCost.Name.Contains(correctName, StringComparison.InvariantCultureIgnoreCase)
                             || correctName.Contains(
                                 baseUnitCost.Name.Substring(baseUnitCost.Name.IndexOf('-') + 2).Trim(), 
                                 StringComparison.InvariantCultureIgnoreCase))
                            && baseUnitCost.ValidFrom.GetFormatedDate().Equals(startYearAndMonth));

                var doctorTariffData = GetTarrifItemCodeData(tariffs, fileItemCode, PracType.Doctor)
                                       ?? GetTarrifItemCodeData(tariffs, fileItemCode, PracType.Specialist);

                if (doctorTariffData == null && correctBaseUnitCost1 != null)
                {
                    InsertTariffCodes(
                        dataAccessHelper, 
                        correctName, 
                        fileItemCode, 
                        fileDoctorRecommendedUnit, 
                        fileSpecialistRecommendedUnit, 
                        correctBaseUnitCost1, 
                        newTariffNotLoadedPath);

                    continue;
                }

                if (doctorTariffData.TariffBaseUnitCost == null)
                {
                    continue;
                }

                var tariffBaseUnitCost = doctorTariffData.TariffBaseUnitCost;

                if (!isHeaderCreated)
                {
                    compareFilePath.WriteToFile(
                        string.Format("{0};{1};{2};{3}", "Database Name", "Gazet Name", "Item code", "BaseCodeUnitID"));

                    isHeaderCreated = true;
                }

                if (!string.IsNullOrEmpty(tariffBaseUnitCost.Name)
                    && (tariffBaseUnitCost.Name.Contains(correctName, StringComparison.InvariantCultureIgnoreCase)
                        || correctName.Contains(
                            tariffBaseUnitCost.Name.Substring(tariffBaseUnitCost.Name.IndexOf('-') + 2).Trim(), 
                            StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                var validFrom = tariffBaseUnitCost.ValidFrom.GetFormatedDate();

                var correctBaseUnitCost =
                    uow.Repository<TariffBaseUnitCost>()
                        .Get(
                            baseUnitCost =>
                            (baseUnitCost.Name.Contains(correctName, StringComparison.InvariantCultureIgnoreCase)
                             || correctName.Contains(
                                 baseUnitCost.Name.Substring(baseUnitCost.Name.IndexOf('-') + 2).Trim(), 
                                 StringComparison.InvariantCultureIgnoreCase))
                            && baseUnitCost.ValidFrom.GetFormatedDate().Equals(validFrom));

                if (correctBaseUnitCost == null)
                {
                    GenerateInsertScript(
                        tariffBaseUnitCost, 
                        fileItemCode, 
                        fileDoctorRecommendedUnit, 
                        newTariffNotLoadedPath, 
                        doctorTariffData, 
                        originalGazetteName, 
                        dataAccessHelper);
                    continue;
                }

                compareFilePath.WriteToFile(
                    string.Format(
                        "{0};{1};{2};{3}", 
                        tariffBaseUnitCost.Name, 
                        originalGazetteName, 
                        doctorTariffData.ItemCode, 
                        tariffBaseUnitCost.TariffBaseUnitCostID));

                var script = string.Format(
                    "UPDATE Medical.Tariff set TariffBaseUnitCostID = {0} where ItemCode = '{1}' and ValidFrom = '{2}' and TariffTypeId = {3}", 
                    correctBaseUnitCost.TariffBaseUnitCostID, 
                    doctorTariffData.ItemCode,
                    correctBaseUnitCost.ValidFrom,
                    Constant.CoidTariffTypeId);

                scriptFilePath.WriteToFile(script);
            }

            file.Close();

            if (File.Exists(compareFilePath) && new FileInfo(compareFilePath).Length > 0)
            {
                System.Diagnostics.Process.Start(compareFilePath);
            }

            if (File.Exists(scriptFilePath) && new FileInfo(scriptFilePath).Length > 0)
            {
                System.Diagnostics.Process.Start(scriptFilePath);
            }

            if (File.Exists(newTariffNotLoadedPath) && new FileInfo(newTariffNotLoadedPath).Length > 0)
            {
                System.Diagnostics.Process.Start(newTariffNotLoadedPath);
            }
        }

        /// <summary>
        /// Inserts the tariff codes.
        /// </summary>
        /// <param name="dataAccessHelper">
        /// The data access helper.
        /// </param>
        /// <param name="correctName">
        /// Name of the correct.
        /// </param>
        /// <param name="fileItemCode">
        /// The file item code.
        /// </param>
        /// <param name="fileDoctorRecommendedUnit">
        /// The file doctor recommended unit.
        /// </param>
        /// <param name="fileSpecialistRecommendedUnit">
        /// The file specialist recommended unit.
        /// </param>
        /// <param name="correctBaseUnitCost1">
        /// The correct base unit cost1.
        /// </param>
        /// <param name="newTariffNotLoadedPath">
        /// The new tariff not loaded path.
        /// </param>
        private static void InsertTariffCodes(
            TariffsDataAccessHelper dataAccessHelper, 
            string correctName, 
            string fileItemCode, 
            decimal fileDoctorRecommendedUnit, 
            decimal fileSpecialistRecommendedUnit, 
            TariffBaseUnitCost correctBaseUnitCost1, 
            string newTariffNotLoadedPath)
        {
            if (correctName.Contains(Constant.RadiationOncology, StringComparison.InvariantCultureIgnoreCase))
            {
                var radiologySpecialistValues = ConfigurationManager.AppSettings[Constant.RadOncoSpecialistConfigKey];
                InsertTariffRecord(
                    dataAccessHelper, 
                    fileItemCode, 
                    fileSpecialistRecommendedUnit, 
                    correctBaseUnitCost1, 
                    newTariffNotLoadedPath, 
                    radiologySpecialistValues);
                return;
            }

            if (correctName.Contains(Constant.ClinicalProcedures, StringComparison.InvariantCultureIgnoreCase))
            {
                var clinicalProcedures = ConfigurationManager.AppSettings[Constant.ClinicalProcedureGPConfigKey];
                var clinicalProceduresSpecialist =
                    ConfigurationManager.AppSettings[Constant.ClinicalProcedureSpecialistConfigKey];

                InsertTariffRecord(
                    dataAccessHelper, 
                    fileItemCode, 
                    fileDoctorRecommendedUnit, 
                    correctBaseUnitCost1, 
                    newTariffNotLoadedPath, 
                    clinicalProcedures);
                InsertTariffRecord(
                    dataAccessHelper, 
                    fileItemCode, 
                    fileSpecialistRecommendedUnit, 
                    correctBaseUnitCost1, 
                    newTariffNotLoadedPath, 
                    clinicalProceduresSpecialist);
                return;
            }

            if (correctName.Contains(Constant.Radiology, StringComparison.InvariantCultureIgnoreCase))
            {
                var radiologistDoctors = ConfigurationManager.AppSettings[Constant.RadiologyGPConfigKey];
                var radiologistDoctorsSpecialist =
                    ConfigurationManager.AppSettings[Constant.RadiologySpecialistConfigKey];

                InsertTariffRecord(
                    dataAccessHelper, 
                    fileItemCode, 
                    fileDoctorRecommendedUnit, 
                    correctBaseUnitCost1, 
                    newTariffNotLoadedPath, 
                    radiologistDoctors);
                InsertTariffRecord(
                    dataAccessHelper, 
                    fileItemCode, 
                    fileSpecialistRecommendedUnit, 
                    correctBaseUnitCost1, 
                    newTariffNotLoadedPath, 
                    radiologistDoctorsSpecialist);
                return;
            }

            if (!correctName.Contains(Constant.Pathology, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var pathologyDoctors = ConfigurationManager.AppSettings[Constant.PathologyGPConfigKey];
            var pathologyDoctorsSpecialist = ConfigurationManager.AppSettings[Constant.PathologySpecialistConfigKey];

            InsertTariffRecord(
                dataAccessHelper, 
                fileItemCode, 
                fileDoctorRecommendedUnit, 
                correctBaseUnitCost1, 
                newTariffNotLoadedPath, 
                pathologyDoctors);
            InsertTariffRecord(
                dataAccessHelper, 
                fileItemCode, 
                fileSpecialistRecommendedUnit, 
                correctBaseUnitCost1, 
                newTariffNotLoadedPath, 
                pathologyDoctorsSpecialist);
        }

        /// <summary>
        /// Inserts the tariff record.
        /// </summary>
        /// <param name="dataAccessHelper">
        /// The data access helper.
        /// </param>
        /// <param name="fileItemCode">
        /// The file item code.
        /// </param>
        /// <param name="recommendedUnits">
        /// The recommended units.
        /// </param>
        /// <param name="correctBaseUnitCost1">
        /// The correct base unit cost1.
        /// </param>
        /// <param name="newTariffNotLoadedPath">
        /// The new tariff not loaded path.
        /// </param>
        /// <param name="radiologySpecialistValues">
        /// The radiology specialist values.
        /// </param>
        private static void InsertTariffRecord(
            TariffsDataAccessHelper dataAccessHelper, 
            string fileItemCode, 
            decimal recommendedUnits, 
            TariffBaseUnitCost correctBaseUnitCost1, 
            string newTariffNotLoadedPath, 
            string radiologySpecialistValues)
        {
            if (recommendedUnits == 0)
            {
                return;
            }

            InsertNewTariffScript(
                radiologySpecialistValues, 
                fileItemCode, 
                recommendedUnits, 
                correctBaseUnitCost1, 
                newTariffNotLoadedPath, 
                dataAccessHelper);
        }

        /// <summary>
        /// The map file name.
        /// </summary>
        /// <param name="fileUnitType">
        /// The file unit type.
        /// </param>
        /// <param name="correctName">
        /// The correct name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string MapFileName(string fileUnitType, string correctName)
        {
            switch (fileUnitType.ToUpper().Trim())
            {
                case Constant.UnitTypeAntPath:
                    {
                        correctName = Constant.AnatomicalPathalogy;
                    }

                    break;
                case Constant.UnitTypeComTomogr:
                    {
                        correctName = Constant.ComputedTomograpphy;
                    }

                    break;
                case Constant.UnitTypeClinPath:
                    {
                        correctName = Constant.ClinicalPathology;
                    }

                    break;
                case Constant.UnitTypeClinical:
                    {
                        correctName = Constant.ClinicalProcedures;
                    }

                    break;
                case Constant.UnitTypeRadOnc:
                    {
                        correctName = Constant.RadiationOncology;
                    }

                    break;
                default:
                    correctName = fileUnitType;
                    break;
            }

            return correctName;
        }

        /// <summary>
        /// Generates the insert script.
        /// </summary>
        /// <param name="tariffBaseUnitCost">
        /// The tariff base unit cost.
        /// </param>
        /// <param name="fileItemCode">
        /// The file item code.
        /// </param>
        /// <param name="fileDoctorRecommendedUnit">
        /// The file doctor recommended unit.
        /// </param>
        /// <param name="newTariffNotLoadedPath">
        /// The new tariff not loaded path.
        /// </param>
        /// <param name="doctorTariffData">
        /// The doctor tariff data.
        /// </param>
        /// <param name="originalGazetteName">
        /// Name of the original gazette.
        /// </param>
        /// <param name="dataAccessHelper">
        /// The data access helper.
        /// </param>
        private static void GenerateInsertScript(
            TariffBaseUnitCost tariffBaseUnitCost, 
            string fileItemCode, 
            decimal fileDoctorRecommendedUnit, 
            string newTariffNotLoadedPath, 
            Tariff doctorTariffData, 
            string originalGazetteName, 
            TariffsDataAccessHelper dataAccessHelper)
        {
            var databaseName = tariffBaseUnitCost.Name;

            var pathologyGPValues = ConfigurationManager.AppSettings[Constant.PathologyGPConfigKey];
            var pathologySpecialistValues = ConfigurationManager.AppSettings[Constant.PathologySpecialistConfigKey];
            var clinicalProceduresGPValues = ConfigurationManager.AppSettings[Constant.ClinicalProcedureGPConfigKey];
            var clinicalProceduresSpecialistValues =
                ConfigurationManager.AppSettings[Constant.ClinicalProcedureSpecialistConfigKey];
            var radiologyGpValues = ConfigurationManager.AppSettings[Constant.RadiologyGPConfigKey];
            var radiologySpecialistValues = ConfigurationManager.AppSettings[Constant.RadiologySpecialistConfigKey];

            if (databaseName.Contains(Constant.Pathology, StringComparison.InvariantCultureIgnoreCase))
            {
                InsertNewTariffScript(
                    pathologyGPValues, 
                    fileItemCode, 
                    fileDoctorRecommendedUnit, 
                    tariffBaseUnitCost, 
                    newTariffNotLoadedPath, 
                    dataAccessHelper);

                return;
            }

            if (databaseName.Contains(Constant.Radiology, StringComparison.InvariantCultureIgnoreCase))
            {
                InsertNewTariffScript(
                    radiologyGpValues, 
                    fileItemCode, 
                    fileDoctorRecommendedUnit, 
                    tariffBaseUnitCost, 
                    newTariffNotLoadedPath, 
                    dataAccessHelper);

                return;
            }

            newTariffNotLoadedPath.WriteToFile(
                string.Format("{0} - {1}", doctorTariffData.ItemCode, originalGazetteName));
        }

        /// <summary>
        /// Inserts the new tariff script.
        /// </summary>
        /// <param name="practitionerTypeIds">
        /// The practitioner type ids.
        /// </param>
        /// <param name="fileItemCode">
        /// The file item code.
        /// </param>
        /// <param name="fileDoctorRecommendedUnit">
        /// The file doctor recommended unit.
        /// </param>
        /// <param name="tariffBaseUnitCost">
        /// The tariff base unit cost.
        /// </param>
        /// <param name="newTariffNotLoadedPath">
        /// The new tariff not loaded path.
        /// </param>
        /// <param name="dataAccessHelper">
        /// The data access helper.
        /// </param>
        private static void InsertNewTariffScript(
            string practitionerTypeIds, 
            string fileItemCode, 
            decimal fileDoctorRecommendedUnit, 
            TariffBaseUnitCost tariffBaseUnitCost, 
            string newTariffNotLoadedPath, 
            TariffsDataAccessHelper dataAccessHelper)
        {
            if (string.IsNullOrEmpty(practitionerTypeIds))
            {
                return;
            }

            var pathologyPractitionerTypes = practitionerTypeIds.Split(',');

            var units = fileDoctorRecommendedUnit.ToString(CultureInfo.InvariantCulture).Replace(',', '.');

            var section = dataAccessHelper.GetTariff(fileItemCode);

            /*DateTime? validFrom = tariffBaseUnitCost.ValidFrom;
            var currentDateTime = DateTime.Now;
            DateTime? validTo = tariffBaseUnitCost.ValidTo;
            
            if (validFrom != null && validFrom.Value.Year < currentDateTime.Year)
            {
                var financialEnd = FinancialYearHelper.GetFinancialYearEnd(validFrom.Value);
                 validTo = DateTime.Parse(financialEnd);
            }*/
            foreach (var practitionerTypeId in pathologyPractitionerTypes)
            {
                var sectionId = 0;

                if (section != null)
                {
                    sectionId = section.SectionID;
                }

                var validfromDate = tariffBaseUnitCost.ValidFrom;

                var financialStart = string.Empty;
                var financialEnd = string.Empty;

                if (validfromDate.HasValue)
                {
                    financialStart = FinancialYearHelper.GetFinancialYearStart(validfromDate.Value);
                    financialEnd =
                        FinancialYearHelper.GetFinancialYearEnd(
                            DateTime.ParseExact(financialStart, Constant.DateFormat, null));
                }

                var scriptLine =
                    string.Format(
                        "EXEC Medical.USP_MaintainMedicalTariff '{0}',{1},{2},{3},'{4}','{5}',{6},{7},{8},NULL,'{9}','{10}'", 
                        fileItemCode, 
                        Constant.CoidTariffTypeId, 
                        units, 
                        tariffBaseUnitCost.TariffBaseUnitCostID, 
                        financialStart, 
                        financialEnd, 
                        practitionerTypeId, 
                        Constant.VatConstId, 
                        sectionId, 
                        "Tariff Upload - Gazzette", 
                        DateTime.Now);

                newTariffNotLoadedPath.WriteToFile(scriptLine);
            }
        }

        /// <summary>
        /// Creates the tariff script.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="tariffs">
        /// The tariffs.
        /// </param>
        /// <param name="startYearAndMonth">
        /// The start year and month.
        /// </param>
        private static void CreateTariffScript(string fileName, IQueryable<Tariff> tariffs, string startYearAndMonth)
        {
            // var fileExtension = fileType.GetDescription();
            var path = fileName + "_" + DateTime.Now.ToString("MM_dd_yyyy"); // + fileExtension;

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var sw = File.CreateText(path))
            {
                var file = new StreamReader(fileName);
                string line;
                var isHeaderCreated = false;

                while ((line = file.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    var lineValues = line.Split(';');

                    if (!lineValues.Any())
                    {
                        continue;
                    }

                    if (lineValues.Count() <= 14)
                    {
                        continue;
                    }

                    var itemCode = lineValues[0];

                    if (itemCode.Contains("Code", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    var unitType = lineValues[1];
                    var specialistunits = lineValues[2];
                    var gpUnits = lineValues[3];
                    var specialistAmount = lineValues[9];
                    var gpAmount = lineValues[10];
                    var specialistAmountVatInclusive = lineValues[12];
                    var gpAmountVatInclusive = lineValues[13];

                    if (string.IsNullOrEmpty(itemCode) || string.IsNullOrEmpty(unitType)
                        || string.IsNullOrEmpty(specialistunits) || string.IsNullOrEmpty(gpUnits)
                        || string.IsNullOrEmpty(specialistAmount) || string.IsNullOrEmpty(gpAmount)
                        || string.IsNullOrEmpty(specialistAmountVatInclusive)
                        || string.IsNullOrEmpty(gpAmountVatInclusive) || specialistAmount == "*" || gpUnits == "*"
                        || specialistAmount == "#VALUE!" || gpAmount == "#VALUE!")
                    {
                        continue;
                    }

                    var fileItemCode = itemCode.Trim();
                    var fileUnitType = unitType.Trim();
                    var fileSpecialistRecommendedUnit = decimal.Parse(specialistunits.Trim());
                    var fileDoctorRecommendedUnit = decimal.Parse(gpUnits.Trim());
                    var fileSpecialistAmountVatExclusive = specialistAmount.Trim();
                    var fileDoctAmountVatExclusive = gpAmount.Trim();
                    var fileSpecialistAmountVatInclusive = specialistAmountVatInclusive.Trim();
                    var fileDoctorAmountVatInclusive = gpAmountVatInclusive.Trim();

                    var originalGazetName = fileUnitType;
                    var correctName = fileUnitType.ToUpper().Trim();

                    switch (fileUnitType.ToUpper().Trim())
                    {
                        case Constant.UnitTypeAntPath:
                            {
                                correctName = Constant.AnatomicalPathalogy;
                            }

                            break;
                        case Constant.UnitTypeComTomogr:
                            {
                                correctName = Constant.ComputedTomograpphy;
                            }

                            break;
                        case Constant.UnitTypeClinPath:
                            {
                                correctName = Constant.ClinicalPathology;
                            }

                            break;
                        case Constant.UnitTypeClinical:
                            {
                                correctName = Constant.ClinicalProcedures;
                            }

                            break;
                    }

                    var doctorTariffData = GetTarrifData(tariffs, fileItemCode, correctName, PracType.Doctor);

                    if (doctorTariffData != null && doctorTariffData.RecommendedUnits != fileDoctorRecommendedUnit)
                    {
                        sw.WriteLine(
                            UpdateScript(
                                fileDoctorRecommendedUnit, 
                                fileItemCode, 
                                doctorTariffData.ValidFrom, 
                                doctorTariffData.TariffBaseUnitCost.Name, 
                                0));
                    }

                    ProcessSpecialistUpdate(
                        tariffs, 
                        fileItemCode, 
                        correctName, 
                        fileSpecialistRecommendedUnit, 
                        fileDoctorRecommendedUnit, 
                        sw);

                    // ProcessSpecialistUpdate(specialistTariffData, fileSpecialistRecommendedUnit, correctName, streamWriter, fileItemCode);

                    /*if (!isHeaderCreated)
                    {
                        streamWriter.WriteLine("{0};{1};{2};{3};{4};{5};{6}", "Database Name", "Gazet Name", "Database Value", "Gazzet Value", "Gazzet Value VAT Inclusive", "Valid From", "Valid To");
                        isHeaderCreated = true;
                    }

                    if (tbuc == null)
                    {
                        //Insert new tariff
                    }
                    else
                    {
                        var vatInclusive = double.Parse(fileSpecialityValue) * Constant.Vat;
                        streamWriter.WriteLine("{0};{1};{2};{3};{4};{5};{6}", tbuc.Name, originalGazetteName, tbuc.UnitPrice, fileSpecialityValue, vatInclusive, tbuc.ValidFrom, tbuc.ValidTo);
                        //update tariff
                    }*/
                }

                file.Close();
            }

            if (File.Exists(path) && new FileInfo(path).Length > 0)
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        /// <summary>
        /// Processes the specialist update.
        /// </summary>
        /// <param name="tariffs">
        /// The tariffs.
        /// </param>
        /// <param name="fileItemCode">
        /// The file item code.
        /// </param>
        /// <param name="correctName">
        /// Name of the correct.
        /// </param>
        /// <param name="fileSpecialistRecommendedUnit">
        /// The file specialist recommended unit.
        /// </param>
        /// <param name="doctorRecommendedUnit">
        /// The doctor recommended unit.
        /// </param>
        /// <param name="streamWriter">
        /// The stream writer.
        /// </param>
        private static void ProcessSpecialistUpdate(
            IQueryable<Tariff> tariffs, 
            string fileItemCode, 
            string correctName, 
            decimal fileSpecialistRecommendedUnit, 
            decimal doctorRecommendedUnit, 
            StreamWriter streamWriter)
        {
            var pracValue = (int)PracType.Specialist;

            var specialistData =
                tariffs.Where(
                    t =>
                    t.ItemCode == fileItemCode && t.PractitionerType.IsSpecialist == pracValue
                    && t.RecommendedUnits != fileSpecialistRecommendedUnit);

            if (!specialistData.Any())
            {
                return;
            }

            if (correctName.Contains(Constant.Pathology, StringComparison.InvariantCultureIgnoreCase))
            {
                correctName = Constant.Pathology;
            }

            foreach (var specialistTarifData in specialistData)
            {
                if (specialistTarifData == null || specialistTarifData.RecommendedUnits == fileSpecialistRecommendedUnit)
                {
                    continue;
                }

                var recommendedUnits = GetSpecialistRecommendedUnits(
                    correctName, 
                    fileSpecialistRecommendedUnit, 
                    doctorRecommendedUnit, 
                    specialistTarifData);

                if (recommendedUnits == specialistTarifData.RecommendedUnits)
                {
                    continue;
                }

                streamWriter.WriteLine(
                    UpdateSpecialistScript(
                        recommendedUnits, 
                        fileItemCode, 
                        specialistTarifData.ValidFrom, 
                        specialistTarifData.TariffBaseUnitCost.Name, 
                        pracValue, 
                        specialistTarifData.TariffID));
            }
        }

        /// <summary>
        /// Gets the specialist recommended units.
        /// </summary>
        /// <param name="correctName">
        /// Name of the correct.
        /// </param>
        /// <param name="fileSpecialistRecommendedUnit">
        /// The file specialist recommended unit.
        /// </param>
        /// <param name="doctorRecommendedUnit">
        /// The doctor recommended unit.
        /// </param>
        /// <param name="specialistTariffData">
        /// The specialist tariff data.
        /// </param>
        /// <returns>
        /// The calculated specialist amount
        /// </returns>
        private static decimal GetSpecialistRecommendedUnits(
            string correctName, 
            decimal fileSpecialistRecommendedUnit, 
            decimal doctorRecommendedUnit, 
            Tariff specialistTariffData)
        {
            if (specialistTariffData.PractitionerType != null
                && !string.IsNullOrEmpty(specialistTariffData.PractitionerType.Name)
                && specialistTariffData.PractitionerType.Name.Trim()
                       .Contains(correctName, StringComparison.InvariantCultureIgnoreCase))
            {
                return fileSpecialistRecommendedUnit;
            }

            if (specialistTariffData.PractitionerType != null
                && (correctName.Equals(Constant.Pathology, StringComparison.InvariantCultureIgnoreCase)
                    && !specialistTariffData.PractitionerType.Name.Trim()
                            .Contains(Constant.Pathology, StringComparison.InvariantCultureIgnoreCase)))
            {
                return doctorRecommendedUnit;
            }

            if (specialistTariffData.PractitionerType != null
                && (correctName.Equals(Constant.Radiology, StringComparison.CurrentCultureIgnoreCase)
                    && !specialistTariffData.PractitionerType.Name.Trim()
                            .Contains(Constant.Radiology, StringComparison.InvariantCultureIgnoreCase)))
            {
                return doctorRecommendedUnit;
            }

            if (specialistTariffData.PractitionerType != null
                && (!correctName.Equals(Constant.Pathology, StringComparison.InvariantCultureIgnoreCase)
                    && specialistTariffData.PractitionerType.Name.Trim()
                           .Contains(Constant.Pathology, StringComparison.InvariantCultureIgnoreCase)))
            {
                return doctorRecommendedUnit;
            }

            if (specialistTariffData.PractitionerType != null
                && (!correctName.Equals(Constant.Radiology, StringComparison.InvariantCultureIgnoreCase)
                    && specialistTariffData.PractitionerType.Name.Trim()
                           .Contains(Constant.Radiology, StringComparison.InvariantCultureIgnoreCase)))
            {
                return doctorRecommendedUnit;
            }

            return fileSpecialistRecommendedUnit;
        }

        /// <summary>
        /// Gets the tarrif item code data.
        /// </summary>
        /// <param name="tariffs">
        /// The tariffs.
        /// </param>
        /// <param name="fileItemCode">
        /// The file item code.
        /// </param>
        /// <param name="practitionerType">
        /// Type of the practitioner.
        /// </param>
        /// <returns>
        /// The <see cref="Tariff"/>.
        /// </returns>
        private static Tariff GetTarrifItemCodeData(
            IQueryable<Tariff> tariffs, 
            string fileItemCode, 
            PracType practitionerType)
        {
            var pracValue = (int)practitionerType;

            var values =
                tariffs.Where(t => t.ItemCode == fileItemCode && t.PractitionerType.IsSpecialist == pracValue).ToList();

            return values.FirstOrDefault();
        }

        /* private static string GetCorrectTbuc(IQueryable<Tariff> tariffs,
            string fileItemCode)
        {
            
        }**/

        /// <summary>
        /// Gets the tarrif data.
        /// </summary>
        /// <param name="tariffs">
        /// The tariffs.
        /// </param>
        /// <param name="fileItemCode">
        /// The file item code.
        /// </param>
        /// <param name="correctName">
        /// Name of the correct.
        /// </param>
        /// <param name="practitionerType">
        /// Type of the practitioner.
        /// </param>
        /// <returns>
        /// The <see cref="Tariff"/>.
        /// </returns>
        private static Tariff GetTarrifData(
            IQueryable<Tariff> tariffs, 
            string fileItemCode, 
            string correctName, 
            PracType practitionerType)
        {
            var pracValue = (int)practitionerType;

            var values =
                tariffs.Where(t => t.ItemCode == fileItemCode && t.PractitionerType.IsSpecialist == pracValue).ToList();

            if (!values.Any())
            {
                return null;
            }

            return
                values.FirstOrDefault(
                    t =>
                    t.TariffBaseUnitCost.Name.Contains(correctName, StringComparison.InvariantCultureIgnoreCase)
                    && correctName.Contains(
                        t.TariffBaseUnitCost.Name.Substring(t.TariffBaseUnitCost.Name.IndexOf('-') + 2).Trim(), 
                        StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Updates the specialist script.
        /// </summary>
        /// <param name="recommendedUnits">
        /// The recommended units.
        /// </param>
        /// <param name="itemCode">
        /// The item code.
        /// </param>
        /// <param name="validFrom">
        /// The valid from.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="isSpecialist">
        /// The is specialist.
        /// </param>
        /// <param name="tariffId">
        /// The tariff identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string UpdateSpecialistScript(
            decimal recommendedUnits, 
            string itemCode, 
            DateTime validFrom, 
            string name, 
            int isSpecialist, 
            int tariffId)
        {
            var strRecommendedUnit = recommendedUnits.ToString(CultureInfo.CurrentCulture);
            strRecommendedUnit = strRecommendedUnit.Replace(",", ".");

            return
                string.Format(
                    @"UPDATE t SET t.RecommendedUnits = '{0}' FROM Medical.Tariff t(NOLOCK) INNER JOIN Medical.PractitionerType pt(NOLOCK) ON t.PractitionerTypeID = pt.PractitionerTypeID INNER JOIN Medical.TariffBaseUnitCost tbuc(NOLOCK) ON tbuc.TariffBaseUnitCostID = t.TariffBaseUnitCostID WHERE pt.IsSpecialist = {4} AND t.ItemCode = '{1}'  AND t.ValidFrom = '{2}' and tbuc.Name = '{3}' and t.TariffId = {5};", 
                    strRecommendedUnit, 
                    itemCode, 
                    validFrom, 
                    name, 
                    isSpecialist, 
                    tariffId);
        }

        /// <summary>
        /// The update script.
        /// </summary>
        /// <param name="recommendedUnits">
        /// The recommended units.
        /// </param>
        /// <param name="itemCode">
        /// The item code.
        /// </param>
        /// <param name="validFrom">
        /// The valid from.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="isSpecialist">
        /// The is specialist.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string UpdateScript(
            decimal recommendedUnits, 
            string itemCode, 
            DateTime validFrom, 
            string name, 
            int isSpecialist)
        {
            return
                string.Format(
                    @"UPDATE t SET t.RecommendedUnits = {0} FROM Medical.Tariff t(NOLOCK) INNER JOIN Medical.PractitionerType pt(NOLOCK) ON t.PractitionerTypeID = pt.PractitionerTypeID INNER JOIN Medical.TariffBaseUnitCost tbuc(NOLOCK) ON tbuc.TariffBaseUnitCostID = t.TariffBaseUnitCostID WHERE pt.IsSpecialist = {4} AND t.ItemCode = '{1}'  AND t.ValidFrom = '{2}' and tbuc.Name = '{3}';", 
                    recommendedUnits, 
                    itemCode, 
                    validFrom, 
                    name, 
                    isSpecialist);
        }
    }
}