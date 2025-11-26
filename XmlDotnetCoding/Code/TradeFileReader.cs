using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using PortlandTradeReporterLib.Xml.UK.Sending.DerivativesTradeReport;

namespace XmlDotnetCoding
{
    /// <summary>
    /// Provides functionality to read and parse trade report XML files
    /// </summary>
    public class TradeFileReader
    {
        /// <summary>
        /// Reads a trade file and extracts the DerivativesTradeReportV03 from the Pyld element
        /// </summary>
        /// <param name="filePath">Path to the XML file to read</param>
        /// <returns>DerivativesTradeReportV03 object deserialized from the Pyld element</returns>
        /// <exception cref="ArgumentNullException">Thrown when filePath is null or empty</exception>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist</exception>
        /// <exception cref="XmlException">Thrown when the XML is malformed</exception>
        /// <exception cref="InvalidOperationException">Thrown when the Pyld element is not found or deserialization fails</exception>
        public DerivativesTradeReportV03 ReadTradeFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Trade file not found: {filePath}", filePath);
            }

            try
            {
                // Load the XML document
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);

                // Get the Pyld element
                var pyldElement = GetPyldElement(xmlDoc);

                if (pyldElement == null)
                {
                    throw new InvalidOperationException("Pyld element not found in the XML document");
                }

                // Extract the Document element from within Pyld
                var documentElement = pyldElement.FirstChild as XmlElement;

                if (documentElement == null || documentElement.LocalName != "Document")
                {
                    throw new InvalidOperationException("Document element not found within Pyld element");
                }

                // Deserialize the Document to get DerivativesTradeReportV03
                var derivativesTradeReport = DeserializeDocument(documentElement);

