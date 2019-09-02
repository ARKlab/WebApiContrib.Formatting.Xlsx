﻿using System;
using System.Data;
using System.Linq;
using SQAD.MTNext.Business.Models.Core.DeliverySource;
using SQAD.MTNext.Interfaces.WebApiContrib.Formatting.Xlsx.Interfaces;
using SQAD.MTNext.WebApiContrib.Formatting.Xlsx.Serialisation.Base;
using SQAD.MTNext.WebApiContrib.Formatting.Xlsx.Serialisation.Plans;

namespace SQAD.MTNext.WebApiContrib.Formatting.Xlsx.Serialisation.DeliverySources
{
    public class SqadDeliverySourceXlsxSerializer : IXlsxSerialiser
    {
        public SerializerType SerializerType => SerializerType.Default;

        public bool CanSerialiseType(Type valueType, Type itemType)
        {
            return valueType == typeof(DeliverySourceExportDataModel);
        }

        public void Serialise(Type itemType,
                              object value,
                              IXlsxDocumentBuilder document,
                              string sheetName,
                              string columnPrefix,
                              SqadXlsxPlanSheetBuilder sheetbuilderOverride)
        {
            if (!(value is DeliverySourceExportDataModel exportData))
            {
                throw new ArgumentException($"{nameof(value)} has invalid type!");
            }

            var dataTable = CreateDataTable(exportData);
            var sheetBuilder = new SqadDeliverySourceDataSheetBuilder("Data", dataTable, exportData.DeliveryPeriods.Count);
            document.AppendSheet(sheetBuilder);
        }

        private static DataTable CreateDataTable(DeliverySourceExportDataModel exportData)
        {
            var table = new DataTable();

            table.Columns.Add("market", typeof(string)).Caption = "MARKET";
            table.Columns.Add("subtype", typeof(string)).Caption = "SUBTYPE";
            table.Columns.Add("demo", typeof(string)).Caption = "DEMO";

            foreach (var period in exportData.DeliveryPeriods)
            {
                table.Columns.Add($"period_{period.ID}", typeof(decimal)).Caption = period.Caption ?? period.PendingCaption;
            }

            var valuesLookup = exportData.DeliveryValues
                                         .ToDictionary(x => $"{x.MarketID}-{x.SubtypeID}-{x.DemoID}-{x.PeriodID}",
                                                       x => x.Value);

            foreach (var market in exportData.Markets)
            {
                foreach (var subtype in exportData.Subtypes)
                {
                    foreach (var demo in exportData.Demos)
                    {
                        var dataRow = table.NewRow();

                        dataRow["market"] = market.Name;
                        dataRow["subtype"] = subtype.Name;
                        dataRow["demo"] = demo.Name;

                        foreach (var period in exportData.DeliveryPeriods)
                        {
                            var key = $"{market.ID}-{subtype.ID}-{demo.ID}-{period.ID}";
                            if (!valuesLookup.ContainsKey(key))
                            {
                                continue;
                            }

                            var value = valuesLookup[key];
                            dataRow[$"period_{period.ID}"] = value;
                        }

                        table.Rows.Add(dataRow);
                    }
                }
            }

            return table;
        }
    }
}