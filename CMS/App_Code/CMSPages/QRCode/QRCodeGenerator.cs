using System;
using System.Collections.Generic;
using System.Drawing;

using CMS;
using CMS.Base;
using CMS.Helpers;
using CMS.IO;

using ThoughtWorks.QRCode.Codec;

[assembly: RegisterImplementation(typeof(IQRCodeGenerator), typeof(QRCodeGenerator))]

/// <summary>
/// QR Code generator
/// </summary>
public class QRCodeGenerator : IQRCodeGenerator
{
    /// <summary>
    /// List of QR code versions that are supported by the system
    /// </summary>
    private static readonly List<bool?> mQRVersionsSupported = new List<bool?>();


    /// <summary>
    /// Generates the QR code as an image using the given parameters
    /// </summary>
    /// <param name="code">Code to generate by the QR code</param>
    /// <param name="qrCodeSettings">QR code settings</param>
    public Image GenerateQRCode(string code, QRCodeSettings qrCodeSettings)
    {
        QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();

        qrCodeEncoder.QRCodeEncodeMode = GetEncoding(qrCodeSettings.Encoding);

        // Set scale
        try
        {
            qrCodeEncoder.QRCodeScale = qrCodeSettings.Size;
        }
        catch
        {
            qrCodeEncoder.QRCodeScale = 4;
        }

        qrCodeEncoder.QRCodeErrorCorrect = GetErrorCorrection(qrCodeSettings.Correction);

        // Set colors
        qrCodeEncoder.QRCodeForegroundColor = qrCodeSettings.FgColor ?? Color.Black;
        qrCodeEncoder.QRCodeBackgroundColor = qrCodeSettings.BgColor ?? Color.White;

        Image image = null;

        // Attempt to process all versions
        while (qrCodeSettings.Version <= 40)
        {
            if (!QRVersionSupported(qrCodeSettings.Version))
            {
                // Move to higher version
                qrCodeSettings.Version++;

                if (qrCodeSettings.Version > 40)
                {
                    throw new Exception("Version higher than 40 is not supported.");
                }
                continue;
            }

            try
            {
                // Try to get requested version
                qrCodeEncoder.QRCodeVersion = qrCodeSettings.Version;

                image = qrCodeEncoder.Encode(code);

                break;
            }
            catch (IndexOutOfRangeException)
            {
                // Try next version to fit the data
                qrCodeSettings.Version++;

                if (qrCodeSettings.Version > 40)
                {
                    throw;
                }
            }
        }

        return image;
    }


    /// <summary>
    /// Gets the encoding settings from the given parameter
    /// </summary>
    /// <param name="encoding">Encoding parameter</param>
    public static QRCodeEncoder.ENCODE_MODE GetEncoding(string encoding)
    {
        // Set encoding
        var em = QRCodeEncoder.ENCODE_MODE.BYTE;
        switch (encoding.ToLowerCSafe())
        {
            case "an":
                em = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
                break;

            case "n":
                em = QRCodeEncoder.ENCODE_MODE.NUMERIC;
                break;
        }

        return em;
    }


    /// <summary>
    /// Gets the error correction settings from the given parameter
    /// </summary>
    /// <param name="correction">Correction parameter</param>
    public static QRCodeEncoder.ERROR_CORRECTION GetErrorCorrection(string correction)
    {
        // Set error correction
        var ec = QRCodeEncoder.ERROR_CORRECTION.M;
        switch (correction.ToLowerCSafe())
        {
            case "l":
                ec = QRCodeEncoder.ERROR_CORRECTION.L;
                break;

            case "q":
                ec = QRCodeEncoder.ERROR_CORRECTION.Q;
                break;

            case "h":
                ec = QRCodeEncoder.ERROR_CORRECTION.H;
                break;
        }
        return ec;
    }


    /// <summary>
    /// Returns true if the given QR code version if supported
    /// </summary>
    /// <param name="version">Version to check</param>
    private static bool QRVersionSupported(int version)
    {
        // Ensure the number of items in the list
        while (mQRVersionsSupported.Count < version)
        {
            mQRVersionsSupported.Add(null);
        }

        bool? supported = mQRVersionsSupported[version - 1];
        if (supported == null)
        {
            // Validate by the file existence if the version is supported
            string path = "~/App_Data/CMSModules/QRCode/[Resources.zip]/qrvfr" + version + ".dat";
            path = StorageHelper.GetFullFilePhysicalPath(path);

            supported = File.Exists(path);
            mQRVersionsSupported[version - 1] = supported;
        }

        return supported.Value;
    }
}