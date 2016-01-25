#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using Microsoft.Win32;

using Umehluko.Tools.DataModel;
using Umehluko.Tools.Utils.Common;
using Umehluko.Tools.Utils.Extensions;

#endregion

namespace Umehluko.Tools.UI.Helper
{
    /// <summary>
    /// The file helper.
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// The get file name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetFileName()
        {
            // Create OpenFileDialog
            var dlg = new OpenFileDialog { DefaultExt = ".csv", Filter = "Text documents (.csv)|*.csv" };

            // Set filter for file extension and default file extension

            // Display OpenFileDialog by calling ShowDialog method
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            return result == true ? dlg.FileName : null;
        }


        /// <summary>
        /// The base code tariffs upload.
        /// </summary>
        /// <param name="fileType">
        /// The file type.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="tariffs">
        /// The tariffs.
        /// </param>
        public static void CreateTBUC(FileType fileType, string filename, List<TariffBaseUnitCost> tariffs)
        {
            if (!tariffs.Any())
            {
                return;
            }

            var fileExtension = fileType.GetDescription();

            var path = filename + "_" + DateTime.Now.ToString("MM_dd_yyyy H-mm-ss") + fileExtension;

            if (File.Exists(path))
            {
                File.Delete(path);
            }


            /*var fileSpecialityName = speciality.Trim();
            var fileSpecialityValue = specialityValue.Trim();
            // fileSpecialityValue = fileSpecialityValue.Replace(",", ".");

            fileSpecialityName = MapTBUCNames(fileSpecialityName);

            var price = double.Parse(fileSpecialityValue) * Constant.Vat;
            var vatDecimal = (decimal)price;

            var unitPrice = price.ToString(CultureInfo.CurrentCulture).Replace(",", ".");


            var name = string.Format("COIDA {0} - {1}", year, fileSpecialityName);
            var description = string.Format("{0} @ {1} Excl", name, fileSpecialityValue);

            sw.WriteLine(
                "INSERT INTO [Medical].[TariffBaseUnitCost] ([Name],[Description],[PublicationID],[UnitPrice],[UnitTypeID],[ValidFrom],[ValidTo],[TariffTypeID],[LastChangedBy],[LastChangedDate],[TariffBaseUnitCostTypeId],[IsCopiedFromNRPL]) VALUES('{0}','{1}',{2},'{3}',{4},'{5}','{6}',{7},'{8}','{9}',{10},{11});",
                name,
                description,
                2,
                unitPrice,
                2,
                validFromDateTime,
                validToDateTime,
                2,
                "Tariff Upload",
                DateTime.Now,
                5,
                0);*/

        }

        /// <summary>
        /// The base code tariffs upload.
        /// </summary>
        /// <param name="fileType">
        /// The file type.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="tariffs">
        /// The tariffs.
        /// </param>
        public static void BaseCodeTariffsUpload(FileType fileType, string filename, List<TariffBaseUnitCost> tariffs)
        {
            if (!tariffs.Any())
            {
                return;
            }

            var fileExtension = fileType.GetDescription();

            var path = filename + "_" + DateTime.Now.ToString("MM_dd_yyyy H-mm-ss") + fileExtension;

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var sw = File.CreateText(path))
            {
                var file = new StreamReader(filename);
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

                    if (lineValues.Count() <= 4)
                    {
                        continue;
                    }

                    var speciality = lineValues[0];
                    var specialityValue = string.IsNullOrEmpty(lineValues[1]) ? lineValues[4] : lineValues[1];

                    if (string.IsNullOrEmpty(specialityValue) || string.IsNullOrEmpty(speciality))
                    {
                        continue;
                    }

                    var fileSpecialityName = speciality.Trim();
                    var fileSpecialityValue = specialityValue.Trim();

                    var originalGazetName = fileSpecialityName;

                    fileSpecialityName = MapTBUCNames(fileSpecialityName);

                    var tbuc =
                        tariffs.FirstOrDefault(
                            tariffBaseUnitCost =>
                            !string.IsNullOrEmpty(tariffBaseUnitCost.Name)
                            && (tariffBaseUnitCost.Name.Contains(
                                fileSpecialityName,
                                StringComparison.InvariantCultureIgnoreCase)
                                || fileSpecialityName.Contains(
                                    tariffBaseUnitCost.Name.Substring(tariffBaseUnitCost.Name.IndexOf('-') + 2).Trim(),
                                    StringComparison.InvariantCultureIgnoreCase)));

                    if (!isHeaderCreated)
                    {
                        sw.WriteLine(
                            "{0};{1};{2};{3};{4};{5};{6};{7}",
                            "Database Name",
                            "Gazet Name",
                            "Database Value",
                            "Gazzet Value",
                            "Gazzet Value VAT Inclusive",
                            "Valid From",
                            "Valid To",
                            "TariffBaseUnitCostID");

                        isHeaderCreated = true;
                    }

                    if (tbuc == null)
                    {
                        // Insert new tariff
                    }
                    else
                    {
                        var vatInclusive = double.Parse(fileSpecialityValue) * Constant.Vat;

                        sw.WriteLine(
                            "{0};{1};{2};{3};{4};{5};{6};{7}",
                            tbuc.Name,
                            originalGazetName,
                            tbuc.UnitPrice,
                            fileSpecialityValue,
                            vatInclusive,
                            tbuc.ValidFrom,
                            tbuc.ValidTo,
                            tbuc.TariffBaseUnitCostID);

                        // update tariff
                    }
                }

