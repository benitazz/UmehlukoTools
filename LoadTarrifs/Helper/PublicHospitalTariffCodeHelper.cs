#region

using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Umehluko.Tools.DataModel;
using Umehluko.Tools.DataModel.DataAccessHelper;
using Umehluko.Tools.Utils.Common;
using Umehluko.Tools.Utils.Extensions;

#endregion

namespace Umehluko.Tools.UI.Helper
{
    /// <summary>
    /// The public hospital tariff code helper.
    /// </summary>
    public class PublicHospitalTariffCodeHelper
    {
        /// <summary>
        /// The uow.
        /// </summary>
        private static readonly GenericUnitOfWork uow = new GenericUnitOfWork();

        /// <summary>
        /// The process upfs.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <exception cref="FileNotFoundException">
        /// The file cannot be found. 
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="path"/> includes an incorrect or invalid syntax for file name, directory name, or volume label. 
        /// </exception>
        public static void ProcessUpfs(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            var scriptFilePath = fileName + "Script" + "_" + DateTime.Now.ToString("MM_dd_yyyy")
                                      + FileType.TextFile.GetDescription();

            if (File.Exists(scriptFilePath))
            {
                File.Delete(scriptFilePath);
            }
            
            var newTariffNotLoadedPath = fileName + "UpfsTariffs" + "_" + DateTime.Now.ToString("MM_dd_yyyy")
                                        + FileType.TextFile.GetDescription();

            if (File.Exists(newTariffNotLoadedPath))
            {
                File.Delete(newTariffNotLoadedPath);
            }

            const string correctName = "UPFS - 2000";
            var tariffBaseUnitCost =
                   uow.Repository<TariffBaseUnitCost>()
                       .Get(
                           baseUnitCost =>
                           (baseUnitCost.Name.Contains(correctName, StringComparison.InvariantCultureIgnoreCase)
                            || correctName.Contains(baseUnitCost.Name)));

            using (var file = new StreamReader(fileName))
            {
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    var lineArray = line.Split(';');

                    if (lineArray.Length < 11)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(lineArray[1]))
                    {
                        continue;
                    }

                    var discipline = lineArray[3];

                    if (string.IsNullOrEmpty(discipline) || !discipline.IsNumeric())
                    {
                        continue;
                    }

                    var medicalItemName = lineArray[2];

                    if (!string.IsNullOrEmpty(medicalItemName))
                    {
                        medicalItemName = medicalItemName.RemoveSpecialCharacters();
                        medicalItemName = Regex.Replace(medicalItemName, @"\s+", " ");
                        medicalItemName = medicalItemName.Trim();
                    }

                    var tariffCode = lineArray[1];
                    var units = lineArray[8];

                    if (!DataAccessHelper.IsMedicalItemExists(tariffCode))
                    {
                        var script = string.Format(
                            @"INSERT INTO [Medical].[MedicalItem]
                                   ([Name]
                                   ,[Description]
                                   ,[ItemCode]
                                   ,[TreatmentCodeID]
                                   ,[NAPPICodeID]
                                   ,[IsActive]
                                   ,[MedicalItemTypeID]
                                   ,[DefaultQuantity]
                                   ,[MinServiceIntervalDays]
                                   ,[AcuteMedicalAuthNeededTypeID]
                                   ,[ChronicMedicalAuthNeededTypeID]
                                   ,[LastChangedBy]
                                   ,[LastChangedDate]
                                   ,[IsAllowSameDayTreatment])
                                VALUES
                                    ('{0}'
                                    ,'{1}'
                                    ,'{2}'
                                    ,0
                                    ,0
                                    ,1
                                    ,1
                                    ,1.00
                                    ,NULL
                                    ,0
                                    ,0
                                    ,'No Access'
                                    ,GETDATE()
                                    ,0)",
                            medicalItemName.Length > 50 ? medicalItemName.Substring(0, 50) : medicalItemName,
                            medicalItemName,
                            tariffCode);

                        scriptFilePath.WriteToFile(script);
                    }

                    if (!DataAccessHelper.IsTariffExists(tariffCode, tariffBaseUnitCost))
                    {
                        var publicHospitalsPracticeNumber = ConfigurationManager.AppSettings[Constant.ProvincialHospitalConfigKey];

                        var publicHospitalNumbers = publicHospitalsPracticeNumber.Split(',');

                        foreach (var practitionerTypeId in publicHospitalNumbers)
                        {
                            var sectionId = 0;
                            var scriptLine =
                             string.Format(
                                 "EXEC Medical.USP_MaintainMedicalTariff '{0}',{1},{2},{3},'{4}','{5}',{6},{7},{8},NULL,'{9}','{10}'",
                                 tariffCode,
                                 Constant.CoidTariffTypeId,
                                 units,
                                 tariffBaseUnitCost.TariffBaseUnitCostID,
                                 tariffBaseUnitCost.ValidFrom,
                                 tariffBaseUnitCost.ValidTo,
                                 practitionerTypeId,
                                 Constant.VatConstId,
                                 sectionId,
                                 "Tariff Upload - Gazette",
                                 DateTime.Now);

                            newTariffNotLoadedPath.WriteToFile(scriptLine);
                        }
                    }

                   /* if (DataAccessHelper.IsMedicalItemExists(itemCode, medicalItemName))
                    {
                       var publicHospitalsPracticeNumber = ConfigurationManager.AppSettings[Constant.ProvincialHospitalConfigKey];

                        var publicHospitalNumbers = publicHospitalsPracticeNumber.Split(',');

                        foreach (var practitionerTypeId in publicHospitalNumbers)
                        {
                            var sectionId = 0;
                               var scriptLine =
                                string.Format(
                                    "EXEC Medical.USP_MaintainMedicalTariff '{0}',{1},{2},{3},'{4}','{5}',{6},{7},{8},NULL,'{9}','{10}'",
                                    tariffCode,
                                    Constant.CoidTariffTypeId,
                                    units,
                                    tariffBaseUnitCost.TariffBaseUnitCostID,
                                    tariffBaseUnitCost.ValidFrom,
                                    tariffBaseUnitCost.ValidTo,
                                    practitionerTypeId,
                                    Constant.VatConstId,
                                    sectionId,
                                    "Tariff Upload - Gazette",
                                    DateTime.Now);

                            newTariffNotLoadedPath.WriteToFile(scriptLine);
                        }    
                    }*/
                    
                }
            }
            
            if (File.Exists(scriptFilePath) && new FileInfo(scriptFilePath).Length > 0)
            {
                Process.Start(scriptFilePath);
            }

            if (File.Exists(newTariffNotLoadedPath) && new FileInfo(newTariffNotLoadedPath).Length > 0)
            {
                Process.Start(newTariffNotLoadedPath);
            }
        }
    }
}