                return derivativesTradeReport;
            }
            catch (XmlException ex)
            {
                throw new XmlException($"Error parsing XML file: {filePath}", ex);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error reading trade file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Extracts the Pyld XmlElement from the XML document
        /// </summary>
        /// <param name="xmlDoc">The XML document to search</param>
        /// <returns>The Pyld XmlElement, or null if not found</returns>
        private XmlElement GetPyldElement(XmlDocument xmlDoc)
        {
            // The Pyld element is in the head.003.001.01 namespace
            var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            namespaceManager.AddNamespace("biz", "urn:iso:std:iso:20022:tech:xsd:head.003.001.01");

            // Find the Pyld element using XPath
            var pyldNode = xmlDoc.SelectSingleNode("//biz:Pyld", namespaceManager);

            return pyldNode as XmlElement;
        }

        /// <summary>
        /// Deserializes the Document XmlElement to extract DerivativesTradeReportV03
        /// </summary>
        /// <param name="documentElement">The Document XmlElement from the Pyld</param>
        /// <returns>DerivativesTradeReportV03 object</returns>
        /// <exception cref="InvalidOperationException">Thrown when deserialization fails</exception>
        private DerivativesTradeReportV03 DeserializeDocument(XmlElement documentElement)
        {
            try
            {
                // Create an XmlSerializer for the Document class
                var serializer = new XmlSerializer(typeof(Document));

                // Create a reader from the element
                using (var reader = new XmlNodeReader(documentElement))
                {
                    // Deserialize the Document
                    var document = (Document)serializer.Deserialize(reader);

                    if (document == null)
                    {
                        throw new InvalidOperationException("Deserialization of Document returned null");
                    }

                    // Return the DerivsTradRpt property
                    return document.DerivsTradRpt;
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Failed to deserialize Document element", ex);
            }
        }

        /// <summary>
        /// Alternative method that returns the full Document object instead of just DerivativesTradeReportV03
        /// </summary>
        /// <param name="filePath">Path to the XML file to read</param>
        /// <returns>Document object containing the DerivativesTradeReportV03</returns>
        public Document ReadTradeFileAsDocument(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Trade file not found: {filePath}", filePath);
            }

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);

                var pyldElement = GetPyldElement(xmlDoc);

                if (pyldElement == null)
                {
                    throw new InvalidOperationException("Pyld element not found in the XML document");
                }

                var documentElement = pyldElement.FirstChild as XmlElement;

                if (documentElement == null || documentElement.LocalName != "Document")
                {
                    throw new InvalidOperationException("Document element not found within Pyld element");
                }

                var serializer = new XmlSerializer(typeof(Document));

                using (var reader = new XmlNodeReader(documentElement))
                {
                    var document = (Document)serializer.Deserialize(reader);

                    if (document == null)
                    {
                        throw new InvalidOperationException("Deserialization of Document returned null");
                    }

                    return document;
                }
            }
            catch (XmlException ex)
            {
                throw new XmlException($"Error parsing XML file: {filePath}", ex);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error reading trade file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Extracts TradeReport32Choice__1 items from the TradeData57Choice__1.Items array
        /// </summary>
        /// <param name="tradeReport">The DerivativesTradeReportV03 containing the trade data</param>
        /// <returns>Array of TradeReport32Choice__1 objects (report items)</returns>
        /// <exception cref="ArgumentNullException">Thrown when tradeReport is null</exception>
        public TradeReport32Choice__1[] GetTradeReportItems(DerivativesTradeReportV03 tradeReport)
        {
            if (tradeReport == null)
            {
                throw new ArgumentNullException(nameof(tradeReport), "Trade report cannot be null");
            }

            // Check if TradData exists
            if (tradeReport.TradData == null)
            {
                return new TradeReport32Choice__1[0];
            }

            // Check if Items array exists
            if (tradeReport.TradData.Items == null || tradeReport.TradData.Items.Length == 0)
            {
                return new TradeReport32Choice__1[0];
            }

            // Filter and cast items to TradeReport32Choice__1
            // The Items array can contain either TradeReport32Choice__1 or ReportPeriodActivity1Code
            return tradeReport.TradData.Items
                .OfType<TradeReport32Choice__1>()
                .ToArray();
        }

        /// <summary>
        /// Extracts TradeReport32Choice__1 items from a TradeData57Choice__1 object
        /// </summary>
        /// <param name="tradeData">The TradeData57Choice__1 containing the items</param>
        /// <returns>Array of TradeReport32Choice__1 objects (report items)</returns>
        /// <exception cref="ArgumentNullException">Thrown when tradeData is null</exception>
        public TradeReport32Choice__1[] GetTradeReportItems(TradeData57Choice__1 tradeData)
        {
            if (tradeData == null)
            {
                throw new ArgumentNullException(nameof(tradeData), "Trade data cannot be null");
            }

            // Check if Items array exists
            if (tradeData.Items == null || tradeData.Items.Length == 0)
            {
                return new TradeReport32Choice__1[0];
            }

            // Filter and cast items to TradeReport32Choice__1
            return tradeData.Items
                .OfType<TradeReport32Choice__1>()
                .ToArray();
        }

        /// <summary>
        /// Gets all trade report items with their type information
        /// </summary>
        /// <param name="tradeReport">The DerivativesTradeReportV03 containing the trade data</param>
        /// <returns>List of tuples containing the report item and its type name</returns>
        public List<(TradeReport32Choice__1 Report, string ReportType)> GetTradeReportItemsWithType(DerivativesTradeReportV03 tradeReport)
        {
            if (tradeReport == null)
            {
                throw new ArgumentNullException(nameof(tradeReport), "Trade report cannot be null");
            }

            var results = new List<(TradeReport32Choice__1, string)>();

            var reportItems = GetTradeReportItems(tradeReport);

            foreach (var item in reportItems)
            {
                string reportType = GetReportType(item);
                results.Add((item, reportType));
            }

            return results;
        }

        /// <summary>
        /// Determines the type of trade report (New, Mod, Crrctn, etc.)
        /// </summary>
        /// <param name="reportItem">The TradeReport32Choice__1 item</param>
        /// <returns>String representing the report type</returns>
        private string GetReportType(TradeReport32Choice__1 reportItem)
        {
            if (reportItem?.Item == null)
            {
                return "Unknown";
            }

            var itemType = reportItem.Item.GetType().Name;

            // Map the type names to readable report types
            switch (itemType)
            {
                case "TradeData42__1":
                    return "New";
                case "TradeData42__2":
                    return "Modification";
                case "TradeData42__3":
                    return "Correction";
                case "TradeData42__4":
                    return "Termination";
                case "TradeData42__5":
                    return "PositionComponent";
                case "TradeData42__6":
                    return "ValuationUpdate";
                case "TradeData42__7":
                    return "Error";
                case "TradeData42__8":
                    return "Revive";
                default:
                    return itemType;
            }
        }
    }
}
