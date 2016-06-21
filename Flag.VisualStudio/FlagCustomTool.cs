﻿using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Text;

namespace Flag.VisualStudio
{
    [ComVisible(true)]
    // TODO: Replace with your GUID
    [Guid("7C6AF83B-C209-4930-BB58-A4B56AFF51CE")]
    public class FlagCustomTool : IVsSingleFileGenerator
    {
        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".cs";  // the extension must include the leading period
            return VSConstants.S_OK; // signal successful completion
        }


        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
          IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            pcbOutput = ApplyText(bstrInputFileContents, rgbOutputFileContents);
            return VSConstants.S_OK;
        }

        private static uint ApplyText(string text, IntPtr[] rgbOutputFileContents)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            int length = bytes.Length;
            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(length);
            Marshal.Copy(bytes, 0, rgbOutputFileContents[0], length);
            return (uint)length;
        }
    }
}