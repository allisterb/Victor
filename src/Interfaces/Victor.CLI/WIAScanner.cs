using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

namespace Victor.CLI
{
    class WIAScanner
    {
        const string wiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";
        const string wiaFormatPNG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}";
        const string wiaFornatTIFF = "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}";

        class WIA_DPS_DOCUMENT_HANDLING_SELECT
        {
            public const uint FEEDER = 0x00000001;
            public const uint FLATBED = 0x00000002;
        }

        class WIA_DPS_DOCUMENT_HANDLING_STATUS
        {
            public const uint FEED_READY = 0x00000001;
        }

        class WIA_PROPERTIES
        {
            public const uint WIA_RESERVED_FOR_NEW_PROPS = 1024;
            public const uint WIA_DIP_FIRST = 2;
            public const uint WIA_DPA_FIRST = WIA_DIP_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPC_FIRST = WIA_DPA_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            //
            // Scanner only device properties (DPS)
            //
            public const uint WIA_DPS_FIRST = WIA_DPC_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPS_DOCUMENT_HANDLING_STATUS = WIA_DPS_FIRST + 13;
            public const uint WIA_DPS_DOCUMENT_HANDLING_SELECT = WIA_DPS_FIRST + 14;

            public const string WIA_SCAN_COLOR_MODE = "6146";
            public const string WIA_PREVIEW = "3100";
            public const string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
            public const string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
            public const string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
            public const string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
            public const string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
            public const string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";
            public const string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
            public const string WIA_SCAN_CONTRAST_PERCENTS = "6155";
        }

        
        /// <summary>
        /// Use scanner to scan an image (scanner is selected by its unique id).
        /// </summary>
        /// <param name="scannerName"></param>
        /// <returns>Scanned images.</returns>
        public static List<byte[]> Scan(string scannerId, DUController controller)
        {
            List<byte[]> images = new List<byte[]>();
            bool hasMorePages = true;
            while (hasMorePages)
            {
                // select the correct scanner using the provided scannerId parameter
                WIA.DeviceManager manager = new WIA.DeviceManager();
                WIA.Device device = null;

                foreach (WIA.DeviceInfo info in manager.DeviceInfos)
                {
                    if (info.DeviceID == scannerId)
                    {
                        // connect to scanner
                        device = info.Connect();
                        break;
                    }
                }

                // device was not found
                if (device == null)
                {
                    // enumerate available devices
                    string availableDevices = "";
                    foreach (WIA.DeviceInfo info in manager.DeviceInfos)
                    {
                        availableDevices +=  Environment.NewLine;
                    }
                    controller.SayErrorLine("The device with provided ID could not be found. Available Devices:" + Environment.NewLine + availableDevices);
                    return null;
                }
                var props = device.Properties;
                foreach(var p in props)
                {
                    WIA.IProperty prop = (WIA.IProperty)p;
                    string n = prop.Name;
                    if (n == "Description")
                    {
                        string v = (string)prop.get_Value();
                        if (!string.IsNullOrEmpty(v))
                        {
                            controller.SayInfoLine($"Using scanner {v}...");
                        }
                        break;
                    }
                }


                WIA.Item item = device.Items[1];
                SetWIAProperty(item.Properties, WIA_PROPERTIES.WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, 200f);
                SetWIAProperty(item.Properties, WIA_PROPERTIES.WIA_VERTICAL_SCAN_RESOLUTION_DPI, 200f);
                SetWIAProperty(item.Properties, WIA_PROPERTIES.WIA_HORIZONTAL_SCAN_SIZE_PIXELS, 8.5f * 200f);
                SetWIAProperty(item.Properties, WIA_PROPERTIES.WIA_VERTICAL_SCAN_SIZE_PIXELS, 11f * 200f);
                SetWIAProperty(item.Properties, WIA_PROPERTIES.WIA_SCAN_COLOR_MODE, 1);
                //SetWIAProperty(item.Properties, WIA_PROPERTIES.WIA_PREVIEW, 1);

                try
                {
                    // scan image
                    //WIA.ICommonDialog wiaCommonDialog = new WIA.CommonDialog();
                    controller.SayInfoLine("Scanning...");
                    controller.StartBeeper();
                    
                    WIA.ImageFile image = (WIA.ImageFile)item.Transfer(wiaFormatPNG); //wiaCommonDialog.ShowTransfer(item, wiaFormatBMP, false);
                    //WIA.ImageProcess imageProcess = new WIA.ImageProcess();
        
                    //imageProcess.Filters.Add(imageProcess.FilterInfos.get_Item("Scale").FilterID);
                    //imageProcess.Filters[0].Properties.get_Item("MaximumWidth").set_Value(1024);
                    //imageProcess.Filters[0].Properties.get_Item("PreserveAspectRation").set_Value(true);
                    //imageProcess.Filters.Add(imageProcess.FilterInfos.get_Item("Convert").FilterID);
                    //imageProcess.Filters[0].Properties.get_Item("FormatID").set_Value(wiaFormatJPEG);
                    //imageProcess.Filters[0].Properties.get_Item("Quality").set_Value(75);
                    //WIA.ImageFile image2 = imageProcess.Apply(image);
                    /*
                    //With.Filters(1)
                    //    .Properties("MaximumWidth").Value = 1024
                        .Properties("MaximumHeight").Value = 1024
                        .Properties("PreserveAspectRatio").Value = True
                    End With
                    .Filters.Add.FilterInfos.Item("Convert").FilterID
                    With.Filters(2)
                        .Properties("FormatID").Value = wiaFormatJPEG
                        .Properties("Quality").Value = 50 '1 to 100 percent, higher is better.
                    End With
                End With
                    */
                    controller.StopBeeper();
                    // save to temp file
                    string fileName = Path.GetTempFileName();
                    File.Delete(fileName);
                    image.SaveFile(fileName);
                    var ifactory = new ImageProcessor.ImageFactory(false);
                    ImageProcessor.Imaging.Formats.ISupportedImageFormat format = new ImageProcessor.Imaging.Formats.PngFormat { Quality = 70 };
                    var ib = File.ReadAllBytes(fileName);
                    ifactory.Load(ib);
                    ifactory.Resize(new Size(1280, 720));
                    MemoryStream ms = new MemoryStream();
                    ifactory.Save(ms);
                    
                    // add file to output list
                    images.Add(ms.ToArray());
                }
                catch (Exception exc)
                {
                    controller.SayErrorLine($"An error occurred during scanning: {exc.Message}.");
                    return null;
                }
                finally
                {
                    item = null;

                    //determine if there are any more pages waiting
                    WIA.Property documentHandlingSelect = null;
                    WIA.Property documentHandlingStatus = null;

                    foreach (WIA.Property prop in device.Properties)
                    {
                        if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_SELECT)
                            documentHandlingSelect = prop;

                        if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_STATUS)
                            documentHandlingStatus = prop;
                    }

                    // assume there are no more pages
                    hasMorePages = false;

                    // may not exist on flatbed scanner but required for feeder
                    if (documentHandlingSelect != null)
                    {
                        // check for document feeder
                        if ((Convert.ToUInt32(documentHandlingSelect.get_Value()) && WIA_DPS_DOCUMENT_HANDLING_SELECT.FEEDER) != 0)
                        {
                            hasMorePages = ((Convert.ToUInt32(documentHandlingStatus.get_Value()) && WIA_DPS_DOCUMENT_HANDLING_STATUS.FEED_READY) != 0);
                        }
                    }
                }
            }

            return images;
        }

        /// <summary>
        /// Gets the list of available WIA devices.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDevices()
        {
            List<string> devices = new List<string>();
            WIA.DeviceManager manager = new WIA.DeviceManager();

            foreach (WIA.DeviceInfo info in manager.DeviceInfos)
            {
                devices.Add(info.DeviceID);
            }

            return devices;
        }

        private static void SetWIAProperty(WIA.IProperties properties,
               object propName, object propValue)
        {
            WIA.Property prop = properties.get_Item(ref propName);
            prop.set_Value(ref propValue);
        }
       

        public enum WIAPageSize
        {
            A4, // 8.3 x 11.7 in  (210 x 297 mm)
            Letter, // 8.5 x 11 in (216 x 279 mm)
            Legal, // 8.5 x 14 in (216 x 356 mm)
        }

        public enum WIAScanQuality
        {
            Preview,
            Final,
        }
    }
}