                file.Close();
            }

            Process.Start(path);
        }

        /// <summary>
        /// The generate base code cript.
        /// </summary>
        /// <param name="fileType">
        /// The file type.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="tariffs">
        /// The tariffs.
        /// </param>
        /// <param name="validFrom">
        /// The valid from.
        /// </param>
        /// <param name="validTo">
        /// The valid to.
        /// </param>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null. </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="decimals" /> is less than 0 or greater than 28. </exception>
        public static void GenerateBaseCodeCript(
            FileType fileType,
            string filename,
            List<TariffBaseUnitCost> tariffs,
            string validFrom,
            string validTo)
        {
            var fileExtension = fileType.GetDescription();

            var path = filename + "_" + DateTime.Now.ToString("MM_dd_yyyy H-mm-ss") + "_Script" + fileExtension;
            var year = DateTime.ParseExact(validFrom, Constant.DateFormat, null).Year;
            var validFromDateTime = DateTime.ParseExact(validFrom, Constant.DateFormat, null);
            var validToDateTime = DateTime.ParseExact(validTo, Constant.DateFormat, null).AddHours(23);
            validToDateTime = validToDateTime.AddMinutes(59);
            validToDateTime = validToDateTime.AddSeconds(59);

            using (var sw = File.CreateText(path))
            {
                var file = new StreamReader(filename);
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

                    if (lineValues.Count() <= 1)
                    {
                        continue;
                    }

                    var speciality = lineValues[0];
                    var specialityValue = string.IsNullOrEmpty(lineValues[1]) ? lineValues[4] : lineValues[1];

                    if (string.IsNullOrEmpty(specialityValue) || string.IsNullOrEmpty(speciality))
                    {
                        continue;
                    }

                    var fileSpecialityName = speciality.Trim();
                    var fileSpecialityValue = specialityValue.Trim();
                    // fileSpecialityValue = fileSpecialityValue.Replace(",", ".");

                    fileSpecialityName = MapTBUCNames(fileSpecialityName);

                    var tbuc =
                        tariffs.FirstOrDefault(
                            tariffBaseUnitCost =>
                            !string.IsNullOrEmpty(tariffBaseUnitCost.Name)
                            && (tariffBaseUnitCost.Name.Contains(
                                fileSpecialityName,
                                StringComparison.InvariantCultureIgnoreCase)
                                || fileSpecialityName.Contains(
                                    tariffBaseUnitCost.Name.Substring(tariffBaseUnitCost.Name.IndexOf('-') + 2).Trim(),
                                    StringComparison.InvariantCultureIgnoreCase)));

                    var price = double.Parse(fileSpecialityValue) * Constant.Vat;
                    price = Math.Round(price, 2);
                    var vatDecimal = (decimal)price;
                    vatDecimal = Math.Round(vatDecimal, 2);

                    var unitPrice = price.ToString(CultureInfo.CurrentCulture).Replace(",", ".");

                    if (tbuc == null)
                    {
                        var name = string.Format("COIDA {0} - {1}", year, fileSpecialityName);
                        var description = string.Format("{0} @ {1} Excl", name, fileSpecialityValue);

                        sw.WriteLine(
                            "INSERT INTO [Medical].[TariffBaseUnitCost] ([Name],[Description],[PublicationID],[UnitPrice],[UnitTypeID],[ValidFrom],[ValidTo],[TariffTypeID],[LastChangedBy],[LastChangedDate],[TariffBaseUnitCostTypeId],[IsCopiedFromNRPL]) VALUES('{0}','{1}',{2},'{3}',{4},'{5}','{6}',{7},'{8}','{9}',{10},{11});",
                            name,
                            description,
                            2,
                            unitPrice,
                            2,
                            validFromDateTime,
                            validToDateTime,
                            2,
                            "Tariff Upload",
                            DateTime.Now,
                            5,
                            0);
                    }

                    var dbUnitPrice = Math.Round(tbuc.UnitPrice, 2);

                    if (tbuc != null && vatDecimal != dbUnitPrice)
                    {
                        sw.WriteLine(
                            "UPDATE medical.TariffBaseUnitCost SET UnitPrice = '{0}' WHERE TariffBaseUnitCostID = {1};",
                            unitPrice,
                            tbuc.TariffBaseUnitCostID);
                    }
                }

                file.Close();
            }

            if (File.Exists(path))
            {
                Process.Start(path);
            }
        }

        /// <summary>
        /// The map tbuc names.
        /// </summary>
        /// <param name="fileSpecialityName">
        /// The file speciality name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string MapTBUCNames(string fileSpecialityName)
        {
            switch (fileSpecialityName.ToUpper())
            {
                case Constant.IncorrectAnatomicalPathalogy:
                    {
                        return Constant.AnatomicalPathalogy;
                    }

                case Constant.IncorrectClinicalPathology:
                    {
                        return Constant.ClinicalPathology;
                    }

                default:
                    {
                        return fileSpecialityName;
                    }
            }
        }

        /// <summary>
        /// The tariffs upload.
        /// </summary>
        /// <param name="fileType">
        /// The file type.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="tariffs">
        /// The tariffs.
        /// </param>
        /// <exception cref="Win32Exception">An error occurred when opening the associated file. </exception>
        public static void TariffsUpload(FileType fileType, string filename, List<Tariff> tariffs)
        {
            if (!tariffs.Any())
            {
                return;
            }

            var fileExtension = fileType.GetDescription();

            var path = filename + "_" + DateTime.Now.ToString("MM_dd_yyyy") + fileExtension;

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var sw = File.CreateText(path))
            {
                var file = new StreamReader(filename);
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
                        || string.IsNullOrEmpty(gpAmountVatInclusive))
                    {
                        continue;
                    }

                    var fileItemCode = itemCode.Trim();
                    var fileUnitType = unitType.Trim();
                    var fileSpecialistUnit = specialistunits.Trim();
                    var fileGpUnit = gpUnits.Trim();
                    var fileSpecialistAmount = specialistAmount.Trim();
                    var fileGpAmount = gpAmount.Trim();
                    var fileSpecialistAmountVat = specialistAmountVatInclusive.Trim();
                    var fileGpAmountVat = gpAmountVatInclusive.Trim();

                    var originalGazetName = fileType;

                    switch (fileType)
                    {
                    }

                    var tarrifData =
                        tariffs.FirstOrDefault(
                            tariff =>
                            tariff.ItemCode == fileItemCode && tariff.TariffTypeID == Constant.CoidTariffTypeId
                            && tariff.TariffBaseUnitCostID.ToString().Length == 4);

                    /*if (!isHeaderCreated)
                    {
                        sw.WriteLine("{0};{1};{2};{3};{4};{5};{6}", "Database Name", "Gazet Name", "Database Value", "Gazzet Value", "Gazzet Value VAT Inclusive", "Valid From", "Valid To");
                        isHeaderCreated = true;
                    }

                    if (tbuc == null)
                    {
                        //Insert new tariff
                    }
                    else
                    {
                        var vatInclusive = double.Parse(fileSpecialityValue) * Constant.Vat;
                        sw.WriteLine("{0};{1};{2};{3};{4};{5};{6}", tbuc.Name, originalGazetName, tbuc.UnitPrice, fileSpecialityValue, vatInclusive, tbuc.ValidFrom, tbuc.ValidTo);
                        //update tariff
                    }*/
                }

                file.Close();
            }

            if (File.Exists(path))
            {
                Process.Start(path);
            }
        }
    }
}