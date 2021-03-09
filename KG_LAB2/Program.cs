using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using FreeImageAPI;
using FreeImageAPI.Metadata;
using System.Collections.Generic;
using System.IO;

namespace KG_LAB2
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Hello!");

            while (true)
            {
                var choise = String.Empty;

                Console.WriteLine("Press:");
                Console.WriteLine("\t1: Choose file");
                Console.WriteLine("\t2: Choose directory");
                Console.WriteLine("\t0: exit");

                choise = Console.ReadLine();

                switch (choise)
                {
                    case "1":
                        GetFileInfo();
                        break;
                    case "2":
                        GetDirectoryInfo();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Incorrect input, try again");
                        break;
                }

            }
        }

        static void GetFileInfo()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            FIBITMAP dib = new FIBITMAP();
            try
            {
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.FileName = "";
                StringBuilder fileDialogFilterBuilder = new StringBuilder();
                fileDialogFilterBuilder.Append("BMP files(*.BMP)|*.BMP");
                fileDialogFilterBuilder.Append("|JPG files(*.JPG)|*.JPG");
                fileDialogFilterBuilder.Append("|GIF files(*.GIF)|*.GIF");
                fileDialogFilterBuilder.Append("|PNG files(*.PNG)|*.PNG");
                fileDialogFilterBuilder.Append("|TIF files(*.TIF)|*.TIF");
                fileDialogFilterBuilder.Append("|PCX files(*.PCX)|*.PCX");
                ofd.Filter = fileDialogFilterBuilder.ToString();
                ofd.Multiselect = false;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FREE_IMAGE_FORMAT formatR = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
                    dib = FreeImage.LoadEx(ofd.FileName, ref formatR);

                    if (dib.IsNull)
                        throw new Exception("dib is empty - image haven't been loaded!");

                    Console.WriteLine($"File name: {ofd.SafeFileName}");

                    string imgType = ofd.SafeFileName.Substring(ofd.SafeFileName.LastIndexOf('.') + 1,
                        ofd.SafeFileName.Length - 1 - ofd.SafeFileName.LastIndexOf('.'));

                    if (imgType.Equals("pcx", StringComparison.OrdinalIgnoreCase))
                    {
                       
                        Console.WriteLine($"File size: {FreeImage.GetHeight(dib) * FreeImage.GetWidth(dib)} px");
                        Console.WriteLine($"File size: {FreeImage.GetWidth(dib)}x{FreeImage.GetHeight(dib)}");
                        Console.WriteLine($"File resolution: {FreeImage.GetResolutionX(dib)}x{FreeImage.GetResolutionY(dib)}");
                        Console.WriteLine($"File bits per pixel: {FreeImage.GetBPP(dib)}bpp");
                    }
                    else
                    {
                        Image bitmap = Image.FromFile(ofd.FileName);
                        Console.WriteLine($"File size: {bitmap.Height * bitmap.Width} px");
                        Console.WriteLine($"File size: {bitmap.Width}x{bitmap.Height}");
                        Console.WriteLine($"File resolution: {bitmap.HorizontalResolution}x{bitmap.VerticalResolution}");
                        Console.WriteLine($"File bits per pixel: {Image.GetPixelFormatSize(bitmap.PixelFormat)}bpp");
                        bitmap.Dispose();
                    }

                    Console.Write("File compression: ");

                    ImageMetadata iMetadata = new ImageMetadata(dib);

                    foreach (MetadataModel metadataModel in iMetadata)
                    {
                        if (metadataModel.ToString() == "FIMD_EXIF_MAIN")
                        {
                            var tag = metadataModel.GetTag("Compression");

                            if (tag == null)
                            {
                                Console.WriteLine("Unknown");
                                break;
                            }

                            Console.WriteLine(tag.ToString().Split('(')[0].Trim());
                        }
                    }

                    FreeImage.UnloadEx(ref dib);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    Console.WriteLine("\n///////////////////////////////////////////////////////////////////\n");
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                MessageBox.Show(ex.ToString(), "Exception caught");
            }
            // Clean up
            finally
            {
                ofd.Dispose();
                FreeImage.UnloadEx(ref dib);
            }
        }

        static void GetDirectoryInfo()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            FIBITMAP dib = new FIBITMAP();
            try
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    var filenames = Directory.GetFiles(fbd.SelectedPath).ToList()
                        .Where(s => s.EndsWith(".gif") ||
                                    s.EndsWith(".png") ||
                                    s.EndsWith(".bmp") ||
                                    s.EndsWith(".tif") ||
                                    s.EndsWith(".pcx") ||
                                    s.EndsWith(".jpg"));


                    foreach (var filename in filenames)
                    {
                        FREE_IMAGE_FORMAT formatR = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
                        dib = FreeImage.LoadEx(filename, ref formatR);

                        if (dib.IsNull)
                            throw new Exception("dib is empty - image haven't been loaded!");

                        string saveFileName = Path.GetFileName(filename);

                        Console.WriteLine($"File name: {saveFileName}");

                        string imgType = saveFileName.Substring(saveFileName.LastIndexOf('.') + 1,
                            saveFileName.Length - 1 - saveFileName.LastIndexOf('.'));

                        if (imgType.Equals("pcx", StringComparison.OrdinalIgnoreCase))
                        {

                            Console.WriteLine($"File size: {FreeImage.GetHeight(dib) * FreeImage.GetWidth(dib)} px");
                            Console.WriteLine($"File size: {FreeImage.GetWidth(dib)}x{FreeImage.GetHeight(dib)}");
                            Console.WriteLine($"File resolution: {FreeImage.GetResolutionX(dib)}x{FreeImage.GetResolutionY(dib)}");
                            Console.WriteLine($"File bits per pixel: {FreeImage.GetBPP(dib)}bpp");
                        }
                        else
                        {
                            Image bitmap = Image.FromFile(filename);
                            Console.WriteLine($"File size: {bitmap.Height * bitmap.Width} px");
                            Console.WriteLine($"File size: {bitmap.Width}x{bitmap.Height}");
                            Console.WriteLine($"File resolution: {bitmap.HorizontalResolution}x{bitmap.VerticalResolution}");
                            Console.WriteLine($"File bits per pixel: {Image.GetPixelFormatSize(bitmap.PixelFormat)}bpp");
                            bitmap.Dispose();
                        }

                        Console.Write("File compression: ");

                        ImageMetadata iMetadata = new ImageMetadata(dib);

                        foreach (MetadataModel metadataModel in iMetadata)
                        {
                            if (metadataModel.ToString() == "FIMD_EXIF_MAIN")
                            {
                                var tag = metadataModel.GetTag("Compression");

                                if (tag == null)
                                {
                                    Console.WriteLine("Unknown");
                                    break;
                                }

                                Console.WriteLine(tag.ToString().Split('(')[0].Trim());
                            }
                        }

                        FreeImage.UnloadEx(ref dib);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        Console.WriteLine("\n///////////////////////////////////////////////////////////////////\n");
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.ToString(), "Exception caught");
            }
            // Clean up
            finally
            {
                fbd.Dispose();
                FreeImage.UnloadEx(ref dib);
            }
        }
    }
}
